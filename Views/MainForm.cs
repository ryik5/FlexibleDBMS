using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlexibleDBMS
{

    public partial class Form1 : Form
    {
        //Application Modes
        AppModes OperatingModes = AppModes.User;

        //Application's Main interface Turn Up
        static readonly System.Diagnostics.FileVersionInfo appFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
        static readonly string appName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
        static readonly string appCfgFilePath = Application.StartupPath + "\\" + appName + ".cfg";
        static readonly string appRegistryKey = @"SOFTWARE\YuriRyabchenko\AutoAnalyse";
        Bitmap bmpLogo;
        static NotifyIcon notifyIcon;
        static ContextMenu contextMenu;
        ToolTip tooltip = new ToolTip();

        //DB
        SelectDBForm selectDB;
        ISQLConnectionSettings currentSQLConnection = null;

        ISqlDbConnector dBOperations;// = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);
        IModelDBable<ModelDBTable> db;
        DbSchema schemaDB = null;
        IList<string> tablesDB;

        //SHow datatables in DataGridView
        DataGridView dgv;
        //datatable source
        DataTable dtForShow;
        DataTable dtForStore;

        IModelDBable<ModelDBColumn> filtersTable = null; // меню фильтров в строке статуса
        IDictionary<string, string> dicTranslator = null; //Перверодчик англ-русс


        //import a txt file in DB
        FileReaderModels<CarAndOwner> reader;

        const int MAX_ELEMENTS_COLLECTION = 100000;


        //Registry
        static RegistryManager regOperator;
        readonly string regSubKeyMenu = "Menu";
        readonly string regSubKeyRecent = "Recent";


        //Configuration
        //const of default connection
        const string MAIN = "Main";
        const string DB_LIST = "Recent";
        const string DEFAULT_CONNECTION = "Default";

        AbstractConfigParameter currentParameterConfig = null;
        AbstractUnitConfigParameterList currentUnitConfig = null;
        AbstractConfigList currentConfig = null;

        IReadable readerConfig = null;
        /*
         Configuration:
           Unit1: A) Name -> "name of connection1"
                     ListParameters: AA) Name -> QueryExtra
                                         ListParameters: AAA) Name -> QueryExtra1
                                                              ListParameters: Name Value and Value
                                                         BBB) Name -> QueryExtra2
                                                              ListParameters: Name Value and Value
           Unit2: A) Name -> "name of connection2"
                     ListParameters: AA) Name -> QueryExtra
                                         ListParameters: AAA) Name -> QueryExtra1
                                                              ListParameters: Name Value and Value
                                                         BBB) Name -> QueryExtra2
                                                              ListParameters: Name Value and Value
             
             */

        public Form1()
        {
            InitializeComponent();
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            currentSQLConnection = new SQLConnectionSettings();

            currentParameterConfig = new ConfigParameter();
            currentUnitConfig = new ConfigUnitParameterList();
            currentUnitConfig.Add(currentParameterConfig);
            currentConfig = new ConfigList();
            currentConfig.Add(currentUnitConfig);

            regOperator = new RegistryManager(appRegistryKey);

            //Check Up Inputed Environment parameters
            CheckEnvironment();

            //Turn Up Application
            TurnUpAplication();

            //registration manager RegestryWork

            if (OperatingModes == AppModes.Admin)
            { (regOperator as RegistryManager).EvntStatusInfo += AddLineAtTextboxLog; }
            else if (OperatingModes == AppModes.User)
            { try { (regOperator as RegistryManager).EvntStatusInfo -= AddLineAtTextboxLog; } catch { } }


            //Turn Up Menu
            TurnUpToolStripMenuItems();

            ReadLastSettingsFromRegistry();

            //Turn Up StatusStrip
            TurnUpStatusStripMenuItems();

            //show TextBox Log as main view 
            ShowLogViewTextbox(MainViewMode.Textbox);


            // (currentConfig as ConfigParameter).EvntConfigChanged += CurrentConfiguration_EvntConfigChanged;

          //  (currentSQLConnection as SQLConnectionSettings).EvntConfigChanged += CurrentSQLConnection_EvntConfigChanged;

            await ReadCfgAsyncFromFile();
        }




        /// <summary>
        /// Read Settings from Registry
        /// </summary>
        private void ReadLastSettingsFromRegistry()
        {
            IList<RegistryEntity> registryEntity = regOperator.ReadRegistryKeys();
            string key;

            bool firstRun = registryEntity.Select(x => x.Key.Contains("Recent")).Count() == 0;

            foreach (var entry in registryEntity)
            {
                key = entry?.Key;
                switch (key)
                {
                    case "Recent":
                        {
                         
                            ISQLConnectionSettings connection = ReadConnectionParametersFromRegistry(entry?.Value?.ToString());

                            WriteConnectionInRegistry(connection);
                            SetCurrentSQLConfiguration(connection);
                            break;
                        }
                    default:
                        {
                            AddLineAtTextboxLog($"Найдены необработанные настройки в реестре '{key}':{Environment.NewLine}{entry.Value}");
                            break;
                        }
                }
            }

            if (firstRun)
            {
                GetNewConnection();
            }
            else
            {
                IList<ToolStripMenuItem> menuItems =
                    regOperator.ReadRegistryKeys(regSubKeyRecent)?.ToToolStripMenuItemsList(ToolStripMenuType.RecentConnection);
                AddQueriesFromRegistryToToolStripMenu(changeBaseToolStripMenuItem, menuItems, ToolStripMenuType.RecentConnection);

                if (currentSQLConnection == null)
                {
                    AddLineAtTextboxLog("SQLConnectionSettings is empty. Will do new SQLConnectionSettings");

                    currentSQLConnection = new SQLConnectionSettings
                    {
                        Name = "default",
                        Host = "local",
                        ProviderName = SQLProvider.SQLite
                    };
                }

                if (currentSQLConnection?.ProviderName == SQLProvider.SQLite) //Check Up Local DB schema
                { CheckUpSelectedSQLiteDB(currentSQLConnection.Database); }

            }
        }







        /// <summary>
        /// StatusStrip
        /// </summary>
        private void TurnUpStatusStripMenuItems()
        {
            StatusInfoMain.Text = "";

            SplitImage1.Image = bmpLogo;
            StatusInfoFilter.Text = "Фильтры";
            StatusInfoFilter.ToolTipText = "Чтоб использовать фильтры предварительно необходимо выбрать пункт меню 'Вид', а затем -'Обновить фильтры'";

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
                Icon = Icon,
                Visible = true,
                BalloonTipText = "Developed by " + appFileVersionInfo.LegalCopyright,
                Text = appFileVersionInfo.ProductName + "\nv." + appFileVersionInfo.FileVersion + "\n" + appFileVersionInfo.CompanyName,
                ContextMenu = contextMenu
            };
            notifyIcon.ShowBalloonTip(500);

            //Other controls
            txtBodyQuery.KeyPress += TxtbQuery_KeyPress;
            tooltip.SetToolTip(txtBodyQuery, "Составьте (напишите) SQL-запрос к базе данных и нажмите ENTER");
            txtBodyQuery.LostFocus += SetToolTipFromTextBox;
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
        }

        private void TurnUpToolStripMenuItems()
        {
            //Administrator
            {
                administratorMenu.Text = "Администратор";
                createLocalDBMenuItem.Click += CreateLocalDBMenuItem_Click;

                selectNewDBMenuItem.Enabled = true;
                selectNewDBMenuItem.Click += SelectNewDBMenuItem_Click;

                connectToSQLServerToolStripMenuItem.Text = "Подключить новую базу данных";
                connectToSQLServerToolStripMenuItem.Click += ConnectToSQLServerToolStripMenuItem_Click;

                importFromTextFileMenuItem.Click += ImportFromTextFileMenuItem_Click;
                importFromTextFileMenuItem.ToolTipText = "Import Text File in local DB";
                writeModelsListMenuItem.ToolTipText = "Write List with Models in DB";

                printConfigToolStripMenuItem.Click += PrintConfigToolStripMenuItem_Click;
                readFileToolStripMenuItem.Click += ReadCfgFromFileMenuItem_Click;
                writeFileToolStripMenuItem.Click += WriteFileToolStripMenuItem_Click;
            }

            //view menu
            {
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
                recoverDataTableAfterQueryMenuItem.Text = "Восстановить таблицу до применнения фильтров";
                recoverDataTableAfterQueryMenuItem.Click += RecoverDataTableAfterQueryMenuItem_Click;
            }

            //query menu
            {
                queryMenu.Text = "Найти";
                queryMenu.ToolTipText = "Сохраненные запросы";
                queryMenu.Click += EnableAddQueryMenuItem_queryMenu_DropDownOpened;

                queriesStandartMenu.Text = "Стандартные";
                queriesStandartMenu.ToolTipText = "Предустановленные поисковые запросы";
                addToStandartMenuItem.Text = "Добавить новый запрос в Стандартное меню";
                addToStandartMenuItem.ToolTipText = "Формат слова которое будет искаться - {}. Вставить в месте запроса";

                addToExtraMenuItem.Text = "Добавить новый запрос в Пользовательское меню";
                addToExtraMenuItem.ToolTipText = "Составленный запрос запомнить и добавить его в Пользовательское меню";
                addToExtraMenuItem.Click += AddExtraQueryMenuItem_Click;

                //query standart menu
                {
                    //query standart menu items
                    schemeLocalDBMenuItem.Click += GetSchemaLocalDBMenuItem_Click;

                    loadData0MenuItem.Text = "Данные по крупным владельцам автопарков, limit 100";
                    loadData0MenuItem.Tag = "select distinct a.City,a.District,a.ManufactureYear,a.name, a.edrpou, a.factory, a.model, a.plate from CarAndOwner a " +
                        "inner join " +
                        "(select District,City,ManufactureYear,name,edrpou,amount from " +
                        "(select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                        "where amount > 50) b on a.edrpou=b.edrpou " +
                        "order by b.amount, a.edrpou, a.factory, a.model limit 100";
                    loadData0MenuItem.Click += QueryMenuItem_Click;

                    loadData1MenuItem.Text = "Данные по крупным владельцам автопарков, limit 400";

                    // "select a.city,a.name as 'наименование', a.edrpou, a.factory as 'марка',a.manufactureyear as 'год', count(*) as 'количество'  from carandowner a where edrpou > 1 group by a.city,a.name,a.factory limit 400"
                    loadData1MenuItem.Tag = "select distinct a.City,a.District,a.ManufactureYear,a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                        "inner join " +
                        "(select District,City,ManufactureYear,name,edrpou,amount from " +
                        "(select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                        "where amount > 50) b on a.edrpou=b.edrpou " +
                        "order by b.amount, a.edrpou, a.factory, a.model limit 400";
                    loadData1MenuItem.Click += QueryMenuItem_Click;

                    loadData2MenuItem.Text = "Данные по крупным владельцам автопарков, limit 1000";
                    loadData2MenuItem.Tag = "select distinct a.City,a.District,a.ManufactureYear,a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                        "inner join " +
                        "(select District,City,ManufactureYear,name,edrpou,amount from " +
                        "(select distinct District,City,ManufactureYear,name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou is not '0' group by edrpou order by amount desc) " +
                        "where amount > 50) b on a.edrpou=b.edrpou " +
                        "order by b.amount, a.edrpou, a.factory, a.model limit 1000";
                    loadData2MenuItem.Click += QueryMenuItem_Click;

                    getFIOMenuItem.Text = "Физлица в БД";
                    getFIOMenuItem.Tag = "select distinct City,District,ManufactureYear,f,i,o,drfo,count(f) as amount " +
                        "from CarAndOwner " +
                        "group by f,i,o order by amount desc " +
                        "limit 200";
                    getFIOMenuItem.Click += QueryMenuItem_Click;

                    getEnterpriseMenuItem.Text = "Предприятия в БД, limit 100";
                    getEnterpriseMenuItem.Tag = "select distinct City,District,ManufactureYear,name,edrpou,count(edrpou) as amount " +
                        "from CarAndOwner where edrpou > 0 " +
                        "group by edrpou order by amount desc " +
                        "limit 100";
                    getEnterpriseMenuItem.Click += QueryMenuItem_Click;

                    //look for
                    {
                        lookForFamiliyNameToolStripMenuItem.Text = "Искать машины зарегистрированные за введенными фамилией/именем"; ;
                        lookForFamiliyNameToolStripMenuItem.Click += QueryMenuItem_Click;

                        lookForOrganizationToolStripMenuItem.Text = "Искать машины зарегистрированные за введенными названием организации";
                        lookForOrganizationToolStripMenuItem.Click += QueryMenuItem_Click;

                        lookForNumberToolStripMenuItem.Text = "Искать машины по совпадению с введенным номером";
                        lookForNumberToolStripMenuItem.Click += QueryMenuItem_Click;
                    }
                }

                //query extra menu 
                queriesExtraMenu.Text = "Пользовательские запросы";
                queriesExtraMenu.ToolTipText = "Запросы созданные на данном ПК";
                // queriesExtraMenu.DropDown.Closing += (o, e) => { e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked; };//не закрывать меню при отметке

                removeQueryExtraMenuItem.Text = "Удалить отмеченные пользовательские запросы";
                removeQueryExtraMenuItem.ToolTipText = "Отметить можно только запросы созданные на данном ПК (подменю 'Пользовательские')";
                removeQueryExtraMenuItem.Click += RemoveCheckedInQueryExtraMenuItem_Click;
            }

            //help menu
            {
                helpMenu.Text = "Помощь";
                aboutMenuItem.Text = "О программе";
                aboutMenuItem.Click += ApplicationAbout;
                useApplicationMenuItem.Text = "Порядок использования программы";
            }

            //
            changeBaseToolStripMenuItem.Text = "Сменить";
            changeBaseToolStripMenuItem.ToolTipText = "Сменить банк";
        }
        

        private void ConnectToSQLServerToolStripMenuItem_Click(object sender, EventArgs e)
        { GetNewConnection(); }

        private void GetNewConnection()
        {
            SQLConnection.Settings = new SQLConnectionSettings(currentSQLConnection);
            selectDB = new SelectDBForm();
            selectDB.Owner = this;
            selectDB.Icon = Icon.FromHandle(bmpLogo.GetHicon());
            selectDB.Text = appFileVersionInfo.Comments + " " + appFileVersionInfo.LegalCopyright;

            selectDB.Show();
            Hide();
            selectDB.FormClosing += SelectDB_FormClosing;
        }

        private void SelectDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetCurrentSQLConfiguration(selectDB.settings);
            //Write in Registry Last Connection and its parameters


            //Destroy SelectDB Form
            selectDB.Dispose();
            SQLConnection.Settings = null;


            //Show Main Form
            Show();


            //Check Data
            AddLineAtTextboxLog($"Name: {currentSQLConnection.Name}");
            AddLineAtTextboxLog($"ProviderName: {currentSQLConnection.ProviderName}");
            AddLineAtTextboxLog($"Host: {currentSQLConnection.Host}");
            AddLineAtTextboxLog($"Port: {currentSQLConnection.Port}");
            AddLineAtTextboxLog($"Username: {currentSQLConnection.Username}");
            AddLineAtTextboxLog($"Password: {currentSQLConnection.Password}");
            AddLineAtTextboxLog($"Database: {currentSQLConnection.Database}");
            AddLineAtTextboxLog($"Table: {currentSQLConnection.Table}");
        }


        private void BaseChangeMenuItems_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            ISQLConnectionSettings connection = ReadConnectionParametersFromRegistry(item.Text);

            //Write in Registry Last Connection and its parameters
            WriteConnectionInRegistry(connection);
            SetCurrentSQLConfiguration(connection);
            MakeCurrentFullConfig();
        }

        private void SetCurrentSQLConfiguration(ISQLConnectionSettings settings)
        {
            (currentSQLConnection as SQLConnectionSettings).Set(settings);

            //dBOperations renew
            switch (currentSQLConnection.ProviderName)
            {
                case SQLProvider.SQLite:
                    {
                        dBOperations = new SQLiteDBOperations(currentSQLConnection);
                        break;
                    }
                case SQLProvider.My_SQL:
                    {
                        dBOperations = new MySQLUtils(currentSQLConnection);
                        break;
                    }
                default:
                    {
                        dBOperations = null;
                        break;
                    }
            }
            
            StatusLabelExtraInfo.Text = $"Данные в таблице(ах) {currentSQLConnection.Table}";
        }

        private ISQLConnectionSettings ReadConnectionParametersFromRegistry(string text)
        {
            ISQLConnectionSettings settings = regOperator?
                                      .ReadRegistryKeys($"{regSubKeyRecent}\\{text}")?
                                          .ToSQLConnectionSettings();

            return settings;
        }

        /// <summary>
        /// /Write in Registry Last Connection and its parameters
        /// </summary>
        /// <param name="settings"></param>
        private void WriteConnectionInRegistry(ISQLConnectionSettings settings)
        {
            IDictionary<string, string> dic = settings.GetPropertyValues(50);
            //write parameters of connection
            regOperator.Write(dic, $"{regSubKeyRecent}\\{settings.Name}");
            //write last connection' name
            regOperator.Write(regSubKeyRecent, settings.Name);
            //write connection and name as pair key-value
            regOperator.Write(settings.Name, settings.Name, regSubKeyRecent);
        }



        private void EnableAddQueryMenuItem_queryMenu_DropDownOpened(object sender, EventArgs e)
        {
            string nameQuery = txtbNameQuery.Text.Trim();
            string bodyQuery = txtBodyQuery.Text.Trim();
            addToStandartMenuItem.Enabled = false;
            addToExtraMenuItem.Enabled = false;
            lookForFamiliyNameToolStripMenuItem.Enabled = false;
            lookForNumberToolStripMenuItem.Enabled = false;
            lookForOrganizationToolStripMenuItem.Enabled = false;

            if (!string.IsNullOrWhiteSpace(bodyQuery))
            {
                lookForNumberToolStripMenuItem.Enabled = true;
                lookForFamiliyNameToolStripMenuItem.Enabled = true;
                lookForOrganizationToolStripMenuItem.Enabled = true;

                lookForNumberToolStripMenuItem.Tag = "select distinct " +
                    $"City as Область,District as Район,f as Фамилия,i as Имя,o as Отчество,drfo,name,edrpou,factory as Марка,model as Модель,plate as Номер " +
                    $"from CarAndOwner " +  //UPPER
                    $"where ((length(f)+length(i)) > 0 OR (length(name)) > 0) " +
                    $"AND (UPPER(plate) like '%{bodyQuery.ToUpper()}%') " +
                    $"group by District,City,f,i,o,name " +
                    $"order by District,City,f,i,o,name asc " +
                    "limit 10000";
                lookForFamiliyNameToolStripMenuItem.Tag = "select distinct " +
                    $"City as Область,District as Район,f as Фамилия,i as Имя,o as Отчество,drfo,factory as Марка,model as Модель,plate as Номер " +
                    $"from CarAndOwner " +
                    $"where ((length(f)+length(i)) > 0 ) " +
                    $"AND (UPPER(f) like '%{bodyQuery.ToUpper()}%' OR UPPER(i) like '%{bodyQuery.ToUpper()}%') " +
                    $"group by District,City,f,i,o " +
                    $"order by District,City,f,i,o asc " +
                    "limit 1000";
                lookForOrganizationToolStripMenuItem.Tag = "select distinct " +
                    $"City as Область,District as Район,name as Название,edrpou,factory as Марка,model as Модель,plate as Номер " +
                    $"from CarAndOwner " +
                    $"where (length(name) > 0) " +
                    $"AND (UPPER(name) like '%{bodyQuery.ToUpper()}%') " +
                    $"group by District,City,name " +
                    $"order by District,City,name asc " +
                    "limit 1000";

                lookForNumberToolStripMenuItem.ToolTipText = $"Искать машину у которой номер совпадает с - {bodyQuery}";
                lookForFamiliyNameToolStripMenuItem.ToolTipText = $"Искать фамилию/организацию - {bodyQuery}";
                lookForOrganizationToolStripMenuItem.ToolTipText = $"Искать фамилию/организацию - {bodyQuery}";

                if (!string.IsNullOrWhiteSpace(nameQuery))
                {
                    addToExtraMenuItem.Enabled = true;
                    addToExtraMenuItem.ToolTipText = $"Запомнить запрос: {nameQuery}{Environment.NewLine}{bodyQuery}";

                    if (nameQuery.Contains("{}"))
                    {
                        addToStandartMenuItem.Enabled = true;
                        addToExtraMenuItem.ToolTipText = $"Запомнить запрос: {nameQuery}{Environment.NewLine}{bodyQuery}";
                    }
                }
            }
        }

        private void RecoverDataTableAfterQueryMenuItem_Click(object sender, EventArgs e)
        {
            if (dtForStore?.Rows?.Count > 0 && dtForStore?.Columns?.Count > 0)
            {
                dtForShow = dtForStore.Copy();
            }

            ShowLogViewTextbox(MainViewMode.DataGridView);
        }

        private async void ExportMenuItem_Click(object sender, EventArgs e)
        {
            StatusInfoMain.Text = $"Идет генерация отчетов...";
            dgv.Enabled = false;
            viewMenu.Enabled = false;
            queryMenu.Enabled = false;
            txtBodyQuery.Enabled = false;

            await WriteDataTableInTableExcel(dtForShow);

            txtBodyQuery.Enabled = true;
            queryMenu.Enabled = true;
            viewMenu.Enabled = true;
            dgv.Enabled = true;
            StatusInfoMain.Text = $"Готово!";
        }

        /*
Вывод количества машин(задвоения по госномерам удалены):
Select a.city as 'Область',a.name as 'наименование', a.edrpou as 'ЕДРПОУ', a.factory as 'марка',a.manufactureyear as 'год', count(*)as 'Количество' from  (Select distinct a.city,a.name, a.edrpou, a.factory, a.manufactureyear,a.plate  from carandowner a where edrpou > 1 group by a.city,a.name,a.factory,a.plate) a group by a.city,a.name, a.edrpou, a.factory,a.manufactureyear


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
            StatusInfoMain.Text = "Построение фильтров...";
            dgv.Enabled = false;
            txtbResultShow.Enabled = false;
            viewMenu.Enabled = false;
            queryMenu.Enabled = false;
            txtBodyQuery.Enabled = false;

            await BuildFilters();
            await Task.Run(() => BuildFiltersMenues(filtersTable));

            StatusInfoFilter.IsLink = true;
            StatusInfoFilter.LinkBehavior = LinkBehavior.AlwaysUnderline;
            StatusInfoFilter.Name = "StatusInfoFilter";
            StatusInfoFilter.Size = new Size(71, 22);
            //   StatusInfoFilter.Tag = "http://search.microsoft.com/search/search.aspx?";
            StatusInfoFilter.ToolTipText = "Поиск с использованием выбранных фильтров в текущей таблице";
            StatusInfoFilter.Click += new EventHandler(StatusInfoFilter_Click);

            txtBodyQuery.Enabled = true;
            txtbResultShow.Enabled = true;
            queryMenu.Enabled = true;
            viewMenu.Enabled = true;
            dgv.Enabled = true;
            StatusInfoMain.Text = "Построение фильтров завершено.";
        }

        private void StatusInfoFilter_Click(object sender, EventArgs e)
        {
            //  ToolStripLabel toolStripLabel1 = (ToolStripLabel)sender;
            ToolStripSplitButton f;

            string word = string.Join(",", dicTranslator.Values.ToArray()) + "," + string.Join(", ", dicTranslator.Keys.ToArray());

            string columns = string.Empty;
            foreach (DataColumn column in dtForStore?.Columns)
            {
                columns += column.ColumnName + ",";
            }

            string[] columnsInDGV = columns.Split(',');
            string[] wordsInDicTranslator = word.Split(',');

            string res = string.Empty;
            string txt = string.Empty;
            string tag = string.Empty;
            foreach (var filter in statusFilters.Items)
            {
                if (filter is ToolStripSplitButton)
                {
                    f = (filter as ToolStripSplitButton);
                    txt = f?.Text;
                    tag = f?.Tag?.ToString();
                    if (txt?.Length > 0 && tag?.Length > 0)
                    {
                        int amount = wordsInDicTranslator.Where(x => x == txt).Count()
                           + (txt == "Нет" ? 1 : 0) + (tag == "Нет" ? 1 : 0);

                        if (amount == 0 && columnsInDGV.Where(x => x == tag).Count() == 1)
                        {
                            //Учесть выводимые колонки!!!
                            if (res?.Length > 0)
                            { res += " AND " + tag + " LIKE '" + txt + "' "; }
                            else
                            { res += tag + " LIKE '" + txt + "' "; }
                        }
                    }
                }
            }
            AddLineAtTextboxLog("К таблице применен фильтр: " + res);

            if (dgv != null && dtForStore?.Rows?.Count > 0 && dtForStore?.Columns?.Count > 0)
            {
                using (DataTable dtTemp = dtForStore.Select(res).CopyToDataTable())
                {
                    dgv.DataSource = dtTemp;
                }
            }
            //  System.Diagnostics.Process.Start("IEXPLORE.EXE", toolStripLabel1.Tag.ToString());
            //  Set the LinkVisited property to true to change the color.
            //  toolStripLabel1.LinkVisited = true;
        }


        private async Task BuildFilters()
        {
            AddLineAtTextboxLog("Выполняется чтение базы и формирование библиотеки уникальных слов.");

            //Подписи колонок в меню
            dicTranslator = new Dictionary<string, string>
            {
                { "District", "Район" },
                { "City", "Область" },
                { "Factory", "Марка" },
                { "ManufactureYear", "Год" }//,
              //  { "Type", "ЧЛ/Предприятие" }
            };

            AddLineAtTextboxLog("Фильтры строятся на основе данных алиасов колонок:");
            AddLineAtTextboxLog(string.Join(", ", dicTranslator.Values.ToArray()));
            AddLineAtTextboxLog();

            await Task.Run(() => filtersTable = (dBOperations as SQLiteDBOperations).GetFilterList(dicTranslator, "CarAndOwner"));

            AddLineAtTextboxLog("Построение фильтров завершено");
        }


        private void BuildFiltersMenues(IModelDBable<ModelDBColumn> filtersTable)
        {
            MenuFiltersMaker menuMaker;
            ToolStripSplitButton filterSplitButton;
            ToolStripMenuItem subFilterMenu;

            foreach (var column in filtersTable.Collection.Take(5))
            {
                menuMaker = new MenuFiltersMaker(dicTranslator);
                if (!(column?.Name?.Length > 0 && column?.Collection?.Count > 0))
                {
                    continue;
                }

                filterSplitButton = menuMaker.MakeFiltersMenu(column?.Name, column?.Alias);
                filterSplitButton.DropDownItemClicked += FilterMenu_DropDownItemClicked;
                if (filterSplitButton == null)
                {
                    continue;
                }
                statusFilters.Items.Add(filterSplitButton);

                foreach (var f in column.Collection.Take(40))
                {
                    subFilterMenu = menuMaker.MakeFiltersSubMenu(f?.Name);
                    if (subFilterMenu == null)
                    {
                        continue;
                    }
                    filterSplitButton.DropDownItems.Add(subFilterMenu);
                }
            }
        }

        //выбранный пункт из ДропДаунМеню сделать названием фильтра
        private void FilterMenu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        { (sender as ToolStripSplitButton).Text = e.ClickedItem.Text; }



        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SelectNewDBMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                string filePath = ofd.OpenFileDialogReturnPath();
                if (filePath?.Length > 0)
                {
                    CheckUpSelectedSQLiteDB(filePath);
                }
                else
                {
                    ShowLogViewTextbox(MainViewMode.Textbox);

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


        private void CheckUpSelectedSQLiteDB(string filePath)
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

                try { (dBOperations as SQLiteDBOperations).EvntInfoMessage -= AddLineAtTextboxLog; } catch { }

                currentSQLConnection.Database = filePath;

                if (OperatingModes == AppModes.Admin)
                { (dBOperations as SQLiteDBOperations).EvntInfoMessage += AddLineAtTextboxLog; }
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
                    txtBodyQuery.Enabled = false;
                    txtbResultShow.Enabled = false;

                    //   connectionSettings = new SQLConnectionSettings();
                    (dBOperations as SQLiteDBOperations).TryMakeLocalDB();
                }
                schemaDB = null;
            }
        }


        private void AddLineAtTextboxLog(object sender, TextEventArgs text)
        { AddLineAtTextboxLog(text?.Message); }

        private void AddLineAtTextboxLog(string text = null)
        { txtbResultShow.AppendLine($"{text}"); }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //write Config to File
               await Task.Run(() => MakeCurrentFullConfig());
            await Task.Delay(100);

               await Task.Run(() => WriteCfgInFile());
            await Task.Delay(500);

            if (OperatingModes == AppModes.Admin)
            { (regOperator as RegistryManager).EvntStatusInfo -= AddLineAtTextboxLog; }
        }



        private async Task UpdateQueryExtraMenuInRegistry()
        {
            //clear registry from menu entries
            await Task.Run(() => regOperator.DeleteSubKeyTreeQueryExtraItems($"{regSubKeyMenu}\\{currentSQLConnection.Name}"));

            await Task.Delay(500);

            await Task.Run(() =>
            {
                IDictionary<string, string> menuItems = queriesExtraMenu.ToDictionary(30);
                regOperator.Write(menuItems, $"{regSubKeyMenu}\\{currentSQLConnection.Name}");
            });
        }

        private void AddQueriesFromRegistryToToolStripMenu(ToolStripMenuItem target, IList<ToolStripMenuItem> source, ToolStripMenuType menuType)
        {
            if (target != null && source?.Count > 0)
            {
                foreach (var m in source.ToArray())
                {
                    switch (menuType)
                    {
                        case ToolStripMenuType.ExtraQuery:
                            {
                                m.Click += QueryMenuItem_Click;
                                break;
                            }
                        case ToolStripMenuType.RecentConnection:
                            {
                                m.Click += BaseChangeMenuItems_Click;
                                break;
                            }
                    }
                    target.DropDownItems.Add(m);
                }
                StatusInfoMain.Text = $"В меню '{target.Text}' добавлено - {source.Count} запросов";

                menuStrip.Update();
                menuStrip.Refresh();
            }
        }



        private async void QueryMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            string queryName = item?.Text?.ToString();
            string queryBody = item?.Tag?.ToString();

            menuStrip.Update();
            menuStrip.Refresh();

            if (queryBody?.Length > 0)
            {
                if (queryName?.Length > 0)
                {
                    txtbNameQuery.Text = queryName;
                    AddLineAtTextboxLog($"Выполняется запрос - '{queryName}':");
                }

                await GetData(queryBody);
            }
        }


        private void RemoveCheckedInQueryExtraMenuItem_Click(object sender, EventArgs e)
        {
            RemoveCheckedMenuItemFromMenuStrip(queriesExtraMenu);
        }

        private async void RemoveCheckedMenuItemFromMenuStrip(ToolStripMenuItem item, bool allMenuChecked = false)
        {
            IList<ToolStripItem> listRemove = item.ToToolStripItemsList();

            if (listRemove.Count > 0)
            {
                string result = $"Запрос(ы)";

                foreach (ToolStripMenuItem m in listRemove)
                {
                    if (m.Checked || allMenuChecked)
                    {
                        result += $" '{m.Text}'";
                        item.DropDownItems.Remove(m);
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

        private async void AddExtraQueryMenuItem_Click(object sender, EventArgs e)
        {
            string nameQuery = txtbNameQuery.Text.Trim();
            string bodyQuery = txtBodyQuery.Text.Trim();

            MenuItem menuItem = new MenuItem(nameQuery, bodyQuery);

            ToolStripMenuItem item = menuItem.ToExtraMenuToolStripMenuItem();

            queriesExtraMenu.DropDownItems.Add(item as ToolStripMenuItem);
            item.Click += QueryMenuItem_Click;
            menuStrip.Update();
            menuStrip.Refresh();

            await Task.Run(() => UpdateQueryExtraMenuInRegistry());


            StatusInfoMain.Text = $"Запрос '{nameQuery}' в меню добавлен сохранен";
        }


        private void ApplicationRestart(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void ApplicationAbout(object sender, EventArgs e)
        {
            HelpForm help = new HelpForm();
            help.Show();
        }

        private void ApplicationExit(object sender, EventArgs e)
        {
            Text = @"Closing application...";

            dtForShow?.Dispose();
            dtForStore?.Dispose();
            dgv?.Dispose();
            tooltip?.Dispose();

            bmpLogo?.Dispose();

            notifyIcon?.Dispose();
            contextMenu?.Dispose();

            Thread.Sleep(500);

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

                    await ExportToFile(fi, dtTemp, appFileVersionInfo.FileVersion);

                    dtTemp.Dispose();
                    fi = null;
                }
            }
            else
            {
                AddLineAtTextboxLog("Сначала отберите данные из БД");
            }

            ShowLogViewTextbox(MainViewMode.Textbox);
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
            AddLineAtTextboxLog($"Запрос:{Environment.NewLine}{query}");
            StatusInfoMain.Text = "Выполняется отбор данных...";

            queriesStandartMenu.Enabled = false;
            queriesExtraMenu.Enabled = false;
            viewMenu.Enabled = false;
            txtbResultShow.Enabled = false;
            dgv.Enabled = false;


            dtForShow = new DataTable();
            try
            {
                await Task.Run(() => dtForShow = dBOperations.GetTable(query));

                //takeBackUp
                dtForStore = dtForShow.Copy();

                dgv.DataSource = dtForShow;

                AddLineAtTextboxLog($"Количество записей в базе: {dtForShow.Rows.Count}");

                StatusInfoMain.Text = $"Количество записей: {dtForShow.Rows.Count}";

                ShowLogViewTextbox(MainViewMode.DataGridView);
            }
            catch (SQLiteException dbsql)
            {
                AddLineAtTextboxLog($"\r\nОшибка в запросе:\r\n-----\r\n{dbsql.Message}\r\n-----\r\n{dbsql.ToString()}\r\n");
                StatusInfoMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(MainViewMode.Textbox);
            }
            catch (OutOfMemoryException e)
            {
                AddLineAtTextboxLog($"\r\nВаш запрос очень общий и тяжелый для БД. Кокретизируйте запрашиваемые поля или уменьшите выборку:\r\n-----\r\n{e.Message}\r\n-----\r\n{e.ToString()}\r\n-----\r\n");
                StatusInfoMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(MainViewMode.Textbox);
            }
            catch (Exception e)
            {
                AddLineAtTextboxLog($"\r\nОбщая ошибка:\r\n-----\r\n{e.ToString()}\r\n-----\r\n");
                StatusInfoMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(MainViewMode.Textbox);
            }

            queriesStandartMenu.Enabled = true;
            queriesExtraMenu.Enabled = true;
            viewMenu.Enabled = true;
            txtbResultShow.Enabled = true;
            dgv.Enabled = true;
        }



        private void GetSchemaLocalDBMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(MainViewMode.Textbox);
            if (currentSQLConnection.ProviderName == SQLProvider.SQLite)
            {
                schemaDB = DbSchema.LoadDB(currentSQLConnection.Database);
                tablesDB = new List<string>();

                AddLineAtTextboxLog(Properties.Resources.DashedSymbols);
                AddLineAtTextboxLog($"-  Scheme of local DB: '{currentSQLConnection.Database}':");
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
                AddLineAtTextboxLog($"-  End of Scheme of local DB: '{currentSQLConnection.Database}':");
                AddLineAtTextboxLog(Properties.Resources.DashedSymbols);
            }
            else
            {
                AddLineAtTextboxLog("Проверить схему можно только локальной БД SQLite");
            }
        }

        private void allColumnsInTableQueryMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(MainViewMode.Textbox);
            if (currentSQLConnection.ProviderName == SQLProvider.SQLite)
            {
                schemaDB = DbSchema.LoadDB(currentSQLConnection.Database);
                tablesDB = new List<string>();

                AddLineAtTextboxLog(Properties.Resources.DashedSymbols);
                AddLineAtTextboxLog($"- Selected DB: '{currentSQLConnection.Database}'");

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

                    AddLineAtTextboxLog($" ---  ---");
                }

                AddLineAtTextboxLog($"-  The End  -:");
                AddLineAtTextboxLog(Properties.Resources.DashedSymbols);
                AddLineAtTextboxLog();
            }
            else
            {
                AddLineAtTextboxLog("Проверить схему можно только локальной БД SQLite");
            }
        }

        private void CreateLocalDBMenuItem_Click(object sender, EventArgs e)
        {
            (dBOperations as SQLiteDBOperations).TryMakeLocalDB();
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

                if ((
                    arrQuery[0].ToLower() == "select" && arrQuery.Length > 3
                    && arrQuery.Where(w => w.ToLower().Contains("select")).Count() > 0
                    && arrQuery.Where(w => w.ToLower().Contains("from")).Count() > 0
                    ) || OperatingModes == AppModes.Admin)
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
                        ShowLogViewTextbox(MainViewMode.Textbox);
                        AddLineAtTextboxLog("Отмена задания.");
                    }
                }
                else
                {
                    ShowLogViewTextbox(MainViewMode.Textbox);
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

            ShowLogViewTextbox(MainViewMode.Textbox);

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

        public async Task ImportData(string filePath)
        {
            reader = new FileReaderModels<CarAndOwner>();
            txtbResultShow.Clear();

            reader.EvntCollectionFull += Reader_collectionFull;
            await reader.GetContent(filePath, MAX_ELEMENTS_COLLECTION);
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

                (dBOperations as SQLiteDBOperations).WriteListInLocalDB(list);

                StatusInfoMain.Text = $"Количество записей: {readRows}";

                AddLineAtTextboxLog($"First Element{1}: plate: {list.ElementAt(0).Plate} factory: {list.ElementAt(0).Factory}, model: {list.ElementAt(0).Model}");
                AddLineAtTextboxLog($"Last Element{list.Count - 1}: plate: {list.ElementAt(list.Count - 1).Plate} factory: {list.ElementAt(list.Count - 1).Factory}, model: {list.ElementAt(list.Count - 1).Model}");
            }
        }




        public void CheckEnvironment()
        {
            CheckCommandLineApplicationArguments();
        }

        /// <summary>
        /// show Administrator Menu: -a  
        /// </summary>
        public void CheckCommandLineApplicationArguments()
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
                if (envParameter.StartsWith("a"))
                {
                    OperatingModes = AppModes.Admin;
                    administratorMenu.Enabled = true;
                }
            }
            else
            {
                OperatingModes = AppModes.User;
                administratorMenu.Enabled = false;
            }

            arguments.EvntInfoMessage -= AddLineAtTextboxLog;
        }




        private void PrintConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintFullConfig();
        }

        private async void ReadCfgFromFileMenuItem_Click(object sender, EventArgs e)
        {
            await ReadCfgAsyncFromFile();
        }

        public void PrintFullConfig()
        {
            AddLineAtTextboxLog("  -= static SQLConnection.Settings =-  ");
            AddLineAtTextboxLog(SQLConnection.Settings?.GetPropertyValues()?.AsString());
            AddLineAtTextboxLog();

            AddLineAtTextboxLog("-= current SQL connectionSettings =-");
            AddLineAtTextboxLog(currentSQLConnection?.GetPropertyValues()?.AsString());
            AddLineAtTextboxLog();

            AddLineAtTextboxLog("-= currentConfig =-");
            PrintConfig(currentConfig);
            AddLineAtTextboxLog();
        }

        public void PrintConfig(AbstractConfigList config)
        {
            AddLineAtTextboxLog($"-=   =-");
            AddLineAtTextboxLog($"Config:");
            AddLineAtTextboxLog($"{config?.Get?.Count}");
            string defaultConnection = null;
            ISQLConnectionSettings connectionDefault = new SQLConnectionSettings(null);

            if (config?.Get?.Count > 0)
            {
                foreach (var confUnit in config.Get)
                {
                    AbstractUnitConfigParameterList unit = confUnit.Value;
                    AddLineAtTextboxLog($"--   --");
                    AddLineAtTextboxLog($"unit.Name - {unit?.Name} : {unit?.Desciption} : {unit?.LastModification} : {unit?.Version} ");
                    
                    if (unit.Get?.Count > 0)
                    {
                        foreach (var confParameter in unit.Get)
                        {
                            AddLineAtTextboxLog($"{unit?.Name}  confParameter.Key - {confParameter.Key}:");

                            if (confParameter.Key == nameof(ISQLConnectionSettings) && unit?.Name == MAIN)
                            {

                                AddLineAtTextboxLog($"-= =-  -=  =- -=  =- -=  =- -=  =- -=  =-");
                                AddLineAtTextboxLog($"-= =-  -=  =- -=  =- -=  =- -=  =- -=  =-");
                                AddLineAtTextboxLog($"{nameof(ISQLConnectionSettings)}:");

                                if (confParameter.Value?.Get?.Count > 0)
                                {
                                    foreach (var p in confParameter.Value?.Get)
                                    {
                                        AddLineAtTextboxLog($"{p.Key} - =- {p.Value}");
                                    }

                                    connectionDefault.Name = confParameter.Value.Get[nameof(connectionDefault.Name)];
                                    connectionDefault.Database = confParameter.Value.Get[nameof(connectionDefault.Database)];
                                    connectionDefault.Table = confParameter.Value.Get[nameof(connectionDefault.Table)];
                                    connectionDefault.ProviderName = confParameter.Value.Get[nameof(connectionDefault.ProviderName)].GetSQLProvider();
                                    connectionDefault.Host = confParameter.Value.Get[nameof(connectionDefault.Host)];
                                    connectionDefault.Port = int.TryParse(confParameter.Value.Get[nameof(connectionDefault.Port)], out int port) ? port : 0;
                                    connectionDefault.Username = confParameter.Value.Get[nameof(connectionDefault.Username)];
                                    connectionDefault.Password = confParameter.Value.Get[nameof(connectionDefault.Password)];

                                    AddLineAtTextboxLog($"-= =-  -=  =- -=  =- -=  =- -=  =- -=  =-");
                                    AddLineAtTextboxLog($"-= =-  -=  =- -=  =- -=  =- -=  =- -=  =-");
                                }
                            }
                            else
                            {
                                AbstractConfigParameter parameters = confParameter.Value as ConfigParameter;
                                AddLineAtTextboxLog($"parameters.Name - {parameters?.Name}");

                                if (parameters?.Get?.Count > 0)
                                {
                                    foreach (var parameter in parameters?.Get)
                                    {
                                        AddLineAtTextboxLog($"unit.Name '{unit.Name}', parameters.Name '{parameters.Name}',  parameter.Value - {  parameter.Value}");

                                        if (unit.Name.Equals(MAIN) && parameters.Name.Equals(DEFAULT_CONNECTION))
                                        {
                                            AddLineAtTextboxLog($"Default connection is {parameter.Value}");
                                            defaultConnection = parameter.Value.ToString();
                                        }

                                        AddLineAtTextboxLog($"parameter.Key '{parameter.Key}', Value '{parameter.Value}'");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            AddLineAtTextboxLog($"-= =-  -=  =- -=  =- -=  =- -=  =- -=  =-");
            AddLineAtTextboxLog("connectionDefault - " + connectionDefault?.GetPropertyValues()?.AsString());
            AddLineAtTextboxLog($"-= =-  -=  =- -=  =- -=  =- -=  =- -=  =-");
        }


        public async Task ReadCfgAsyncFromFile()
        {
            readerConfig = new FileReader();
            (readerConfig as FileReader).EvntInfoMessage += AddLineAtTextboxLog;

            await (readerConfig as FileReader).ReadConfigAsync(appCfgFilePath);

            if ((readerConfig as FileReader)?.config != null && (readerConfig as FileReader)?.config?.Count > 0)
            {
                currentConfig = new ConfigList((readerConfig as FileReader)?.config);
            }
            (readerConfig as FileReader).EvntInfoMessage -= AddLineAtTextboxLog;
        }



        private async void WriteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddLineAtTextboxLog();
            AddLineAtTextboxLog("  -= Prepare MakeCurrentFullConfig =-  ");
            await Task.Run(() => MakeCurrentFullConfig());

            AddLineAtTextboxLog("-= PrintConfig currentConfig =-");
            PrintConfig(currentConfig);
            AddLineAtTextboxLog();

            AddLineAtTextboxLog();
            AddLineAtTextboxLog("  -= Prepare WriteCfgInFile =-  ");
            AddLineAtTextboxLog("  -= Fake =-  ");
            await Task.Run(() => WriteCfgInFile());
        }



        public async Task WriteCfgInFile()
        {
            IWriterable writer = new FileWriter();
            (writer as FileWriter).EvntInfoMessage += AddLineAtTextboxLog;
            await writer.Write(appCfgFilePath, currentConfig);
            (writer as FileWriter).EvntInfoMessage -= AddLineAtTextboxLog;
            writer = null;
        }

        public void MakeCurrentFullConfig()
        {
            //Unit1
            //Main Config
            AbstractUnitConfigParameterList parametersList = new ConfigUnitParameterList();

            AbstractConfigParameter parameters = new ConfigParameter();
            parameters.Name = DB_LIST;
            foreach (var menuItem in changeBaseToolStripMenuItem.ToDictionary())
            { parameters.Add(menuItem.Key, menuItem.Value); }
            parametersList.Add(parameters);//add the whole parameter in unit

            parameters = new ConfigParameter();
            parameters.Name = nameof(ISQLConnectionSettings);
            foreach (var item in currentSQLConnection.GetPropertyValues())
            { parameters.Add(item.Key, item.Value); }

            parametersList.Name = MAIN;
            parametersList.Add(parameters);//add the whole parameter in unit

            currentConfig.Name = "CurrentFullConfig";
            currentConfig.Desciption = "Current Full Configuration";
            currentConfig.Add(parametersList); //add the whole unit in Config

            
            //Unit2
            //connections
            parametersList = new ConfigUnitParameterList();


            //queriesExtraMenu
            parameters = new ConfigParameter();
            parameters.Name = $"{nameof(queriesExtraMenu)}";
            foreach (var menuItem in queriesExtraMenu.ToDictionary())
            {
                parameters.Add(menuItem.Key, menuItem.Value);
            }
            parametersList.Name = currentSQLConnection.Name;
            parametersList.Add(parameters);//add the whole parameter in unit


            //queriesStandartMenu
            parameters = new ConfigParameter();
            foreach (var menuItem in queriesStandartMenu.ToDictionary())
            {
                parameters.Add(menuItem.Key, menuItem.Value);
            }
            parameters.Name = $"{nameof(queriesStandartMenu)}";
            parametersList.Add(parameters);//add the whole parameter in unit

            //newConfig
            currentConfig.Add(parametersList); //add the whole unit in Config

            //Unit3 .....

            //(currentConfig as ConfigParameter).Set(newConfig);
        }




        //App View Mode
        private void ChangeViewPanelviewMenuItem_Click(object sender, EventArgs e)
        { ChangeViewPanelviewMenuItem(); }

        bool logView;
        private void ChangeViewPanelviewMenuItem()
        {
            if (logView)
            { ShowLogViewTextbox(MainViewMode.DataGridView); logView = false; }
            else
            { ShowLogViewTextbox(MainViewMode.Textbox); logView = true; }
        }

        private void ShowLogViewTextbox(MainViewMode mode)
        {
            switch (mode)
            {
                case MainViewMode.Textbox:
                    {
                        dgv?.Hide();

                        txtbResultShow.Show();
                        changeViewPanelviewMenuItem.Text = "Табличный";
                        break;
                    }

                case MainViewMode.DataGridView:
                    {
                        txtbResultShow.Hide();

                        if (dgv != null && dtForShow?.Rows?.Count > 0 && dtForShow?.Columns?.Count > 0)
                        {
                            if (dtForStore?.Rows?.Count != dtForShow?.Rows?.Count &&
                                dtForStore?.Columns?.Count != dtForShow?.Columns?.Count)
                            { dtForStore = dtForShow.Copy(); }

                            dgv.DataSource = dtForShow;
                            dgv?.Update();
                            dgv?.Refresh();
                            dgv?.Show();
                        }

                        changeViewPanelviewMenuItem.Text = "Текстовый";
                        StatusInfoMain.Text = "доступны пункты меню Загрузки и Анализа данных";
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }


    }
}
