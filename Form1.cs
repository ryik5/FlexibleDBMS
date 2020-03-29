using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoAnalyse
{
    public partial class Form1 : Form
    {
        const int MAX_ELEMENTS_COLLECTION = 100000;

        public Form1()
        {
            InitializeComponent();
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ImportData();
        }

        FileReader<CarAndOwner> reader;
        public void ImportData()
        {
            reader = new FileReader<CarAndOwner>();

            // reader.collection.CollectionChanged += ReaderCollection_CollectionChanged;
            reader.collectionFull += Reader_collectionFull;

            reader.GetContent("11.txt", MAX_ELEMENTS_COLLECTION);

            textBox1.AppendText("CarAndOwner:\r\n");
            textBox1.AppendText("reader.collection.Count: " + MAX_ELEMENTS_COLLECTION * reader.listModels.Count + "\r\n");
            textBox1.AppendText("numberLine: " + reader.counterLines + "\r\n");
            textBox1.AppendText("\r\n");

            textBox1.AppendText("\r\n");
            textBox1.AppendText("Finished!!!" + "\r\n");
            reader.collectionFull -= Reader_collectionFull;
            reader = null;

        }

        private void Reader_collectionFull(object sender, BoolEventArgs e)
        {
            if (e.Status)
            {
                IList<CarAndOwner> list = reader.listModels.ToList();

                textBox1.AppendText($"listModels.ElementAt(0).Plate: {list.ElementAt(0).Plate} factory: {list.ElementAt(0).Factory}, model: {list.ElementAt(0).Model}, всего элементов {list.Count} \r\n");
            }
        }

        //private void ReaderCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    //CarAndOwner car = e.NewItems[0] as CarAndOwner;
        //    //MessageBox.Show(car.Model);
        //    ObservableCollection<CarAndOwner> collection = sender as ObservableCollection<CarAndOwner>;
        //    if (collection?.Count == MAX_ELEMENTS_COLLECTION)
        //        switch (e.Action)
        //        {
        //            // Следующая строка сработает если в коллекцию был добавлен элемент.
        //            case NotifyCollectionChangedAction.Add:
        //                CarAndOwner newDrink = e.NewItems[0] as CarAndOwner;
        //                textBox1.AppendText($"Добавлен новый объект: {newDrink.Name}, всего элементов {collection.Count}" + "\r\n");//
        //                break;
        //            // Следующая строка если элемент был удален из коллекции.
        //            case NotifyCollectionChangedAction.Remove:
        //                CarAndOwner oldDrink = e.OldItems[0] as CarAndOwner;
        //                textBox1.AppendText($"Удален объект: {oldDrink.Name}, всего элементов {collection.Count}" + "\r\n");//, всего элементов {collection.Count}
        //                break;
        //            // Следующая строка сработает если элемент был перемещен.
        //            case NotifyCollectionChangedAction.Replace:
        //                CarAndOwner replacedDrink = e.OldItems[0] as CarAndOwner;
        //                CarAndOwner replacingDrink = e.NewItems[0] as CarAndOwner;
        //                textBox1.AppendText($"Обьект {replacedDrink.Name} был заменен объектом {replacingDrink.Name}, всего элементов {collection.Count}" + "\r\n");//, всего элементов {collection.Count}
        //                break;
        //            case NotifyCollectionChangedAction.Reset:
        //                int index = e.NewStartingIndex;
        //                textBox1.AppendText($"index {index} \r\n, всего элементов {collection.Count}");//, всего элементов {collection.Count}
        //                break;
        //        }
        //}
   
    }


    public interface IRow
    {
        int ID { get; set; }
        string Name { get; set; }
    }

    public class FileReader<T> where T : IRow
    {
        private static readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private const int DefaultBufferSize = 4096;
        private const FileOptions DefaultOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
        
        public int counterLines = 0;
        string nameColumns = null;
        string currentLine;
        T model;
        
        public IList<T> listModels;
        public delegate void CollectionFull(object sender, BoolEventArgs e);
        public event CollectionFull collectionFull;

        public void GetContent(string filePath, Encoding encoding, int maxElementsInDictionary)
        {
            listModels = new List<T>(maxElementsInDictionary);

            counterLines = 0;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultBufferSize, DefaultOptions))
            {
                using (var reader = new StreamReader(stream, encoding))
                {
                    while ((currentLine = reader.ReadLine())?.Trim()?.Length > 10)
                    {
                        if (nameColumns == null)
                        {
                            counterLines = 0;
                            nameColumns = currentLine;
                        } //first found not_empty_line containes name columns
                        else
                        {
                            model = GetModel<T>.ToModel(currentLine, nameColumns);

                            if (model != null && model.Name.Length > 0)
                            {
                                counterLines++;
                                listModels.Add(model);

                                if (counterLines == maxElementsInDictionary)
                                {
                                    collectionFull?.Invoke(this, new BoolEventArgs(true));//collection is full

                                    Task.Delay(10).Wait(10);

                                    counterLines = 0;
                                    listModels = new List<T>(maxElementsInDictionary);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetContent(string filePath, int maxElementsInDictionary)
        {
            GetContent(filePath, _encoding, maxElementsInDictionary);
        }
    }

}
