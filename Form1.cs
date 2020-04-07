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
        //Application Modes
        AppModes OperatingModes = AppModes.User;

        //Application's Main interface Turn Up
        static readonly string localAppFolderPath = Application.StartupPath; //Environment.CurrentDirectory
        static readonly System.Diagnostics.FileVersionInfo appFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
        static readonly string appRegistryKey = @"SOFTWARE\YuriRyabchenko\AutoAnalyse";
        Bitmap bmpLogo;
        static NotifyIcon notifyIcon;
        static ContextMenu contextMenu;
        ToolTip tooltip = new ToolTip();

        //DB
        static string appDbPath = "MainDB.db";
        static string sqLiteConnectionString = $"Data Source = {appDbPath}; Version=3;";
        static FileInfo dbFileInfo = new FileInfo(appDbPath);
        SQLiteDBOperations dBOperations = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);
        IModelDBable<ModelDBTable> db;
        DbSchema schemaDB = null;
        IList<string> tablesDB;

        //SHow datatables in DataGridView
        DataGridView dgv;
        //datatable source
        DataTable dt;

        //import a txt file in DB
        FileReader<CarAndOwner> reader;
        const int MAX_ELEMENTS_COLLECTION = 100000;

        //Registry
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

            //add additional Query Extra Menu items from Registry
            AddQueriesFromRegistryToToolStripMenu(regSubKeyMenu);

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

            if (OperatingModes == AppModes.Admin)
            { (regOperator as RegistryManager).EvntStatusInfo += AddLineAtTextboxLog; }
            else if (OperatingModes == AppModes.User)
            { try { (regOperator as RegistryManager).EvntStatusInfo -= AddLineAtTextboxLog; } catch { } }
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
            updateFiltersMenuItem.ToolTipText = "Процедура обновления фильтров длительная по времени(до 30 минут) и затратная по ресурсам!";
            updateFiltersMenuItem.Click += UpdateFiltersMenuItem_Click;

            allColumnsInTableQueryMenuItem.Text = "Все столбцы таблицы как запрос";
            allColumnsInTableQueryMenuItem.Click += allColumnsInTableQueryMenuItem_Click;

            exportMenuItem.Text = "Експорт в Excel";
            exportMenuItem.Click += ExportMenuItem_Click;

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

            loadData0MenuItem.Text = "Данные по крупным владельцам автопарков, limit 100";
            loadData0MenuItem.Tag = "select distinct a.District,a.City,a.ManufactureYear,a.name, a.edrpou, a.factory, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select District,City,ManufactureYear,name,edrpou,amount from " +
                "(select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by b.amount, a.edrpou, a.factory, a.model limit 100";
            loadData0MenuItem.Click += QueryMenuItem_Click;

            loadData1MenuItem.Text = "Данные по крупным владельцам автопарков, limit 400";

            // "select a.city,a.name as 'наименование', a.edrpou, a.factory as 'марка',a.manufactureyear as 'год', count(*) as 'количество'  from carandowner a where edrpou > 1 group by a.city,a.name,a.factory limit 400"
            loadData1MenuItem.Tag = "select distinct a.District,a.City,a.ManufactureYear,a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select District,City,ManufactureYear,name,edrpou,amount from " +
                "(select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by b.amount, a.edrpou, a.factory, a.model limit 400";
            loadData1MenuItem.Click += QueryMenuItem_Click;

            loadData2MenuItem.Text = "Данные по крупным владельцам автопарков, limit 1000";
            loadData2MenuItem.Tag = "select distinct a.District,a.City,a.ManufactureYear,a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select District,City,ManufactureYear,name,edrpou,amount from " +
                "(select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou is not '0' group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by b.amount, a.edrpou, a.factory, a.model limit 1000";
            loadData2MenuItem.Click += QueryMenuItem_Click;
            
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
        }

        private async  void ExportMenuItem_Click(object sender, EventArgs e)
        {
            StatusInfoMain.Text = $"Идет генерация отчетов...";
            dgv.Enabled = false;
            viewMenu.Enabled = false;
            queryMenu.Enabled = false;
            txtbBodyQuery.Enabled = false;

            await WriteDataTableInTableExcel(dt);

            txtbBodyQuery.Enabled = true;
            queryMenu.Enabled = true;
            viewMenu.Enabled = true;
            dgv.Enabled = true;
            StatusInfoMain.Text = $"Готово!";
        }

        /*
Вывод количества машин(задвоения по госномерам удалены):
Select a.city as 'Город',a.name as 'наименование', a.edrpou as 'ЕДРПОУ', a.factory as 'марка',a.manufactureyear as 'год', count(*)as 'Количество' from  (Select distinct a.city,a.name, a.edrpou, a.factory, a.manufactureyear,a.plate  from carandowner a where edrpou > 1 group by a.city,a.name,a.factory,a.plate) a group by a.city,a.name, a.edrpou, a.factory,a.manufactureyear


Вывод госномеров:
Select distinct a.city,a.name, a.edrpou, a.factory, a.manufactureyear,a.plate  from carandowner a where edrpou > 1 group by a.city,a.name,a.factory,a.plate
             */

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateFiltersMenuItem_Click(object sender, EventArgs e)
        {
            StatusInfoMain.Text="Построение фильтров...";
            dgv.Enabled = false;
            txtbResultShow.Enabled = false;
            viewMenu.Enabled = false;
            queryMenu.Enabled = false;
            txtbBodyQuery.Enabled = false;
            
            await BuildFilters();
            await Task.Run(()=> BuildFiltersMenues(filtersTable));

            txtbBodyQuery.Enabled = true;
            txtbResultShow.Enabled = true;
            queryMenu.Enabled = true;
            viewMenu.Enabled = true;
            dgv.Enabled = true;
            StatusInfoMain.Text = "Построение фильтров завершено.";
        }


        IModelDBable<ModelDBColumn> filtersTable = null;
        IDictionary<string, string> dicTranslator=null;

        private async Task BuildFilters()
        {
            AddLineAtTextboxLog("Чтение базы и формирование библиотеки уникальных слов.");
            string[] columns = { "District", "City", "Factory", "ManufactureYear", "Type"};
            //Подписи колонок в меню
            dicTranslator = new Dictionary<string, string>
            {
                { "District", "Область" },
                { "City", "Город" },
                { "Factory", "Марка" },
                { "ManufactureYear", "Год" },
                { "Type", "ЧЛ/Предприятие" }
            };
            
            string phrase = string.Join(", ", dicTranslator.Values.ToArray());
            AddLineAtTextboxLog("Фильтры строятся на основе данных колонок:");
            AddLineAtTextboxLog(phrase);
            AddLineAtTextboxLog();

            await Task.Run(() => filtersTable = dBOperations.GetFilterList(columns, "CarAndOwner"));

            AddLineAtTextboxLog("Построение фильтров завершено");
        }


        private void BuildFiltersMenues(IModelDBable<ModelDBColumn> filtersTable)
        {
            MenuFiltersMaker menuMaker;
            ToolStripSplitButton filterSplitButton;
            ToolStripMenuItem subFilterMenu;
            //  Task.Run(() =>
            //              {
            foreach (var column in filtersTable.Collection.Take(5))
            {
                menuMaker = new MenuFiltersMaker(dicTranslator);
                filterSplitButton = menuMaker.MakeFiltersMenu(column.Name);
                statusFilters.Items.Add(filterSplitButton);

                foreach (var f in column.Collection.Take(40))
                {
                    subFilterMenu = menuMaker.MakeFiltersSubMenu(f.Name);
                 //   subFilterMenu.Click += SubFilterMenu_Click;
                    filterSplitButton.DropDownItems.Add(subFilterMenu);
                }
            }            

            //        });
        }

        private void SubFilterMenu_Click(object sender, EventArgs e)
        {
            string text = (sender as ToolStripMenuItem).Text;
            string tag = (sender as ToolStripMenuItem).Tag.ToString();
            (sender as ToolStripMenuItem).GetCurrentParent().Text = text;
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

            //print db structure     IModelDBable<ModelDBTable> db ;
            string result = string.Empty;
            if (db.Collection?.Count > 0)
            {
                foreach (var t in db.Collection)
                {
                    result += "table: " + t.Name + "\r\n";
                    if (t.Collection?.Count > 0)
                    {
                        foreach (var c in t.Collection)
                        {
                            result += "column: " + c.Name + ", тип: " + c.Type + "\r\n";
                            if (c.Collection?.Count > 0)
                            {
                                foreach (var f in c.Collection)
                                {
                                    result += "filter: " + f.Name + "\r\n";
                                }
                            }
                        }
                    }
                }
            }

            AddLineAtTextboxLog($"{result}");
        }
        

        private void CheckUpSelectedDB(string filePath)
        {
            IModelDBable<ModelDBColumn> table;
            IModelDBable<ModelDBFilter> column;

            db = new ModelDB();
            db.Name = "currentDB";
            db.Collection = new List<ModelDBTable>();
            (db as ModelDB).FilePath = filePath;
            (db as ModelDB).SqlConnectionString = $"Data Source = {filePath}; Version=3;";

            try
            {
                schemaDB = DbSchema.LoadDB(filePath);
                tablesDB = new List<string>();
                string tableName = null;

                foreach (var tbl in schemaDB.Tables)
                {
                    table = new ModelDBTable();
                    table.Name = tbl.Value.TableName;
                    table.Collection = new List<ModelDBColumn>();

                    foreach (var clmn in tbl.Value.Columns)
                    {
                        column = new ModelDBColumn();
                        column.Name = clmn.ColumnName;
                        (column as ModelDBColumn).Type = clmn.ColumnType.ToString();
                        table.Collection.Add((ModelDBColumn)column);
                    }

                    db.Collection.Add((ModelDBTable)table);

                    //old
                    tableName += $" '{tbl.Value.TableName}'";
                    tablesDB.Add(tbl.Value.TableName);
                }
                StatusLabelExtraInfo.Text = $"Данные в таблице(ах) {tableName}";

                try { dBOperations.EvntInfoMessage -= AddLineAtTextboxLog; } catch { }

                appDbPath = filePath;
                sqLiteConnectionString = $"Data Source = {appDbPath}; Version=3;";
                dbFileInfo = new FileInfo(appDbPath);
                dBOperations = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);

                if (OperatingModes == AppModes.Admin)
                { dBOperations.EvntInfoMessage += AddLineAtTextboxLog; }
            }
            catch (Exception e)
            {
                StatusLabelExtraInfo.Text = $"Ошибка в БД: {e.Message}";
                AddLineAtTextboxLog($"Ошибка в БД: {e.Message}");
                AddLineAtTextboxLog($"{e.ToString()}");
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

            if (OperatingModes == AppModes.Admin)
            { (regOperator as RegistryManager).EvntStatusInfo -= AddLineAtTextboxLog; }
        }
        

        private async Task UpdateQueryExtraMenuInRegistry()
        {
            //clear registry from menu entries
            await Task.Run(() => regOperator.DeleteSubKeyTreeQueryExtraItems(regSubKeyMenu));

            await Task.Delay(500);

            await Task.Run(() =>
            {
                IDictionary<string, string> menuItems = queriesExtraMenu.ToDictionary(30);
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

            ToolStripMenuItem item = menuItem.ToExtentendedMenuToolStripMenuItem();

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

            dt?.Dispose();
            dgv?.Dispose();
            tooltip?.Dispose();

            bmpLogo?.Dispose();

            notifyIcon?.Dispose();
            contextMenu?.Dispose();

            System.Threading.Thread.Sleep(500);

            Application.Exit();
        }


        private async Task WriteDataTableInTableExcel(DataTable source)
        {
            if (source != null || source?.Columns?.Count > 0 || source?.Rows?.Count > 0)
            {
                FileInfo fi = null;
                DataTable dtTemp = null;
                
                //muliplier of skipping millions
                int muliplier = (int)Math.Ceiling((decimal)source.Rows.Count / (decimal)1000000);

                for (int part = 0; part < muliplier; part++)
                {
                    dtTemp = source.Clone();
                    source.AsEnumerable()
                        .Skip(part * 1000000)
                        .Take(1000000)
                        .CopyToDataTable(dtTemp, LoadOption.OverwriteChanges);
                    fi = new FileInfo($"{appFileVersionInfo.ProductName}_{part}_export.xlsx");

                    AddLineAtTextboxLog($"{fi.FullName}");

                    await  ExportToFile(fi, dtTemp, appFileVersionInfo.FileVersion);

                    dtTemp.Dispose();
                    fi = null;
                }
            }
            else
            {
                AddLineAtTextboxLog("Сначала отберите данные из БД");
            }

            ShowLogViewTextbox(true);
        }

        private async Task ExportToFile(FileInfo fi, DataTable dtTemp, string nameSheet)
        {
            try
            {
                 await Task.Run(() =>
                dtTemp
                 .ExportToExcel($"{fi.FullName}", nameSheet, TypeOfPivot.NonePivot, null, null, true));

                AddLineAtTextboxLog($"Таблица данных экспортирована в файл: '{fi.FullName}' упешно.");
            }
            catch (Exception err)
            {
                AddLineAtTextboxLog(nameSheet);
                AddLineAtTextboxLog(err.ToString());
            }
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

            
             dt = new DataTable();
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

                string query = (sender as TextBox).Text.Trim();

                AddLineAtTextboxLog($"Query:\r\n{query}");
                string[] arrQuery = query.Split(' ');

                if (
                    arrQuery[0].ToLower() == "select" && arrQuery.Length > 3
                    && arrQuery.Where(w => w.ToLower().Contains("select")).Count() > 0
                    && arrQuery.Where(w => w.ToLower().Contains("from")).Count() > 0
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
            string filePath;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                filePath = ofd.OpenFileDialogReturnPath();
                if (filePath?.Length > 0)
                {
                    await Task.Run(() => ImportData(filePath));
                    StatusInfoMain.Text = "Finished!!!";

                    administratorMenu.Enabled = true;
                    queriesStandartMenu.Enabled = true;
                    queriesExtraMenu.Enabled = true;
                    txtbResultShow.Enabled = true;
                }
                else { AddLineAtTextboxLog($"Файл '{filePath}' пустой или не выбран"); }
            }
        }

        public void ImportData(string filePath)
        {
            reader = new FileReader<CarAndOwner>();
            txtbResultShow.Clear();

            reader.EvntCollectionFull += Reader_collectionFull;
            reader.GetContent(filePath, MAX_ELEMENTS_COLLECTION);
            reader.EvntCollectionFull -= Reader_collectionFull;

            AddLineAtTextboxLog("");
            AddLineAtTextboxLog("CarAndOwner:");
            AddLineAtTextboxLog($"Total imported Rows: {reader.importedRows}");

            reader = null;
            AddLineAtTextboxLog("");
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
                    OperatingModes = AppModes.Admin;
                    administratorMenu.Enabled = true;
                }
                else if (envParameter.StartsWith("config"))
                {
                    appDbPath = envParameter.Trim('\\', '/', ':', ';', '|', ' ').Replace("config", "");
                }
                else if (envParameter.StartsWith("n"))
                {
                    OperatingModes = AppModes.User;
                    administratorMenu.Enabled = false;
                }
            }
            else
            {
                OperatingModes = AppModes.User;
                administratorMenu.Enabled = false;
            }

            sqLiteConnectionString = $"Data Source = {appDbPath}; Version=3;";

            arguments.EvntInfoMessage -= AddLineAtTextboxLog;
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


    }
}
