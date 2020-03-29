using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAnalyse
{
    public class FileReader<T> where T : IAbstractModel
    {
        private static readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private const int DefaultBufferSize = 4096;
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;

        public int importedRows = 0;
        string nameColumns = null;
        string currentRow;
        T model;

        public IList<T> listModels;
        public delegate void CollectionFull(object sender, BoolEventArgs e);
        public event CollectionFull EvntCollectionFull;

        public async Task GetContent(string filePath, Encoding encoding, int maxElementsInDictionary)
        {
            listModels = new List<T>(maxElementsInDictionary);

            importedRows = 0;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            {
                using (var reader = new StreamReader(stream, encoding))
                {
                    while ((currentRow = await reader.ReadLineAsync())?.Trim()?.Length > 10)
                    {
                        if (nameColumns == null)
                        {
                            importedRows = 0;
                            nameColumns = currentRow;
                        } //first found not_empty_line containes name columns
                        else
                        {
                            model = GetModel<T>.ToModel(currentRow, nameColumns);

                            if (model != null && model?.ID > 0)
                            {
                                importedRows++;
                                listModels.Add(model);

                                if (importedRows > 0 && importedRows % maxElementsInDictionary == 0)
                                {
                                    EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//collection is full

                                    await Task.Delay(10).ConfigureAwait(true);

                                    listModels = new List<T>(maxElementsInDictionary);
                                }
                            }
                        }
                    }
                }
            }

            if (listModels?.Count > 0)
            {
                EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//last part of the collection

                await Task.Delay(10).ConfigureAwait(true);
            }

        }

        public async Task GetContent(string filePath, int maxElementsInDictionary)
        {
            await GetContent(filePath, _encoding, maxElementsInDictionary);
        }
    }

}
