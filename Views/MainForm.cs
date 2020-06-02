using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public partial class MainForm : Form
    {
        static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //method = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //logger.Trace("-= " + method + " =-");

        //Application Modes
        AppModes OperatingModes = AppModes.User;

        //Application's Main interface Turn Up


        //   static readonly string appRegistryKey = @"SOFTWARE\YuriRyabchenko\FlexibleDBMS";
        //private void WriteConnectionInRegistry(ISQLConnectionSettings settings)
        //{
        //    IDictionary<string, string> dic = settings.DoObjectPropertiesAsStringDictionary(50);
        //    //write parameters of connection
        //    //       regOperator.Write(dic, $"{PublicConst.regSubKeyRecent}\\{settings.Name}");
        //    //write last connection' name
        //    //   regOperator.Write(PublicConst.regSubKeyRecent, settings.Name);
        //    //write connection and name as pair key-value
        //    //    regOperator.Write(settings.Name, settings.Name, PublicConst.regSubKeyRecent);
        //}


        Bitmap bmpLogo;
        static NotifyIcon notifyIcon;
        static ContextMenu contextMenu;
        ToolTip tooltip = new ToolTip();

        //Forms
        HelpForm helpForm;
        AdministratorForm administratorForm;
        
        //SHow datatables in DataGridView
        DataGridView dgv;
        DataTable dtForShow;  //datatable source of show
        DataTable dtForStore; //datatable store data
        
        SqlAbstractConnector dBOperations;// = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);
        ConfigStore Configuration;
        SQLConnectionStore currentSQLConnectionStore = null;
        IModelEntityDB<DBColumnModel> filtersTable = null; // меню фильтров в строке статуса

        MenuAbstractStore recentStore;
        MenuAbstractStore queryExtraStore;
        MenuAbstractStore queryStandartStore;
        MenuAbstractStore tableStore;
        BindingList<string> lstColumn = new BindingList<string>();

        ItemFlipper statusInfoMainText;

        ApplicationUpdater Updater;


        ///-////-/////-//////-///////////////////////////////////////////
        ///-////-/////-//////-///////////////////////////////////////////

        public MainForm()
        {
            InitializeComponent();

            //Блок проверки уровня настройки логгирования
            logger.Info("Test Info message");
            logger.Trace("Test1 Trace message");
            logger.Debug("Test2 Debug message");
            logger.Warn("Test3 Warn message");
            logger.Error("Test4 Error message");
            logger.Fatal("Test5 Fatal message");

            Updater = new ApplicationUpdater(); 
            Configuration = new ConfigStore();

            currentSQLConnectionStore = new SQLConnectionStore();
            recentStore = new MenuItemStore();
            queryExtraStore = new MenuItemStore();
            queryStandartStore = new MenuItemStore();
            tableStore = new MenuItemStore();
            (tableStore as MenuItemStore).EvntCollectionChanged += new MenuItemStore.ItemAddedInCollection<BoolEventArgs>(TablesStore_EvntCollectionChanged);

            statusInfoMainText = new ItemFlipper();
            statusInfoMainText.EvntSetText += new ItemFlipper.AddedItemInCollection<TextEventArgs>(StatusInfoMainText_SetTemporaryText);

            Updater.EvntReset += new ApplicationUpdater.Reset(ApplicationExit);
            Updater.EvntStatus += new ApplicationUpdater.InfoMessage<TextEventArgs>(StatusInfoMainText_SetTemporaryText); // new ApplicationUpdater.InfoMessage(AddLineAtTextboxLog);
        }


        ///-////-/////-//////-///////////////////////////////////////////
        ///-////-/////-//////-///////////////////////////////////////////

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxColumns.Sorted = true;
            comboBoxColumns.DataSource = lstColumn;

            //Check Up Inputed Environment parameters
            CheckEnvironment();

            //Turn Up Application
            TurnAplication();

            //Turn Up Menu
            TurnUpToolStripMenuItems();

            //Turn Up StatusStrip
            TurnStatusLabelMenues();

            ConfigFull<ConfigAbstract> tmpConfig = ReadCfgFromFile(CommonConst.AppCfgFilePath);
            ISQLConnectionSettings tmpConnection = GetDefaultConnectionFromConfig(tmpConfig);

            AddLineAtTextboxLog($"{Properties.Resources.EqualSymbols}{Properties.Resources.EqualSymbols}");
            AddLineAtTextboxLog($"Имя последнего коннекта:{Environment.NewLine}{tmpConnection?.Name}");

            if (tmpConfig != null && !(string.IsNullOrWhiteSpace(tmpConnection?.Name)))
            {
                ConfigUnitStore unitConfig = GetConfigUnitByName(tmpConfig, tmpConnection?.Name);

                currentSQLConnectionStore.Set(unitConfig?.SQLConnection);

                queryExtraStore.Set(unitConfig?.QueryExtraMenuStore?.GetAllItems());
                queryStandartStore.Set(unitConfig?.QueryStandartMenuStore?.GetAllItems());
                recentStore.Set(unitConfig?.RecentMenuStore?.GetAllItems());

                //Установить полную конфигурацию(Иначе считать, что это первый запуск ПО и конфигурация - пустая)
                Configuration.Set(tmpConfig);
            }

            if (string.IsNullOrWhiteSpace(tmpConnection?.Name) || string.IsNullOrWhiteSpace(tmpConnection?.Database))
            { RunAdministratorForm(); }

            currentSQLConnectionStore.EvntConfigChanged += new SQLConnectionStore.ConfigChanged<BoolEventArgs>(CurrentSQLConnectionStore_EvntConfigChanged);
            (recentStore as MenuItemStore).EvntCollectionChanged += new MenuItemStore.ItemAddedInCollection<BoolEventArgs>(RecentStore_EvntCollectionChanged);
            (queryExtraStore as MenuItemStore).EvntCollectionChanged += new MenuItemStore.ItemAddedInCollection<BoolEventArgs>(QueryExtraStore_EvntCollectionChanged);
            (queryStandartStore as MenuItemStore).EvntCollectionChanged += new MenuItemStore.ItemAddedInCollection<BoolEventArgs>(QueryStandartStore_EvntCollectionChanged);

            currentSQLConnectionStore.Refresh();
            menuStrip.Update();
            menuStrip.Refresh();

            // Переключиться на лог
            SelectLog();
            Task.Run(() => CheckUpdatePeriodicaly()).Wait();
        }



        ///-////-/////-//////-///////////////////////////////////////////
        ///-////-/////-//////-///////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newConfig">Configuration.Get</param>
        /// <param name="newConnection">currentSQLConnectionStore?.GetCurrent()</param>
        public void MakeCurrentFullConfig(ConfigFull<ConfigAbstract> newConfig = null, ISQLConnectionSettings newConnection = null)
        {
            ConfigFull<ConfigAbstract> configFull = newConfig;

            if (newConfig == null)
            { configFull = new ConfigFull<ConfigAbstract>(); }

            if (newConnection == null)
            { newConnection = currentSQLConnectionStore?.GetCurrent(); }

            ConfigAbstract configUnit = new Config
            {
                Name = CommonConst.MAIN,
                Version= CommonConst.AppVersion,
                ConfigDictionary = new Dictionary<string, object>()
            };
            ConfigAbstract config = new Config
            {
                Name = nameof(ISQLConnectionSettings),
                ConfigDictionary = newConnection?.DoObjectPropertiesAsObjectDictionary()
            };
            configUnit.ConfigDictionary[config.Name] = config;
            config = new Config
            {
                Name = nameof(ToolStripMenuType.ExtraQuery),
                ConfigDictionary = queryExtraStore?.GetAllItems()?.AsObjectDictionary()
            };
            configUnit.ConfigDictionary[config.Name] = config;
            config = new Config
            {
                Name = nameof(ToolStripMenuType.StandartQuery),
                ConfigDictionary = queryStandartStore?.GetAllItems()?.AsObjectDictionary()
            };
            configUnit.ConfigDictionary[config.Name] = config;
            configFull.Add(configUnit);

            ////New Connection
            configUnit = new Config
            {
                Name = newConnection?.Name,
                ConfigDictionary = new Dictionary<string, object>()
            };

            config = new Config
            {
                Name = nameof(ISQLConnectionSettings),
                ConfigDictionary = newConnection?.DoObjectPropertiesAsObjectDictionary()
            };
            configUnit.ConfigDictionary[config.Name] = config;
            configFull.Add(configUnit);
            
            config = new Config
            {
                Name = nameof(ToolStripMenuType.ExtraQuery),
                ConfigDictionary = queryExtraStore?.GetAllItems()?.AsObjectDictionary()
            };
            configUnit.ConfigDictionary[config.Name] = config;
            config = new Config
            {
                Name = nameof(ToolStripMenuType.StandartQuery),
                ConfigDictionary = queryStandartStore?.GetAllItems()?.AsObjectDictionary()
            };
            configUnit.ConfigDictionary[config.Name] = config;
            if (!string.IsNullOrWhiteSpace(configUnit.Name))
            { configFull.Add(configUnit); }


            ////
            configUnit.Name = CommonConst.RECENT;
            config = new Config
            {
                Name = newConnection?.Name,
                ConfigDictionary = recentStore?.GetAllItems()?.AsObjectDictionary()
            };
            if (!string.IsNullOrWhiteSpace(config?.Name))
            {
                configUnit.ConfigDictionary[config.Name] = config;
            }

            configFull.Add(configUnit);
            configFull.LastModification = CommonConst.DateTimeStamp;
            configFull.Version = CommonConst.AppVersion;
            Configuration.Set(configFull);
        }

        private  void CurrentSQLConnectionStore_EvntConfigChanged(object sender, BoolEventArgs args)
        {
            _ = CurrentSQLConnectionStore_ConfigChanged();
        }

        private async Task CurrentSQLConnectionStore_ConfigChanged()
        {
            ISQLConnectionSettings oldSettings = currentSQLConnectionStore?.GetPrevious();
            ISQLConnectionSettings newSettings = currentSQLConnectionStore?.GetCurrent();

            //Save current state
            await Task.Run(() => SaveControlState());
            await Task.Run(() => LockControl());

            await Task.Run(() =>
            {
                AddLineAtTextboxLog($"{Properties.Resources.DashedSymbols}{Properties.Resources.DashedSymbols}");
                AddLineAtTextboxLog("Переключаюсь на новые настройки...");
            });


            if (!(string.IsNullOrWhiteSpace(newSettings?.Name)))
            {
                recentStore.Add(new ToolStripMenuItem(newSettings.Name));

                await Task.Run(() => MakeCurrentFullConfig(Configuration?.Get, oldSettings));

                await Task.Run(() =>
                {
                    ConfigUnitStore applicationConfig = GetConfigUnitByName(Configuration.Get, newSettings.Name);
                    queryStandartStore.Set(applicationConfig?.QueryStandartMenuStore?.GetAllItems());
                    queryExtraStore.Set(applicationConfig?.QueryExtraMenuStore?.GetAllItems());
                });

                if (!(string.IsNullOrWhiteSpace(newSettings?.Database)))
                {
                    dBOperations = SQLSelector.SetConnector(newSettings);
                    dBOperations.EvntInfoMessage += new SqlAbstractConnector.Message<TextEventArgs>(DBOperations_EvntInfoMessage);

                    if (oldSettings?.Database != newSettings?.Database)
                    {
                        await Task.Run(() =>
                        {
                            AddLineAtTextboxLog($"Выбрано подключение к серверу: '{newSettings?.Host}'{Environment.NewLine}" +
                            $" БД: '{newSettings?.Database}'{Environment.NewLine}" +
                            $" Основная таблица: '{newSettings?.Table}'{Environment.NewLine}");

                            statusInfoMainText.SetConstText($"Выбрана база: {newSettings.Database}");
                        });

                        statusInfoMainText.SetTempText($"Получаю список таблиц...");

                        var cancellationToken = new System.Threading.CancellationTokenSource(5000).Token;
                        //cancellationToken.CancelAfter(200);
                        int timeout = 10000;
                        var task = SetTables(cancellationToken, newSettings);
                        if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) == task)
                        {
                            // Task completed within timeout.
                            // Consider that the task may have faulted or been canceled.
                            await task;
                        }

                        if (tableStore?.GetAllItems()?.Count > 0)
                        {
                            AddLineAtTextboxLog($"Список таблиц содержит: {tableStore?.GetAllItems()?.Count} элемента(ов)");
                            AddLineAtTextboxLog($"{Properties.Resources.DashedSymbols}{Properties.Resources.DashedSymbols}");
                        }
                        else
                        {
                            AddLineAtTextboxLog($"{Properties.Resources.SosSlashSymbols}{Properties.Resources.SosSlashSymbols}");
                            statusInfoMainText.SetTempText($"Список таблиц с сервера не получен.");
                            AddLineAtTextboxLog($"{Properties.Resources.SosSlashSymbols}{Properties.Resources.SosSlashSymbols}");
                        }
                        //Восстановить предыдущее состояние контролов
                        await Task.Run(() => RestoreControlState());

                    }
                    else if (oldSettings?.Database == newSettings?.Database && oldSettings?.Table != newSettings?.Table)
                    {
                        statusInfoMainText.SetTempText($"База: {newSettings.Database}, сменилась таблица на {newSettings.Table}");
                    }
                }
            }

            //Восстановить предыдущее состояние контролов
            await Task.Run(() => RestoreControlState());
        }

        Task SetTables(System.Threading.CancellationToken token, ISQLConnectionSettings newSettings)
        {
            return Task.Run(() => tableStore.Set(SQLSelector.GetTables(newSettings).GetToolStipMenuItemList()),token);
        }

        private void DBOperations_EvntInfoMessage(object sender, TextEventArgs e)
        {
            AddLineAtTextboxLog(e.Message);
        }

        /// <summary>
        /// Переключение текста  временно при установке текста как временного
        /// </summary>
        /// <param name="sender">StatusInfoMain</param>
        /// <param name="e">true - временно, false - постоянно</param>
        private async void StatusInfoMainText_SetTemporaryText(object sender, TextEventArgs e)
        {
            logger.Trace("StatusInfoMainText: " + e.Message);
            AddLineAtTextboxLog(e?.Message);

            StatusInfoMain.Text = e.Message;
            await Task.Delay(3500);
            StatusInfoMain.Text = statusInfoMainText.GetConstText;
        }

        private  void TablesStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        { StatusTables_anotherThreadAccess(); }

        
        private void StatusTables_anotherThreadAccess() //add string into  from other threads
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(delegate
                {
                    StatusTables.Enabled = false;

                    if (StatusTables.DropDownItems?.Count > 0)
                        StatusTables.DropDownItems.Clear();

                    if (tableStore?.GetAllItems()?.Count > 0)
                    {
                        StatusTables.DropDownItems.AddRange(tableStore.GetAllItems().ToArray());
                    }

                    StatusTables.DropDown.Refresh();
                    StatusTables.Text = "Таблицы";
                    StatusTables.Enabled = true;
                }));
            else
            {
                StatusTables.Enabled = false;

                if (StatusTables.DropDownItems?.Count > 0)
                    StatusTables.DropDownItems.Clear();

                if (tableStore?.GetAllItems()?.Count > 0)
                {
                    StatusTables.DropDownItems.AddRange(tableStore.GetAllItems().ToArray());
                }

                StatusTables.DropDown.Refresh();
                StatusTables.Text = "Таблицы";
                StatusTables.Enabled = true;
            }
        }


        ///-////-/////-//////-///////////////////////////////////////////


        #region Update Application
        private void PrepareUpdateMenuItem_Click(object sender, EventArgs e)
        {
            UserAD user = null;
            string serverURL = null;
            _ = Updater.SetOptionsAsync(user, serverURL);
            Updater.PrepareUpdateFiles();
        }

        private void CheckUpdateMenuItem_Click(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void CheckUpdate()
        {
            UserAD user = new UserAD();
            //настройки имени и пароля вставить в конфиг - файл            
            string serverURL = null;

           _= Updater.SetOptionsAsync(user, serverURL);
            if (!string.IsNullOrWhiteSpace(Updater?.Options?.serverUpdateURI))
            {
                string constText = statusInfoMainText.GetConstText;

                Updater.RunUpdate();

                Task.Delay(3000).Wait();
                statusInfoMainText.SetConstText(constText);
            }
            else
            {
                statusInfoMainText.SetTempText("Адрес сервера с обновлениями не найден.");
            }
        }

        /// <summary>
        /// Check and download new update 
        /// </summary>
        /// <param name="minutes">check period of a new Update</param>
        /// <returns></returns>
        private Task CheckUpdatePeriodicaly(int minutes = 1)
        {
            UserAD user = new UserAD();
            string serverURL = null;
           _= Updater.SetOptionsAsync(user, serverURL);
            return Updater.CheckUpdatePeriodicaly(minutes);
        }

        private void UploadUpdateMenuItem_Click(object sender, EventArgs e)
        {
            UploadUpdate();
        }

        private void UploadUpdate()
        {
            UserAD user = new UserAD();
            //настройки имени и пароля вставить в конфиг - файл
            string serverURL = null;
            string pathToExternalUpdateZip = null;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                pathToExternalUpdateZip = ofd.OpenFileDialogReturnPath(Properties.Resources.OpenDialogZipFile,
                    "Выберите заранее подготовленный архив с обновлением или нажмите Отмена, для автоматической генерации системой:");
            }

           _= Updater.SetOptionsAsync(user, serverURL, pathToExternalUpdateZip);
            if (!string.IsNullOrWhiteSpace(Updater?.Options?.serverUpdateURI))
            {
                string constText = statusInfoMainText.GetConstText;
                statusInfoMainText.SetConstText(Updater?.Options?.serverUpdateURI);

                Updater.UploadUpdate();

                Task.Delay(3000).Wait();
                statusInfoMainText.SetConstText(constText);
            }
            else
            {
                statusInfoMainText.SetTempText("Адрес сервера с обновлениями не найден.");
            }
        }
        #endregion


        ///-////-/////-//////-///////////////////////////////////////////

        #region Configuration Unit Block
        private ConfigUnitStore GetConfigUnitByName(ConfigFull<ConfigAbstract> tmpConfigFull, string text)
        {
            AddLineAtTextboxLog($"{Properties.Resources.EqualSymbols}{Properties.Resources.EqualSymbols}");
            AddLineAtTextboxLog("Ищу: " + text);
            AddLineAtTextboxLog();

            ISQLConnectionSettings connection = null;
            MenuAbstractStore queryStandart = null;
            MenuAbstractStore queryExtra = null;
            MenuAbstractStore recent = null;
            string result;

            if (tmpConfigFull?.Count() > 0)
            {
                IList<string> recentList = tmpConfigFull.GetUnitConfigNames();
                if (recentList?.Count > 0)
                {
                    result = $"{Properties.Resources.SosSymbols}{Environment.NewLine}" +
                        $"Recent menu:";

                    recent = new MenuItemStore();

                    foreach (var menu in recentList)
                    {
                        result += $"{Environment.NewLine}{menu}";

                        MenuItem item = new MenuItem(menu);
                        recent.Add(item.ToQueryMenuToolStripMenuItem());
                    }

                    AddLineAtTextboxLog(result);
                }

                foreach (var confUnit in tmpConfigFull?.Config)
                {
                    ConfigAbstract unit = confUnit.Value;

                    if (confUnit.Key.Equals(text) && unit?.ConfigDictionary?.Count() > 0) //Нашел!
                    {
                        AddLineAtTextboxLog("Нашел!");

                        ConfigAbstract tmpConfigUnit;

                        bool exist = unit.ConfigDictionary.TryGetValue(nameof(ISQLConnectionSettings), out object tmpConf);
                        if (exist)
                        {
                            tmpConfigUnit = tmpConf as Config;
                            connection = tmpConfigUnit?.ConfigDictionary?.ToISQLConnectionSettings();

                            AddLineAtTextboxLog($"{Properties.Resources.SosSymbols}{Environment.NewLine}" +
                                $"SQL connection:{Environment.NewLine}{connection.AsString()}");
                        }

                        exist = unit.ConfigDictionary.TryGetValue(nameof(ToolStripMenuType.StandartQuery), out tmpConf);
                        if (exist)
                        {
                            tmpConfigUnit = tmpConf as Config;
                            queryStandart = new MenuItemStore();
                            IList<MenuItem> data = tmpConfigUnit?.ConfigDictionary?.ToMenuItems();
                            if (data?.Count > 0)
                            {
                                result = $"{Properties.Resources.SosSymbols}{Environment.NewLine}" +
                                    $"Standart menu:";
                                foreach (var menu in data)
                                {
                                    result += $"{Environment.NewLine}{menu?.Text}:{menu?.Tag}";

                                    MenuItem item = new MenuItem(menu?.Text, menu?.Tag);
                                    queryStandart.Add(item.ToQueryMenuToolStripMenuItem());
                                }
                                AddLineAtTextboxLog(result);
                            }
                        }

                        exist = unit.ConfigDictionary.TryGetValue(nameof(ToolStripMenuType.ExtraQuery), out tmpConf);
                        if (exist)
                        {
                            tmpConfigUnit = tmpConf as Config;
                            queryExtra = new MenuItemStore();
                            IList<MenuItem> data = tmpConfigUnit?.ConfigDictionary?.ToMenuItems();
                            if (data?.Count > 0)
                            {
                                result = $"{Properties.Resources.SosSymbols}{Environment.NewLine}" +
                                   $"Extra menu:";

                                foreach (var menu in data)
                                {
                                    result += $"{Environment.NewLine}{menu?.Text}:{menu?.Tag}";

                                    MenuItem item = new MenuItem(menu?.Text, menu?.Tag);
                                    queryExtra.Add(item.ToQueryMenuToolStripMenuItem());
                                }
                                AddLineAtTextboxLog(result);
                            }
                        }

                        tmpConf = null;
                        break;
                    }
                }
            }

            ConfigUnitStore applicationNewConfig = new ConfigUnitStore
            {
                SQLConnection = connection,
                QueryStandartMenuStore = queryStandart,
                QueryExtraMenuStore = queryExtra,
                RecentMenuStore = recent
            };

            AddLineAtTextboxLog($"{Properties.Resources.DashedSymbols}{Properties.Resources.DashedSymbols}");

            return applicationNewConfig;
        }

        public ISQLConnectionSettings GetDefaultConnectionFromConfig(ConfigFull<ConfigAbstract> config)
        {
            ISQLConnectionSettings connectionDefault = new SQLConnectionSettings(null);
            ConfigAbstract configUnit;

            if (config?.Count() > 0)
            {
                foreach (var confUnit in config?.Config)
                {
                    ConfigAbstract unit = confUnit.Value;
                    if (unit?.Name == CommonConst.MAIN && unit.ConfigDictionary?.Count() > 0)
                    {
                        foreach (var confParameter in unit.ConfigDictionary)
                        {
                            configUnit = confParameter.Value as Config;

                            if (unit?.Name == CommonConst.MAIN && confParameter.Key == nameof(ISQLConnectionSettings) && configUnit?.ConfigDictionary?.Count > 0)
                            {
                                connectionDefault = configUnit?.ConfigDictionary.ToISQLConnectionSettings();
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            return connectionDefault;
        }

        private IList<string> ReturnRecentConnectionNamesFromConfig(ConfigFull<ConfigAbstract> config)
        {
            IList<string> connections = new List<string>();

            if (config?.Count() > 0) // Configuration.Get
            {
                connections = config.GetUnitConfigNames();
                connections.Remove(CommonConst.MAIN);
                connections.Remove(CommonConst.RECENT);
            }
            return connections;
        }


        public ConfigFull<ConfigAbstract> ReadCfgFromFile(string pathToConfig)
        {
            ConfigFull<ConfigAbstract> tmpConfig = null;
            IReadable readerConfig = new FileReader();
            (readerConfig as FileReader).EvntInfoMessage += new FileReader.InfoMessage(AddLineAtTextboxLog);

            (readerConfig as FileReader).ReadConfig(pathToConfig);

            if ((readerConfig as FileReader)?.config != null && (readerConfig as FileReader)?.config?.Config?.Count() > 0)
            {
                tmpConfig = (readerConfig as FileReader)?.config;
            }
            else
            {
                AddLineAtTextboxLog(Properties.Resources.EqualSymbols);
                AddLineAtTextboxLog($"Config '{CommonConst.AppCfgFilePath}' is empty or broken.");
                AddLineAtTextboxLog(Properties.Resources.EqualSymbols);
            }

          (readerConfig as FileReader).EvntInfoMessage -= AddLineAtTextboxLog;

            readerConfig = null;
            return tmpConfig;
        }

        ConfigFull<ConfigAbstract> loadedExternalConfig;
        private void LoadConfigMenuItem_Click(object sender, EventArgs e)
        {
            MenuAbstractStore tmpConfigStore = new MenuItemStore();
            string pathToFile = null;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                pathToFile = ofd.OpenFileDialogReturnPath(Properties.Resources.OpenDialogCfgFile, "Выберите файл с конфигурацией:");
                if (ofd.CheckFileExists)
                { 
                    loadedExternalConfig = ReadCfgFromFile(CommonConst.AppCfgFilePath);
                    tmpConfigStore.Set(loadedExternalConfig.GetUnitConfigNames().ToToolStripMenuItemList());
                    MakeMenuItemDropDownFromMenuStore(selectedConfigToolStripMenuItem, tmpConfigStore, ToolStripMenuType.ExternalConfig);
                 selectedConfigToolStripMenuItem.Text =$"Загружена конфигурация - {loadedExternalConfig.Version} от ({loadedExternalConfig.LastModification})";
                }
            }
        }
        

        void PrintConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtbResultShow.Clear();
            PrintApplicationFullConfig(Configuration.Get);
        }

        void PrintApplicationFullConfig(ConfigFull<ConfigAbstract> config)
        {
            AddLineAtTextboxLog(Properties.Resources.EqualSymbols);
            AddLineAtTextboxLog("-= current SQL connectionSettings =-");
            AddLineAtTextboxLog(currentSQLConnectionStore.GetCurrent()?.ToString());
            AddLineAtTextboxLog();
            AddLineAtTextboxLog(Properties.Resources.DashedSymbols);

            AddLineAtTextboxLog("-= currentConfig connectionSettings Names =-");
            AddLineAtTextboxLog(ReturnRecentConnectionNamesFromConfig(config).ToStringNewLine());
            AddLineAtTextboxLog();
            AddLineAtTextboxLog(Properties.Resources.DashedSymbols);


            AddLineAtTextboxLog("-= currentConfig =-");
            PrintConfig(config);
            AddLineAtTextboxLog(Properties.Resources.DashedSymbols);

            AddLineAtTextboxLog();
        }

        void PrintConfig(ConfigFull<ConfigAbstract> config)
        {
            AddLineAtTextboxLog($"Config units(count): - {config?.Count()}");
            string defaultConnection = null;
            ISQLConnectionSettings connectionDefault = new SQLConnectionSettings(null);

            if (config?.Count() > 0)
            {
                foreach (var confUnit in config?.Config)
                {
                    ConfigAbstract unit = confUnit.Value;
                    AddLineAtTextboxLog(Properties.Resources.SosSymbols);
                    AddLineAtTextboxLog($"Name - {confUnit.Key} ");

                    if (unit.ConfigDictionary?.Count() > 0)
                    {
                        foreach (var confParameter in unit.ConfigDictionary)
                        {
                            AddLineAtTextboxLog($"Parameter.Key - {confParameter.Key}:");
                            ConfigAbstract configUnit = confParameter.Value as Config;

                            if (unit?.Name == CommonConst.MAIN && confParameter.Key == nameof(ISQLConnectionSettings)) //SQLConnectionData
                            {
                                if (configUnit?.ConfigDictionary?.Count > 0)
                                {
                                    connectionDefault = configUnit?.ConfigDictionary.ToISQLConnectionSettings();
                                }
                            }
                            else
                            {
                                if (configUnit?.ConfigDictionary?.Count > 0)
                                {
                                    foreach (var parameter in configUnit?.ConfigDictionary)
                                    {
                                        AddLineAtTextboxLog($"parameters.Name '{parameter.Key}',  parameter.Value - {  parameter.Value}");

                                        if (unit.Name.Equals(CommonConst.MAIN) && configUnit.Name.Equals(CommonConst.DEFAULT_CONNECTION))
                                        {
                                            AddLineAtTextboxLog($"Default connection is {parameter.Value}");
                                            defaultConnection = parameter.Value.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            AddLineAtTextboxLog($"-= =-  -=  =- -=  =- -=  =- -=  =- -=  =-");
            AddLineAtTextboxLog("connectionDefault:");
            AddLineAtTextboxLog(connectionDefault?.ToString());
            AddLineAtTextboxLog($"-= * =- * -= * =- * -= * =- * -= * =- * -= * =- * -= * =- * -= * =- * -= * =- * -= * =- * -= * =- * -= * =- -= * =-");
        }

        private void PrintActiveConnectionConfigMenuItem_Click(object sender, EventArgs e)
        {
            txtbResultShow.Clear();
            PrintSelectedConfigConnection(Configuration.Get,currentSQLConnectionStore.GetCurrent().Name); 
        }
        
        void PrintSelectedConfigConnection(ConfigFull<ConfigAbstract> fullConfig, string selectedConfigName)
        {
            ConfigUnitStore selectedConfig = GetConfigUnitByName(fullConfig, selectedConfigName);

            AddLineAtTextboxLog($"{Properties.Resources.EqualSymbols}{Properties.Resources.EqualSymbols}");
            AddLineAtTextboxLog("-= SQL Connection Settings =-");
            AddLineAtTextboxLog(selectedConfig?.SQLConnection?.ToString());
            AddLineAtTextboxLog();
            AddLineAtTextboxLog(Properties.Resources.SosSymbols);

            AddLineAtTextboxLog("-= Query Standart =-");
            IList<ToolStripMenuItem> list = selectedConfig?.QueryStandartMenuStore?.GetAllItems();
            if (list?.Count > 0)
                foreach (var m in list)
                { AddLineAtTextboxLog($"{m?.Text}: {m?.Tag}"); }
            AddLineAtTextboxLog();
            AddLineAtTextboxLog(Properties.Resources.SosSymbols);
            
            AddLineAtTextboxLog("-= Query Extra =-");
            list = selectedConfig?.QueryExtraMenuStore?.GetAllItems();
            if (list?.Count > 0)
                foreach (var m in list)
                { AddLineAtTextboxLog($"{m?.Text}: {m?.Tag}"); }
            AddLineAtTextboxLog();
            AddLineAtTextboxLog($"{Properties.Resources.EqualSymbols}{Properties.Resources.EqualSymbols}");
            AddLineAtTextboxLog();
        }

        private void WriteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteConfig();
        }

        private void WriteConfig()
        {
            var t = Task.Run(() =>
            MakeCurrentFullConfig(Configuration.Get, currentSQLConnectionStore.GetCurrent()));
            var c = t.ContinueWith((antecedent) =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    WriteCfgInFile(Configuration.Get);
                }
            });
        }

        public void WriteCfgInFile(ConfigFull<ConfigAbstract> config)
        {
            IWriterable writer = new FileWriter();
            // (writer as FileWriter).EvntInfoMessage -= AddLineAtTextboxLog;
            (writer as FileWriter).EvntInfoMessage += AddLineAtTextboxLog;
            writer.Write(CommonConst.AppCfgFilePath, config);
            //  (writer as FileWriter).EvntInfoMessage -= AddLineAtTextboxLog;
        }
        #endregion

         ///-////-/////-//////-///////////////////////////////////////////

        #region Dynamical Menues - RecentConnection, QueryStandart, QueryExtra
        private void QueryExtraStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        { MakeMenuItemDropDownFromMenuStore(queryExtraMenu, queryExtraStore, ToolStripMenuType.ExtraQuery); }

        private void QueryStandartStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        { MakeMenuItemDropDownFromMenuStore(queryStandartMenu, queryStandartStore, ToolStripMenuType.StandartQuery); }

        private void RecentStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        {             MakeMenuItemDropDownFromMenuStore(changeBaseMenuItem, recentStore, ToolStripMenuType.RecentConnection);         }
        
        private void MakeMenuItemDropDownFromMenuStore(ToolStripMenuItem target, MenuAbstractStore source, ToolStripMenuType menuType)
        {
            IList<ToolStripMenuItem> menuItems = source.GetAllItems();
            AddToMenuItemDropDownItems(target, menuItems, menuType);
        }

        private void AddToMenuItemDropDownItems(ToolStripMenuItem target, IList<ToolStripMenuItem> source, ToolStripMenuType menuType)
        {
            if (target != null && source?.ToArray()?.Count() > 0)
            {
                target.DropDownItems.Clear();

                method = System.Reflection.MethodBase.GetCurrentMethod().Name;
                logger.Info("-= " + method + " =-");

                IList<ToolStripMenuItem> sourceList = new List<ToolStripMenuItem>();
                foreach (var m in source.ToArray())
                {
                    switch (menuType)
                    {
                        case ToolStripMenuType.ExtraQuery:
                            {
                                m.MouseDown -= ExtraQueryMenuItem_MouseDown;
                                m.MouseDown += new MouseEventHandler(ExtraQueryMenuItem_MouseDown);
                                break;
                            }
                        case ToolStripMenuType.StandartQuery:
                            {
                                m.MouseDown -= StandartQueryMenuItem_MouseDown;
                                m.MouseDown += new MouseEventHandler(StandartQueryMenuItem_MouseDown);
                                break;
                            }
                        case ToolStripMenuType.RecentConnection:
                            {
                                m.MouseDown -= RecentConnectionMenuItem_MouseDown;
                                m.MouseDown += new MouseEventHandler(RecentConnectionMenuItem_MouseDown);
                                break;
                            }
                        case ToolStripMenuType.ExternalConfig:
                            {
                                m.MouseDown -= SelectedConfigMenuItem_MouseDown;
                                m.MouseDown += new MouseEventHandler(SelectedConfigMenuItem_MouseDown);
                                break;
                            }
                    }
                    sourceList.Add(m);
                }
                target.DropDownItems.AddRange(sourceList.ToArray());

                statusInfoMainText.SetTempText($"В меню '{target.Text}' добавлено - {source.Count} запросов");
                menuStrip.Update();
                menuStrip.Refresh();
            }
        }

        //removeQueryMenuItem.Text = "Удалить отмеченные пользовательские запросы";
        //removeQueryMenuItem.ToolTipText = "Отметить можно только запросы созданные на данном ПК (подменю 'Пользовательские')";
        private  void ExtraQueryMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            ToolStripMenuItem toolStrip = sender as ToolStripMenuItem;
            string text = toolStrip.Text;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    string queryBody = MenuItemToQuery(toolStrip);
                    if (!(string.IsNullOrWhiteSpace(queryBody)))
                    {
                        _ = GetData(queryBody);
                    }
                    break;

                case MouseButtons.Right:
                    {
                        queryExtraStore.Remove(text);
                        statusInfoMainText.SetTempText($"Из меню '{queryExtraMenu.Text}' удален запрос '{Text}'");
                        break;
                    }
            }
        }
        private  void StandartQueryMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            ToolStripMenuItem toolStrip = sender as ToolStripMenuItem;
            string text = toolStrip.Text;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        string queryBody = MenuItemToQuery(toolStrip);
                        queryBody = queryBody.Replace("{}", $"{txtBodyQuery.Text}");
                        _ = GetData(queryBody);
                        break;
                    }
                case MouseButtons.Right:
                    {
                        queryStandartStore.Remove(text);
                        statusInfoMainText.SetTempText($"Из меню '{queryStandartMenu.Text}' удален запрос '{Text}'");
                        break;
                    }
            }
        }
        private void RecentConnectionMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            ToolStripMenuItem toolStrip = sender as ToolStripMenuItem;
            string text = toolStrip.Text;

            switch (e.Button)
            {
                case MouseButtons.Left:

                    if (Configuration.Get?.Count() > 0)
                    {
                        ConfigUnitStore applicationConfig = GetConfigUnitByName(Configuration.Get, text);

                        ISQLConnectionSettings tmpConnection = applicationConfig.SQLConnection;
                        if (!(string.IsNullOrWhiteSpace(tmpConnection?.Name)))
                        { currentSQLConnectionStore.Set(tmpConnection); }
                    }
                    break;

                case MouseButtons.Right:
                    {
                        if (recentStore?.GetAllItems().Count > 0)
                        {
                            recentStore.Remove(text);
                            statusInfoMainText.SetTempText($"Из меню '{queryStandartMenu.Text}' удален запрос '{Text}'");
                            try
                            {
                                ConfigFull<ConfigAbstract> configFull = Configuration?.Get;
                                configFull.Remove(text);
                                Configuration.Set(configFull);

                                AddLineAtTextboxLog($"Блок {text} из конфигурации удален");
                            }
                            catch
                            { AddLineAtTextboxLog($"Блок {text} в конфигурации отсутствует"); }
                        }
                        break;
                    }
            }
        }
        private void SelectedConfigMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            ToolStripMenuItem toolStrip = sender as ToolStripMenuItem;
            string text = toolStrip.Text;

            switch (e.Button)
            {
                case MouseButtons.Left:

                    if (loadedExternalConfig?.Get()?.Count() > 0)
                    {
                        txtbResultShow.Clear();

                        PrintSelectedConfigConnection(loadedExternalConfig?.Get(), text);
                    }
                    break;

                case MouseButtons.Right:
                    {
                        //todo
                        //Перезаписать выбранную конфигурацию в систему
                        break;
                    }
            }
        }

        private string MenuItemToQuery(ToolStripMenuItem item)
        {
            string queryName = item?.Text?.ToString();
            string queryBody = item?.Tag?.ToString();

            if (queryBody?.Length > 0)
            {
                if (queryName?.Length > 0)
                {
                    txtbNameQuery.Text = queryName;
                    AddLineAtTextboxLog($"Выполняется запрос - '{queryName}':");
                }
            }
            return queryBody;
        }
        #endregion

        ///-////-/////-//////-///////////////////////////////////////////

        #region Start Application settings
        /// <summary>
        /// StatusStrip
        /// </summary>
        private void TurnStatusLabelMenues()
        {
            statusInfoMainText.SetConstText("");

            StatusInfoFilter.Text = "Фильтры";
            StatusInfoFilter.ToolTipText = "Чтоб использовать фильтры предварительно необходимо выбрать пункт меню 'Вид', а затем -'Обновить фильтры'";
            StatusApp.Text = $"{CommonConst.appFileVersionInfo.ProductName} ver.{CommonConst.AppVersion}";


            StatusTables.Text = "Таблицы";

            StatusTables.DropDownItemClicked -= Menu_DropDownItemClicked;
            StatusTables.DropDownItemClicked += Menu_DropDownItemClicked;

            StatusTables.MouseUp -= StatusSplitButton1_Click;
            StatusTables.MouseUp += StatusSplitButton1_Click;
        }

        private void StatusSplitButton1_Click(object sender, MouseEventArgs e)
        {
            string text = null;
            if (sender is ToolStripMenuItem)
            {
                text = (sender as ToolStripMenuItem).Text;

            }
            else if (sender is ToolStripSplitButton)
            {
                text = (sender as ToolStripSplitButton).Text;
            }

            if (!(string.IsNullOrWhiteSpace(text)))
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        Clipboard.SetText(text);
                        break;

                    case MouseButtons.Right:
                        //Clipboard.GetText();
                        break;
                }
            }
        }

        private void TurnAplication()
        {
            //Main Application
            bmpLogo = Properties.Resources.LogoRYIK;
            Text = CommonConst.appFileVersionInfo.Comments + " " + CommonConst.appFileVersionInfo.LegalCopyright;
            Icon = Icon.FromHandle(bmpLogo.GetHicon());
            SplitImage1.Image = bmpLogo;

            //Context Menu for notification
            contextMenu = new ContextMenu();  //Context Menu on notify Icon
            contextMenu.MenuItems.Add("About", HelpAbout);
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add("Restart", ApplicationRestart);
            contextMenu.MenuItems.Add("Exit", ApplicationExit);
            //Notification
            notifyIcon = new NotifyIcon
            {
                Icon = Icon,
                Visible = true,
                BalloonTipText = "Developed by " + CommonConst.appFileVersionInfo.LegalCopyright,
                Text = CommonConst.appFileVersionInfo.ProductName + "\nv." + CommonConst.AppVersion + "\n" + CommonConst.appFileVersionInfo.CompanyName,
                ContextMenu = contextMenu
            };
            notifyIcon.ShowBalloonTip(500);

            //Other controls
            txtBodyQuery.KeyPress += new KeyPressEventHandler(TxtbQuery_KeyPress);
            tooltip.SetToolTip(txtBodyQuery, "Составьте (напишите) SQL-запрос к базе данных и нажмите ENTER");
            txtBodyQuery.LostFocus += new EventHandler(SetToolTipFromTextBox);
            txtbNameQuery.LostFocus += new EventHandler(SetToolTipFromTextBox);

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

            tabPageTable.Controls.Add(dgv);
            dgv.BringToFront();
        }

        private void TurnUpToolStripMenuItems()
        {
            #region Main
            mainMenu.Text = "Главное";
            mainMenu.ToolTipText = "Смена БД, работа с сохраненными запросами, печать отчетов";

            updateFiltersMenuItem.Text = "Обновить фильтры";
            updateFiltersMenuItem.ToolTipText = "Процедура обновления фильтров длительная по времени(до 30 минут) и затратная по ресурсам!";
            updateFiltersMenuItem.Click += new EventHandler(UpdateFiltersMenuItem_Click);

            ExportMenuItem.Text = "Експорт отчета в Excel";
            ExportMenuItem.Click += new EventHandler(ExportMenuItem_Click);

            quitMenuItem.Text = "Выход";
            quitMenuItem.Click += new EventHandler(ApplicationExit);

            changeBaseMenuItem.Text = "Сменить базу";
            changeBaseMenuItem.ToolTipText = "Сменить базу на одну из ранее сохраненных";

            queryStandartMenu.Text = "Стандартные запросы (запросы по маске)";
            queryStandartMenu.ToolTipText = "Предустановленные поисковые запросы." +
                " Формат слова замена которого будет искаться {}. " +
                "Вставить в запросе в месте, где должна быть подмена искомого";

            queryExtraMenu.Text = "Пользовательские запросы";
            queryExtraMenu.ToolTipText = "Сохраненые поисковые запросы, созданные ранее на данном ПК";

            // queriesExtraMenu.DropDown.Closing += (o, e) => { e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked; };//не закрывать меню при отметке
            #endregion

            #region Manage
            managerMenu.Text = "Управление";
            managerMenu.ToolTipText = "Управление функционалом системы, его настройка, управление базами данных (БД)";
           
            administratorMenuItem.Text = "Администратор баз данных(БД)";
            administratorMenuItem.Click += new EventHandler(administratorMenu_Click);

            configurationToolStripMenuItem.Text = "Конфигурация";
            configurationToolStripMenuItem.ToolTipText = "Конфигурация приложения";

            printConfigMenuItem.Text = "Печать конфигурации";
            printConfigMenuItem.ToolTipText = "Печать конфигурации на экран";
            printConfigMenuItem.Click += new EventHandler(PrintConfigToolStripMenuItem_Click);

            printCurrentConfigToolStripMenuItem.Text = "Печать текущей конфигурации";
            printCurrentConfigToolStripMenuItem.ToolTipText = "Печать конфигурации данного подключения на экран";
            printCurrentConfigToolStripMenuItem.Click += new EventHandler(PrintActiveConnectionConfigMenuItem_Click);
            selectedConfigToolStripMenuItem.Text = "Выбрать конфигурацию";
            selectedConfigToolStripMenuItem.ToolTipText = "Работа с выбраной конфигурацией подключения";
            loadConfigMenuItem.Text = "Загрузить конфигурацию с диска";
            loadConfigMenuItem.ToolTipText = "Прочитать конфигурацию";
            loadConfigMenuItem.Click += new EventHandler(LoadConfigMenuItem_Click);

            writeConfigMenuItem.Text = "Записать конфигурацию";
            writeConfigMenuItem.ToolTipText = "Записать конфигурацию в файл на диске";
            writeConfigMenuItem.Click += new EventHandler(WriteFileToolStripMenuItem_Click);

            updateToolStripMenuItem.Text = "Обновление приложения";
            updateToolStripMenuItem.ToolTipText = "Работа с обновлениями приложения - подготовка, деплой, получение";
            downloadUpdateToolStripMenuItem.Text = "Скачивание/проверка нового обновления";
            uploadUpdateToolStripMenuItem.Text = "Деплой обновления на сервер";
            prepareUpdateToolStripMenuItem.Text = "Подготовить пакет обновлений";
            prepareUpdateToolStripMenuItem.ToolTipText = "Подготовить пакет обновлений для выгрузки его на сервер вручную";
            prepareUpdateToolStripMenuItem.Click += new EventHandler(PrepareUpdateMenuItem_Click);
            #endregion

            #region help
            helpMenu.Text = "Помощь";
            helpAboutMenuItem.Text = "О программе";
            helpAboutMenuItem.Click += new EventHandler(HelpAbout);
            helpUsingMenuItem.Text = "Порядок использования программы";
            #endregion

            foreach (var m in menuStrip.Items)
            {
                if (m is ToolStripMenuItem)
                { (m as ToolStripMenuItem).MouseHover += new EventHandler(ToolStripMenuItem_MouseHover); }
                else if (m is ToolStripItem)
                { (m as ToolStripItem).MouseHover += new EventHandler(ToolStripMenuItem_MouseHover); }
            }
        }
        #endregion

        ///-////-/////-//////-///////////////////////////////////////////

        #region AdministratorForm
        /// <summary>
        /// /////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void administratorMenu_Click(object sender, EventArgs e)
        {
            RunAdministratorForm();
        }

        private void RunAdministratorForm()
        {
            ISQLConnectionSettings tmpConnection = currentSQLConnectionStore?.GetCurrent();

            administratorForm = new AdministratorForm
            {
                Owner = this,
                Icon = Icon.FromHandle(bmpLogo.GetHicon()),
                Text = CommonConst.appFileVersionInfo.Comments + " " + CommonConst.appFileVersionInfo.LegalCopyright,
                currentSQLConnectionStore =
                string.IsNullOrWhiteSpace(tmpConnection?.Name) == true ?
                new SQLConnectionStore() :
                new SQLConnectionStore(tmpConnection)
            };

            administratorForm.FormClosing += new FormClosingEventHandler(AdministratorForm_FormClosing);
            administratorForm.Show();

            Hide();
        }

        private void AdministratorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ISQLConnectionSettings tmp = administratorForm?.currentSQLConnectionStore?.GetCurrent();
            //Set connection, DBConnection, toolstrip and Config
            if (tmp?.Database != null && tmp?.Name != null)
            {
                currentSQLConnectionStore.Set(tmp);
            }

            //Destroy AdministratorForm
            administratorForm?.Dispose();

            //Show Main Form
            Show();

            //Check Data
            AddLineAtTextboxLog($"Set {currentSQLConnectionStore}:");
            AddLineAtTextboxLog($"Name: {currentSQLConnectionStore?.GetCurrent()?.Name}");
            AddLineAtTextboxLog($"ProviderName: {currentSQLConnectionStore?.GetCurrent()?.ProviderName}");
            AddLineAtTextboxLog($"Host: {currentSQLConnectionStore?.GetCurrent()?.Host}");
            AddLineAtTextboxLog($"Port: {currentSQLConnectionStore?.GetCurrent()?.Port}");
            AddLineAtTextboxLog($"Username: {currentSQLConnectionStore?.GetCurrent()?.Username}");
            AddLineAtTextboxLog($"Password: {currentSQLConnectionStore?.GetCurrent()?.Password}");
            AddLineAtTextboxLog($"Database: {currentSQLConnectionStore?.GetCurrent()?.Database}");
            AddLineAtTextboxLog($"Table: {currentSQLConnectionStore?.GetCurrent()?.Table}");
        }
        #endregion

        ///-////-/////-//////-///////////////////////////////////////////

        #region Filters
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void UpdateFiltersMenuItem_Click(object sender, EventArgs e)
        {
            statusInfoMainText.SetTempText("Построение фильтров...");

            dgv.Enabled = false;
            txtbResultShow.Enabled = false;
            mainMenu.Enabled = false;
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
            mainMenu.Enabled = true;
            dgv.Enabled = true;

            statusInfoMainText.SetTempText("Построение фильтров завершено.");
        }

        private void StatusInfoFilter_Click(object sender, EventArgs e)
        {
            ToolStripSplitButton f;

            string word = string.Join(",", CommonConst.TRANSLATION.Values.ToArray()) + "," + string.Join(", ", CommonConst.TRANSLATION.Keys.ToArray());

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
        }

        private async Task BuildFilters()
        {
            AddLineAtTextboxLog("Выполняется чтение базы и формирование библиотеки уникальных слов.");
            AddLineAtTextboxLog("Фильтры строятся на основе данных алиасов колонок:");
            AddLineAtTextboxLog(string.Join(", ", CommonConst.TRANSLATION.Values.ToArray()));
            AddLineAtTextboxLog();

            await Task.Run(() => filtersTable = (dBOperations as SQLiteModelDBOperations).GetFilterList(CommonConst.TRANSLATION, "CarAndOwner"));

            AddLineAtTextboxLog("Построение фильтров завершено");
        }

        private void BuildFiltersMenues(IModelEntityDB<DBColumnModel> filtersTable)
        {
            MenuFiltersMaker menuMaker;
            ToolStripSplitButton filterSplitButton = null;
            ToolStripMenuItem subFilterMenu;

            foreach (var column in filtersTable.ColumnCollection.Take(5))
            {
                menuMaker = new MenuFiltersMaker(CommonConst.TRANSLATION);
                if (!(column?.Name?.Length > 0 && column?.ColumnCollection?.Count > 0))
                {
                    continue;
                }

                filterSplitButton = menuMaker.MakeDropDownToSplitButton(column?.Name, column?.Alias);
                filterSplitButton.DropDownItemClicked += new ToolStripItemClickedEventHandler(Menu_DropDownItemClicked);
                if (filterSplitButton == null)
                {
                    continue;
                }
                statusFilters.Items.Add(filterSplitButton);

                foreach (var f in column.ColumnCollection.Take(40))
                {
                    subFilterMenu = menuMaker.MakeFilterMenuItem(f?.Name);
                    if (subFilterMenu == null)
                    {
                        continue;
                    }
                    filterSplitButton.DropDownItems.Add(subFilterMenu);
                }
            }
        }

        //выбранный пункт из ДропДаунМеню сделать названием фильтра
        private void Menu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string text = (e.ClickedItem.Text).Split('(')[0].Trim();
            (sender as ToolStripSplitButton).Text = text;

            ISQLConnectionSettings tmpSettings = currentSQLConnectionStore?.GetCurrent();
            tmpSettings.Table = text;
            if (currentSQLConnectionStore?.GetCurrent()?.Table.Equals(tmpSettings.Table) == false)
            {
                statusInfoMainText.SetTempText($"База: {tmpSettings.Database}, сменилась таблица на {tmpSettings.Table}");
            }

            lstColumn.Clear();
            foreach (var col in SQLSelector.GetColumns(tmpSettings).ToModelList())
            {
                string name = col.Name.Equals(col.Alias) ? col.Name : $"{col.Name} ({col.Alias})";
                lstColumn.Add(name);
            }
            comboBoxColumns.Update();
            comboBoxColumns.Refresh();
        }
        #endregion

        ///-////-/////-//////-///////////////////////////////////////////


        #region Get Data from DB  
        ControlState state = new ControlState();
        ControlStateCaretaker controlState = new ControlStateCaretaker();

        /// <summary>
        /// Запомнить текущее состояние контролов
        /// </summary>
        private void SaveControlState()
        {
            bool[] enabledControls = new bool[]
                       {
                        mainMenu.Enabled,
                        managerMenu.Enabled,
                        helpMenu.Enabled,
                        txtBodyQuery.Enabled,
                        txtbNameQuery.Enabled,
                        txtbResultShow.ReadOnly,
                        dgv.Enabled,
                        StatusStripInfo.Enabled
                        };

            //Запомнить текущее состояние контролов
            state.SetState(enabledControls);
            controlState?.History?.Push(state?.SaveState());
        }

        /// <summary>
        /// Заблокировать контролы
        /// </summary>
        private void LockControl()
        {
            mainMenu.Enabled = false;
            managerMenu.Enabled = false;
            helpMenu.Enabled = false;
            txtBodyQuery.Enabled = false;
            txtbNameQuery.Enabled = false;
            txtbResultShow.ReadOnly = true;
            dgv.Enabled = false;
            StatusStripInfo.Enabled = false;
        }

        /// <summary>
        /// Восстановить предыдущее состояние контролов
        /// </summary>
        private void RestoreControlState()
        {
            state?.RestoreStateq(controlState?.History?.Pop());

            mainMenu.Enabled = state.ControlEnabled[0];
            managerMenu.Enabled = state.ControlEnabled[1];
            helpMenu.Enabled = state.ControlEnabled[2];
            txtBodyQuery.Enabled = state.ControlEnabled[3];
            txtbNameQuery.Enabled = state.ControlEnabled[4];
            txtbResultShow.ReadOnly = state.ControlEnabled[5];
            dgv.Enabled = state.ControlEnabled[6];
            StatusStripInfo.Enabled = state.ControlEnabled[7];
            Task.Delay(500).Wait();
        }


        private async Task GetData(string query)
        {
            dgv.Columns.Clear();

            string currentDb = currentSQLConnectionStore.GetCurrent().Database;
            AddLineAtTextboxLog($"Запрос:{Environment.NewLine}{query}{Environment.NewLine}к базе данных:{Environment.NewLine}{currentDb}");
            AddLineAtTextboxLog();

            string constText = statusInfoMainText.GetConstText;
            await Task.Run(() => statusInfoMainText.SetConstText($"Ждите. Выполняется поиск в БД - '{currentSQLConnectionStore.GetCurrent().Database}'..."));
            await Task.Run(() => statusInfoMainText.SetTempText(constText));

            await Task.Run(() => SaveControlState());
            await Task.Run(() => LockControl());

            dtForStore = await dBOperations.GetDataTable(query);
            dtForShow = dtForStore.Copy();
            dgv.DataSource = dtForShow;

            await Task.Run(() => statusInfoMainText.SetConstText(constText));
            AddLineAtTextboxLog($"В '{currentSQLConnectionStore.GetCurrent().Database}' найдено: {dtForShow?.Rows?.Count} записей");
            statusInfoMainText.SetTempText($"Найдено записей: {dtForShow?.Rows?.Count}");
            
            // Переключиться на таблицу
            SelectTable();

            //Восстановить предыдущее состояние контролов
            await Task.Run(() => RestoreControlState());
        }
        #endregion

        ///-////-/////-//////-///////////////////////////////////////////

        #region Export to Excel
        private async void ExportMenuItem_Click(object sender, EventArgs e)
        {
            statusInfoMainText.SetTempText($"Идет генерация отчетов...");
            dgv.Enabled = false;
            mainMenu.Enabled = false;
            txtBodyQuery.Enabled = false;

            await WriteDataTableInTableExcel(dtForShow);

            txtBodyQuery.Enabled = true;
            mainMenu.Enabled = true;
            dgv.Enabled = true;
        }

        private async Task WriteDataTableInTableExcel(DataTable source)
        {
            if (source != null || source?.Columns?.Count > 0 || source?.Rows?.Count > 0)
            {
                string report = txtbNameQuery.Text.Trim();
                string reportBody = txtBodyQuery.Text.Trim();
                int countWordsInQuery = reportBody.Split(' ').Length;

                //muliplier of skipping millions
                int muliplier = (int)Math.Ceiling((decimal)source.Rows.Count / (decimal)1000000);

                string fileName;
                FileInfo fi;
                DataTable dtTemp;

                for (int part = 0; part < muliplier; part++)
                {
                    if (countWordsInQuery < 4)
                    {
                        if (string.IsNullOrWhiteSpace(reportBody))
                            fileName = $"{report}";
                        else
                            fileName = $"{report}_{reportBody}";
                    }
                    else
                    {
                        fileName = $"{CommonConst.appFileVersionInfo.ProductName}";

                        if (!string.IsNullOrWhiteSpace(report))
                        { fileName += $"_{report}"; }
                    }

                    if (muliplier > 1)
                    { fileName += $"_{part}"; }
                    
                    fileName += $".xlsx";

                    dtTemp = source.Clone();
                    source.AsEnumerable()
                        .Skip(part * 1000000)
                        .Take(1000000)
                        .CopyToDataTable(dtTemp, LoadOption.OverwriteChanges);
                    fi = new FileInfo(fileName);

                    await ExportToFile(fi, dtTemp, CommonConst.AppVersion);

                    dtTemp?.Dispose();
                }
            }
            else
            {
                AddLineAtTextboxLog("В отчете отсутствуют какие-либо данные");
            }
        }

        private async Task ExportToFile(FileInfo fi, DataTable dtTemp, string nameSheet)
        {
            try
            {
                await Task.Run(() =>
               dtTemp.ExportToExcel($"{fi.FullName}", nameSheet, TypeOfPivot.NonePivot, null, null, true));

                statusInfoMainText.SetTempText($"Результаты отчета сохранены в файле: '{fi.FullName}'");
            }
            catch (Exception err)
            {
                statusInfoMainText.SetTempText($"Ошибка сохранения отчета {nameSheet}");
                AddLineAtTextboxLog(err.ToString());
            }
        }
        #endregion

        ///-////-/////-//////-///////////////////////////////////////////

        #region Quit Menu
        private void ApplicationRestart(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MakeCurrentFullConfig(Configuration.Get, currentSQLConnectionStore.GetCurrent());
            WriteCfgInFile(Configuration.Get);

            ApplicationQuit();
        }

        private void ApplicationExit(object sender, EventArgs e)
        {
            ApplicationQuit();
        }

        private void ApplicationQuit()
        {
            Text = @"Closing application...";

            currentSQLConnectionStore.EvntConfigChanged -= CurrentSQLConnectionStore_EvntConfigChanged;

            (recentStore as MenuItemStore).EvntCollectionChanged -= RecentStore_EvntCollectionChanged;
            (queryExtraStore as MenuItemStore).EvntCollectionChanged -= QueryExtraStore_EvntCollectionChanged;
            (queryStandartStore as MenuItemStore).EvntCollectionChanged -= QueryStandartStore_EvntCollectionChanged;
            (tableStore as MenuItemStore).EvntCollectionChanged -= TablesStore_EvntCollectionChanged;
            statusInfoMainText.EvntSetText -= StatusInfoMainText_SetTemporaryText;
            logger.Info("");
            logger.Info("");
            logger.Info($"{Properties.Resources.SosSlashSymbols}{Properties.Resources.SosSlashSymbols}");


            dtForShow?.Dispose();
            dtForStore?.Dispose();
            dgv?.Dispose();
            tooltip?.Dispose();
            administratorForm?.Dispose();
            helpForm?.Dispose();
            bmpLogo?.Dispose();
            notifyIcon?.Dispose();
            contextMenu?.Dispose();

            Application.Exit();
        }
        #endregion

        ///-////-/////-//////-///////////////////////////////////////////

        #region Help Menu
        private void HelpAbout(object sender, EventArgs e)
        {
            helpForm = new HelpForm();
            helpForm.Show();
        }
        #endregion

        ///-////-/////-//////-///////////////////////////////////////////

        #region Control Behavior
        /// <summary>
        ///  Переключиться на лог
        /// </summary>
        void SelectLog()
        {
            PresenterTabCotrol.SelectedTab = tabPageTextBox;
            txtbResultShow.SelectionStart = txtbResultShow.Text.Length > 0 ? txtbResultShow.Text.Length - 1 : 0;// txtbResultShow.Text.Length;
            txtbResultShow.ScrollToCaret();
        }

        /// <summary>
        /// Переключиться на таблицу
        /// </summary>
        void SelectTable()
        {
            PresenterTabCotrol.SelectedTab = tabPageTable;
        }

        private void ToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            string text = null;
            if (sender is ToolStripMenuItem)
                text = (sender as ToolStripMenuItem).ToolTipText;
            else if (sender is ToolStripItem)
                text = (sender as ToolStripItem).ToolTipText;

            if (!string.IsNullOrWhiteSpace(text))
                statusInfoMainText.SetTempText(text);
        }

        private void AddLineAtTextboxLog(object sender, TextEventArgs text)
        { AddLineAtTextboxLog(text?.Message); }

        private void AddLineAtTextboxLog(string text = null)
        {
            if (OperatingModes == AppModes.Admin)
            {
                txtbResultShow.AppendLine($"{text}");
                logger.Trace($"{text}");
            }
            else
            {
                logger.Info($"{text}");
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

        private void TxtbQuery_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)//если нажата Enter
            {
                AddLineAtTextboxLog(Properties.Resources.DashedSymbols);

                string query = (sender as TextBox).Text.Trim();

                AddLineAtTextboxLog($"Query:{Environment.NewLine}{query}");
                string[] arrQuery = query.Split(' ');

                if ((
                    arrQuery[0].ToLower() == "select" && arrQuery.Length > 3
                    && arrQuery.Where(w => w.ToLower().Contains("select")).Count() > 0
                    && arrQuery.Where(w => w.ToLower().Contains("from")).Count() > 0
                    ) || OperatingModes == AppModes.Admin)
                {
                    //add query at queryStore collections
                    string nameQuery = txtbNameQuery.Text.Trim();
                    if (!(string.IsNullOrWhiteSpace(nameQuery)) && arrQuery.Length > 2)
                    {
                        ToolStripMenuItem menu = new ToolStripMenuItem(nameQuery)
                        {
                            Tag = query
                        };

                        if (query.Contains(@"{}"))
                        {
                            queryStandartStore.Add(menu);
                            statusInfoMainText.SetTempText($"В меню Стандартные запросы сохранен запрос '{nameQuery}'");
                            AddLineAtTextboxLog($"В меню Стандартные запросы сохранен запрос - '{nameQuery}':{Environment.NewLine}{query}");
                            return;
                        }
                        else
                        {
                            queryExtraStore.Add(menu);
                            statusInfoMainText.SetTempText($"В меню Пользовательские запросы сохранен запрос '{nameQuery}'");
                            AddLineAtTextboxLog($"В меню Пользовательские запросы сохранен запрос - '{nameQuery}' :{Environment.NewLine}{query}");
                        }
                    }

                    if (arrQuery.Length < 3)
                    {
                        MessageBox.Show("Для использования Стандартных запросов, после ввода параметра в строке поиска, " +
                            "вам нужно выбрать в меню один из ранее сохраненных Стандартных запросов");
                        return;
                    }

                    DialogResult doQuery =
                        MessageBox.Show($"Выполнить ваш запрос?\r\n{query}", "Проверьте свой запрос", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (doQuery == DialogResult.OK)
                    {
                        _ = GetData(query);
                    }
                    else
                    {
                        AddLineAtTextboxLog("Отмена задания.");
                    }
                }
                else
                {
                    MessageBox.Show("Разрешено использование только выборок без модификации БД!\r\nПроверьте свой запрос на правльность!",
                        "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DgvDataSourceAccessFromOtherThread(DataTable dt)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(delegate
                {
                    dgv.DataSource = dt;
                }));
            else
                dgv.DataSource = dt;
        }

        #endregion
        ///-////-/////-//////-///////////////////////////////////////////

        #region Start Environment Application
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
            arguments.EvntInfoMessage += new CommandLineArguments.InfoMessage(AddLineAtTextboxLog);
            arguments.CheckCommandLineArguments(args);

            if (args?.Length > 1)
            {
                //remove delimiters
                string envParameter = args[1]?.Trim()?.TrimStart('-', '/')?.ToLower();
                if (envParameter.StartsWith("a"))
                {
                    OperatingModes = AppModes.Admin;
                    managerMenu.Enabled = true;
                }
                else if (envParameter.StartsWith("u"))
                {
                    OperatingModes = AppModes.Updater;
                    managerMenu.Enabled = true;
                }
            }
            else
            {
                OperatingModes = AppModes.User;
                managerMenu.Enabled = false;
            }

            arguments.EvntInfoMessage -= AddLineAtTextboxLog;
        }


        #endregion

        ///-////-/////-//////-///////////////////////////////////////////
    }
}