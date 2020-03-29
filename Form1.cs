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
            btnImportTextFile.Click += btnImportTextFile_Click;
            btnImportTextFile.Text = "Import Text File";
        }

        private void btnImportTextFile_Click(object sender, EventArgs e)
        {
            btnImportTextFile.Enabled = false;
            ImportData();
            btnImportTextFile.Enabled = true;
        }

        FileReader<CarAndOwner> reader;
        public async void ImportData()
        {
            reader = new FileReader<CarAndOwner>();

            // reader.collection.CollectionChanged += ReaderCollection_CollectionChanged;
            reader.EvntCollectionFull += Reader_collectionFull;

            await reader.GetContent("11.txt", MAX_ELEMENTS_COLLECTION);

            textBox1.AppendText("CarAndOwner:\r\n");
            textBox1.AppendText("importedRows: " + reader.importedRows + "\r\n");
            textBox1.AppendText("\r\n");

            textBox1.AppendText("\r\n");
            textBox1.AppendText("Finished!!!" + "\r\n");
            reader.EvntCollectionFull -= Reader_collectionFull;
            reader = null;

        }

        private void Reader_collectionFull(object sender, BoolEventArgs e)
        {
            if (e.Status)
            {
                IList<CarAndOwner> list = reader.listModels.ToList();
                int readRows = reader.importedRows;
                textBox1.AppendText($"readRows: {readRows} ");
                textBox1.AppendText($"First Element{1}: plate: {list.ElementAt(0).Plate} factory: {list.ElementAt(0).Factory}, model: {list.ElementAt(0).Model}\r\n");

                textBox1.AppendText($"Last Element{list.Count - 1}: plate: {list.ElementAt(list.Count-1).Plate} factory: {list.ElementAt(list.Count - 1).Factory}, model: {list.ElementAt(list.Count - 1).Model}\r\n");
            }
        }
    }


    public interface IAbstractModel
    {
         int ID { get; set; }
         string Name { get; set; }
    }

}
