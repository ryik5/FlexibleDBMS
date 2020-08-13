using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public partial class SelectDBForm : Form
    {

        const string MYSQL = "mysql";
        ToolTip tp = new ToolTip();

        internal SQLConnectionStore currentSQLConnectionStore { get; set; }// = new SQLConnectionStore();
        ModelCommonStringStore tableStore = new ModelCommonStringStore();

        public SelectDBForm()
        {
            InitializeComponent();
        }


        private void SelectDBForm_Load(object sender, EventArgs e)
        {
            btnCheck.Click += btnCheck_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            cmbSQLProvider.SelectedIndexChanged += CmbSQLProvider_SelectedIndexChanged;
            cmbDataBases.SelectedIndexChanged += CmbDataBases_SelectedIndexChanged;
            cmbTables.SelectedIndexChanged += CmbTables_SelectedIndexChanged;
            tableStore.EvntCollectionChanged += TableStore_EvntCollectionChanged;
            currentSQLConnectionStore.EvntConfigChanged += CurrentSQLConnectionStore_EvntConfigChanged;

            SetStartedControls();
        }

        private void CurrentSQLConnectionStore_EvntConfigChanged(object sender, BoolEventArgs args)
        {
            ISQLConnectionSettings oldSettings = currentSQLConnectionStore?.GetPrevious();
            ISQLConnectionSettings newSettings = currentSQLConnectionStore?.GetCurrent();

            if (oldSettings != null)
            {
                if (oldSettings.Name == newSettings.Name) return;
                if (oldSettings.Database == newSettings.Database) return;
            }
            
            ModelCommonStringStore tables = SQLSelector.GetTables(currentSQLConnectionStore?.GetCurrent());
            tableStore.Set(tables);
        }

        private void TableStore_EvntCollectionChanged(object sender, BoolEventArgs e)
        {
            //for correct clear from a previous list of tables
            cmbTables.DataSource = null;
            cmbTables.Items.Clear();

            if (tableStore?.ToList()?.Count > 0)
            {
                cmbTables.DataSource = tableStore.ToList();
            }
        }

        private void CmbTables_SelectedIndexChanged(object sender, EventArgs e)
        { tbName.Text = currentSQLConnectionStore?.GetCurrent()?.Name; }

        private void CmbSQLProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            btnCheck.Enabled = true;

            foreach (TextBox tb in Controls.OfType<TextBox>())
            { tb.Enabled = true; }

            SQLProvider provider = cmbSQLProvider.SelectedItem.ToString().GetSQLProvider();
            switch (provider)
            {
                case SQLProvider.MS_SQL:
                    tbPort.Text = "1433";
                    tp.SetToolTip(tbPort, "Port can be 1433 or 1434");
                    break;

                case SQLProvider.My_SQL:
                    tbPort.Text = "3306";
                    tp.SetToolTip(tbPort, "Port can be 3306");
                    break;

                case SQLProvider.SQLite:
                    tbHost.Text = "local";
                    tbPort.Text = "0";
                    foreach (TextBox tb in this.Controls.OfType<TextBox>())
                    { tb.Enabled = false; }
                    tp.SetToolTip(tbPort, "Port doesn't set");
                    break;
                 default:
                    btnCheck.Enabled = false;
                    tbPort.Text = "0";
                    tp.SetToolTip(tbPort, "Port doesn't set");
                    break;
            }

            tbName.Text = currentSQLConnectionStore?.GetCurrent()?.Name;
        }


        private void CmbDataBases_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDB();
        }

        private void ChangeDB()
        {
            cmbTables.Enabled = false;
            cmbTables.DataSource = null;
            cmbTables.Items.Clear();

            ISQLConnectionSettings newSettings = MakeFromControls(currentSQLConnectionStore?.GetCurrent(), "");

            tbResultShow.AppendLine("New Connection settings:" + Environment.NewLine + newSettings.AsString());

            currentSQLConnectionStore.Set(newSettings);

            tableStore.Set(SQLSelector.GetTables(newSettings));
            tbResultShow.AppendLine("таблиц: " + tableStore.ToList().Count);
            if (tableStore?.ToList()?.Count > 1)
                cmbTables.Enabled = true;

            tbName.Text = currentSQLConnectionStore?.GetCurrent()?.Name;
        }


        #region Check SQL DB
        private void btnCheck_Click(object sender, EventArgs e)
        { CheckDB(); }

        private void CheckDB()
        {
            ISQLConnectionSettings newSettings = MakeFromControls(currentSQLConnectionStore?.GetCurrent(), "");

            tbResultShow.AppendLine("Will check:");

            bool existDb = false;

            switch (newSettings.ProviderName)
            {
                case SQLProvider.My_SQL:
                    {
                        if (!string.IsNullOrWhiteSpace(tbHost?.Text))
                        {
                            newSettings = CheckMySQLDB(newSettings);

                            if (cmbDataBases?.Items?.Count > 0 && newSettings != null)
                            { existDb = true; }
                        }
                        else
                        {
                            tbResultShow.AppendLine("Check Host name: " + tbHost?.Text);
                            return;
                        }

                        break;
                    }

                case SQLProvider.MS_SQL:
                    {
                        if (!string.IsNullOrWhiteSpace(tbHost?.Text))
                        {
                            newSettings = CheckMSSQLDB(newSettings);

                            if (cmbDataBases?.Items?.Count > 0 && newSettings != null)
                            { existDb = true; }
                        }
                        else
                        {
                            tbResultShow.AppendLine("Check Host name: " + tbHost?.Text);
                            return;
                        }

                        break;
                    }

                case SQLProvider.SQLite:
                    {
                        newSettings = CheckSQLiteDB();

                        if (!string.IsNullOrWhiteSpace(newSettings?.Database))
                        { existDb = true; }

                        break;
                    }
                
            }

            tbResultShow.AppendLine("Новые настройки.");

            if (existDb)
            {
                if (cmbTables?.Items?.Count > 0)
                { cmbTables.Enabled = cmbTables?.Items?.Count > 1; }

                tbHost.Text = newSettings.Host;
                tbPort.Text = newSettings.Port.ToString();

                tbPassword.Text = newSettings.Password;
                tbUserName.Text = newSettings.Username;

                tbName.Text = newSettings?.Name;

                btnSave.Enabled = true;
                currentSQLConnectionStore.Set(newSettings);
                tbResultShow.AppendLine("Корректные:");
            }
            else
            {
                tbResultShow.AppendLine("Не корректные!. Проверьте их:");
            }

            tbResultShow.AppendLine(newSettings?.AsString());
        }

        private ISQLConnectionSettings CheckMySQLDB(ISQLConnectionSettings tmpSettings)
        {
            ISQLConnectionSettings newSettings = MakeFromControls(tmpSettings, MYSQL);

            MySQLUtils mySQL = new MySQLUtils(newSettings);

            try
            {
                using DataTable dt = mySQL.GetTable("SHOW DATABASES", 5);
                tbResultShow.AppendLine(mySQL.GetConnection().ToString());
                tbResultShow.AppendLine("Строка подключения: " + mySQL.connString);

                IList<string> list = (from DataRow r in dt.Rows select r[0]?.ToString()).ToList();

                if (list?.Count > 0)
                {
                    cmbDataBases.DataSource = list;
                    cmbDataBases.Enabled = true;
                    newSettings.Database = list[0];

                    tbResultShow.AppendLine("The inputed data are correct.");
                }
                else
                {
                    tbResultShow.AppendLine("Указаны некорректны данные или отсутствует подключение к серверу!");
                }
            }
            catch (MySqlException error)
            {
                newSettings = null;
                tbResultShow.AppendLine("My SQL error - " + error.Message + ":");
                tbResultShow.AppendLine(error.ToString());
            }
            catch (Exception error)
            {
                newSettings = null;
                tbResultShow.AppendLine(error.Message + ":");
                tbResultShow.AppendLine(error.ToString());
            }

            return newSettings;
        }

        private ISQLConnectionSettings CheckMSSQLDB(ISQLConnectionSettings tmpSettings)
        {
            ISQLConnectionSettings newSettings = MakeFromControls(tmpSettings, "");

            MsSqlUtils msSQL = new MsSqlUtils(newSettings);

            try
            {
                using (DataTable dt = msSQL.GetTable("SELECT Name FROM sys.databases", 5))
                {
                    tbResultShow.AppendLine(msSQL.GetConnection().ToString());
                    tbResultShow.AppendLine("Строка подключения: " + msSQL.connString);

                    if (dt?.Rows?.Count > 0)
                    {
                        IList<string> list = new List<string>();
                        foreach (DataRow r in dt.Rows)
                        {
                            list.Add(r[0].ToString());
                        }

                        if (list?.Count > 0)
                        {
                            cmbDataBases.DataSource = list;
                            cmbDataBases.Enabled = true;
                            newSettings.Database = list[0];

                            tbResultShow.AppendLine("The inputed data are correct.");
                        }
                    }
                    else
                    {
                        tbResultShow.AppendLine("Указаны некорректны данные или отсутствует подключение к серверу!");
                    }
                }
            }
            catch (Exception excpt)
            {
                newSettings = null;
                tbResultShow.AppendLine(excpt.Message + ":");
                tbResultShow.AppendLine(excpt.ToString());
            }

            return newSettings;
        }

        private ISQLConnectionSettings CheckSQLiteDB()
        {
            string db = null;
            using (OpenFileDialog ofd = new OpenFileDialog())
            { db = ofd.OpenFileDialogReturnPath(Properties.Resources.DialogDbFile, "Выберите файл с БД SQLite:"); }

            ISQLConnectionSettings newSettings = new SQLConnectionSettings
            {
                Database = db,
                ProviderName = SQLProvider.SQLite
            };

            if (!string.IsNullOrWhiteSpace(newSettings.Database) && File.Exists(newSettings.Database))
            {
                DbSchema schemaDb = null;

                try
                {
                    schemaDb = DbSchema.LoadDB(newSettings.Database);
                    IList<string> tablesDb = schemaDb.Tables.Select(tbl => tbl.Value.TableName).ToList();

                    if (tablesDb?.Count > 0)
                    {
                        newSettings.Table = tablesDb[0];

                        cmbDataBases.DataSource = new List<string> { newSettings.Database };
                        cmbTables.DataSource = tablesDb;
                        tbResultShow.AppendLine("The inputed data are correct.");
                    }
                }
                catch (Exception e)
                {
                    newSettings = null;
                    tbResultShow.AppendLine($"Ошибка в БД: {e.Message}");
                    tbResultShow.AppendLine($"{e}");
                }
                finally
                {
                    if (schemaDb?.Tables?.Count == 0)
                    {
                        newSettings = null;
                        tbResultShow.AppendLine("Подключенная база данных пустая или же в ней отсутствуют какие-либо таблицы с данными!");
                        tbResultShow.AppendLine("Предварительно создайте базу данных, таблицы и импортируйте/добавьте в них данные...");
                        tbResultShow.AppendLine("Заблокирован функционал по получению данных из таблиц...");
                    }
                }
            }

            return newSettings;
        }
        #endregion


        ISQLConnectionSettings MakeFromControls(ISQLConnectionSettings settings, string dbDefault)
        {
            string db = cmbDataBases?.SelectedItem?.ToString();

            ISQLConnectionSettings newSettings = new SQLConnectionSettings(settings)
            {
                Host = tbHost?.Text,
                Port = int.TryParse(tbPort?.Text, out int port) ? port : 0,
                Database = string.IsNullOrWhiteSpace(db) ? dbDefault : db,
                Username = tbUserName?.Text,
                Password = tbPassword?.Text,
                ProviderName = cmbSQLProvider.SelectedItem.ToString().GetSQLProvider()
            };

            return newSettings;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            ISQLConnectionSettings newSettings = currentSQLConnectionStore?.GetCurrent();

            if (!string.IsNullOrWhiteSpace(tbName?.Text) && !string.IsNullOrWhiteSpace(tbHost?.Text))
            {
                newSettings.ProviderName = cmbSQLProvider.SelectedItem.ToString().GetSQLProvider();
                newSettings.Host = tbHost?.Text;
                newSettings.Port = int.TryParse(tbPort?.Text, out int port) ? port : 0;
                newSettings.Username = tbUserName?.Text;
                newSettings.Password = tbPassword?.Text;
                newSettings.Database = cmbDataBases?.Items?.Count > 0 ? cmbDataBases?.SelectedItem?.ToString() : null;
                newSettings.Table = cmbTables?.Items?.Count > 0 ? cmbTables?.SelectedItem?.ToString() : null;
                currentSQLConnectionStore.Set(newSettings);

                ActiveForm.Close();
            }
            else
            {
                tbResultShow.AppendLine("Check corectness inputed data:");
                tbResultShow.AppendLine("Name: " + tbName?.Text);
                tbResultShow.AppendLine("Host: " + tbHost?.Text);
                tbResultShow.AppendLine("DataBase: " + cmbDataBases?.SelectedItem?.ToString());
                return;
            }
        }


        private void ShowMessage(string message)
        { MessageBox.Show(message); }

        private void BtnCancel_Click(object sender, EventArgs e)
        { ActiveForm.Close(); }

        private void SetStartedControls()
        {
            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                SetDefaultFontTextBox(tb);

                if (tb.Name != nameof(tbResultShow.Name))
                { tb.Click += Reset_Click; }

                tb.LostFocus += TbSetDefault_LostFocus;

                if (tb.Name == nameof(tbPort))
                {
                    tb.KeyPress += TbPort_KeyPress;
                    tp.SetToolTip(tb, "Port");
                    tb.Text = "0";
                }
                else
                {
                    tp.SetToolTip(tb, tb.Text);
                }

                tb.Refresh();
            }

            cmbSQLProvider.DataSource = SQLConnectionExtensions.GetSQLProvider();

            if (currentSQLConnectionStore?.GetCurrent() != null)
            {
                tbResultShow.AppendLine(currentSQLConnectionStore?.GetCurrent()?.ToString());

                tbHost.Text = currentSQLConnectionStore?.GetCurrent().Host;
                tbPort.Text = currentSQLConnectionStore?.GetCurrent().Port.ToString();
                cmbDataBases.SelectedText = currentSQLConnectionStore?.GetCurrent().Database;
                cmbTables.SelectedText = currentSQLConnectionStore?.GetCurrent().Table;
                tbUserName.Text = currentSQLConnectionStore?.GetCurrent().Username;
                tbPassword.Text = currentSQLConnectionStore?.GetCurrent().Password;
                try { cmbSQLProvider.SelectedIndex = cmbSQLProvider.FindString(currentSQLConnectionStore?.GetCurrent().ProviderName.ToString()); }
                catch (Exception expt) { tbResultShow.AppendLine(expt.ToString()); }
            }
            else
            {
                cmbSQLProvider.SelectedIndex = cmbSQLProvider.FindString(SQLProvider.None.ToString());
            }

            cmbDataBases.Enabled = false;
            cmbTables.Enabled = false;
            btnSave.Enabled = false;
            tbName.Enabled = false;
            tbPassword.PasswordChar = '*';
            tp.SetToolTip(cmbSQLProvider, "Select SQL's Provider");
            tp.SetToolTip(cmbDataBases, "Select DataBase");
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;

            TextBox tb = sender as TextBox;

            if (tb.Name == nameof(tbPort))
            { tb.Text = "0"; }
            else { tb.Clear(); }

            SetCorrectFontTextBox(tb);
        }

        private void SetDefaultFontTextBox(TextBox tb)
        {
            tb.Font = new Font("Arial", 9, FontStyle.Italic);
            tb.ForeColor = Color.Gray;
        }

        private void SetCorrectFontTextBox(TextBox tb)
        {
            tb.Font = new Font("Arial", 9, FontStyle.Regular);
            tb.ForeColor = Color.Black;
        }

        private void TbSetDefault_LostFocus(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            int len = tb.Text.Trim().Length;

            if (tb.Name == nameof(tbPort))
            {
                string text = tb.Text;

                if (!int.TryParse(text, out int result))
                {
                    ShowMessage("Номер порта может содержать только цифры!");
                    tb.Text = "0";
                    SetDefaultFontTextBox(tb);
                }
                else if (result == 0)
                {
                    tb.Text = "0";
                    SetDefaultFontTextBox(tb);
                }
                else
                {
                    tb.Text = result.ToString();
                    SetCorrectFontTextBox(tb);
                }
            }
            else
            {
                if (len == 0)
                {
                    SetDefaultFontTextBox(tb);
                    tb.Text = tp.GetToolTip(tb);
                }
                else
                {
                    SetCorrectFontTextBox(tb);
                }
            }
        }

        private void TbPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8 || e.KeyChar >= 48 && e.KeyChar <= 57) return;
            ShowMessage(@"Номер порта может содержать только цифры!");
            e.Handled = true;
        }
    }
}