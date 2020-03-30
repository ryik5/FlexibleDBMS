using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAnalyse
{
    public class FileReader<T> where T : IModel
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

        public void GetContent(string filePath, Encoding encoding, int maxElementsInDictionary)
        {
            listModels = new List<T>(maxElementsInDictionary);

            importedRows = 0;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            {
                using (var reader = new StreamReader(stream, encoding))
                {
                    while ((currentRow =  reader.ReadLine())?.Trim()?.Length > 10)
                    {
                        if (nameColumns == null)
                        {
                            importedRows = 0;
                            nameColumns = currentRow;
                        } //first found not_empty_line containes name columns
                        else
                        {
                            model = GetModel<T>.ToModel(currentRow, nameColumns);

                            if (model != null )
                            {
                                importedRows++;
                                listModels.Add(model);

                                if (importedRows > 0 && importedRows % maxElementsInDictionary == 0)
                                {
                                    EvntCollectionFull?.Invoke(this, new BoolEventArgs(true));//collection is full

                                    Task.Delay(100);

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

                Task.Delay(10);
            }
        }

        public void GetContent(string filePath, int maxElementsInDictionary)
        {
            GetContent(filePath, _encoding, maxElementsInDictionary);
        }
    }

}
