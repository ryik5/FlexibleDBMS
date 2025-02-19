using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public partial class MainForm : Form
    {
        static string method = MethodBase.GetCurrentMethod().Name;
        //method = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //CommonExtesions.Logger(LogTypes.Info,"-= " + method + " =-");

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

        ConfigStore Configuration;
        SQLConnectionStore currentSQLConnectionStore = null;
        IModelEntityDB<DBColumnModel> filtersTable = null; // меню фильтров в строке статуса

        MenuAbstractStore recentStore;
        MenuAbstractStore queryExtraStore;
        MenuAbstractStore queryStandartStore;
        MenuAbstractStore tableNameStore;
        BindingList<string> lstColumn = new BindingList<string>();

        ItemFlipper statusInfoMainText;

        ApplicationUpdater Updater;



        ///-////-/////-//////-///////////////////////////////////////////
        ///-////-/////-//////-///////////////////////////////////////////


        public MainForm()
        {
            InitializeComponent();

            Updater = new ApplicationUpdater();
            Configuration = new ConfigStore();

            currentSQLConnectionStore = new SQLConnectionStore();
            recentStore = new MenuItemStore();
            queryExtraStore = new MenuItemStore();
            queryStandartStore = new MenuItemStore();
            tableNameStore = new MenuItemStore();
            (tableNameStore as MenuItemStore).EvntCollectionChanged += new MenuItemStore.ItemAddedInCollection<BoolEventArgs>(TablesStore_EvntCollectionChanged);

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
            PrepareAplication();

            //Turn Up Menu
            TurnUpToolStripMenuItems();

            //Turn Up StatusStrip
            TurnStatusLabelMenues();

            ConfigFull<ConfigAbstract> tmpConfig = LoadConfig(CommonConst.AppCfgFilePath);
            ISQLConnectionSettings tmpConnection = GetDefaultConnectionFromConfig(tmpConfig);

            AddLineAtTextboxLog($"{Properties.Resources.SymbolsEqual}{Properties.Resources.SymbolsEqual}");
            AddLineAtTextboxLog($"Name of the last connection:{Environment.NewLine}{tmpConnection?.Name}");

            if (tmpConfig != null && !(string.IsNullOrWhiteSpace(tmpConnection?.Name)))
            {
                ConfigUnitStore unitConfig = GetConfigUnitStoreFromFullConfigByName(tmpConfig, tmpConnection?.Name);

                currentSQLConnectionStore.Set(unitConfig?.SQLConnection);

                queryExtraStore.Set(unitConfig?.QueryExtraMenuStore?.GetAllItems());
                queryStandartStore.Set(unitConfig?.QueryStandartMenuStore?.GetAllItems());

                recentStore.Set(tmpConfig?.GetUnitConfigNames()?.ToToolStripMenuItemList());

                // Set the full configuration (Otherwise, consider this as the first launch of the software and the configuration is empty)
                Configuration.Set(tmpConfig);
            }

            if (string.IsNullOrWhiteSpace(tmpConnection?.Name) || string.IsNullOrWhiteSpace(tmpConnection?.Database))
            {
                RunAdministratorForm();
            }

            currentSQLConnectionStore.EvntConfigChanged += new SQLConnectionStore.ConfigChanged<BoolEventArgs>(CurrentSQLConnectionStore_EvntConfigChanged);
            (recentStore as MenuItemStore).EvntCollectionChanged += new MenuItemStore.ItemAddedInCollection<BoolEventArgs>(RecentStore_EvntCollectionChanged);
            (queryExtraStore as MenuItemStore).EvntCollectionChanged += new MenuItemStore.ItemAddedInCollection<BoolEventArgs>(QueryExtraStore_EvntCollectionChanged);
            (queryStandartStore as MenuItemStore).EvntCollectionChanged += new MenuItemStore.ItemAddedInCollection<BoolEventArgs>(QueryStandartStore_EvntCollectionChanged);

            currentSQLConnectionStore.Refresh();
            menuStrip.Update();
            menuStrip.Refresh();

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
        void MakeCurrentFullConfig(ConfigFull<ConfigAbstract> newConfig = null, ISQLConnectionSettings newConnection = null)
        {
            ConfigFull<ConfigAbstract> configFull = newConfig;

            if (newConfig == null)
            {
                configFull = new ConfigFull<ConfigAbstract>();
            }

            if (newConnection == null)
            {
                newConnection = currentSQLConnectionStore?.GetCurrent();
            }

            ConfigAbstract configUnit = new Config
            {
                Name = CommonConst.MAIN,
                Version = CommonConst.AppVersion,
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
            {
                configFull.Add(configUnit);
            }


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

        #region SQL Connection was Changed
        void CurrentSQLConnectionStore_EvntConfigChanged(object sender, BoolEventArgs args)
        {
            _ = CurrentSQLConnectionStore_ConfigChanged();
        }

        async Task CurrentSQLConnectionStore_ConfigChanged()
        {
            ISQLConnectionSettings oldSettings = currentSQLConnectionStore?.GetPrevious();
            ISQLConnectionSettings newSettings = currentSQLConnectionStore?.GetCurrent();
            MakeCurrentFullConfig(Configuration?.Get, oldSettings);
            queryStandartStore.Clear();
            queryExtraStore.Clear();

            //Save a current state of the interface Controls
            await Task.Run(() => SaveControlState());

            //Block interface from a user influence
            await Task.Run(() => BlockControl());

            //await Task.Run(() =>
            //{
            AddLineAtTextboxLog($"{Properties.Resources.SymbolsDashedLong}{Environment.NewLine}" +
                $"Switching to new settings...");
            //});


            if (!(string.IsNullOrWhiteSpace(newSettings?.Name)))
            {
                recentStore.Add(new ToolStripMenuItem(newSettings.Name));

                //await Task.Run(() =>
                //{
                ConfigUnitStore applicationConfig = GetConfigUnitStoreFromFullConfigByName(Configuration.Get, newSettings.Name);
                queryStandartStore.Set(applicationConfig?.QueryStandartMenuStore?.GetAllItems());
                queryExtraStore.Set(applicationConfig?.QueryExtraMenuStore?.GetAllItems());
                //});

                if (!(string.IsNullOrWhiteSpace(newSettings?.Database)))
                {

                    if (oldSettings?.Database != newSettings?.Database)
                    {
                        //await Task.Run(() =>
                        //{
                        AddLineAtTextboxLog($"Selected server connection: '{newSettings?.Host}'{Environment.NewLine}" +
                                                $" Database: '{newSettings?.Database}'{Environment.NewLine}" +
                                                $" Main table: '{newSettings?.Table}'{Environment.NewLine}");
                        statusInfoMainText.SetConstText($"Selected database: {newSettings.Database}");
                        //});


                        //fix the case when a DB on the server was not found - cancelation task after 10 sec waiting
                        statusInfoMainText.SetTempText($"Fetching table list...");
                        var cancellationToken = new System.Threading.CancellationTokenSource(10000).Token;//timeout = 10 sec
                        int timeout = 10000; //timeout = 10 sec
                        var task = SetTables(cancellationToken, newSettings);
                        if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) == task)
                        {
                            await task;
                        }

                        if (tableNameStore?.GetAllItems()?.Count > 0)
                        {
                            AddLineAtTextboxLog($"The table list contains: {tableNameStore?.GetAllItems()?.Count} items");
                        }
                        else
                        {
                            tableNameStore.Clear();
                            AddLineAtTextboxLog($"{Environment.NewLine}{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}{Environment.NewLine}{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}{Environment.NewLine}{Properties.Resources.SymbolsSosSlashBack}" +
                                $"            " +
                                $"The list of tables from the server was not received." +
                                $"            " +
                                $"{Properties.Resources.SymbolsSosSlash}{Environment.NewLine}{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}{Environment.NewLine}{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}{Environment.NewLine}");
                        }
                    }
                    else if (oldSettings?.Database == newSettings?.Database && oldSettings?.Table != newSettings?.Table)
                    {
                        statusInfoMainText.SetTempText($"Database: {newSettings.Database}, table changed to {newSettings.Table}");
                    }
                }
            }

            AddLineAtTextboxLog($"{Properties.Resources.SymbolsDashedLong}");

            //Restore previous state of controls
            await Task.Run(() => RestoreControlState());
        }

        Task SetTables(System.Threading.CancellationToken token, ISQLConnectionSettings newSettings)
        {
            return Task.Run(() => tableNameStore.Set(SQLSelector.GetTables(newSettings).GetToolStipMenuItemList()), token);
        }
        #endregion


        /// <summary>
        /// Switching text temporarily when setting text as temporary
        /// </summary>
        /// <param name="sender">StatusInfoMain</param>
        /// <param name="e">true - temporary, false - permanent</param>
        async void StatusInfoMainText_SetTemporaryText(object sender, TextEventArgs e)
        {
            CommonExtensions.Logger(LogTypes.Info, "StatusInfoMainText: " + e.Message);
            AddLineAtTextboxLog(e?.Message);

            StatusInfoMain.Text = e.Message;
            await Task.Delay(3500);
            StatusInfoMain.Text = statusInfoMainText.GetConstText;
        }

        void TablesStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        {
            StatusTables_anotherThreadAccess();
        }

        void StatusTables_anotherThreadAccess() //add string into  from other threads
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(delegate
                {
                    StatusTables.Enabled = false;

                    if (StatusTables.DropDownItems?.Count > 0)
                        StatusTables.DropDownItems.Clear();

                    if (tableNameStore?.GetAllItems()?.Count > 0)
                        StatusTables.DropDownItems.AddRange(tableNameStore.GetAllItems().ToArray());

                    StatusTables.DropDown.Refresh();
                    StatusTables.Text = "Tables";
                    StatusTables.Enabled = true;
                }));
            else
            {
                StatusTables.Enabled = false;

                if (StatusTables.DropDownItems?.Count > 0)
                    StatusTables.DropDownItems.Clear();

                if (tableNameStore?.GetAllItems()?.Count > 0)
                    StatusTables.DropDownItems.AddRange(tableNameStore.GetAllItems().ToArray());

                StatusTables.DropDown.Refresh();
                StatusTables.Text = "Tables";
                StatusTables.Enabled = true;
            }
        }


        ///-////-/////-//////-///////////////////////////////////////////


        #region Update Application
        void PrepareUpdateMenuItem_Click(object sender, EventArgs e)
        {
            UserAD user = null;
            string serverURL = null;
            _ = Updater.SetOptionsAsync(user, serverURL);
            Updater.PrepareUpdateFiles();
        }

        void CheckUpdateMenuItem_Click(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        void CheckUpdate()
        {
            UserAD user = new UserAD();
            //настройки имени и пароля вставить в конфиг - файл            
            string serverURL = null;

            _ = Updater.SetOptionsAsync(user, serverURL);
            if (!string.IsNullOrWhiteSpace(Updater?.Options?.serverUpdateURI))
            {
                string constText = statusInfoMainText.GetConstText;

                Updater.RunUpdate();

                Task.Delay(3000).Wait();
                statusInfoMainText.SetConstText(constText);
            }
            else
            {
                statusInfoMainText.SetTempText("The update server address was not found.");
            }
        }

        /// <summary>
        /// Check and download new update 
        /// </summary>
        /// <param name="minutes">check period of a new Update</param>
        /// <returns></returns>
        Task CheckUpdatePeriodicaly(int minutes = 1)
        {
            UserAD user = new UserAD();
            string serverURL = null;
            _ = Updater.SetOptionsAsync(user, serverURL);
            return Updater.CheckUpdatePeriodicaly(minutes);
        }

        void UploadUpdateMenuItem_Click(object sender, EventArgs e)
        {
            UploadUpdate();
        }

        void UploadUpdate()
        {
            UserAD user = new UserAD();
            //insert username and password settings into config file
            string serverURL = null;
            string pathToExternalUpdateZip = null;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                pathToExternalUpdateZip = ofd.OpenFileDialogReturnPath(Properties.Resources.DialogZipFile,
                    "Select a pre-prepared update archive or click Cancel to generate automatically by the system:");
            }

            _ = Updater.SetOptionsAsync(user, serverURL, pathToExternalUpdateZip);
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
                statusInfoMainText.SetTempText("Update server address not found.");
            }
        }
        #endregion


        ///-////-/////-//////-///////////////////////////////////////////


        #region Configuration Unit Block
        ConfigFull<ConfigAbstract> loadedExternalConfig;

        ConfigFull<ConfigAbstract> LoadConfig(string pathToConfig)
        {
            ConfigFull<ConfigAbstract> tmpConfig = null;
            FileReader readerConfig = new FileReader();
            readerConfig.EvntInfoMessage += new FileReader.InfoMessage(AddLineAtTextboxLog);

            readerConfig.ReadConfig(pathToConfig);

            if (readerConfig?.config != null && readerConfig?.config?.Config?.Count() > 0)
            {
                tmpConfig = (readerConfig)?.config;
            }
            else
            {
                AddLineAtTextboxLog(Properties.Resources.SymbolsEqual);
                AddLineAtTextboxLog($"Config '{CommonConst.AppCfgFilePath}' is empty or broken.");
                AddLineAtTextboxLog(Properties.Resources.SymbolsEqual);
            }

            readerConfig.EvntInfoMessage -= AddLineAtTextboxLog;

            readerConfig = null;
            return tmpConfig;
        }

        void LoadConfigMenuItem_Click(object sender, EventArgs e)
        {
            MenuAbstractStore tmpConfigStore = new MenuItemStore();
            string pathToFile = null;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                pathToFile = ofd.OpenFileDialogReturnPath(Properties.Resources.DialogBakFile, "Select the configuration file:");
                if (ofd.CheckFileExists)
                {
                    loadedExternalConfig = LoadConfig(CommonConst.AppCfgFilePath);
                    tmpConfigStore.Set(loadedExternalConfig.GetUnitConfigNames().ToToolStripMenuItemList());
                    MakeMenuItemDropDownFromMenuStore(selectedConfigToolStripMenuItem, tmpConfigStore, ToolStripMenuType.ExternalConfig);
                    selectedConfigToolStripMenuItem.Text = $"Loaded configuration - {loadedExternalConfig.Version} from ({loadedExternalConfig.LastModification})";
                    applyLoadedConfigToolStripMenuItem.ToolTipText = $"Loaded configuration - {loadedExternalConfig.Version} from ({loadedExternalConfig.LastModification})";
                }
            }
        }

        ConfigUnitStore GetConfigUnitStoreFromFullConfigByName(ConfigFull<ConfigAbstract> tmpConfigFull, string text)
        {
            AddLineAtTextboxLog($"{Properties.Resources.SymbolsEqual}{Properties.Resources.SymbolsEqual}");
            AddLineAtTextboxLog("Searching for: " + text);
            AddLineAtTextboxLog();

            ISQLConnectionSettings connection = null;
            MenuAbstractStore queryStandart = null;
            MenuAbstractStore queryExtra = null;
            MenuAbstractStore recent = null;
            string version = null;
            DateTime timeStamp = DateTime.Now;

            if (tmpConfigFull?.Count() > 0)
            {
                IList<string> recentList = tmpConfigFull.GetUnitConfigNames();
                if (recentList?.Count > 0)
                {
                    //result = $"{Properties.Resources.SosSymbols}{Environment.NewLine}" + $"Recent menu:";

                    recent = new MenuItemStore();

                    foreach (var menu in recentList)
                    {
                        //result += $"{Environment.NewLine}{menu}";

                        MenuItem item = new MenuItem(menu);
                        recent.Add(item.ToQueryMenuToolStripMenuItem());
                    }

                    //  AddLineAtTextboxLog(result);
                }

                foreach (var confUnit in tmpConfigFull?.Config)
                {
                    ConfigAbstract unit = confUnit.Value;

                    if (confUnit.Key.Equals(text) && unit?.ConfigDictionary?.Count() > 0) //Нашел!
                    {
                        AddLineAtTextboxLog("Found!");

                        ConfigAbstract tmpConfigUnit;

                        bool exist = unit.ConfigDictionary.TryGetValue(nameof(ISQLConnectionSettings), out object tmpConf);
                        if (exist)
                        {
                            tmpConfigUnit = tmpConf as Config;
                            connection = tmpConfigUnit?.ConfigDictionary?.ToISQLConnectionSettings();

                            //AddLineAtTextboxLog($"{Properties.Resources.SosSymbols}{Environment.NewLine}" + $"SQL connection:{Environment.NewLine}{connection.AsString()}");
                        }
                        version = (tmpConf as Config).Version;
                        timeStamp = (tmpConf as Config).LastModification;

                        exist = unit.ConfigDictionary.TryGetValue(nameof(ToolStripMenuType.StandartQuery), out tmpConf);
                        if (exist)
                        {
                            tmpConfigUnit = tmpConf as Config;
                            queryStandart = new MenuItemStore();
                            IList<MenuItem> data = tmpConfigUnit?.ConfigDictionary?.ToMenuItems();
                            if (data?.Count > 0)
                            {
                                //result = $"{Properties.Resources.SosSymbols}{Environment.NewLine}" +  $"Standart menu:";

                                foreach (var menu in data)
                                {
                                    //result += $"{Environment.NewLine}{menu?.Text}:{menu?.Tag}";

                                    MenuItem item = new MenuItem(menu?.Text, menu?.Tag);
                                    queryStandart.Add(item.ToQueryMenuToolStripMenuItem());
                                }
                                //AddLineAtTextboxLog(result);
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
                                //result = $"{Properties.Resources.SosSymbols}{Environment.NewLine}" +  $"Extra menu:";

                                foreach (var menu in data)
                                {
                                    //result += $"{Environment.NewLine}{menu?.Text}:{menu?.Tag}";

                                    MenuItem item = new MenuItem(menu?.Text, menu?.Tag);
                                    queryExtra.Add(item.ToQueryMenuToolStripMenuItem());
                                }
                                //AddLineAtTextboxLog(result);
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
                RecentMenuStore = recent,
                Version = version,
                TimeStamp = timeStamp
            };

            //AddLineAtTextboxLog($"{Properties.Resources.DashedSymbols}{Properties.Resources.DashedSymbols}");

            return applicationNewConfig;
        }

        ISQLConnectionSettings GetDefaultConnectionFromConfig(ConfigFull<ConfigAbstract> config)
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

        IList<string> ReturnRecentConnectionNamesFromConfig(ConfigFull<ConfigAbstract> config)
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

        void PrintConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtbResultShow.Clear();
            PrintFullConfig(Configuration.Get);
        }

        void PrintFullConfig(ConfigFull<ConfigAbstract> config)
        {
            AddLineAtTextboxLog($"{Properties.Resources.SymbolsEqualLong}{Environment.NewLine}{Properties.Resources.SymbolsEqualLong}{Environment.NewLine}" +
                $"-= currentConfig connectionSettings Names =-{Environment.NewLine}" +
                $"{ReturnRecentConnectionNamesFromConfig(config).ToStringNewLine()}{Environment.NewLine}{Properties.Resources.SymbolsDashed}{Environment.NewLine}" +
                $"-= SQL connection =-{Environment.NewLine}{currentSQLConnectionStore.GetCurrent()?.ToString()}{Properties.Resources.SymbolsDashed}{Environment.NewLine}" +
                $"-= Loaded Full Config =-");
            IList<string> names = ReturnRecentConnectionNamesFromConfig(config);
            foreach (var name in names)
            {
                AddLineAtTextboxLog($"{Environment.NewLine}{Properties.Resources.SymbolsSos}{Environment.NewLine}" +
                    $"-= {name} =-{Environment.NewLine}");
                PrintSelectedConfigConnection(config, name);
            }

            AddLineAtTextboxLog($"{Environment.NewLine}{Properties.Resources.SymbolsEqualLong}{Environment.NewLine}{Properties.Resources.SymbolsEqualLong}{Environment.NewLine}");
        }


        void PrintSelectedConfigConnectionMenuItem_Click(object sender, EventArgs e)
        {
            txtbResultShow.Clear();
            PrintSelectedConfigConnection(Configuration.Get, currentSQLConnectionStore.GetCurrent().Name);
        }


        void PrintSelectedConfigConnection(ConfigFull<ConfigAbstract> fullConfig, string selectedConfigName)
        {
            ConfigUnitStore selectedConfig = GetConfigUnitStoreFromFullConfigByName(fullConfig, selectedConfigName);

            AddLineAtTextboxLog($"{Properties.Resources.SymbolsEqual}{Environment.NewLine}" +
                $"-= SQL Connection Settings =-{Environment.NewLine}" +
                $"ver. {selectedConfig?.Version} {selectedConfig?.TimeStamp.ToString()}{Environment.NewLine}{Environment.NewLine}" +
                $"{selectedConfig?.SQLConnection?.ToString()}{Environment.NewLine}{Environment.NewLine}{Properties.Resources.SymbolsSos}{Environment.NewLine}" +

                $"-= Query Standart =-");
            IList<ToolStripMenuItem> list = selectedConfig?.QueryStandartMenuStore?.GetAllItems();
            if (list?.Count > 0)
                foreach (var m in list)
                {
                    AddLineAtTextboxLog($"{m?.Text}: {m?.Tag}");
                }

            AddLineAtTextboxLog($"{Environment.NewLine}{Properties.Resources.SymbolsEqual}{Environment.NewLine}" +

                $"-= Query Extra =-");
            list = selectedConfig?.QueryExtraMenuStore?.GetAllItems();
            if (list?.Count > 0)
                foreach (var m in list)
                {
                    AddLineAtTextboxLog($"{m?.Text}: {m?.Tag}");
                }

            AddLineAtTextboxLog($"{Environment.NewLine}{Properties.Resources.SymbolsEqual}{Environment.NewLine}");
        }


        void WriteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                filePath = sfd.SaveFileDialogReturnPath($"{CommonConst.AppName}", Properties.Resources.DialogCfgFile,
                    "Specify the file location and name:");
            }

            WriteConfig(filePath ?? CommonConst.AppCfgFilePath);
        }

        void WriteConfig(string fileName)
        {
            var t = Task.Run(() =>
            MakeCurrentFullConfig(Configuration.Get, currentSQLConnectionStore.GetCurrent()));
            var c = t.ContinueWith((antecedent) =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    WriteCfgInFile(loadedExternalConfig ?? Configuration.Get, fileName);
                }
            });
        }

        void WriteCfgInFile(ConfigFull<ConfigAbstract> config, string fileName)
        {
            CommonExtensions.Logger(LogTypes.Info, fileName);
            IWriterable writer = new FileWriter();
            // (writer as FileWriter).EvntInfoMessage -= AddLineAtTextboxLog;
            (writer as FileWriter).EvntInfoMessage += AddLineAtTextboxLog;
            writer.Write(fileName, config);
            //  (writer as FileWriter).EvntInfoMessage -= AddLineAtTextboxLog;
        }
        #endregion


        ///-////-/////-//////-///////////////////////////////////////////


        #region Dynamical Menues - RecentConnection, QueryStandart, QueryExtra
        void QueryExtraStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        {
            MakeMenuItemDropDownFromMenuStore(queryExtraMenu, queryExtraStore, ToolStripMenuType.ExtraQuery);
        }

        void QueryStandartStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        {
            MakeMenuItemDropDownFromMenuStore(queryStandartMenu, queryStandartStore, ToolStripMenuType.StandartQuery);
        }

        void RecentStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        {
            MakeMenuItemDropDownFromMenuStore(changeBaseMenuItem, recentStore, ToolStripMenuType.RecentConnection);
        }

        void MakeMenuItemDropDownFromMenuStore(ToolStripMenuItem target, MenuAbstractStore source, ToolStripMenuType menuType)
        {
            IList<ToolStripMenuItem> menuItems = source.GetAllItems();
            AddToMenuItemDropDownItems(target, menuItems, menuType);
        }

        void AddToMenuItemDropDownItems(ToolStripMenuItem target, IList<ToolStripMenuItem> source, ToolStripMenuType menuType)
        {
            if (target != null && source?.ToArray()?.Count() > 0)
            {
                target.DropDownItems.Clear();

                method = MethodBase.GetCurrentMethod().Name;
                CommonExtensions.Logger(LogTypes.Info, "-= " + method + " =-");

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

                statusInfoMainText.SetTempText($"Added {source.Count} queries to the '{target.Text}' menu");
                menuStrip.Update();
                menuStrip.Refresh();
            }
        }

        //removeQueryMenuItem.Text = "Удалить отмеченные пользовательские запросы";
        //removeQueryMenuItem.ToolTipText = "Отметить можно только запросы созданные на данном ПК (подменю 'Пользовательские')";
        void ExtraQueryMenuItem_MouseDown(object sender, MouseEventArgs e)
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
                        statusInfoMainText.SetTempText($"Query '{Text}' has been removed from the '{queryExtraMenu.Text}' menu");
                        break;
                    }
            }
        }
        void StandartQueryMenuItem_MouseDown(object sender, MouseEventArgs e)
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
                        statusInfoMainText.SetTempText($"Query '{Text}' has been removed from the '{queryStandartMenu.Text}' menu");
                        break;
                    }
            }
        }
        void RecentConnectionMenuItem_MouseDown(object sender, MouseEventArgs e)
        {
            ToolStripMenuItem toolStrip = sender as ToolStripMenuItem;
            string text = toolStrip.Text;

            switch (e.Button)
            {
                case MouseButtons.Left:

                    if (Configuration.Get?.Count() > 0)
                    {
                        ConfigUnitStore applicationConfig = GetConfigUnitStoreFromFullConfigByName(Configuration.Get, text);

                        ISQLConnectionSettings tmpConnection = applicationConfig.SQLConnection;
                        if (!(string.IsNullOrWhiteSpace(tmpConnection?.Name)))
                        {
                            currentSQLConnectionStore.Set(tmpConnection);
                        }
                    }
                    break;

                case MouseButtons.Right:
                    {
                        if (recentStore?.GetAllItems().Count > 0)
                        {
                            recentStore.Remove(text);
                            statusInfoMainText.SetTempText($"Query '{Text}' has been removed from the '{queryStandartMenu.Text}' menu");
                            try
                            {
                                ConfigFull<ConfigAbstract> configFull = Configuration?.Get;
                                configFull.Remove(text);
                                Configuration.Set(configFull);

                                AddLineAtTextboxLog($"Block {text} has been removed from the configuration");
                            }
                            catch
                            {
                                AddLineAtTextboxLog($"Block {text} is not found in the configuration");
                            }
                        }
                        break;
                    }
            }
        }
        void SelectedConfigMenuItem_MouseDown(object sender, MouseEventArgs e)
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
                        ApplySelectedConfig(text);
                        break;
                    }
            }
        }

        void ApplySelectedConfig(string text)
        {
            DialogResult dialog = MessageBox.Show("Apply the selected configuration to the system?", $"Apply {text}:", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.OK)
            {
                ConfigAbstract selectedConfig = loadedExternalConfig.GetUnit(text);
                ConfigFull<ConfigAbstract> tmpConfig = Configuration.Get;
                tmpConfig.Add(selectedConfig);
                Configuration.Set(tmpConfig);
                recentStore.Set(tmpConfig?.GetUnitConfigNames()?.ToToolStripMenuItemList());

                statusInfoMainText.SetTempText($"Configuration added '{text}' ver. '{(selectedConfig as Config).Version}' from '{(selectedConfig as Config).LastModification}'");
            }
            else
            {
                statusInfoMainText.SetTempText($"Configuration not changed. Queries '{Configuration.Get.GetUnitConfigNames().Count}' - " +
                    $"ver. '{Configuration.Get.Version}' from '{Configuration.Get.LastModification}'");
            }
        }

        void ApplyLoadedConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Replace system configuration with the loaded one?", $"Replace with {loadedExternalConfig.Version} from {loadedExternalConfig.LastModification}:", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.OK)
            {
                Configuration.Set(loadedExternalConfig);

                statusInfoMainText.SetTempText($"Configuration replaced with ver. '{loadedExternalConfig.Version}' from '{loadedExternalConfig.LastModification}'");
            }
            else
            {
                statusInfoMainText.SetTempText($"Loaded configuration has been ignored.");
            }
        }

        string MenuItemToQuery(ToolStripMenuItem item)
        {
            string queryName = item?.Text?.ToString();
            string queryBody = item?.Tag?.ToString();

            if (queryBody?.Length > 0)
            {
                if (queryName?.Length > 0)
                {
                    txtbNameQuery.Text = queryName;
                    AddLineAtTextboxLog($"Executing query - '{queryName}':");
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
        void TurnStatusLabelMenues()
        {
            statusInfoMainText.SetConstText("");

            StatusInfoFilter.Text = "Filters";
            StatusInfoFilter.ToolTipText = "To use filters, you must first select the 'View' menu item, and then 'Update filters'";
            StatusApp.Text = $"{CommonConst.appFileVersionInfo.ProductName} ver.{CommonConst.AppVersion}";


            StatusTables.Text = "Tables";

            StatusTables.DropDownItemClicked -= Menu_DropDownItemClicked;
            StatusTables.DropDownItemClicked += Menu_DropDownItemClicked;

            StatusTables.MouseUp -= StatusSplitButton1_Click;
            StatusTables.MouseUp += StatusSplitButton1_Click;
        }

        void StatusSplitButton1_Click(object sender, MouseEventArgs e)
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

        void PrepareAplication()
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
            tooltip.SetToolTip(txtBodyQuery, "Compose (write) an SQL query to the database and press ENTER");
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

        void TurnUpToolStripMenuItems()
        {
            #region Main
            mainMenu.Text = "Main";
            mainMenu.ToolTipText = "Switch Database, work with saved queries, print reports";

            updateFiltersMenuItem.Text = "Update Filters";
            updateFiltersMenuItem.ToolTipText = "Filter update procedure is time-consuming (up to 30 minutes) and resource-intensive!";
            updateFiltersMenuItem.Click += new EventHandler(UpdateFiltersMenuItem_Click);

            ExportMenuItem.Text = "Export Report to Excel";
            ExportMenuItem.Click += new EventHandler(ExportMenuItem_Click);

            quitMenuItem.Text = "Exit";
            quitMenuItem.Click += new EventHandler(ApplicationExit);

            changeBaseMenuItem.Text = "Switch Database";
            changeBaseMenuItem.ToolTipText = "Switch to one of the previously saved databases";

            queryStandartMenu.Text = "Standard Queries (Queries by Mask)";
            queryStandartMenu.ToolTipText = "Pre-set search queries." +
                " The format of the word to be searched for is {}. " +
                "Insert in the query where the search word should be replaced";

            queryExtraMenu.Text = "User Queries";
            queryExtraMenu.ToolTipText = "Saved search queries created earlier on this PC";

            // queriesExtraMenu.DropDown.Closing += (o, e) => { e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked; };//не закрывать меню при отметке
            #endregion

            #region Manager
            managerMenu.Text = "Management";
            managerMenu.ToolTipText = "Management of system functionality, its configuration, and database managemen";

            administratorMenuItem.Text = "Database Administrator";
            administratorMenuItem.Click += new EventHandler(administratorMenu_Click);

            configurationToolStripMenuItem.Text = "Configuration";
            configurationToolStripMenuItem.ToolTipText = "Application configuration";

            loadConfigMenuItem.Text = "Load Configuration from Disk";
            loadConfigMenuItem.ToolTipText = "Read configuration";
            loadConfigMenuItem.Click += new EventHandler(LoadConfigMenuItem_Click);

            printConfigMenuItem.Text = "Print Entire Configuration";
            printConfigMenuItem.ToolTipText = "Print entire configuration on screen";
            printConfigMenuItem.Click += new EventHandler(PrintConfigToolStripMenuItem_Click);

            printCurrentConfigToolStripMenuItem.Text = "Print Active Connection Configuration";
            printCurrentConfigToolStripMenuItem.ToolTipText = "Print configuration of the current connection on screen";
            printCurrentConfigToolStripMenuItem.Click += new EventHandler(PrintSelectedConfigConnectionMenuItem_Click);
            selectedConfigToolStripMenuItem.Text = "Select Configuration";
            selectedConfigToolStripMenuItem.ToolTipText = "Work with the selected connection configuration";

            writeConfigMenuItem.Text = "Write Configuration";
            writeConfigMenuItem.ToolTipText = "Save configuration to a file on disk";
            writeConfigMenuItem.Click += new EventHandler(WriteFileToolStripMenuItem_Click);

            applyLoadedConfigToolStripMenuItem.Text = "ЗReplace Entire Configuration with Loaded One";
            applyLoadedConfigToolStripMenuItem.Click += new EventHandler(ApplyLoadedConfigToolStripMenuItem_Click);

            updateToolStripMenuItem.Text = "Application Update";
            updateToolStripMenuItem.ToolTipText = "Work with application updates - preparation, deployment, and retrieval";
            downloadUpdateToolStripMenuItem.Text = "Download/Check for New Update";
            uploadUpdateToolStripMenuItem.Text = "Deploy Update to Server";
            prepareUpdateToolStripMenuItem.Text = "Prepare Update Package";
            prepareUpdateToolStripMenuItem.ToolTipText = "Prepare update package for manual upload to server";
            prepareUpdateToolStripMenuItem.Click += new EventHandler(PrepareUpdateMenuItem_Click);
            #endregion

            #region help
            helpMenu.Text = "Help";
            helpAboutMenuItem.Text = "About";
            helpAboutMenuItem.Click += new EventHandler(HelpAbout);
            helpUsingMenuItem.Text = "Program Usage Instructions";
            #endregion

            foreach (var m in menuStrip.Items)
            {
                if (m is ToolStripMenuItem)
                {
                    (m as ToolStripMenuItem).MouseHover += new EventHandler(ToolStripMenuItem_MouseHover);
                }
                else if (m is ToolStripItem)
                {
                    (m as ToolStripItem).MouseHover += new EventHandler(ToolStripMenuItem_MouseHover);
                }
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
        void administratorMenu_Click(object sender, EventArgs e)
        {
            RunAdministratorForm();
        }

        void RunAdministratorForm()
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

        void AdministratorForm_FormClosing(object sender, FormClosingEventArgs e)
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
        async void UpdateFiltersMenuItem_Click(object sender, EventArgs e)
        {
            statusInfoMainText.SetTempText("Building filters...");

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
            StatusInfoFilter.ToolTipText = "Search using selected filters in the current table";
            StatusInfoFilter.Click += new EventHandler(StatusInfoFilter_Click);

            txtBodyQuery.Enabled = true;
            txtbResultShow.Enabled = true;
            mainMenu.Enabled = true;
            dgv.Enabled = true;

            statusInfoMainText.SetTempText("Building filters completed.");
        }

        void StatusInfoFilter_Click(object sender, EventArgs e)
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
                            {
                                res += " AND " + tag + " LIKE '" + txt + "' ";
                            }
                            else
                            {
                                res += tag + " LIKE '" + txt + "' ";
                            }
                        }
                    }
                }
            }
            AddLineAtTextboxLog("A filter has been applied to the table: " + res);

            if (dgv != null && dtForStore?.Rows?.Count > 0 && dtForStore?.Columns?.Count > 0)
            {
                using (DataTable dtTemp = dtForStore.Select(res).CopyToDataTable())
                {
                    dgv.DataSource = dtTemp;
                }
            }
        }

        SqlAbstractConnector dBOperations;// = new SQLiteDBOperations(sqLiteConnectionString, dbFileInfo);
        async Task BuildFilters()
        {
            AddLineAtTextboxLog("Reading database and forming a library of unique words.");
            AddLineAtTextboxLog("Filters are built based on column alias data:");
            AddLineAtTextboxLog(string.Join(", ", CommonConst.TRANSLATION.Values.ToArray()));
            AddLineAtTextboxLog();

            dBOperations = SQLSelector.SetConnector(currentSQLConnectionStore?.GetCurrent());
            dBOperations.EvntInfoMessage += new SqlAbstractConnector.Message<TextEventArgs>(DBOperations_EvntInfoMessage);

            await Task.Run(() => filtersTable = (dBOperations as SQLiteModelDBOperations).GetFilterList(CommonConst.TRANSLATION, "CarAndOwner"));

            AddLineAtTextboxLog("Filter construction completed.");
        }

        void DBOperations_EvntInfoMessage(object sender, TextEventArgs e)
        {
            AddLineAtTextboxLog(e.Message);
        }

        void BuildFiltersMenues(IModelEntityDB<DBColumnModel> filtersTable)
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

        // Selected item from DropDown menu to make filter name
        void Menu_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string text = (e.ClickedItem.Text).Split('(')[0].Trim();
            (sender as ToolStripSplitButton).Text = text;

            ISQLConnectionSettings tmpSettings = currentSQLConnectionStore?.GetCurrent();
            tmpSettings.Table = text;
            if (currentSQLConnectionStore?.GetCurrent()?.Table.Equals(tmpSettings.Table) == false)
            {
                statusInfoMainText.SetTempText($"Database: {tmpSettings.Database}, table changed to {tmpSettings.Table}");
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
        readonly ControlState state = new ControlState();
        readonly ControlStateCaretaker controlState = new ControlStateCaretaker();

        /// <summary>
        /// Remember the current state of controls
        /// </summary>
        void SaveControlState()
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

            //Remember the current state of controls
            state.SetState(enabledControls);
            controlState?.History?.Push(state?.SaveState());
        }

        /// <summary>
        /// Block controls
        /// </summary>
        void BlockControl()
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
        /// Restore previous state of controls
        /// </summary>
        void RestoreControlState()
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

        /// <summary>
        /// timeout of this task = 30 sec
        /// </summary>
        /// <param name="query">Search words in Cyrilic looks like - '"SELECT * from таблиці WHERE CustomLike(стовпчик, 'текст')"'</param>
        /// <returns></returns>
        async Task GetData(string query)
        {
            dgv.Columns.Clear();
            ISQLConnectionSettings newSettings = currentSQLConnectionStore?.GetCurrent();

            AddLineAtTextboxLog($"{Environment.NewLine}{Properties.Resources.SymbolsDashedLong}" +
                $"{Environment.NewLine}Query executed{Environment.NewLine}{Properties.Resources.SymbolsResult}{query}" +
                $"{Environment.NewLine}to database{Environment.NewLine}{Properties.Resources.SymbolsResult}{newSettings.Database}" +
                $"{Environment.NewLine}{Properties.Resources.SymbolsDashed}{Environment.NewLine}");

            string constText = statusInfoMainText.GetConstText;
            statusInfoMainText.SetConstText($"Please wait. Searching in DB {Properties.Resources.SymbolsResult} '{newSettings.Database}'...");
            statusInfoMainText.SetTempText(constText);

            //Save a current state of the interface Controls
            await Task.Run(() => SaveControlState());

            //Block interface from a user influence
            await Task.Run(() => BlockControl());

            if (!(string.IsNullOrWhiteSpace(newSettings?.Database)))
            {
                //timeout = 120 sec
                var cancellation = new System.Threading.CancellationTokenSource();
                cancellation.CancelAfter(120000);
                var cancellationToken = cancellation.Token;// new System.Threading.CancellationTokenSource(30000).Token;
                int timeout = 120000;
                var task = GetTables(cancellationToken, newSettings, query);
                if (await Task.WhenAny(task, Task.Delay(timeout, cancellationToken)) == task)
                {
                    await task;
                }

                await Task.Run(() => statusInfoMainText.SetConstText(constText));

                if (ResultData?.GetDataTable()?.Rows?.Count > 0)
                {
                    dtForStore = ResultData.GetDataTable();

                    // Switch to table
                    SelectTable();

                    AddLineAtTextboxLog($"In '{newSettings.Database}' found: {dtForStore?.Rows?.Count} records");
                    statusInfoMainText.SetTempText($"Found records: {dtForStore?.Rows?.Count}");
                }
                else
                {
                    dtForStore = new DataTable();

                    // Switch to log
                    SelectLog();

                    AddLineAtTextboxLog($"{Environment.NewLine}{Properties.Resources.SymbolsSosSlashBack}{Environment.NewLine}" +
                        $"Error retrieving data{Environment.NewLine}{Properties.Resources.SymbolsResult}" +
                        $"{ResultData?.Errors}{Environment.NewLine}" +
                        $"Check database availability{Environment.NewLine}{Properties.Resources.SymbolsResult}" +
                        $"{newSettings.Database}{Environment.NewLine}" +
                        $"and query correctness{Environment.NewLine}{Properties.Resources.SymbolsResult}" +
                        $"{query}{Environment.NewLine}{Properties.Resources.SymbolsSosSlashBack}{Environment.NewLine}");
                }

                dtForShow = dtForStore.Copy();
                dgv.DataSource = dtForShow;
                cancellation.Dispose();
            }

            //Восстановить предыдущее состояние контролов
            await Task.Run(() => RestoreControlState());
            AddLineAtTextboxLog($"{Properties.Resources.SymbolsDashedLong}");
        }


        DataTableStore ResultData = new DataTableStore();
        Task GetTables(System.Threading.CancellationToken token, ISQLConnectionSettings settings, string query)
        {
            return Task.Run(() => ResultData.Set(SQLSelector.GetDataTableStore(settings, query)), token);
        }

        #endregion


        ///-////-/////-//////-///////////////////////////////////////////


        #region Export to Excel
        async void ExportMenuItem_Click(object sender, EventArgs e)
        {
            statusInfoMainText.SetTempText("Generating reports...");
            dgv.Enabled = false;
            mainMenu.Enabled = false;
            txtBodyQuery.Enabled = false;
            string report = txtbNameQuery.Text.Trim();
            string reportBody = txtBodyQuery.Text.Trim();
            int countWordsInQuery = reportBody.Split(' ').Length;
            string filePath;
            if (countWordsInQuery < 4)
            {
                if (string.IsNullOrWhiteSpace(reportBody))
                    filePath = $"{report}";
                else
                    filePath = $"{report}_{reportBody}";
            }
            else
            {
                filePath = $"{CommonConst.appFileVersionInfo.ProductName}";

                if (!string.IsNullOrWhiteSpace(report))
                {
                    filePath += $"_{report}";
                }
            }


            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                filePath = sfd.SaveFileDialogReturnPath(filePath, Properties.Resources.DialogExcelFile,
                    "Specify the file location and name, if necessary:");
            }

            await WriteDataTableInTableExcel(dtForShow, filePath);

            txtBodyQuery.Enabled = true;
            mainMenu.Enabled = true;
            dgv.Enabled = true;
        }

        async Task WriteDataTableInTableExcel(DataTable source, string fullFileName)
        {
            if (source != null || source?.Columns?.Count > 0 || source?.Rows?.Count > 0)
            {
                string report = txtbNameQuery.Text.Trim();
                string reportBody = txtBodyQuery.Text.Trim();
                int countWordsInQuery = reportBody.Split(' ').Length;

                //muliplier of skipping millions
                int muliplier = (int)Math.Ceiling((decimal)source.Rows.Count / (decimal)1000000);

                string fileName = string.Empty; ;
                FileInfo fi;
                DataTable dtTemp;

                for (int part = 0; part < muliplier; part++)
                {


                    if (!string.IsNullOrWhiteSpace(fullFileName))
                    {
                        fileName = fullFileName;
                    }


                    if (muliplier > 1)
                    {
                        fileName += $"_{part}";
                    }

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
                AddLineAtTextboxLog("The report does not contain any data");
            }
        }

        async Task ExportToFile(FileInfo fi, DataTable dtTemp, string nameSheet)
        {
            try
            {
                await Task.Run(() =>
                dtTemp.ExportToExcel($"{fi.FullName}", nameSheet, TypeOfPivot.NonePivot, null, null, true));

                statusInfoMainText.SetTempText($"Report results saved to file: '{fi.FullName}'");
            }
            catch (Exception err)
            {
                statusInfoMainText.SetTempText($"Error saving report {nameSheet}");
                AddLineAtTextboxLog(err.ToString());
            }
        }
        #endregion


        ///-////-/////-//////-///////////////////////////////////////////


        #region Quit Menu
        void ApplicationRestart(object sender, EventArgs e)
        {
            Application.Restart();
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MakeCurrentFullConfig(Configuration.Get, currentSQLConnectionStore.GetCurrent());
            WriteCfgInFile(Configuration.Get, CommonConst.AppCfgFilePath);

            ApplicationQuit();
        }

        void ApplicationExit(object sender, EventArgs e)
        {
            ApplicationQuit();
        }

        void ApplicationQuit()
        {
            Text = "Closing application...";

            currentSQLConnectionStore.EvntConfigChanged -= CurrentSQLConnectionStore_EvntConfigChanged;

            (recentStore as MenuItemStore).EvntCollectionChanged -= RecentStore_EvntCollectionChanged;
            (queryExtraStore as MenuItemStore).EvntCollectionChanged -= QueryExtraStore_EvntCollectionChanged;
            (queryStandartStore as MenuItemStore).EvntCollectionChanged -= QueryStandartStore_EvntCollectionChanged;
            (tableNameStore as MenuItemStore).EvntCollectionChanged -= TablesStore_EvntCollectionChanged;
            statusInfoMainText.EvntSetText -= StatusInfoMainText_SetTemporaryText;
            CommonExtensions.Logger(LogTypes.Info, "");
            CommonExtensions.Logger(LogTypes.Info, "");
            CommonExtensions.Logger(LogTypes.Info, $"{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}");


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
        void HelpAbout(object sender, EventArgs e)
        {
            helpForm = new HelpForm();
            helpForm.Show();
        }
        #endregion


        ///-////-/////-//////-///////////////////////////////////////////


        #region Control Behavior
        /// <summary>
        /// Switch to log
        /// </summary>
        void SelectLog()
        {
            PresenterTabCotrol.SelectedTab = tabPageTextBox;
            txtbResultShow.SelectionStart = txtbResultShow.Text.Length > 0 ? txtbResultShow.Text.Length - 1 : 0;// txtbResultShow.Text.Length;
            txtbResultShow.ScrollToCaret();
        }

        /// <summary>
        /// Switch to the tab with the table display form
        /// </summary>
        void SelectTable()
        {
            PresenterTabCotrol.SelectedTab = tabPageTable;
        }

        void ToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            string text = null;
            if (sender is ToolStripMenuItem)
                text = (sender as ToolStripMenuItem).ToolTipText;
            else if (sender is ToolStripItem)
                text = (sender as ToolStripItem).ToolTipText;

            if (!string.IsNullOrWhiteSpace(text))
                statusInfoMainText.SetTempText(text);
        }

        void AddLineAtTextboxLog(object sender, TextEventArgs text)
        {
            AddLineAtTextboxLog(text?.Message);
        }

        void AddLineAtTextboxLog(string text = null)
        {
            if (OperatingModes == AppModes.Admin)
            {
                txtbResultShow.AppendLine($"{text}");
                CommonExtensions.Logger(LogTypes.Info, $"{text}");
            }
            else
            {
                CommonExtensions.Logger(LogTypes.Info, $"{text}");
            }
        }

        void SetToolTipFromTextBox(object sender, EventArgs e)
        {
            string text = (sender as TextBox).Text;
            if (!(string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)))
            {
                tooltip = new ToolTip();
                tooltip.SetToolTip((sender as TextBox), text);
            }
        }

        void TxtbQuery_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)//if pressed 'Enter'
            {
                AddLineAtTextboxLog(Properties.Resources.SymbolsDashed);

                string query = (sender as TextBox).Text.Trim();

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
                            statusInfoMainText.SetTempText($"Query saved to Standard Queries menu: '{nameQuery}'");
                            AddLineAtTextboxLog($"Query saved to Standard Queries menu - '{nameQuery}':{Environment.NewLine}{query}");
                            return;
                        }
                        else
                        {
                            queryExtraStore.Add(menu);
                            statusInfoMainText.SetTempText($"Query saved to Custom Queries menu: '{nameQuery}'");
                            AddLineAtTextboxLog($"Query saved to Custom Queries menu - '{nameQuery}' :{Environment.NewLine}{query}");
                        }
                    }

                    if (arrQuery.Length < 3)
                    {
                        MessageBox.Show("To use standard queries, after entering the parameter in the search box, " +
                            "you need to select one of the previously saved standard queries from the menu.");
                        return;
                    }

                    DialogResult doQuery =
                        MessageBox.Show($"Execute your query?\r\n{query}", "Check your query", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (doQuery == DialogResult.OK)
                    {
                        _ = GetData(query);
                    }
                    else
                    {
                        AddLineAtTextboxLog("Canceling task.");
                    }
                }
                else
                {
                    MessageBox.Show("Only non-modifying database queries are allowed!\r\nPlease check your query for correctness!",
                   "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
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

        private void helpUsingMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        ///-////-/////-//////-///////////////////////////////////////////
    }
}