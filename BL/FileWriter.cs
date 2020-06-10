using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FlexibleDBMS
{

    public class FileWriter : IWriterable
    {

        public int importedRows = 0;
        public IList<string> list;

        public delegate void WriteFinished(object sender, BoolEventArgs e);

        public event WriteFinished EvntWriteFinished;

        public delegate void InfoMessage(object sender, TextEventArgs e);
        public event InfoMessage EvntInfoMessage;

        public async Task Write(string filePath, IList<string> content, Encoding encoding)
        {
            const int DefaultBufferSize = 4096;
            const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, DefaultBufferSize, DefaultOptions))
            {
                using (var writer = new StreamWriter(stream, encoding))
                {
                    foreach (var line in content)
                    {
                        await writer.WriteLineAsync(line);
                    }

                    await writer.FlushAsync();
                }
                //   await stream.FlushAsync();
            }

            EvntWriteFinished?.Invoke(this, new BoolEventArgs(true));//last part of the collection
        }

        public async Task Write(string filePath, string content, Encoding encoding)
        {
            const int DefaultBufferSize = 4096;
            const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, DefaultBufferSize, DefaultOptions))
            {
                using (var writer = new StreamWriter(stream, encoding))
                {
                    await writer.WriteAsync(content);

                    await writer.FlushAsync();
                }
                //  await stream.FlushAsync();
            }

            EvntWriteFinished?.Invoke(this, new BoolEventArgs(true));//last part of the collection
        }

        public Task Write(string filePath, IList<string> content)
        {
            Encoding _encoding = Encoding.GetEncoding(1251);

            return Write(filePath, content, _encoding);
        }

        public Task Write(string filePath, string content)
        {
            Encoding _encoding = Encoding.GetEncoding(1251);

            return Write(filePath, content, _encoding);
        }

        private void Delete(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"File '{filePath}' was deleted successfully"));
                }
                catch (Exception excpt)
                {
                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"File '{filePath}' wasn't deleted:{Environment.NewLine}{excpt.Message}"));
                }
            }
        }
        private void Move(string oldFileName, string newFileName)
        {
            if (File.Exists(oldFileName))
            {
                try
                {
                    File.Move(oldFileName, newFileName);
                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"File '{oldFileName}' was renamed to '{newFileName}' successfully."));
                }
                catch (Exception excpt)
                {
                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"File '{oldFileName}' wasn't renamed  to '{newFileName}':{Environment.NewLine}{excpt.Message}"));
                }
            }
        }
        private void CreateDirectory(string backupDirectory)
        {
            if (!Directory.Exists(backupDirectory))
            {
                try
                {
                    Directory.CreateDirectory(backupDirectory);
                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"Directory '{backupDirectory}' was created successfully."));
                }
                catch (Exception excpt)
                {
                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"Directory '{backupDirectory}' wasn't created:{Environment.NewLine}{excpt.Message}"));
                }
            }
        }

        public void Write(string filePath, ConfigFull<ConfigAbstract> config)
        {
            string newFileName = Path.Combine(CommonConst.LocalBackupFolder, $"{DateTime.Now.ToString("yyyy-MM-dd HHmmss")} {Path.GetFileName(filePath)}.bak");

            CreateDirectory(CommonConst.LocalBackupFolder);

            Move(filePath, newFileName);

            EvntInfoMessage?.Invoke(this, new TextEventArgs($"Try to write '{nameof(ConfigFull<ConfigAbstract>)}' in file '{filePath}'"));
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, config);

                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"{nameof(ConfigFull<ConfigAbstract>)} was written."));
                }
                catch (Exception err)
                {
                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"Writing error:{Environment.NewLine}{err.ToString()}"));
                }
            }
        }
    }
}
