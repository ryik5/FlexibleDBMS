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
        static readonly string localAppFolderPath = Application.StartupPath; //Environment.CurrentDirectory
        static readonly System.Diagnostics.FileVersionInfo appFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
        static readonly string appRegistryKey = @"SOFTWARE\YuriRyabchenko\AutoAnalyse";
        readonly Bitmap bmpLogo;
        static NotifyIcon notifyIcon;
        static ContextMenu contextMenu;
        static readonly Byte[] byteLogo;

        //  string pathToQueryToCreateMainDb = System.IO.Path.Combine(localAppFolderPath, appDbPath); //System.IO.Path.GetFileNameWithoutExtension(appFilePath)

        static string appDbPath = "MainDB.db";
        static string sqLiteConnectionString = $"Data Source = {appDbPath}; Version=3;";
        System.IO.FileInfo dbFileInfo = new FileInfo(appDbPath);
        SQLiteDBOperations dBOperations;

        const int MAX_ELEMENTS_COLLECTION = 100000;
        FileReader<CarAndOwner> reader;

        DataGridView dgv;


        public Form1()
        {
            InitializeComponent();

            CheckInputParametersEnvironment();

            //Main Application
            bmpLogo = Properties.Resources.LogoRYIK;
            Text = appFileVersionInfo.Comments;
            Icon = Icon.FromHandle(bmpLogo.GetHicon());
            //Context Menu for notification
            contextMenu = new ContextMenu();  //Context Menu on notify Icon
            contextMenu.MenuItems.Add("About", ApplicationAbout);
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add("Restart", ApplicationRestart);
            contextMenu.MenuItems.Add("Exit", ApplicationExit);
            //Notification
            notifyIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Visible = true,
                BalloonTipText = "Developed by " + appFileVersionInfo.LegalCopyright,
                Text = appFileVersionInfo.ProductName + "\nv." + appFileVersionInfo.FileVersion + "\n" + appFileVersionInfo.CompanyName,
                ContextMenu = contextMenu
            };
            notifyIcon.ShowBalloonTip(500);

            administratorMenu.Text = "Administrator";
            createLocalDBMenuItem.Click += CreateLocalDBMenuItem_Click;
            importFromTextFileMenuItem.Click += ImportFromTextFileMenuItem_Click;
            importFromTextFileMenuItem.ToolTipText = "Import Text File in local DB";
            writeModelsListMenuItem.ToolTipText = "Write List with Models in DB";

            analysisDataMenu.Text = "Analysis";
            analysisDataMenu.Enabled = false;
            analysisDataMenu.Click += AnalysisDataMenu_Click;
            loadDataMenuItem.Click += LoadDataMenuItem_Click;
            schemeLocalDBMenuItem.Click += SchemaLocalDBMenuItem_Click;
            getFIOMenuItem.Text = "Все ФИО в DB";
            getFIOMenuItem.Click += GetFIOMenuItem_Click;
            getEnterpriseMenuItem.Text = "Все предприятия in DB";
            getEnterpriseMenuItem.Click += GetEnterpriseMenuItem_Click;

            dataMenu.Text = "Data";
            changeViewPanelviewMenuItem.Text = "Show as table";
            changeViewPanelviewMenuItem.Click += ChangeViewPanelviewMenuItem_Click;

            txtbQuery.KeyPress += TxtbQuery_KeyPress;

            StatusLabel1.Text = "";
            btnImage.Image = bmpLogo;
            StatusLabellInfoDB.Text = "Данные в таблице CarAndOwner";


            //prepare sql connection at local SQLite DB
            dBOperations = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);
        }

        private async void GetEnterpriseMenuItem_Click(object sender, EventArgs e)
        {
            string query = "select distinct name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou is not '0' group by edrpou order by amount desc";
            await ShowDataInDataGridView(query);
        }

        private async void GetFIOMenuItem_Click(object sender, EventArgs e)
        {
            string query = "select distinct f,i,o,drfo,count(f) from CarAndOwner group by f,i,o order by drfo";
            await ShowDataInDataGridView(query);
        }

        private void SchemaLocalDBMenuItem_Click(object sender, EventArgs e)
        {
            txtbResultShow.Visible = false;
            ChangeViewPanelview();

            DbSchema schemaDB = DbSchema.LoadDB(dbFileInfo.FullName);
            txtbResultShow.AppendText("--------------------------------\r\n");
            txtbResultShow.AppendText($"-  Scheme of local DB: '{dbFileInfo.FullName}':\r\n\r\n");

            txtbResultShow.AppendText($"-=  tables: {schemaDB.Tables.Count}  =-\r\n");

            foreach (var table in schemaDB.Tables)
            {
                txtbResultShow.AppendText($"-=     table: '{table.Value.TableName}    =-'\r\ncolumns:\r\n"); ;
                txtbResultShow.AppendText($"-=  columns: {table.Value.Columns.Count}  =-\r\n");
                foreach (var column in table.Value.Columns)
                {
                    txtbResultShow.AppendText($"'{column.ColumnName} '\t type: '{column.ColumnType}'\r\n");
                }
            }

            txtbResultShow.AppendText($"\r\n-  End of Scheme of local DB: '{dbFileInfo.FullName}':\r\n");
            txtbResultShow.AppendText("------------------------\r\n");
        }

        private async void TxtbQuery_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)//если нажата Enter
            {
                txtbResultShow.AppendText("--------------------------------\r\n");

                string query = (sender as TextBox).Text.Trim();

                txtbResultShow.AppendText("Query:\r\n" + query + "\r\n");
                string[] arrQuery = query.Split(' ');

                if (
                    query.ToLower().StartsWith("select ") && arrQuery.Length > 3
                    && arrQuery.Where(w => w.Contains("select")).Count() > 0
                    && arrQuery.Where(w => w.Contains("from")).Count() > 0
                    )
                {
                    DialogResult doQuery =
                        MessageBox.Show($"Выполнить ваш запрос?\r\n{query}", "Проверьте свой запрос", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    if (doQuery == DialogResult.OK)
                    {
                        txtbResultShow.AppendText("Done!\r\n");

                        txtbResultShow.Visible = true;
                        ChangeViewPanelview();

                        await ShowDataInDataGridView(query);
                    }
                    else
                    {
                        txtbResultShow.AppendText("Отмена задания.\r\n");
                    }
                }
                else
                {
                    MessageBox.Show("Разрешено использование только выборок без модификации БД!\r\nПроверьте свой запрос на правльность!",
                        "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void ApplicationRestart(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void ApplicationAbout(object sender, EventArgs e)
        {

        }

        private void ApplicationExit(object sender, EventArgs e)
        {
            Text = @"Closing application...";

            dgv = null;

            bmpLogo?.Dispose();

            notifyIcon?.Dispose();
            contextMenu?.Dispose();

            System.Threading.Thread.Sleep(500);

            Application.Exit();

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


            await ShowDataInDataGridView(query);
        }

        private async Task ShowDataInDataGridView(string query)
        {
            txtbResultShow.AppendText("\r\nЗапрос:\r\n" + query);
            bool analysisEnables = analysisDataMenu.Enabled;

            if (analysisEnables)
            { analysisDataMenu.Enabled = false; }
            dataMenu.Enabled = false;
            txtbResultShow.Enabled = false;

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
            try
            {
                await Task.Run(() => dt = dBOperations.GetTable(query));

                dgv.DataSource = dt;
                panel1.Controls.Add(dgv);
                dgv.Update();
                dgv.Refresh();
                dgv.Show();

                txtbResultShow.AppendText($"Количество записей в базе: {dt.Rows.Count}\r\n");
             //   foreach(var r in dt.Rows)
             //   {
             //   }
                StatusLabel1.Text = $"Количество записей: {dt.Rows.Count}";
            }
            catch (SQLiteException dbsql)
            {
                txtbResultShow.AppendText("\r\nОшибка в запросе:\r\n-----\r\n" + dbsql.Message + "\r\n-----\r\n" + dbsql.ToString() + "\r\n");
                StatusLabel1.Text = "Ошибка в запросе!";
                ChangeViewPanelview();
            }
            catch (OutOfMemoryException e)
            {
                txtbResultShow.AppendText("\r\nВаш запрос очень общий и тяжелый для БД. Кокретизируйте запрашиваемые поля или уменьшите выборку:\r\n-----\r\n" + e.Message + "\r\n-----\r\n" + e.ToString() + "\r\n-----\r\n");
                StatusLabel1.Text = "Ошибка в запросе!";
                ChangeViewPanelview();
            }
            catch (Exception e)
            {
                txtbResultShow.AppendText("\r\nОбщая ошибка:\r\n-----\r\n" + e.ToString() + "\r\n-----\r\n");
                StatusLabel1.Text = "Ошибка в запросе!";
                ChangeViewPanelview();
            }

            if (analysisEnables)
            { analysisDataMenu.Enabled = true; }
            dataMenu.Enabled = true;
            txtbResultShow.Enabled = true;
        }

        private void ChangeViewPanelviewMenuItem_Click(object sender, EventArgs e)
        {
            ChangeViewPanelview();
        }

        private void ChangeViewPanelview()
        {
            if (!txtbResultShow.Visible)
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
                txtbResultShow.Show();
                changeViewPanelviewMenuItem.Text = "Show as table";
            }
            else
            {
                txtbResultShow.Hide();
                analysisDataMenu.Enabled = true;
                changeViewPanelviewMenuItem.Text = "Show as text";
                StatusLabel1.Text = $"доступны пункты меню Загрузки и Анализа данных";
            }
        }

        private void AnalysisDataMenu_Click(object sender, EventArgs e)
        { }

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

            sqLiteConnectionString = $"Data Source = {appDbPath}; Version=3;";
        }

        private async void ImportFromTextFileMenuItem_Click(object sender, EventArgs e)
        {
            administratorMenu.Enabled = false;
            analysisDataMenu.Enabled = false;
            txtbResultShow.Enabled = false;

            StatusLabel1.Text = "Reading and importing data from text file...";
            await Task.Run(() => ImportData());
            StatusLabel1.Text = "Finished!!!";

            administratorMenu.Enabled = true;
            analysisDataMenu.Enabled = true;
            txtbResultShow.Enabled = true;
        }

        public void ImportData()
        {
            reader = new FileReader<CarAndOwner>();
            txtbResultShow.Clear();

            reader.EvntCollectionFull += Reader_collectionFull;
            reader.GetContent("11.txt", MAX_ELEMENTS_COLLECTION);
            reader.EvntCollectionFull -= Reader_collectionFull;

            txtbResultShow.AppendText("\r\n");
            txtbResultShow.AppendText("CarAndOwner:\r\n");
            txtbResultShow.AppendText("Total imported Rows: " + reader.importedRows);

            reader = null;
            txtbResultShow.AppendText("\r\n");
            txtbResultShow.AppendText("\r\n");
        }

        private void Reader_collectionFull(object sender, BoolEventArgs e)
        {
            if (e.Status)
            {
                IList<CarAndOwner> list = reader.listModels.ToList();
                int readRows = reader.importedRows;

                dBOperations.WriteListInDB(list);

                StatusLabel1.Text = $"Количество записей: {readRows}";

                txtbResultShow.AppendText($"First Element{1}: plate: {list.ElementAt(0).Plate} factory: {list.ElementAt(0).Factory}, model: {list.ElementAt(0).Model}\r\n");
                txtbResultShow.AppendText($"Last Element{list.Count - 1}: plate: {list.ElementAt(list.Count - 1).Plate} factory: {list.ElementAt(list.Count - 1).Factory}, model: {list.ElementAt(list.Count - 1).Model}\r\n");
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
