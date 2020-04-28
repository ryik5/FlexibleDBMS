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
        public IList<string> Text;
        public ConfigFullNew<AbstractConfig> config;

        public delegate void CollectionFull(object sender, BoolEventArgs e);
        public event CollectionFull EvntCollectionFull;

        public delegate void InfoMessage(object sender, TextEventArgs e);
        public event InfoMessage EvntInfoMessage;


        public async Task ReadAsync(string filePath, Encoding encoding, int maxElementsInDictionary)
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
        }

        public async Task ReadAsync(string filePath, Encoding encoding)
        {
            await Task.Run(() => ReadAsync(filePath, encoding, _maxElementsInDictionary));
        }

        public async Task ReadAsync(string filePath)
        {
            await Task.Run(() => ReadAsync(filePath, _maxElementsInDictionary));
        }

        public async Task ReadAsync(string filePath, int maxElementsInDictionary)
        {
            await ReadAsync(filePath, _encoding, maxElementsInDictionary);
        }

        public void ReadConfig(string filePath)
        {
            config = ReadSerilizedConfig(filePath);
        }

        public async Task ReadConfigAsync(string filePath)
        {
            await Task.Run(()=> ReadConfig(filePath));
        }

        private ConfigFullNew<AbstractConfig> ReadSerilizedConfig(string filePath)
        {
            ConfigFullNew<AbstractConfig> config=null;
            BinaryFormatter formatter = new BinaryFormatter();
            string message = string.Empty;
            try
            {
                message+=$"Try to read '{filePath}':{Environment.NewLine}";
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    config = (ConfigFullNew<AbstractConfig>)formatter.Deserialize(fs);
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
