using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace FlexibleDBMS
{
    public class ApplicationUpdater
    {
        public delegate void Reset(object sender, BoolEventArgs e);
        public event Reset EvntReset;

        public delegate void InfoMessage<TextEventArgs>(object sender, TextEventArgs e);
        public event InfoMessage<TextEventArgs> EvntStatus;

        static bool uploadingStatus = false;
        bool stopUpdate = false;

        /// <summary>
        /// format - server.domain.subdomain/folder  or   server/folder
        /// </summary>
        string _serverUpdateURI; // format - server.domain.subdomain/folder  or   server/folder
        string _pathToExternalUpdateZip = null;
        UserAD _userAD = null;

        public UpdateOptions Options { get; private set; }


        public ApplicationUpdater() { }

        public ApplicationUpdater(UserAD user, string serverUpdateURI, string pathToExternalUpdateZip = null)
        { _ = SetOptionsAsync(user, serverUpdateURI, pathToExternalUpdateZip); }

        public async Task SetOptionsAsync(UserAD user, string serverUpdateURI, string pathToExternalUpdateZip = null)
        {
            _userAD = user;
            _serverUpdateURI = serverUpdateURI;
            _pathToExternalUpdateZip = pathToExternalUpdateZip;

            if (!Directory.Exists(CommonConst.LocalTempFolder))
                Directory.CreateDirectory(CommonConst.LocalTempFolder);

            if (!Directory.Exists(CommonConst.LocalUpdateFolder))
                Directory.CreateDirectory(CommonConst.LocalUpdateFolder);

            if (!string.IsNullOrWhiteSpace(_serverUpdateURI))
            {
                Options = MakeUpdateOptions();
                stopUpdate = false;
            }
            else
            {
                CommonExtensions.Logger(LogTypes.Info,$"При инициализации не указан адрес сервера обновлений.");
                CommonExtensions.Logger(LogTypes.Info, $"Ищу файл '{CommonConst.PathToUrl}'");
                if (!string.IsNullOrWhiteSpace(CommonConst.PathToUrl) && File.Exists(CommonConst.PathToUrl))
                {
                    IList<string> file = await ReadFileAsync(CommonConst.PathToUrl);
                    if (file?.Count > 0)
                    {
                        foreach (var f in file)
                        {
                            CommonExtensions.Logger(LogTypes.Info,f);
                            this._serverUpdateURI = f;
                            CommonExtensions.Logger(LogTypes.Info,$"Адрес источника обновлений найден в файле как: '{f}'");
                            break;
                        }

                        Options = MakeUpdateOptions();
                        EvntStatus?.Invoke(this, new TextEventArgs($"Адрес сервера обновлений: {Options.serverUpdateURI}"));

                        stopUpdate = false;
                    }
                    else
                    {
                        stopUpdate = true;
                    }
                }
                else
                {
                    stopUpdate = true;
                    EvntStatus?.Invoke(this, new TextEventArgs($"Файл с адресом сервера обновлений '{CommonConst.PathToUrl}' не найден"));
                }
            }

            if (Options?.DoObjectPropertiesAsStringDictionary().Count > 0 && !string.IsNullOrWhiteSpace(CommonConst.PathToUrl))
            {
                CommonExtensions.Logger(LogTypes.Info,"Параметры для скачивания/загрузки обновлений:");
                CommonExtensions.Logger(LogTypes.Info,Options.DoObjectPropertiesAsStringDictionary().AsString());
            }
            else
            {
                CommonExtensions.Logger(LogTypes.Info,$"Операции загрузки/выгрузки обновлений не доступны.");
            }
        }

        private async Task<IList<string>> ReadFileAsync(string filePath)
        {
            FileReader readerConfig = new FileReader();
            IList<string> file = await readerConfig.ReadAsync(filePath, 2);
            return file;
        }

        private UpdateOptions MakeUpdateOptions()
        {
            string localUpdateFolderPath = @"file://" + _serverUpdateURI?.Replace(@"\", @"/") + @"/";
            string appUpdateFolderURI = @"\\" + _serverUpdateURI?.Replace(@"/", @"\") + @"\";
            string appUpdateURL = localUpdateFolderPath + Path.GetFileName(CommonConst.AppFileUpdateXml);

            UpdateOptions parameters = new UpdateOptions
            {
                localAppFolderPath = CommonConst.LocalAppFolder,
                serverUpdateURI = _serverUpdateURI,
                appVersion = CommonConst.AppVersion,
                pathToXml = CommonConst.PathToXml,
                pathToUpdateZip = CommonConst.PathToUpdateZip,
                localUpdateFolderPath = localUpdateFolderPath,
                appUpdateFolderURI = appUpdateFolderURI,
                appUpdateURL = appUpdateURL
            };

            return parameters.Get();
        }


        /// <summary>
        /// Run Update immidiately
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RunUpdate()
        {
            if (!uploadingStatus ||!stopUpdate)//!string.IsNullOrWhiteSpace(Options?.serverUpdateURI))
            {
                EvntStatus?.Invoke(this, new TextEventArgs($"Адрес сервера обновлений: {this._serverUpdateURI}"));
                CommonExtensions.Logger(LogTypes.Info,$"Адрес сервера обновлений: {Options.serverUpdateURI}");

                AutoUpdater.Mandatory = true;
                // AutoUpdater.ReportErrors = true;
                AutoUpdater.UpdateMode = Mode.Forced;

                DoUpdate();
            }
            else
            {
                CommonExtensions.Logger(LogTypes.Info,$"Не указан адрес сервера обновлений: {Options?.serverUpdateURI}");
            }
        }

        private void DoUpdate()
        {
            //https://github.com/ravibpatel/AutoUpdater.NET
            //http://www.cyberforum.ru/csharp-beginners/thread2169711.html

            AutoUpdater.AppTitle = $"{Application.ProductName} on the update page of company '{Application.CompanyName}'";
            AutoUpdater.RunUpdateAsAdmin = false;
            //AutoUpdater.ReportErrors = true;
            AutoUpdater.DownloadPath = CommonConst.LocalUpdateFolder;
            AutoUpdater.UpdateMode = Mode.Normal;

            AutoUpdater.CheckForUpdateEvent += new AutoUpdater.CheckForUpdateEventHandler(CheckUpdate_Event); //write errors if had no access to the folder
            AutoUpdater.ApplicationExitEvent += new AutoUpdater.ApplicationExitEventHandler(ApplicationExit);    //https://archive.codeplex.com/?p=autoupdaterdotnet

            if (_userAD.Login != null && _userAD.Password != null && _userAD.Domain != null)
            {
                AutoUpdater.Start(Options.appUpdateURL, new System.Net.NetworkCredential(_userAD.Login, _userAD.Password, _userAD.Domain));

                //AutoUpdater.CheckForUpdateEvent -= CheckUpdate_Event;
                //AutoUpdater.ApplicationExitEvent -= ApplicationExit;
            }
        }

        private void ApplicationExit()
        {
            EvntReset?.Invoke(this, new BoolEventArgs(true));
        }


        /// <summary>
        /// Run Autoupdate function   Task.Run(() => CheckUpdates());
        /// </summary>
        /// <returns></returns>
        public Task CheckUpdatePeriodicaly(int minutes = 1)
        {
            //Check updates frequently
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = minutes * 60 * 1000       // the interval of checking is set in 2 hours='2 * 60 * 60 * 1000'
            };
            timer.Elapsed += delegate
            {

                if (!uploadingStatus || !stopUpdate)//!string.IsNullOrWhiteSpace(Options?.serverUpdateURI))
                {
                    AutoUpdater.Mandatory = true;
                    AutoUpdater.UpdateMode = Mode.ForcedDownload;
                    AutoUpdater.LetUserSelectRemindLater = false;
                    AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
                    AutoUpdater.RemindLaterAt = 2;

                    DoUpdate();
                }
                else
                {
                    CommonExtensions.Logger(LogTypes.Info,$"Не указан адрес сервера обновлений: {Options?.serverUpdateURI}");
                }
            };
            return Task.Run(() =>
             {
                 timer.Start();
             });
        }

        private void CheckUpdate_Event(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                if (args.IsUpdateAvailable)
                {
                    try
                    {
                        if (AutoUpdater.DownloadUpdate(args))
                        {
                            XmlDocument xmldoc = new XmlDocument();
                            XmlNodeList xmlnode;
                            xmldoc.Load(Options.appUpdateURL);
                            xmlnode = xmldoc.GetElementsByTagName("version");
                            string foundNewVersionApp = xmlnode[0].InnerText;
                            EvntStatus?.Invoke(this, new TextEventArgs($"Программа будет обновлена до версии: {foundNewVersionApp}"));
                            CommonExtensions.Logger(LogTypes.Info,$"Программа будет обновлена до версии: {foundNewVersionApp}");

                            ApplicationExit();
                        }
                    }
                    catch (Exception exception)
                    {
                        EvntStatus?.Invoke(this, new TextEventArgs($"Ошибка проверки обновлений: {exception.Message} | {exception.GetType().ToString()}"));
                        CommonExtensions.Logger(LogTypes.Info,$"Ошибка проверки обновлений: {exception.Message} | {exception.GetType().ToString()}");
                    }
                    // Uncomment the following line if you want to show standard update dialog instead.
                    // AutoUpdater.ShowUpdateForm(args);
                }
            }
        }

        public void PrepareUpdateFiles()
        {
            EvntStatus?.Invoke(this, new TextEventArgs($"Адрес сервера обновлений: {Options?.serverUpdateURI}"));

            if (string.IsNullOrWhiteSpace(_pathToExternalUpdateZip) || !File.Exists(_pathToExternalUpdateZip))
            {
                PrepareUpdateFile();
            }
            else
            {
                Options.pathToUpdateZip = _pathToExternalUpdateZip;
                Options.appVersion = GetVersionFromUpdateFile(Options?.pathToUpdateZip);
            }
            EvntStatus?.Invoke(this, new TextEventArgs($"Обновление подготовлено: '{Options?.pathToUpdateZip}'"));

            WriteFileHashInOptions(Options.pathToUpdateZip);
            EvntStatus?.Invoke(this, new TextEventArgs($"Вычислен хэш файла: '{Options?.pathToUpdateZip}'"));

            MakeUpdateXML();
            EvntStatus?.Invoke(this, new TextEventArgs($"Подготовлен файл с информацией о последнем обновлении: '{Options?.pathToXml}'"));
            EvntStatus?.Invoke(this, new TextEventArgs($"Подготовлен полный пакет файлов обновления для загрузки в: '{Options?.serverUpdateURI}'"));
        }
        
        //Upload App's files to Server        
        public void UploadUpdate() //UploadApplicationToShare()
        {
            uploadingStatus = true;
            UpdateUploader uploader = new UpdateUploader();
            uploader.StatusFinishedUploading += new UpdateUploader.Uploaded<BoolEventArgs>(Uploader_StatusFinishedUploading);
            uploader.StatusText += new UpdateUploader.Info<TextEventArgs>(Uploader_MessageStatus);


            if (!string.IsNullOrWhiteSpace(Options?.serverUpdateURI))
            {
                PrepareUpdateFiles();

                List<string> source = new List<string> { Options.pathToXml, Options.pathToUpdateZip };

                List<IFileInfo> listSource = new List<IFileInfo>();
                source.ForEach(p => listSource.Add((FileInfoBase)ReturnNewFileInfo(p)));

                List<string> target = new List<string> {
                        Options.appUpdateFolderURI +Path.GetFileName( Options.pathToXml),
                        Options.appUpdateFolderURI +Path.GetFileName( Options.pathToUpdateZip)
                        };

                List<IFileInfo> listTarget = new List<IFileInfo>();
                target.ForEach(p => listTarget.Add((FileInfoBase)ReturnNewFileInfo(p)));


                EvntStatus?.Invoke(this, new TextEventArgs($"Начинаю отправку обновления программы версии {Options.appVersion} на сервер..."));
                uploader.Set(Options, listSource, listTarget);

                uploader.Upload();
            }
            else
            {
                CommonExtensions.Logger(LogTypes.Info,$"Не указан адрес сервера обновлений: {Options?.serverUpdateURI}");
            }
            uploader.StatusFinishedUploading -= Uploader_StatusFinishedUploading;
            uploader.StatusText -= Uploader_MessageStatus;
        }

        private void Uploader_StatusFinishedUploading(object sender, BoolEventArgs e)
        {
            if (e.Status)
                EvntStatus?.Invoke(this, new TextEventArgs($"Обновление на сервер в: {_serverUpdateURI} загружено успешно!"));
            else
                EvntStatus?.Invoke(this, new TextEventArgs($"Ошибка! Обновление не загружено в: {_serverUpdateURI}"));
        }

        private void Uploader_MessageStatus(object sender, TextEventArgs e)
        {
            EvntStatus?.Invoke(this, new TextEventArgs(e.Message));
        }

        private void PrepareUpdateFile()
        {
            string pathToBak = Path.Combine(CommonConst.LocalAppFolder, "bak\\" + CommonConst.AppFileUpdateZip);
            string fileName; string fullDestination;

            DeleteFile(Options.pathToUpdateZip);

            string[] files = Directory.GetFiles(CommonConst.LocalAppFolder, "*.exe", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                fileName = file.Replace(CommonConst.LocalAppFolder, ""); // Get the file name
                fullDestination = CommonConst.LocalTempFolder + fileName;  // Complete the uri

                Task.Run(() => DeleteFile(fullDestination)).Wait();
                Task.Run(() => CopyFile(file, fullDestination)).Wait();
            }

            files = Directory.GetFiles(CommonConst.LocalAppFolder, "*.dll", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                fileName = file.Replace(CommonConst.LocalAppFolder, ""); // Get the file name
                fullDestination = CommonConst.LocalTempFolder + fileName;  // Complete the uri

                Task.Run(() => DeleteFile(fullDestination)).Wait();
                Task.Run(() => CopyFile(file, fullDestination)).Wait();
            }

            //Make an archive with the currrent app's version
            try
            {
                //add link  to the assembly, "System.IO.Compression.FileSystem"
                System.IO.Compression.ZipFile.CreateFromDirectory(
                    CommonConst.LocalTempFolder,
                    Options.pathToUpdateZip, System.IO.Compression.CompressionLevel.Optimal, false);
            }
            catch (Exception err)
            {
                CommonExtensions.Logger(LogTypes.Info,"Archieving error: " + err.Message);
                CommonExtensions.Logger(LogTypes.Info,err.ToString());
            }


            //  ClearItemsInFolder(appFolderTempPath);
            files = Directory.GetFiles(CommonConst.LocalTempFolder, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                DeleteFile(file);
            }

            CommonExtensions.Logger(LogTypes.Info,"Делаю бэкап архива");
            CopyFile(Options.pathToUpdateZip, pathToBak);
        }

        private string GetVersionFromUpdateFile(string pathToExternalUpdateZip)
        {
            string version = null;
            Random rnd = new Random();
            string pathToDir = rnd.Next().ToString();

            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(pathToExternalUpdateZip, pathToDir);
                string pathToFile = pathToDir + "\\" + Path.GetFileName(Application.ExecutablePath);
                version = System.Reflection.AssemblyName.GetAssemblyName(pathToFile).Version.ToString();
            }
            catch (Exception err)
            {
                CommonExtensions.Logger(LogTypes.Info,"GetVersion error: " + err.Message);
                CommonExtensions.Logger(LogTypes.Info,err.ToString());
            }

            Directory.Delete(pathToDir, true);

            return version;
        }

        private void WriteFileHashInOptions(string filePath) //pathToUpdateZip
        {
            CommonExtensions.Logger(LogTypes.Info,"Вычисляю хэш обновления");
            if (File.Exists(filePath))
            {
                Options.appUpdateMD5 = CalculateHash(filePath);
            }
            else
            {
                CommonExtensions.Logger(LogTypes.Info,$"Отсутствует файл обновления '{filePath}' для вычисления хэш-суммы.");
            }

        }

        private void MakeUpdateXML()
        {
            CommonExtensions.Logger(LogTypes.Info,"MakeUpdateXML");

            Contract.Requires(Options != null,
                    "Не создан экземпляр UpdatingParameters!");

            Contract.Requires(!string.IsNullOrWhiteSpace(Options.pathToXml),
                    "Отсутствует параметр appFileXml или ссылка пустая!");

            Contract.Requires(!string.IsNullOrWhiteSpace(Options.Get().appVersion),
                    "Отсутствует параметр appVersion или ссылка пустая!");

            //https://stackoverflow.com/questions/44477727/writing-xml-and-reading-it-back-c-sharp
            //clear any xmlns attributes from the root element
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");//clear any xmlns attributes from the root element

            XMLDocument document = new XMLDocument
            {
                version = Options.Get().appVersion,
                url = Path.Combine(Options.Get().serverUpdateURI, Path.GetFileName(Options.pathToUpdateZip))
            };

            if (Options.Get().appUpdateMD5 != null)
            {
                var checksum = new XMLElementChecksum
                {
                    value = Options.Get().appUpdateMD5,
                    algorithm = "MD5"
                };
                document.checksum = checksum;
            }

            //  var nodesToStore = new List<XMLDocument> { document };
            try
            {
                using (FileStream fs = new FileStream(Options.pathToXml, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(document.GetType());//, atribXmlOver
                    serializer.Serialize(fs, document, ns); //clear any xmlns attributes from the root element
                }
                CommonExtensions.Logger(LogTypes.Info,$"XML файл сохранен как {Path.GetFullPath(Options.pathToXml)}");
            }
            catch
            {
                CommonExtensions.Logger(LogTypes.Info,$"Ошибка сохранения XML файла {Options.pathToXml}");
            }
        }

        private void DeleteFile(string file)
        {
            try
            {
                File.Delete(file);
                CommonExtensions.Logger(LogTypes.Info,$"Deleted => '{file}'");
            }
            catch (Exception err)
            {
                CommonExtensions.Logger(LogTypes.Info,$"Delete error {err.Message}, |=> '{file}'");
                CommonExtensions.Logger(LogTypes.Info,err.ToString());
            }
        }

        private void CopyFile(string source, string target)
        {
            try
            {
                File.Copy(source, target, true);
                CommonExtensions.Logger(LogTypes.Info,$"Copied => '{target}'");
            }
            catch (Exception err)
            {
                CommonExtensions.Logger(LogTypes.Info,$"Copy error {err.Message}, from: '{source}' |=> to: '{target}'");
                CommonExtensions.Logger(LogTypes.Info,err.ToString());
            }
        }

        private FileInfo ReturnNewFileInfo(string filePath)
        {
            //System.IO.Abstractions.FileInfoBase;
            return new System.IO.FileInfo(filePath);
        }

        //Calculate MD5 checksum of local file
        private string CalculateHash(string filePath)
        {
            CalculatorHash calculatedHash = new CalculatorHash(filePath);
            return calculatedHash.Calculate();
        }


    }

    public class UpdateUploader
    {
        public delegate void Info<TextEventArgs>(object sender, TextEventArgs e);
        public event Info<TextEventArgs> StatusText;

        public delegate void Uploaded<BoolEventArgs>(object sender, BoolEventArgs e);
        public event Uploaded<BoolEventArgs> StatusFinishedUploading;

        private readonly IFileSystem fileSystem;
        private List<IFileInfo> _sourceList;
        private List<IFileInfo> _targetList;
        private FilePathSourceAndTarget[] _couples;

        private bool uploadingError = false;
        private UpdateOptions _parameters { get; set; }

        public UpdateUploader(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public UpdateUploader() : this(fileSystem: new System.IO.Abstractions.FileSystem())     //use default implementation which calls System.IO
        { }

        public UpdateUploader(UpdateOptions parameters, List<IFileInfo> source, List<IFileInfo> target) : this(fileSystem: new System.IO.Abstractions.FileSystem())     //use default implementation which calls System.IO
        {            Set(parameters, source, target);        }

        public void Set(UpdateOptions parameters, List<IFileInfo> source, List<IFileInfo> target)
        {
            _parameters = parameters;
            _sourceList = source;
            _targetList = target;
        }

        public void Upload()
        {
            CommonExtensions.Logger(LogTypes.Info,"Начало отправки обновления на сервер...");
            uploadingError = false;

            Contract.Requires(_parameters != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(_parameters.localAppFolderPath));
            Contract.Requires(!string.IsNullOrWhiteSpace(_parameters.appUpdateFolderURI));
            Contract.Requires(!string.IsNullOrWhiteSpace(_parameters.pathToUpdateZip));
            Contract.Requires(!string.IsNullOrWhiteSpace(_parameters.pathToXml));
            Contract.Requires(_sourceList?.Count > 0);
            Contract.Requires(_targetList.Count == _sourceList.Count);

            _couples = MakeArrayFilePathesFromTwoListsOfFilePathes(_sourceList, _targetList);

            Task.Run(async () =>
            {
                await _couples.ForEachAsync(2, async file =>   //2 - количество одновременно отправляемых файлов на сервер
                {
                    await ClearShare(file);
                });
                await _couples.ForEachAsync(2, async file =>   //2 - количество одновременно отправляемых файлов на сервер
                {
                    await UploadFileToShare(file);
                });
            }).Wait();

            if (!uploadingError)
            { CommonExtensions.Logger(LogTypes.Info,$"Обновление на сервер доставлено -> {_parameters.serverUpdateURI}"); }
            else
            { CommonExtensions.Logger(LogTypes.Info,$"Ошибки доставлено обновления на сервер -> {_parameters.serverUpdateURI}"); }
        }

        FilePathSourceAndTarget[] MakeArrayFilePathesFromTwoListsOfFilePathes(List<IFileInfo> source, List<IFileInfo> target)
        {
            FilePathSourceAndTarget[] couples = new FilePathSourceAndTarget[source.Count];
            for (int i = 0; i < source.Count; i++)
            {
                couples[i] = new FilePathSourceAndTarget(source[i], target[i]);
            }
            return couples;
        }

        public async Task ClearShare(FilePathSourceAndTarget pathes)
        {
            var source = pathes.Get().SourcePath;
            var target = pathes.Get().TargetPath;
            Contract.Requires(source != null && !string.IsNullOrEmpty(source.FullName) && !source.Equals(target));

            try
            {
                await Task.Run(() => target.Delete());
                CommonExtensions.Logger(LogTypes.Info,$"Файл {target.FullName} удален успешно");
            }
            catch (Exception err)
            {
                CommonExtensions.Logger(LogTypes.Info,$"Файл {target.FullName} удалить не удалось: {err.ToString()}");
                uploadingError = true;
            } //@"\\server\folder\Myfile.txt"

            await Task.WhenAll();
        }

        async Task UploadFileToShare(FilePathSourceAndTarget pathes)
        {
            var source = pathes.Get().SourcePath;
            var target = pathes.Get().TargetPath;
            Contract.Requires(source != null && !string.IsNullOrEmpty(source.FullName) && !source.Equals(target));

            CommonExtensions.Logger(LogTypes.Info,$"Идет отправка файла {source.FullName} -> {target.FullName}");
            //  StatusText?.Invoke(this, new TextEventArgs($"Идет отправка файла {source.FullName} -> {target.FullName}"));

            try
            {
                await Task.Run(() => fileSystem.File.Copy(source.FullName, target.FullName, true));
                CommonExtensions.Logger(LogTypes.Info,$"Файл '{target.FullName}' на сервер доставлен успешно.");
                StatusFinishedUploading?.Invoke(this, new BoolEventArgs(true));
                StatusText?.Invoke(this, new TextEventArgs($"Файл '{target.FullName}' на сервер доставлен успешно."));
            }
            catch (Exception err)
            {
                CommonExtensions.Logger(LogTypes.Info,$"Отправка файла '{target.FullName}' на сервер выполнена с ошибками! {err.ToString()}");
                StatusFinishedUploading?.Invoke(this, new BoolEventArgs(false));
                StatusText?.Invoke(this, new TextEventArgs($"Отправка файла '{target.FullName}' на сервер выполнена с ошибками! {err.ToString()}"));
            }
        }
    }

    public class FilePathSourceAndTarget
    {
        public IFileInfo SourcePath { get; private set; }
        public IFileInfo TargetPath { get; private set; }

        public FilePathSourceAndTarget Get() { return new FilePathSourceAndTarget(SourcePath, TargetPath); }

        public FilePathSourceAndTarget(IFileInfo source, IFileInfo target)
        {
            SourcePath = source;
            TargetPath = target;
        }
    }

    [XmlRoot(ElementName = "item", IsNullable = false)]
    public class XMLDocument //Класс должен иметь модификатор public
    {
        //Класс для сериализации должен иметь стандартный конструктор без параметров.
        //поля или свойства с модификатором private, при сериализации будут игнорироваться.
        [XmlElement]
        public string version { get; set; }

        public string url { get; set; }
        public string changelogUrl { get; set; }
        public XMLElementChecksum checksum { get; set; }

        internal XmlDeclaration CreateXmlDeclaration(string v1, string v2, object p)
        {
            throw new NotImplementedException();
        }
    }

    public class XMLElementChecksum //Класс должен иметь модификатор public
    {
        //Класс для сериализации должен иметь стандартный конструктор без параметров.
        //поля или свойства с модификатором private, при сериализации будут игнорироваться.
        [XmlText]
        public string value { get; set; }

        [XmlAttribute]
        public string algorithm { get; set; }
    }

    public class CalculatorHash
    {
        private string FilName { get; set; }

        public CalculatorHash(string filename)
        {
            FilName = filename;
        }

        public string Calculate(string algorithm = "MD5") //MD5, SHA1, SHA256, SHA384, SHA512
        {
            string fileChecksum = null;
            using (var hashAlgorithm = HashAlgorithm.Create(algorithm))
            {
                using (var stream = System.IO.File.OpenRead(FilName))
                {
                    if (hashAlgorithm != null)
                    {
                        var hash = hashAlgorithm.ComputeHash(stream);
                        fileChecksum = BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
                    }

                    return fileChecksum;
                }
            }
        }
    }
}