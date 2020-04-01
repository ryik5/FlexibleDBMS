using System;
using System.Collections.Generic;
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
        Bitmap bmpLogo;
        static NotifyIcon notifyIcon;
        static ContextMenu contextMenu;

        //  string pathToQueryToCreateMainDb = System.IO.Path.Combine(localAppFolderPath, appDbPath); //System.IO.Path.GetFileNameWithoutExtension(appFilePath)

        static string appDbPath = "MainDB.db";
        static string sqLiteConnectionString = $"Data Source = {appDbPath}; Version=3;";
        System.IO.FileInfo dbFileInfo = new FileInfo(appDbPath);
        SQLiteDBOperations dBOperations;

        DbSchema schemaDB = null;
        IList<string> tablesDB;

        FileReader<CarAndOwner> reader;
        const int MAX_ELEMENTS_COLLECTION = 100000;

        DataGridView dgv;
        ToolTip tooltip;

        static RegistryManager regOperator;
        readonly string regSubKeyMenu = "Menu";


        public Form1()
        {
            InitializeComponent();

            //Check Up Inputed Environment parameters
            CheckInputedParametersEnvironment();

            //TurnUp Application
            TurnUpAplication();

            //TurnUp Menu
            TurnUpMenuItems();

            //Check Local DB schema
            CheckUpLocalDB();

            //show TextBox Log as main view 
            ShowLogViewTextbox(true);
        }

        private void TurnUpAplication()
        {
            //Main Application
            bmpLogo = Properties.Resources.LogoRYIK;
            Text = appFileVersionInfo.Comments + " " + appFileVersionInfo.LegalCopyright;
            Icon = Icon.FromHandle(bmpLogo.GetHicon());
            this.FormClosing += Form1_FormClosing;
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

            //Other controls
            txtbBodyQuery.KeyPress += TxtbQuery_KeyPress;
            txtbBodyQuery.LostFocus += SetToolTipFromTextBox;
            txtbNameQuery.LostFocus += SetToolTipFromTextBox;

            StatusLabelMain.Text = "";
            StatuslabelBtnImage.Image = bmpLogo;

            //init DataGridView
            dgv = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                AllowUserToOrderColumns = true,
                AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedHeaders,
                ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };
            panel1.Controls.Add(dgv);

            regOperator = new RegistryManager(appRegistryKey);
            (regOperator as RegistryManager).EvntStatusInfo += AddTextAtTextboxLog;
        }

        private void TurnUpMenuItems()
        {
            administratorMenu.Text = "Administrator";
            createLocalDBMenuItem.Click += CreateLocalDBMenuItem_Click;
            importFromTextFileMenuItem.Click += ImportFromTextFileMenuItem_Click;
            importFromTextFileMenuItem.ToolTipText = "Import Text File in local DB";
            writeModelsListMenuItem.ToolTipText = "Write List with Models in DB";
            //view menu
            viewMenu.Text = "Вид";
            viewMenu.ToolTipText = "Отобразить данные";
            changeViewPanelviewMenuItem.Text = "Табличный";
            changeViewPanelviewMenuItem.Click += ChangeViewPanelviewMenuItem_Click;
            //query menu
            queryMenu.Text = "Запросы";
            queryMenu.ToolTipText = "Сохраненные запросы к БД";
            queryMenu.DropDownOpened += EnableAddQueryMenuItem_queryMenu_DropDownOpened;
            //add new query extra menu item
            addQueryExtraMenuItem.Text = "Добавить запрос в меню";
            addQueryExtraMenuItem.ToolTipText = "Запомнить новый запрос и добавить его в меню";
            addQueryExtraMenuItem.Enabled = true;
            addQueryExtraMenuItem.Click += AddQueryExtraMenuItem_Click;
            //query standart menu
            queriesStandartMenu.Text = "Стандартные";
            queriesStandartMenu.ToolTipText = "Исходный набор запросов";
            queriesStandartMenu.Enabled = true;
            //query standart menu items
            schemeLocalDBMenuItem.Click += GetSchemaLocalDBMenuItem_Click;

            loadDataMenuItem.Text = "Данные по крупным владельцам автопарков 1";
            loadDataMenuItem.Tag = "select distinct a.name, a.edrpou, a.factory, a.model, a.plate from CarAndOwner a " +
                "inner join (select name,edrpou,amount from " +
                "(select distinct name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                "where amount > 50) b " +
                "on a.edrpou=b.edrpou " +
                "order by b.amount, a.edrpou, a.factory, a.model limit 200";
            loadDataMenuItem.Click += QueryMenuItem_Click;

            loadData1ToolStripMenuItem.Text = "Данные по крупным владельцам автопарков 2";
            loadData1ToolStripMenuItem.Tag = "select distinct a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select name,edrpou,amount from " +
                "(select distinct name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by a.edrpou  limit 200";
            loadData1ToolStripMenuItem.Click += QueryMenuItem_Click;

            loadData2ToolStripMenuItem.Text = "Данные по крупным владельцам автопарков 3";
            loadData2ToolStripMenuItem.Tag = "select distinct a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select name,edrpou,amount from " +
                "(select distinct name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou is not '0' group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by a.edrpou  limit 200";
            loadData2ToolStripMenuItem.Click += QueryMenuItem_Click;

            getFIOMenuItem.Text = "Физлица в БД";
            getFIOMenuItem.Tag = "select distinct f,i,o,drfo,count(f) as amount " +
                "from CarAndOwner " +
                "group by f,i,o order by amount desc " +
                "limit 200";
            getFIOMenuItem.Click += QueryMenuItem_Click;

            getEnterpriseMenuItem.Text = "Предприятия в БД";
            getEnterpriseMenuItem.Tag = "select distinct name,edrpou,count(edrpou) as amount " +
                "from CarAndOwner where edrpou is not '0' " +
                "group by edrpou order by amount desc " +
                "limit 200";
            getEnterpriseMenuItem.Click += QueryMenuItem_Click;

            //query extra menu 
            queriesExtraMenu.Text = "Пользовательские";
            queriesExtraMenu.ToolTipText = "Запросы созданные на данном ПК";
            // queriesExtraMenu.DropDown.Closing += (o, e) => { e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked; };//не закрывать меню при отметке
            //query extra menu items
            removeQueryExtraMenuItem.Text = "Удалить отмеченные пользовательские запросы";
            removeQueryExtraMenuItem.ToolTipText = "Отметить можно только запросы созданные на данном ПК (подменю 'Пользовательские')";
            removeQueryExtraMenuItem.Click += RemoveCheckedInQueryExtraMenuItem_Click;

            //add additional Query Extra Menu items from Registry
            AddQueriesFromRegistryToToolStripMenu(regSubKeyMenu);
        }

        private void CheckUpLocalDB()
        {
            try
            {
                schemaDB = DbSchema.LoadDB(dbFileInfo.FullName);
                tablesDB = new List<string>();
                string tableName = null;

                foreach (var table in schemaDB.Tables)
                {
                    tableName += $" '{table.Value.TableName}'";
                    tablesDB.Add(table.Value.TableName);
                }
                StatusLabelExtraInfo.Text = $"Данные в таблице(ах) {tableName}";

                dBOperations = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);
            }
            catch (Exception e)
            {
                StatusLabelExtraInfo.Text = $"Ошибка в БД: {e.Message}";
            }
            finally
            {
                if (schemaDB.Tables.Count == 0)
                {
                    viewMenu.Enabled = false;
                    if (!administratorMenu.Enabled)
                    {
                        txtbResultShow.Text =
                            "\r\nПодключенная база данных пустая или же в ней отсутствуют какие-либо таблицы с данными!" +
                            "\r\nПредварительно создайте базу данных, таблицы и импортируйте/добавьте в них данные..." +
                            "\r\nЗаблокирован функционал по получению данных из таблиц...";
                        txtbNameQuery.Enabled = false;
                        txtbBodyQuery.Enabled = false;
                        txtbResultShow.Enabled = false;
                    }
                }
                schemaDB = null;
            }
        }


        private void AddTextAtTextboxLog(object sender, TextEventArgs text)
        { AddTextAtTextboxLog(text.Message); }

        private void AddTextAtTextboxLog(string text)
        { txtbResultShow.AppendText($"{text}\r\n"); }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            await Task.Delay(500);

            (regOperator as RegistryManager).EvntStatusInfo -= AddTextAtTextboxLog;
        }



        private async Task UpdateQueryExtraMenuInRegistry()
        {
            //clear registry from menu entries
            await Task.Run(() => regOperator.DeleteSubKeyTreeQueryExtraItems(regSubKeyMenu));

            await Task.Delay(500);

            await Task.Run(() =>
            {
                IDictionary<string, string> menuItems = queriesExtraMenu.ToDictionary(5);
                regOperator.Write(menuItems, regSubKeyMenu);
            });
        }

        private void AddQueriesFromRegistryToToolStripMenu(string subkey)
        {
            IList<ToolStripMenuItem> menuItems = regOperator.ReadRegistryKeys(subkey).ToToolStripMenuItemsList();

            if (menuItems.Count > 0)
            {
                foreach (var m in menuItems.ToArray())
                {
                    m.Click += QueryMenuItem_Click;
                    queriesExtraMenu.DropDownItems.Add(m);
                }
                StatusLabelMain.Text = $"В пользовательское меню добавлено {menuItems.Count} запросов";

                menuStrip.Update();
                menuStrip.Refresh();
            }
        }


        private async void RemoveCheckedInQueryExtraMenuItem_Click(object sender, EventArgs e)
        {
            IList<ToolStripItem> listRemove = queriesExtraMenu.ToToolStripItemsList();

            if (listRemove.Count > 0)
            {
                string result = $"Запрос(ы)";

                foreach (ToolStripMenuItem m in listRemove)
                {
                    if (m.Checked)
                    {
                        result += $" '{m.Text}'";
                        queriesExtraMenu.DropDownItems.Remove(m);
                        m?.Dispose();
                    }
                }
                result += $" из меню удален(ы)";
                StatusLabelMain.Text = result;

                menuStrip.Update();
                menuStrip.Refresh();
            }
            listRemove.Clear();

            await Task.Run(() => UpdateQueryExtraMenuInRegistry());
        }

        private async void AddQueryExtraMenuItem_Click(object sender, EventArgs e)
        {
            string nameQuery = txtbNameQuery.Text.Trim();
            string bodyQuery = txtbBodyQuery.Text.Trim();

            MenuItem menuItem = new MenuItem(nameQuery, bodyQuery);

            ToolStripMenuItem item = menuItem.ToToolStripMenuItem();

            queriesExtraMenu.DropDownItems.Add(item as ToolStripMenuItem);
            item.Click += QueryMenuItem_Click;
            menuStrip.Update();
            menuStrip.Refresh();

            await Task.Run(() => UpdateQueryExtraMenuInRegistry());

            StatusLabelMain.Text = $"Запрос '{nameQuery}' в меню добавлен сохранен";
        }

        private async void QueryMenuItem_Click(object sender, EventArgs e)
        {
            string queryMenu = (sender as ToolStripMenuItem).Name.ToString();
            string queryName = (sender as ToolStripMenuItem).Text.ToString();
            string queryBody = (sender as ToolStripMenuItem).Tag.ToString();
            txtbNameQuery.Text = queryName;
            txtbBodyQuery.Text = queryBody;
            StatusLabelMain.ToolTipText = $"Выполняется запрос {queryName}";

            await GetData(queryBody);
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

            dgv?.Dispose();

            tooltip?.Dispose();

            bmpLogo?.Dispose();

            notifyIcon?.Dispose();
            contextMenu?.Dispose();

            System.Threading.Thread.Sleep(500);

            Application.Exit();
        }


        private async Task GetData(string query)
        {
            AddTextAtTextboxLog($"Запрос:\r\n{query}");
            StatusLabelMain.Text = "Выполняется отбор данных...";
            StatusLabelExtraInfo.ToolTipText = $"Выполняется запрос:\r\n{query}";

            queriesStandartMenu.Enabled = false;
            queriesExtraMenu.Enabled = false;
            viewMenu.Enabled = false;

            DataTable dt = new DataTable();
            try
            {
                await Task.Run(() => dt = dBOperations.GetTable(query));

                dgv.DataSource = dt;

                AddTextAtTextboxLog($"Количество записей в базе: {dt.Rows.Count}");

                StatusLabelMain.Text = $"Количество записей: {dt.Rows.Count}";
                txtbBodyQuery.Text = query;
                ShowLogViewTextbox(false);
            }
            catch (SQLiteException dbsql)
            {
                AddTextAtTextboxLog($"\r\nОшибка в запросе:\r\n-----\r\n{dbsql.Message}\r\n-----\r\n{dbsql.ToString()}\r\n");
                StatusLabelMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(true);
            }
            catch (OutOfMemoryException e)
            {
                AddTextAtTextboxLog($"\r\nВаш запрос очень общий и тяжелый для БД. Кокретизируйте запрашиваемые поля или уменьшите выборку:\r\n-----\r\n{e.Message}\r\n-----\r\n{e.ToString()}\r\n-----\r\n");
                StatusLabelMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(true);
            }
            catch (Exception e)
            {
                AddTextAtTextboxLog($"\r\nОбщая ошибка:\r\n-----\r\n{e.ToString()}\r\n-----\r\n");
                StatusLabelMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(true);
            }

            queriesStandartMenu.Enabled = true;
            queriesExtraMenu.Enabled = true;
            viewMenu.Enabled = true;
            StatusLabelExtraInfo.ToolTipText = $"Последний запрос:\r\n{query}";
        }



        private void GetSchemaLocalDBMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(true);

            // TODO: => make class GetSchema as Ilist<table> where table is Ilist<column>

            schemaDB = DbSchema.LoadDB(dbFileInfo.FullName);
            tablesDB = new List<string>();

            AddTextAtTextboxLog("--------------------------------");
            AddTextAtTextboxLog($"-  Scheme of local DB: '{dbFileInfo.FullName}':\r\n\r\n");
            AddTextAtTextboxLog($"-=  tables: {schemaDB.Tables.Count}  =-");

            foreach (var table in schemaDB.Tables)
            {
                tablesDB.Add(table.Value.TableName);

                AddTextAtTextboxLog($"-=     table: '{table.Value.TableName}    =-'\r\ncolumns:");
                AddTextAtTextboxLog($"-=  columns: {table.Value.Columns.Count}  =-");
                foreach (var column in table.Value.Columns)
                {
                    AddTextAtTextboxLog($"'{column.ColumnName} '\t type: '{column.ColumnType}'");
                }
            }

            schemaDB = null;
            AddTextAtTextboxLog($"\r\n-  End of Scheme of local DB: '{dbFileInfo.FullName}':");
            AddTextAtTextboxLog("------------------------");
        }

        private void CreateLocalDBMenuItem_Click(object sender, EventArgs e)
        {
            dBOperations.TryMakeLocalDB();
        }



        private void EnableAddQueryMenuItem_queryMenu_DropDownOpened(object sender, EventArgs e)
        {
            string nameQuery = txtbNameQuery.Text.Trim();
            string bodyQuery = txtbBodyQuery.Text.Trim();

            if (string.IsNullOrEmpty(nameQuery) || string.IsNullOrEmpty(bodyQuery))
            {
                addQueryExtraMenuItem.Enabled = false;
            }
            else if (!string.IsNullOrEmpty(nameQuery) && !string.IsNullOrEmpty(bodyQuery))
            {
                addQueryExtraMenuItem.Enabled = true;
                addQueryExtraMenuItem.ToolTipText = $"Запомнить запрос: {nameQuery}\r\n{bodyQuery}";
            }
        }

        private void SetToolTipFromTextBox(object sender, EventArgs e)
        {
            string text = (sender as TextBox).Text;
            if (!(string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)))
            {
                tooltip = new ToolTip();
                tooltip.SetToolTip((sender as TextBox), text);
            }
        }

        private async void TxtbQuery_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)//если нажата Enter
            {
                AddTextAtTextboxLog("--------------------------------");

                string query = (sender as TextBox).Text.ToLower().Trim();

                AddTextAtTextboxLog($"Query:\r\n{query}");
                string[] arrQuery = query.Split(' ');

                if (
                    arrQuery[0] == "select" && arrQuery.Length > 3
                    && arrQuery.Where(w => w.Contains("select")).Count() > 0
                    && arrQuery.Where(w => w.Contains("from")).Count() > 0
                    )
                {
                    DialogResult doQuery =
                        MessageBox.Show($"Выполнить ваш запрос?\r\n{query}", "Проверьте свой запрос", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    if (doQuery == DialogResult.OK)
                    {
                        AddTextAtTextboxLog("Done!");

                        await GetData(query);
                    }
                    else
                    {
                        ShowLogViewTextbox(true);
                        AddTextAtTextboxLog("Отмена задания.");
                    }
                }
                else
                {
                    ShowLogViewTextbox(true);
                    MessageBox.Show("Разрешено использование только выборок без модификации БД!\r\nПроверьте свой запрос на правльность!",
                        "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private async void ImportFromTextFileMenuItem_Click(object sender, EventArgs e)
        {
            administratorMenu.Enabled = false;
            queriesStandartMenu.Enabled = false;
            queriesExtraMenu.Enabled = false;
            txtbResultShow.Enabled = false;

            ShowLogViewTextbox(true);

            StatusLabelMain.Text = "Reading and importing data from text file...";
            await Task.Run(() => ImportData());
            StatusLabelMain.Text = "Finished!!!";

            administratorMenu.Enabled = true;
            queriesStandartMenu.Enabled = true;
            queriesExtraMenu.Enabled = true;
            txtbResultShow.Enabled = true;
        }

        public void ImportData()
        {
            reader = new FileReader<CarAndOwner>();
            txtbResultShow.Clear();

            reader.EvntCollectionFull += Reader_collectionFull;
            reader.GetContent("11.txt", MAX_ELEMENTS_COLLECTION);
            reader.EvntCollectionFull -= Reader_collectionFull;

            AddTextAtTextboxLog("");
            AddTextAtTextboxLog("CarAndOwner:");
            AddTextAtTextboxLog($"Total imported Rows: {reader.importedRows}");

            reader = null;
            AddTextAtTextboxLog("");
        }

        private void Reader_collectionFull(object sender, BoolEventArgs e)
        {
            if (e.Status)
            {
                IList<CarAndOwner> list = reader.listModels.ToList();
                int readRows = reader.importedRows;

                dBOperations.WriteListInDB(list);

                StatusLabelMain.Text = $"Количество записей: {readRows}";

                AddTextAtTextboxLog($"First Element{1}: plate: {list.ElementAt(0).Plate} factory: {list.ElementAt(0).Factory}, model: {list.ElementAt(0).Model}");
                AddTextAtTextboxLog($"Last Element{list.Count - 1}: plate: {list.ElementAt(list.Count - 1).Plate} factory: {list.ElementAt(list.Count - 1).Factory}, model: {list.ElementAt(list.Count - 1).Model}");
            }
        }



        /// <summary>
        /// show Import Text File Button: -y  
        /// </summary>
        public void CheckInputedParametersEnvironment()
        {
            string[] args = Environment.GetCommandLineArgs();

            if (args?.Length > 1)
            {
                string envParameter = args[1]?.Trim()?.TrimStart('-', '/')?.ToLower();
                if (envParameter.StartsWith("y"))
                {
                    administratorMenu.Enabled = true;
                }
                else if (envParameter.StartsWith("config"))
                {
                    appDbPath = envParameter.Trim('\\', '/', ':', ';', '|', ' ').Replace("config", "");
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



        private void ChangeViewPanelviewMenuItem_Click(object sender, EventArgs e)
        {
            ChangeViewPanelviewMenuItem();
        }


        bool logView;
        private void ChangeViewPanelviewMenuItem()
        {
            if (logView)
            { ShowLogViewTextbox(true); logView = false; }
            else
            { ShowLogViewTextbox(false); logView = true; }
        }

        private void ShowLogViewTextbox(bool logView)
        {
            if (logView)
            {
                dgv?.Hide();

                txtbResultShow.Show();
                changeViewPanelviewMenuItem.Text = "Табличный";
            }
            else
            {
                txtbResultShow.Hide();

                dgv?.Update();
                dgv?.Refresh();
                dgv?.Show();
                changeViewPanelviewMenuItem.Text = "Текстовый";
                StatusLabelMain.Text = "доступны пункты меню Загрузки и Анализа данных";
            }
        }
    }
}
