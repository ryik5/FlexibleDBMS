using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FlexibleDBMS
{
    public class FileReader : IReadable
    {
        int _maxElementsInDictionary = 1000;
        Encoding _encoding = Encoding.GetEncoding(1251);
        public int importedRows = 0;
        public  IList<string> Text { get; private set; }
        public ConfigFull<ConfigAbstract> config;

        public delegate void CollectionFull(object sender, BoolEventArgs e);
        public event CollectionFull EvntCollectionFull;

        public delegate void InfoMessage(object sender, TextEventArgs e);
        public event InfoMessage EvntInfoMessage;
        
        public async Task<IList<string>> ReadAsync(string filePath, Encoding encoding, int maxElementsInDictionary)
        {
            Text = new List<string>(maxElementsInDictionary);

            const int DefaultBufferSize = 4096;
            const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

            string currentRow;

            importedRows = 0;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            {
                using (var reader = new StreamReader(stream, encoding))
                {
                    while ((currentRow = await reader.ReadLineAsync())?.Trim()?.Length > 1)
                    {
                        Text.Add(currentRow);
                        importedRows++;

                        if (importedRows > 0 && importedRows % maxElementsInDictionary == 0)
                        {
                            EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//collection is full
                            await Task.Delay(TimeSpan.FromSeconds(0.2));

                            Text = new List<string>(maxElementsInDictionary);
                        }
                    }
                }
            }

            if (Text?.Count > 0)
            {
                EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//last part of the collection

                await Task.Delay(TimeSpan.FromSeconds(0.2));
            }
            return Text;
        }

        public async Task<IList<string>> ReadAsync(string filePath, Encoding encoding)
        {
            Text = await ReadAsync(filePath, encoding, _maxElementsInDictionary);
            return Text;
        }

        public async Task<IList<string>> ReadAsync(string filePath, int maxElementsInDictionary)
        {
         Text=   await ReadAsync(filePath, _encoding, maxElementsInDictionary);
            return Text;
        } 
        
        public async Task<IList<string>> ReadAsync(string filePath)
        {
            Text = await  ReadAsync(filePath, _maxElementsInDictionary);
            return Text;
        }


        public IList<string> Read(string filePath, Encoding encoding, int maxElementsInDictionary)
        {
            Text = new List<string>(maxElementsInDictionary);

            const int DefaultBufferSize = 4096;
            const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

            string currentRow;

            importedRows = 0;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            {
                using (var reader = new StreamReader(stream, encoding))
                {
                    while ((currentRow =  reader.ReadLine())?.Trim()?.Length > 0)
                    {
                        Text.Add(currentRow);
                        importedRows++;

                        if (importedRows > 0 && importedRows % maxElementsInDictionary == 0)
                        {
                            EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//collection is full
                            Task.Delay(TimeSpan.FromSeconds(0.2)).Wait();

                            Text = new List<string>(maxElementsInDictionary);
                        }
                    }
                }
            }

            if (Text?.Count > 0)
            {
                EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//last part of the collection

                 Task.Delay(TimeSpan.FromSeconds(0.2)).Wait();
            }
            return Text;
        }

        public IList<string> Read(string filePath, Encoding encoding)
        {
         return   Read(filePath, encoding, _maxElementsInDictionary);
        }

        public IList<string> Read(string filePath, int maxElementsInDictionary)
        {
            return Read(filePath, _encoding, maxElementsInDictionary);
        }

        public IList<string> Read(string filePath)
        {
            return Read(filePath, _maxElementsInDictionary);
        }





        public void ReadConfig(string filePath)
        {
            config = ReadSerilizedConfig(filePath);
        }

        private ConfigFull<ConfigAbstract> ReadSerilizedConfig(string filePath)
        {
            ConfigFull<ConfigAbstract> config=null;
            if (!(File.Exists(filePath)))
                return config;

            FileInfo fi = new FileInfo(filePath);
            if (fi.Length==0)
                return config;


            BinaryFormatter formatter = new BinaryFormatter();
            string message = string.Empty;
            try
            {
                message+=$"Try to read '{filePath}':{Environment.NewLine}";
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    config = (ConfigFull<ConfigAbstract>)formatter.Deserialize(fs);
                    message += "Success!";
                }
            }
            catch (Exception excpt)
            {
                message += $"{excpt.Message}:{Environment.NewLine}{excpt.ToString()}";
            }

            EvntInfoMessage?.Invoke(this, new TextEventArgs(message));
            EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//collection is full

            return config;
        }
    }
}
