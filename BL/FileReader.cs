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
        public AbstractConfigList config;

        public delegate void CollectionFull(object sender, BoolEventArgs e);
        public event CollectionFull EvntCollectionFull;

        public delegate void InfoMessage(object sender, TextEventArgs e);
        public event InfoMessage EvntInfoMessage;


        public async Task Read(string filePath, Encoding encoding, int maxElementsInDictionary)
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

        public async Task Read(string filePath, Encoding encoding)
        {
            await Task.Run(() => Read(filePath, encoding, _maxElementsInDictionary));
        }

        public async Task Read(string filePath)
        {
            await Task.Run(() => Read(filePath, _maxElementsInDictionary));
        }

        public async Task Read(string filePath, int maxElementsInDictionary)
        {
            await Read(filePath, _encoding, maxElementsInDictionary);
        }

        public async Task ReadConfigAsync(string filePath)
        {
            await Task.Run(() => config = ReadSerilizedConfig(filePath));
        }

        public AbstractConfigList ReadConfig(string filePath)
        {
            Task.Run(() => ReadConfigAsync(filePath)).Wait();

            return config;
        }

        private AbstractConfigList ReadSerilizedConfig(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"Try to read '{filePath}'"));
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    config = (AbstractConfigList)formatter.Deserialize(fs);
                }
            }
            catch (Exception excpt)
            {
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"{excpt.Message}"));
            }
            EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//collection is full

            return config;
        }
    }
}
