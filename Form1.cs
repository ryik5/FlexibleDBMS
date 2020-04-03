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

namespace AutoAnalysis
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
        static FileInfo dbFileInfo = new FileInfo(appDbPath);
        SQLiteDBOperations dBOperations = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);
        DbSchema schemaDB = null;
        IList<string> tablesDB;


        FileReader<CarAndOwner> reader;
        const int MAX_ELEMENTS_COLLECTION = 100000;

        DataGridView dgv;
        ToolTip tooltip = new ToolTip();

        static RegistryManager regOperator;
        readonly string regSubKeyMenu = "Menu";


        public Form1()
        {
            InitializeComponent();

            //Check Up Inputed Environment parameters
            CheckCommandLineArguments();

            //Turn Up Application
            TurnUpAplication();

            //Turn Up Menu
            TurnUpToolStripMenuItems();

            //Check Up Local DB schema
            CheckUpSelectedDB(dbFileInfo.FullName);

            //Turn Up StatusStrip
            TurnUpStatusStripMenuItems();

            //show TextBox Log as main view 
            ShowLogViewTextbox(true);
        }

        /// <summary>
        /// StatusStrip
        /// </summary>
        private void TurnUpStatusStripMenuItems()
        {
            StatusInfoMain.Text = "";
            SplitImage1.Image = bmpLogo;
            StatusInfoFilter.Text = "Фильтры";
            SplitImage2.Image = bmpLogo;
            StatusApp.Text = $"{appFileVersionInfo.ProductName} ver.{appFileVersionInfo.ProductVersion}  ";
        }

        private void TurnUpAplication()
        {
            //Main Application
            bmpLogo = Properties.Resources.LogoRYIK;
            Text = appFileVersionInfo.Comments + " " + appFileVersionInfo.LegalCopyright;
            Icon = Icon.FromHandle(bmpLogo.GetHicon());
            FormClosing += Form1_FormClosing;

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
            tooltip.SetToolTip(txtbBodyQuery, "Составьте (напишите) SQL-запрос к базе данных и нажмите ENTER");
            txtbBodyQuery.LostFocus += SetToolTipFromTextBox;
            txtbNameQuery.LostFocus += SetToolTipFromTextBox;

            //init DataGridView
            dgv = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                AllowUserToOrderColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedHeaders,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };
            panel1.Controls.Add(dgv);

            //registration manager RegestryWork
            regOperator = new RegistryManager(appRegistryKey);
            (regOperator as RegistryManager).EvntStatusInfo += AddLineAtTextboxLog;

        }

        private void TurnUpToolStripMenuItems()
        {
            administratorMenu.Text = "Administrator";
            createLocalDBMenuItem.Click += CreateLocalDBMenuItem_Click;

            selectNewDBMenuItem.Enabled = true;
            selectNewDBMenuItem.Click += SelectNewDBMenuItem_Click;

            importFromTextFileMenuItem.Click += ImportFromTextFileMenuItem_Click;
            importFromTextFileMenuItem.ToolTipText = "Import Text File in local DB";
            writeModelsListMenuItem.ToolTipText = "Write List with Models in DB";
            //view menu
            viewMenu.Text = "Вид";
            viewMenu.ToolTipText = "Отобразить данные";
            changeViewPanelviewMenuItem.Text = "Табличный";
            changeViewPanelviewMenuItem.Click += ChangeViewPanelviewMenuItem_Click;

            updateFiltersMenuItem.Text = "Обновить фильтры";
            updateFiltersMenuItem.ToolTipText= "Процедура обновления фильтров длительная по времени(до 30 минут) и затратная по ресурсам!";
            updateFiltersMenuItem.Click += UpdateFiltersMenuItem_Click;

            allColumnsInTableQueryMenuItem.Text = "Все столбцы таблицы как запрос";
            allColumnsInTableQueryMenuItem.Click += allColumnsInTableQueryMenuItem_Click;

            //query menu
            queryMenu.Text = "Запросы";
            queryMenu.ToolTipText = "Сохраненные запросы к БД";
            queryMenu.DropDownOpened += EnableAddQueryMenuItem_queryMenu_DropDownOpened;
            //add new query extra menu item
            addQueryExtraMenuItem.Text = "Добавить запрос в меню";
            addQueryExtraMenuItem.ToolTipText = "Составленный запрос запомнить и добавить его в Пользовательское меню";
            addQueryExtraMenuItem.Click += AddQueryExtraMenuItem_Click;
            //query standart menu
            queriesStandartMenu.Text = "Стандартные";
            queriesStandartMenu.ToolTipText = "Предустановленный набор запросов";
            //query standart menu items
            schemeLocalDBMenuItem.Click += GetSchemaLocalDBMenuItem_Click;

            loadDataMenuItem.Text = "Данные по крупным владельцам автопарков, limit 100";
            loadDataMenuItem.Tag = "select distinct a.District,a.City,a.ManufactureYear,a.name, a.edrpou, a.factory, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select District,City,ManufactureYear,name,edrpou,amount from " +
                "(select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by b.amount, a.edrpou, a.factory, a.model limit 100";
            loadDataMenuItem.Click += QueryMenuItem_Click;

            loadData1ToolStripMenuItem.Text = "Данные по крупным владельцам автопарков, limit 400";
            loadData1ToolStripMenuItem.Tag = "select distinct a.District,a.City,a.ManufactureYear,a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select District,City,ManufactureYear,name,edrpou,amount from " +
                "(select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by b.amount, a.edrpou, a.factory, a.model limit 400";
            loadData1ToolStripMenuItem.Click += QueryMenuItem_Click;

            loadData2ToolStripMenuItem.Text = "Данные по крупным владельцам автопарков, limit 1000";
            loadData2ToolStripMenuItem.Tag = "select distinct a.District,a.City,a.ManufactureYear,a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select District,City,ManufactureYear,name,edrpou,amount from " +
                "(select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou is not '0' group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by b.amount, a.edrpou, a.factory, a.model limit 1000";
            loadData2ToolStripMenuItem.Click += QueryMenuItem_Click;

            getFIOMenuItem.Text = "Физлица в БД";
            getFIOMenuItem.Tag = "select distinct District,City,ManufactureYear,f,i,o,drfo,count(f) as amount " +
                "from CarAndOwner " +
                "group by f,i,o order by amount desc " +
                "limit 200";
            getFIOMenuItem.Click += QueryMenuItem_Click;

            getEnterpriseMenuItem.Text = "Предприятия в БД, limit 100";
            getEnterpriseMenuItem.Tag = "select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount " +
                "from CarAndOwner where edrpou > 0 " +
                "group by edrpou order by amount desc " +
                "limit 100";
            getEnterpriseMenuItem.Click += QueryMenuItem_Click;

            //query extra menu 
            queriesExtraMenu.Text = "Пользовательские";
            queriesExtraMenu.ToolTipText = "Запросы созданные на данном ПК";
            // queriesExtraMenu.DropDown.Closing += (o, e) => { e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked; };//не закрывать меню при отметке

            removeQueryExtraMenuItem.Text = "Удалить отмеченные пользовательские запросы";
            removeQueryExtraMenuItem.ToolTipText = "Отметить можно только запросы созданные на данном ПК (подменю 'Пользовательские')";
            removeQueryExtraMenuItem.Click += RemoveCheckedInQueryExtraMenuItem_Click;

            //add additional Query Extra Menu items from Registry
            AddQueriesFromRegistryToToolStripMenu(regSubKeyMenu);

        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void UpdateFiltersMenuItem_Click(object sender, EventArgs e)
        {
            string[] column = { "Factory", "Model", "ManufactureYear", "Name", "City", "District", "Street" };
            string q = $"SELECT distinct Plate, Factory , Model, ManufactureYear, BodyNumber, ChassisNumber, EngineVolume, Type, DRFO, F, I, O, Birthday, EDRPOU, Name, City, District, Street, Building, BuildingBody, Apartment, CodeOperation, CodeDate FROM CarAndOwner";

        }

        private IList<string> GetList(string column)
        {
            //string  = "Factory";

            string q = $"SELECT distinct Plate,{column} , Model, ManufactureYear, BodyNumber, ChassisNumber, EngineVolume, Type, DRFO, F, I, O, Birthday, EDRPOU, Name, City, District, Street, Building, BuildingBody, Apartment, CodeOperation, CodeDate FROM CarAndOwner";

            IList<string> result = dBOperations.GetList(q);

            return result;
        }


        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void SelectNewDBMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                filePath = ofd.OpenFileDialogReturnPath();
                if (filePath?.Length > 0)
                {
                    CheckUpSelectedDB(filePath);
                }
                else
                {
                    ShowLogViewTextbox(true);
                    AddLineAtTextboxLog($"Не выбрана база данных.");
                }
            }
        }

        private void CheckUpSelectedDB(string filePath)
        {
            try
            {
                schemaDB = DbSchema.LoadDB(filePath);
                tablesDB = new List<string>();
                string tableName = null;

                foreach (var table in schemaDB.Tables)
                {
                    tableName += $" '{table.Value.TableName}'";
                    tablesDB.Add(table.Value.TableName);
                }
                StatusLabelExtraInfo.Text = $"Данные в таблице(ах) {tableName}";

                appDbPath = filePath;
                sqLiteConnectionString = $"Data Source = {appDbPath}; Version=3;";
                dbFileInfo = new FileInfo(appDbPath);
                dBOperations = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);
            }
            catch (Exception e)
            {
                StatusLabelExtraInfo.Text = $"Ошибка в БД: {e.Message}";
            }
            finally
            {
                if (schemaDB?.Tables?.Count == 0)
                {
                    viewMenu.Enabled = false;
                    txtbResultShow.Clear();
                    AddLineAtTextboxLog();
                    AddLineAtTextboxLog("Подключенная база данных пустая или же в ней отсутствуют какие-либо таблицы с данными!");
                    AddLineAtTextboxLog("Предварительно создайте базу данных, таблицы и импортируйте/добавьте в них данные...");
                    AddLineAtTextboxLog("Заблокирован функционал по получению данных из таблиц...");
                    txtbNameQuery.Enabled = false;
                    txtbBodyQuery.Enabled = false;
                    txtbResultShow.Enabled = false;
                }
                schemaDB = null;
            }
        }


        private void AddLineAtTextboxLog(object sender, TextEventArgs text)
        { AddLineAtTextboxLog(text.Message); }
        private void AddLineAtTextboxLog(string text)
        { txtbResultShow.AppendLine($"{text}"); }
        private void AddLineAtTextboxLog()
        { txtbResultShow.AppendLine(); }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            await Task.Delay(500);

            (regOperator as RegistryManager).EvntStatusInfo -= AddLineAtTextboxLog;
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
                StatusInfoMain.Text = $"В пользовательское меню добавлено {menuItems.Count} запросов";

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
                StatusInfoMain.Text = result;

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

            StatusInfoMain.Text = $"Запрос '{nameQuery}' в меню добавлен сохранен";
        }

        private async void QueryMenuItem_Click(object sender, EventArgs e)
        {
            string queryMenu = (sender as ToolStripMenuItem).Name.ToString();
            string queryName = (sender as ToolStripMenuItem).Text.ToString();
            string queryBody = (sender as ToolStripMenuItem).Tag.ToString();
            txtbNameQuery.Text = queryName;
            txtbBodyQuery.Text = queryBody;
            tooltip.SetToolTip(menuStrip, $"Выполняется запрос {queryName}");

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
            AddLineAtTextboxLog($"Запрос:\r\n{query}");
            StatusInfoMain.Text = "Выполняется отбор данных...";

            queriesStandartMenu.Enabled = false;
            queriesExtraMenu.Enabled = false;
            viewMenu.Enabled = false;
            txtbResultShow.Enabled = false;
            dgv.Enabled = false;

            DataTable dt = new DataTable();
            try
            {
                await Task.Run(() => dt = dBOperations.GetTable(query));

                dgv.DataSource = dt;

                AddLineAtTextboxLog($"Количество записей в базе: {dt.Rows.Count}");

                StatusInfoMain.Text = $"Количество записей: {dt.Rows.Count}";
                txtbBodyQuery.Text = query;
                ShowLogViewTextbox(false);
            }
            catch (SQLiteException dbsql)
            {
                AddLineAtTextboxLog($"\r\nОшибка в запросе:\r\n-----\r\n{dbsql.Message}\r\n-----\r\n{dbsql.ToString()}\r\n");
                StatusInfoMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(true);
            }
            catch (OutOfMemoryException e)
            {
                AddLineAtTextboxLog($"\r\nВаш запрос очень общий и тяжелый для БД. Кокретизируйте запрашиваемые поля или уменьшите выборку:\r\n-----\r\n{e.Message}\r\n-----\r\n{e.ToString()}\r\n-----\r\n");
                StatusInfoMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(true);
            }
            catch (Exception e)
            {
                AddLineAtTextboxLog($"\r\nОбщая ошибка:\r\n-----\r\n{e.ToString()}\r\n-----\r\n");
                StatusInfoMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(true);
            }

            queriesStandartMenu.Enabled = true;
            queriesExtraMenu.Enabled = true;
            viewMenu.Enabled = true;
            txtbResultShow.Enabled = true;
            dgv.Enabled = true;
        }



        private void GetSchemaLocalDBMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(true);

            // TODO: => make class GetSchema as Ilist<table> where table is Ilist<column>

            schemaDB = DbSchema.LoadDB(dbFileInfo.FullName);
            tablesDB = new List<string>();

            AddLineAtTextboxLog(Properties.Resources.DashedSymbols);
            AddLineAtTextboxLog($"-  Scheme of local DB: '{dbFileInfo.FullName}':");
            AddLineAtTextboxLog();
            AddLineAtTextboxLog();
            AddLineAtTextboxLog($"-=  tables: {schemaDB.Tables.Count}  =-");

            foreach (var table in schemaDB.Tables)
            {
                tablesDB.Add(table.Value.TableName);

                AddLineAtTextboxLog($"-=     table: '{table.Value.TableName}    =-'\r\ncolumns:");
                AddLineAtTextboxLog($"-=  columns: {table.Value.Columns.Count}  =-");
                foreach (var column in table.Value.Columns)
                {
                    AddLineAtTextboxLog($"'{column.ColumnName} '\t type: '{column.ColumnType}'");
                }
            }

            schemaDB = null;
            AddLineAtTextboxLog();
            AddLineAtTextboxLog($"-  End of Scheme of local DB: '{dbFileInfo.FullName}':");
            AddLineAtTextboxLog(Properties.Resources.DashedSymbols);
        }


        private void allColumnsInTableQueryMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(true);

            schemaDB = DbSchema.LoadDB(dbFileInfo.FullName);
            tablesDB = new List<string>();

            AddLineAtTextboxLog(Properties.Resources.DashedSymbols);
            AddLineAtTextboxLog($"- Selected DB: '{dbFileInfo.FullName}'");

            foreach (var table in schemaDB.Tables)
            {
                string columns = string.Empty;

                tablesDB.Add(table.Value.TableName);

                foreach (var column in table.Value.Columns)
                {
                    columns += $"{column.ColumnName}, ";
                }
                AddLineAtTextboxLog();
                AddLineAtTextboxLog($" --- The Table: '{table.Value.TableName}' ---");
                AddLineAtTextboxLog($" ---  ---");
                string query = $"SELECT {columns.TrimEnd().TrimEnd(',')} FROM {table.Value.TableName} LIMIT 3";
                AddLineAtTextboxLog(query);
                txtbBodyQuery.Text = query;
                AddLineAtTextboxLog($" ---  ---");
            }

            AddLineAtTextboxLog($"-  The End  -:");
            AddLineAtTextboxLog(Properties.Resources.DashedSymbols);
            AddLineAtTextboxLog();
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
                AddLineAtTextboxLog("--------------------------------");

                string query = (sender as TextBox).Text.ToLower().Trim();

                AddLineAtTextboxLog($"Query:\r\n{query}");
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
                        AddLineAtTextboxLog("Done!");

                        await GetData(query);
                    }
                    else
                    {
                        ShowLogViewTextbox(true);
                        AddLineAtTextboxLog("Отмена задания.");
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

            StatusInfoMain.Text = "Reading and importing data from text file...";
            await Task.Run(() => ImportData());
            StatusInfoMain.Text = "Finished!!!";

            administratorMenu.Enabled = true;
            queriesStandartMenu.Enabled = true;
            queriesExtraMenu.Enabled = true;
            txtbResultShow.Enabled = true;
        }

        public void ImportData()
        {
            string filepathLoadedData;
            reader = new FileReader<CarAndOwner>();
            txtbResultShow.Clear();


            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                filepathLoadedData = ofd.OpenFileDialogReturnPath();
                if (filepathLoadedData?.Length > 0)
                {
                    reader.EvntCollectionFull += Reader_collectionFull;
                    reader.GetContent("11.txt", MAX_ELEMENTS_COLLECTION);
                    reader.EvntCollectionFull -= Reader_collectionFull;

                    AddLineAtTextboxLog("");
                    AddLineAtTextboxLog("CarAndOwner:");
                    AddLineAtTextboxLog($"Total imported Rows: {reader.importedRows}");

                    reader = null;
                    AddLineAtTextboxLog("");
                }
                else { AddLineAtTextboxLog($"Файл '{filepathLoadedData}' пустой или не выбран"); }
            }
        }

        private void Reader_collectionFull(object sender, BoolEventArgs e)
        {
            if (e.Status)
            {
                IList<CarAndOwner> list = reader.listModels.ToList();
                int readRows = reader.importedRows;

                dBOperations.WriteListInDB(list);

                StatusInfoMain.Text = $"Количество записей: {readRows}";

                AddLineAtTextboxLog($"First Element{1}: plate: {list.ElementAt(0).Plate} factory: {list.ElementAt(0).Factory}, model: {list.ElementAt(0).Model}");
                AddLineAtTextboxLog($"Last Element{list.Count - 1}: plate: {list.ElementAt(list.Count - 1).Plate} factory: {list.ElementAt(list.Count - 1).Factory}, model: {list.ElementAt(list.Count - 1).Model}");
            }
        }



        /// <summary>
        /// show Import Text File Button: -y  
        /// </summary>
        public void CheckCommandLineArguments()
        {
            //Get args
            string[] args = Environment.GetCommandLineArgs();

            CommandLineArguments arguments = new CommandLineArguments();
            arguments.EvntInfoMessage += AddLineAtTextboxLog;
            arguments.CheckCommandLineArguments(args);

            if (args?.Length > 1)
            {
                //remove delimiters
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

            arguments.EvntInfoMessage -= AddLineAtTextboxLog;
        }

        string ToString(string[] array)
        {
            string text = string.Empty;

            foreach (var s in array)
            {
                text += $"{s.ToString()}\r\n";
            }
            return text;
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
                StatusInfoMain.Text = "доступны пункты меню Загрузки и Анализа данных";
            }
        }

        private void Stat()
        {
            //Menu for StatusStrip/Filters
            statusFilters.Items.AddRange(new ToolStripItem[] { btnFilter1 });
            // 
            // btnFilter1
            // 
            btnFilter1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnFilter1.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem3 });
            btnFilter1.Image = Properties.Resources.LogoRYIK;
            btnFilter1.ImageTransparentColor = Color.Magenta;
            btnFilter1.Name = "btnFilter1";
            btnFilter1.Size = new Size(73, 20);
            btnFilter1.Text = "btnFilter1";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(180, 22);
            toolStripMenuItem3.Text = "toolStripMenuItem3";
            toolStripMenuItem3.Click += ToolStripMenuItem3_Click;
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            btnFilter2.Text = "Filter 3";
        }

    }
}
