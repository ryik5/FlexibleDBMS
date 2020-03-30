using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
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
        static string localAppFolderPath = Application.StartupPath; //Environment.CurrentDirectory
        readonly string appRegistryKey = @"SOFTWARE\YuriRyabchenko\AutoAnalyse";
        static string appDbPath = "MainDB.db";
        string pathToQueryToCreateMainDb = System.IO.Path.Combine(localAppFolderPath, appDbPath); //System.IO.Path.GetFileNameWithoutExtension(appFilePath)

        string sqLiteConnectionString = $"Data Source = {appDbPath}; Version=3;";
        System.IO.FileInfo dbFileInfo = new FileInfo(appDbPath);
        SQLiteDBOperations dBOperations;

        FileReader<CarAndOwner> reader;

        DataGridView dgv;


        public Form1()
        {
            InitializeComponent();

            CheckInputParametersEnvironment();

            administratorMenu.Text = "Administrator";
            createLocalDBMenuItem.Click += CreateLocalDBMenuItem_Click;
            importFromTextFileMenuItem.Click += ImportFromTextFileMenuItem_Click;
            importFromTextFileMenuItem.ToolTipText = "Import Text File in local DB";
            writeModelsListMenuItem.ToolTipText = "Write List with Models in DB";

            analysisDataMenu.Text = "Analysis";
            analysisDataMenu.Enabled = false;
            analysisDataMenu.Click += AnalysisDataMenu_Click;
            loadDataMenuItem.Click += LoadDataMenuItem_Click;

            dataMenu.Text = "Data";
            changeViewPanelviewMenuItem.Text = "Show as table";
            changeViewPanelviewMenuItem.Click += ChangeViewPanelviewMenuItem_Click;

            StatusLabel1.Text = "";

            //prepare sql connection at local SQLite DB
            dBOperations = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);
        }

        private void CreateLocalDBMenuItem_Click(object sender, EventArgs e)
        {
            dBOperations.TryMakeLocalDB();
        }

        private async void LoadDataMenuItem_Click(object sender, EventArgs e)
        {
            string query = "select edrpou, count(plate)  as number from" +
                            " (select distinct edrpou, plate" +
                            " from CarAndOwner" +
                            " where edrpou is not '0'" +
                            " group by edrpou, plate)" +
                            " group by edrpou";

            analysisDataMenu.Enabled = false;
            dataMenu.Enabled = false;

            await ShowDataInDataGridView(query);

            analysisDataMenu.Enabled = true;
            dataMenu.Enabled = true;
        }

        private async Task ShowDataInDataGridView(string query)
        {
            StatusLabel1.Text = "Загрузка данных в таблицу...";
            if (dgv != null)
            {
                dgv?.Hide();
                panel1.Controls.Remove(dgv);
                dgv.DataSource = null;
                dgv?.Dispose();
                dgv = null;
            }

            dgv = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                AllowUserToOrderColumns = true,
                AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedHeaders,
                ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };

            DataTable dt = new DataTable();
            await Task.Run(() => dt = dBOperations.GetTable(query));

            dgv.DataSource = dt;
            panel1.Controls.Add(dgv);
            dgv.Update();
            dgv.Refresh();
            dgv.Show();

            StatusLabel1.Text = $"Количество записей: {dt.Rows.Count}";
        }

        private void ChangeViewPanelviewMenuItem_Click(object sender, EventArgs e)
        {
            ChangeViewPanelview();
        }

        private void ChangeViewPanelview()
        {
            if (!textBox1.Visible)
            {
                if (dgv != null)
                {
                    dgv?.Hide();
                    panel1.Controls.Remove(dgv);
                    dgv.DataSource = null;
                    dgv?.Dispose();
                    dgv = null;
                }

                analysisDataMenu.Enabled = false;
                textBox1.Show();
                changeViewPanelviewMenuItem.Text = "Show as table";
            }
            else
            {
                textBox1.Hide();
                analysisDataMenu.Enabled = true;
                changeViewPanelviewMenuItem.Text = "Show as text";
                StatusLabel1.Text = $"доступны пункты меню Загрузки и Анализа данных";
            }
        }

        private void AnalysisDataMenu_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// show Import Text File Button: -y  
        /// </summary>
        public void CheckInputParametersEnvironment()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args?.Length > 1)
            {
                char[] delimeterChar = { '-', '/' };
                string envParameter = args[1]?.Trim()?.TrimStart('-', '/')?.ToLower();
                if (envParameter.StartsWith("y"))
                {
                    administratorMenu.Enabled = true;
                }
                else if (envParameter.StartsWith("config"))
                {
                    char[] missingChar = { '\\', '/', ':', ';', '|', ' ' };
                    appDbPath = envParameter.Trim(missingChar).Replace("config", "");
                }
                else if (envParameter.StartsWith("n"))
                {
                    administratorMenu.Enabled = false;
                }
            }
            else
            {
                administratorMenu.Enabled = false;
            }
        }

        private async void ImportFromTextFileMenuItem_Click(object sender, EventArgs e)
        {
            administratorMenu.Enabled = false;
            analysisDataMenu.Enabled = false;
            textBox1.Enabled = false;

            await Task.Run(() => ImportData());

            administratorMenu.Enabled = true;
            analysisDataMenu.Enabled = true;
            textBox1.Enabled = true;
        }

        public void ImportData()
        {
            reader = new FileReader<CarAndOwner>();
            textBox1.AppendText("\r\n");
            textBox1.AppendText("Reading and importing data from text file:\r\n");
            textBox1.AppendText("\r\n");

            // reader.collection.CollectionChanged += ReaderCollection_CollectionChanged;
            reader.EvntCollectionFull += Reader_collectionFull;

            reader.GetContent("11.txt", MAX_ELEMENTS_COLLECTION);

            textBox1.AppendText("\r\n");
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

                dBOperations.WriteListInDB(list);

                StatusLabel1.Text = $"Количество записей: {readRows}";

                textBox1.AppendText($"First Element{1}: plate: {list.ElementAt(0).Plate} factory: {list.ElementAt(0).Factory}, model: {list.ElementAt(0).Model}\r\n");
                textBox1.AppendText($"Last Element{list.Count - 1}: plate: {list.ElementAt(list.Count - 1).Plate} factory: {list.ElementAt(list.Count - 1).Factory}, model: {list.ElementAt(list.Count - 1).Model}\r\n");
            }
        }




        private void LoadRegistry()
        {
            //using (Microsoft.Win32.RegistryKey EvUserKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(appRegistryKey, Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey))
            //{
            //    try { sServer1Registry = EvUserKey?.GetValue("SKDServer")?.ToString(); }
            //    catch { logger.Trace("Can't get value of SCA server's name from Registry"); }
            //}
        }

        private void SaveRegistry()
        {
            // Save data in Registry
            //try
            //{
            //    using (Microsoft.Win32.RegistryKey EvUserKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(appRegistryKey))
            //    {
            //        try { EvUserKey.SetValue("SKDServer", sServer1, Microsoft.Win32.RegistryValueKind.String); } catch { }

            //        logger.Info("CreateSubKey: Данные в реестре сохранены");
            //    }
            //}
            //catch (Exception err) { logger.Error("CreateSubKey: Ошибки с доступом на запись в реестр. Данные сохранены не корректно. " + err.ToString()); }
        }
    }
}
