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

        DbSchema schemaDB = null;
        IList<string> tablesDB;

        FileReader<CarAndOwner> reader;
        const int MAX_ELEMENTS_COLLECTION = 100000;

        DataGridView dgv;
        ToolTip tooltip;

        IList<ToolStripMenuItem> listExtraMenu;

        public Form1()
        {
            InitializeComponent();

            CheckInputedParametersEnvironment();

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

            //Menu
            administratorMenu.Text = "Administrator";
            createLocalDBMenuItem.Click += CreateLocalDBMenuItem_Click;
            importFromTextFileMenuItem.Click += ImportFromTextFileMenuItem_Click;
            importFromTextFileMenuItem.ToolTipText = "Import Text File in local DB";
            writeModelsListMenuItem.ToolTipText = "Write List with Models in DB";
            //view menu
            viewMenu.Text = "Вид";
            viewMenu.ToolTipText = "Отобразить данные";
            changeViewPanelviewMenuItem.Text = "Show as table";
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
            loadDataMenuItem.Text = "Данные по крупным владельцам автопарков";
            loadDataMenuItem.Click += GetEnterpriseWithHugeAutoparkMenuItem_Click;
            getFIOMenuItem.Text = "Все ФИО в БД";
            getFIOMenuItem.Click += GetFIOMenuItem_Click;
            getEnterpriseMenuItem.Text = "Все предприятия в БД";
            getEnterpriseMenuItem.Click += GetEnterpriseMenuItem_Click;
            //query extra menu 
            queriesExtraMenu.Text = "Дополнительные";
            queriesExtraMenu.ToolTipText = "Запросы созданные на данном ПК";
            // queriesExtraMenu.DropDown.Closing += (o, e) => { e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked; };//не закрывать меню при отметке
            //query extra menu items

            removeQueryExtraMenuItem.Text = "Удалить отмеченные запросы";
            removeQueryExtraMenuItem.Click += RemoveCheckedInQueryExtraMenuItem_Click;


            //Other controls
            txtbBodyQuery.KeyPress += TxtbQuery_KeyPress;
            txtbBodyQuery.LostFocus += SetToolTipTextBox;
            txtbNameQuery.LostFocus += SetToolTipTextBox;

            StatusLabelMain.Text = "";
            StatuslabelBtnImage.Image = bmpLogo;

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

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StatusLabelMain.Text = "Сохраняю пункты пользовательского меню...";
          await Task.Run(()=> SaveQueryExtraMenuInRegistry());
        }

        private void SaveQueryExtraMenuInRegistry()
        {
            IList<ToolStripItem> toolItems = GetQueryExtraMenuItems();
            IList<MenuItemLast> menuItems = ConvertToMenuItemsList(toolItems);
            WriteRegistry(menuItems);
        }

        private IList<ToolStripItem> GetQueryExtraMenuItems()
        {
            IList<ToolStripItem> list = new List<ToolStripItem>();

            foreach (ToolStripMenuItem m in queriesExtraMenu.DropDownItems)
            {
                list.Add(m);
            }

            return list;
        }
        
        private IList<MenuItemLast> ConvertToMenuItemsList(IList<ToolStripItem> items)
        {
            IList<MenuItemLast> list = new List<MenuItemLast>();

            foreach (var m in items)
            {
                MenuItemLast menuItem = new MenuItemLast(m.Text, m.Tag.ToString());

                list.Add(menuItem);
            }

            return list;
        }


        private void RemoveCheckedInQueryExtraMenuItem_Click(object sender, EventArgs e)
        {
            IList<ToolStripItem> listRemove = GetQueryExtraMenuItems();

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
        }

        private void AddQueryExtraMenuItem_Click(object sender, EventArgs e)
        {
            string nameQuery = txtbNameQuery.Text.Trim();
            string bodyQuery = txtbBodyQuery.Text.Trim();

            MenuItemLast menuItem = new MenuItemLast(nameQuery, bodyQuery);

            ToolStripMenuItem item = MakeQueryMenuItem(menuItem);

            queriesExtraMenu.DropDownItems.Add(item as ToolStripMenuItem);
            item.Click += QueryMenuItem_Click;
            menuStrip.Update();
            menuStrip.Refresh();
            StatusLabelMain.Text = $"Запрос '{nameQuery}' сохранен и в меню добавлен";
        }

        private ToolStripMenuItem MakeQueryMenuItem(MenuItemLast menuItem)
        {
            ToolStripMenuItem item = new ToolStripMenuItem()
            {
                Name = menuItem.Name,
                Text = menuItem.NameQuery,
                Tag = menuItem.BodyQuery,
                Checked = true,
                CheckOnClick = true,
                CheckState = CheckState.Unchecked,
                // DoubleClickEnabled = true,
                Size = new Size(180, 22),
            };
            return item;
        }

        private async void QueryMenuItem_Click(object sender, EventArgs e)
        {
            string queryMenu = (sender as ToolStripMenuItem).Name.ToString();
            string queryName = (sender as ToolStripMenuItem).Text.ToString();
            string queryBody = (sender as ToolStripMenuItem).Tag.ToString();

            StatusLabelMain.ToolTipText = $"Выполняется запрос {queryName}";
            //MessageBox.Show("queryMenu: " + queryMenu + "\r\nqueryName: " + queryName + "\r\nqueryBody: " + queryBody);
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

            dgv = null;

            bmpLogo?.Dispose();

            notifyIcon?.Dispose();
            contextMenu?.Dispose();

            System.Threading.Thread.Sleep(500);

            Application.Exit();

        }


        private async Task GetData(string query)
        {
            txtbResultShow.AppendText("\r\nЗапрос:\r\n" + query);
            StatusLabelMain.Text = "Выполняется отбор данных...";
            StatusLabelExtraInfo.ToolTipText = $"Выполняется запрос:\r\n{query}";

            queriesStandartMenu.Enabled = false;
            queriesExtraMenu.Enabled = false;
            viewMenu.Enabled = false;
            txtbResultShow.Enabled = false;


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
                StatusLabelMain.Text = $"Количество записей: {dt.Rows.Count}";
                txtbBodyQuery.Text = query;
            }
            catch (SQLiteException dbsql)
            {
                txtbResultShow.AppendText("\r\nОшибка в запросе:\r\n-----\r\n" + dbsql.Message + "\r\n-----\r\n" + dbsql.ToString() + "\r\n");
                StatusLabelMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(true);
            }
            catch (OutOfMemoryException e)
            {
                txtbResultShow.AppendText("\r\nВаш запрос очень общий и тяжелый для БД. Кокретизируйте запрашиваемые поля или уменьшите выборку:\r\n-----\r\n" + e.Message + "\r\n-----\r\n" + e.ToString() + "\r\n-----\r\n");
                StatusLabelMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(true);
            }
            catch (Exception e)
            {
                txtbResultShow.AppendText("\r\nОбщая ошибка:\r\n-----\r\n" + e.ToString() + "\r\n-----\r\n");
                StatusLabelMain.Text = "Ошибка в запросе!";
                ShowLogViewTextbox(true);
            }

            queriesStandartMenu.Enabled = true;
            queriesExtraMenu.Enabled = true;
            viewMenu.Enabled = true;
            txtbResultShow.Enabled = true;
            StatusLabelExtraInfo.ToolTipText = $"Последний запрос:\r\n{query}";
        }

        private async void GetEnterpriseMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(false);

            string query =
                "select distinct name,edrpou,count(edrpou) as amount " +
                "from CarAndOwner where edrpou is not '0' " +
                "group by edrpou order by amount desc " +
                "limit 100";
            await GetData(query);
        }

        private async void GetFIOMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(false);

            string query =
                "select distinct f,i,o,drfo,count(f) as amount " +
                "from CarAndOwner " +
                "group by f,i,o order by amount desc " +
                "limit 100";
            await GetData(query);
        }

        private async void GetEnterpriseWithHugeAutoparkMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(false);

            string query1 = "select distinct a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select name,edrpou,amount from " +
                "(select distinct name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by a.edrpou";
            string query =
                "select distinct a.name, a.edrpou, a.model, a.plate from CarAndOwner a " +
                "inner join " +
                "(select name,edrpou,amount from " +
                "(select distinct name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou is not '0' group by edrpou order by amount desc) " +
                "where amount > 50) b on a.edrpou=b.edrpou " +
                "order by a.edrpou";

            string query2 =
                "select distinct a.name, a.edrpou, a.factory, a.model, a.plate from CarAndOwner a " +
                "inner join (select name,edrpou,amount from " +
                "(select distinct name,edrpou,count(edrpou) as amount from CarAndOwner where edrpou > 1 group by edrpou order by amount desc) " +
                "where amount > 50) b " +
                "on a.edrpou=b.edrpou " +
                "order by b.amount, a.edrpou, a.factory, a.model";
            await GetData(query2);
        }

        private void GetSchemaLocalDBMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(true);

            // TODO: => make class GetSchema as Ilist<table> where table is Ilist<column>

            schemaDB = DbSchema.LoadDB(dbFileInfo.FullName);
            tablesDB = new List<string>();

            txtbResultShow.AppendText("--------------------------------\r\n");
            txtbResultShow.AppendText($"-  Scheme of local DB: '{dbFileInfo.FullName}':\r\n\r\n");

            txtbResultShow.AppendText($"-=  tables: {schemaDB.Tables.Count}  =-\r\n");

            foreach (var table in schemaDB.Tables)
            {
                tablesDB.Add(table.Value.TableName);

                txtbResultShow.AppendText($"-=     table: '{table.Value.TableName}    =-'\r\ncolumns:\r\n"); ;
                txtbResultShow.AppendText($"-=  columns: {table.Value.Columns.Count}  =-\r\n");
                foreach (var column in table.Value.Columns)
                {
                    txtbResultShow.AppendText($"'{column.ColumnName} '\t type: '{column.ColumnType}'\r\n");
                }
            }

            schemaDB = null;
            txtbResultShow.AppendText($"\r\n-  End of Scheme of local DB: '{dbFileInfo.FullName}':\r\n");
            txtbResultShow.AppendText("------------------------\r\n");
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

        private void SetToolTipTextBox(object sender, EventArgs e)
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
                        ShowLogViewTextbox(false);

                        await GetData(query);
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



        private async void ImportFromTextFileMenuItem_Click(object sender, EventArgs e)
        {
            administratorMenu.Enabled = false;
            queriesStandartMenu.Enabled = false;
            queriesExtraMenu.Enabled = false;
            txtbResultShow.Enabled = false;

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

                StatusLabelMain.Text = $"Количество записей: {readRows}";

                txtbResultShow.AppendText($"First Element{1}: plate: {list.ElementAt(0).Plate} factory: {list.ElementAt(0).Factory}, model: {list.ElementAt(0).Model}\r\n");
                txtbResultShow.AppendText($"Last Element{list.Count - 1}: plate: {list.ElementAt(list.Count - 1).Plate} factory: {list.ElementAt(list.Count - 1).Factory}, model: {list.ElementAt(list.Count - 1).Model}\r\n");
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



        private void ChangeViewPanelviewMenuItem_Click(object sender, EventArgs e)
        {
            ShowLogViewTextbox(true);
        }

        private void ShowLogViewTextbox(bool logView)
        {
            if (logView)
            {
                if (dgv != null)
                {
                    dgv?.Hide();
                    panel1.Controls.Remove(dgv);
                    dgv.DataSource = null;
                    dgv?.Dispose();
                    dgv = null;
                }

                txtbResultShow.Show();
                changeViewPanelviewMenuItem.Text = "Show as table";
            }
            else
            {
                txtbResultShow.Hide();
                changeViewPanelviewMenuItem.Text = "Show as text";
                StatusLabelMain.Text = $"доступны пункты меню Загрузки и Анализа данных";
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

        /// <summary>
        /// Save data in Registry
        /// </summary>
        /// <param name="list"></param>
        private void WriteRegistry(IList<MenuItemLast> list)
        {
            string errMessage = string.Empty;
            try
            {
                using (Microsoft.Win32.RegistryKey EvUserKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(appRegistryKey))
                {
                    foreach (var parameter in list)
                    {
                        try { EvUserKey.SetValue(parameter.Name, $"{parameter.NameQuery}: {parameter.BodyQuery}", Microsoft.Win32.RegistryValueKind.String); }
                        catch (Exception errLine) { errMessage += errLine.ToString() + "\r\n"; }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show($"Ошибки на запись в реестре.\r\n{err.ToString()}\r\n{errMessage}");
            }
        }

    }

    public class MenuItemLast
    {
        static int number;

        public MenuItemLast(string nameQuery, string bodyQuery)
        {
            number += 1;
            Name = $"menu{number}";
            NameQuery = nameQuery;
            BodyQuery = bodyQuery;
        }

        public string Name { get; }
        public string NameQuery { get; }
        public string BodyQuery { get; }
    }
}
