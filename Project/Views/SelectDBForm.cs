using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public partial class SelectDBForm : Form
    {

        public ISQLConnectionSettings settings { get; set; }
        string selectedDB= "mysql";

        ToolTip tp = new ToolTip();

        public SelectDBForm()
        {
            InitializeComponent();
           

            btnCheck.Click += btnCheck_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            cmbSQLProvider.SelectedIndexChanged += CmbSQLProvider_SelectedIndexChanged;
            cmbDataBases.SelectedIndexChanged += CmbDataBases_SelectedIndexChanged;
            cmbTables.SelectedIndexChanged += CmbTables_SelectedIndexChanged;

            SetStartedControls();
        }

        private void SelectDBForm_Shown(object sender, EventArgs e)
        {
            settings = new SQLConnectionSettings(SQLConnection.Settings);
        }
        private void CmbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbName.Text = SetNameConnection();
        }


        private string SetNameConnection()
        {
            string name = string.Empty;
            name += string.IsNullOrWhiteSpace(tbHost?.Text) ? null : $"{tbHost?.Text} - ";
            name += string.IsNullOrWhiteSpace(cmbDataBases?.SelectedItem?.ToString()) ? null : $"{cmbDataBases?.SelectedItem?.ToString()} - ";
            name += string.IsNullOrWhiteSpace(cmbTables?.SelectedItem?.ToString()) ? null : $"{cmbTables?.SelectedItem?.ToString()}";

            return name;
        }

        private void CmbSQLProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = false;

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                tb.Enabled = true;
            }

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
                case SQLProvider.None:
                default:
                    tbPort.Text = "0";
                    tp.SetToolTip(tbPort, "Port doesn't set");
                    break;
            }

            tbName.Text = SetNameConnection();
        }

        private void CmbDataBases_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDB();
        }

        private void ChangeDB()
        {
            cmbTables.Enabled = false;
            if (settings.ProviderName == SQLProvider.My_SQL)
            {
                if (!string.IsNullOrWhiteSpace(tbHost?.Text))
                {
                    settings.Host = tbHost?.Text;
                    settings.Port = int.TryParse(tbPort?.Text, out int port) ? port : 0;
                    settings.Database = cmbDataBases?.Items?.Count > 0 ? cmbDataBases?.SelectedItem?.ToString() : selectedDB;
                    settings.Username = tbUserName?.Text;
                    settings.Password = tbPassword?.Text;
                }
                else
                {
                    tbResultShow.AppendLine("Check the correctness of the entered data:");
                    tbResultShow.AppendLine("Host: " + tbHost?.Text);
                    return;
                }

                try
                {
                    MySQLUtils mySQL = new MySQLUtils(settings);
                    DataTable dt = mySQL.GetTable("SHOW TABLES");
                    IList<string> list = new List<string>();
                    foreach (DataRow r in dt.Rows)
                    {
                        list.Add(r[0].ToString());
                    }

                    if (list?.Count > 0)
                    {
                        cmbTables.DataSource = list;
                        cmbTables.Enabled = true;

                        settings.Table = cmbTables?.Items?.Count > 0 ? cmbTables?.SelectedItem?.ToString() : null;

                        tbResultShow.AppendLine($"DB exists and table {settings.Table} are selected.");
                        tbResultShow.AppendLine("Please will select needed DataBase!");
                    }
                }
                catch (MySqlException excpt)
                {
                    tbResultShow.AppendLine(excpt.Message + ":");
                    tbResultShow.AppendLine(excpt.ToString());
                }
                catch (Exception excpt)
                {
                    tbResultShow.AppendLine(excpt.Message + ":");
                    tbResultShow.AppendLine(excpt.ToString());
                }
            }
            else if ( settings.ProviderName == SQLProvider.MS_SQL)
            {
                throw new NotImplementedException(); 
            }

            tbName.Text = SetNameConnection();
        }

        private void CheckDB()
        {
            SQLProvider selectedProvider = cmbSQLProvider.SelectedItem.ToString().GetSQLProvider();

            settings.ProviderName = selectedProvider;
            bool existDB = false;

            switch (selectedProvider)
            {
                case SQLProvider.My_SQL:
                    {
                        if (!string.IsNullOrWhiteSpace(tbHost?.Text))
                        {
                            settings = CheckMySQLDB(settings);

                            if (cmbDataBases?.Items?.Count > 0)
                            { existDB = true; }
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
                        settings = CheckSQLiteDB(settings);

                        existDB = !string.IsNullOrWhiteSpace(settings?.Database);
                        break;
                    }
                default:
                    break;
            }

            if (existDB)
            {
                if (cmbTables?.Items?.Count > 0)
                { cmbTables.Enabled = cmbTables?.Items?.Count > 1; }

                btnSave.Enabled = true;
            }

            tbName.Text = SetNameConnection();
        }

        private ISQLConnectionSettings CheckMySQLDB(ISQLConnectionSettings tmpSettings)
        {
            ISQLConnectionSettings newSettings = new SQLConnectionSettings(tmpSettings);
            newSettings.Host = tbHost?.Text;
            newSettings.Port = int.TryParse(tbPort?.Text, out int port) ? port : 0;
            newSettings.Database = cmbDataBases?.SelectedItem?.ToString() ?? selectedDB;
            newSettings.Username = tbUserName?.Text;
            newSettings.Password = tbPassword?.Text;
            MySQLUtils mySQL = new MySQLUtils(newSettings);

            try
            {
                using (DataTable dt = mySQL.GetTable("SHOW DATABASES"))
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
            }
            catch (MySqlException excpt)
            {
                tbResultShow.AppendLine("My SQL error - " + excpt.Message + ":");
                tbResultShow.AppendLine(excpt.ToString());
            }
            catch (Exception excpt)
            {
                tbResultShow.AppendLine(excpt.Message + ":");
                tbResultShow.AppendLine(excpt.ToString());
            }

            return newSettings;
        }

        private ISQLConnectionSettings CheckSQLiteDB(ISQLConnectionSettings tmpSettings)
        {
            ISQLConnectionSettings newSettings = new SQLConnectionSettings(tmpSettings);

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                newSettings.Database = ofd.OpenFileDialogReturnPath();
                    newSettings.Name = "currentDB";
                if (!string.IsNullOrWhiteSpace(newSettings.Database))
                {
                    newSettings = CheckSchemaSQLite(newSettings);
                }
                else
                {
                    DialogResult dialog = MessageBox.Show("Create new DB?", "DB is empty!", MessageBoxButtons.YesNo);

                    if (dialog == DialogResult.Yes)
                    {
                        newSettings.Database = "MainDB.db";
                        SQLiteDBOperations connector = new SQLiteDBOperations(newSettings);
                        connector.TryMakeLocalDB();
                        newSettings = CheckSchemaSQLite(newSettings);
                    }
                }
            }
            return newSettings;
        }

        private ISQLConnectionSettings CheckSchemaSQLite(ISQLConnectionSettings tmpSettings)
        {
            ISQLConnectionSettings newSettings = new SQLConnectionSettings(tmpSettings);

            DbSchema schemaDB = null;
            IList<string> tablesDB;
            DBModel db = new DBModel();

            try
            {
                schemaDB = DbSchema.LoadDB(tmpSettings.Database);
                tablesDB = new List<string>();

                foreach (var tbl in schemaDB.Tables)
                {
                    tablesDB.Add(tbl.Value.TableName);
                }

                if (tablesDB?.Count > 0)
                {
                    newSettings.Table = tablesDB[0];
                    newSettings.ProviderName = SQLProvider.SQLite;

                    cmbDataBases.DataSource = new List<string>() { tmpSettings.Database };
                    cmbTables.DataSource = tablesDB;

                    tbResultShow.AppendLine("The inputed data are correct.");
                }
            }
            catch (Exception e)
            {
                tbResultShow.AppendLine($"Ошибка в БД: {e.Message}");
                tbResultShow.AppendLine($"{e.ToString()}");
            }
            finally
            {
                if (schemaDB?.Tables?.Count == 0)
                {
                    tbResultShow.AppendLine("Подключенная база данных пустая или же в ней отсутствуют какие-либо таблицы с данными!");
                    tbResultShow.AppendLine("Предварительно создайте базу данных, таблицы и импортируйте/добавьте в них данные...");
                    tbResultShow.AppendLine("Заблокирован функционал по получению данных из таблиц...");
                }
            }
            return newSettings;
        }


        private void ShowMessage(string message)
        { MessageBox.Show(message); }
        
        private void BtnCancel_Click(object sender, EventArgs e)
        { ActiveForm.Close(); }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbName?.Text) && !string.IsNullOrWhiteSpace(tbHost?.Text))
            {
                settings.Name = SetNameConnection();
                settings.ProviderName = cmbSQLProvider.SelectedItem.ToString().GetSQLProvider();
                settings.Host = tbHost?.Text;
                settings.Port = int.TryParse(tbPort?.Text, out int port) ? port : 0;
                settings.Username = tbUserName?.Text;
                settings.Password = tbPassword?.Text;
                settings.Database = cmbDataBases?.Items?.Count > 0 ? cmbDataBases?.SelectedItem?.ToString() : null;
                settings.Table = cmbTables?.Items?.Count > 0 ? cmbTables?.SelectedItem?.ToString() : null;

                SQLConnection.Settings = new SQLConnectionSettings(settings);

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

        private void btnCheck_Click(object sender, EventArgs e)
        { CheckDB(); }



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

            cmbSQLProvider.DataSource = SQLProviderExtensions.GetSQLProvider();

            tbResultShow.AppendLine(settings.GetObjectPropertiesValuesToString().AsString());

            if (settings != null)
            {
                tbHost.Text = settings.Host;
                tbPort.Text = settings.Port.ToString();
                cmbDataBases.SelectedText = settings.Database;
                cmbTables.SelectedText = settings.Table;
                tbUserName.Text = settings.Username;
                tbPassword.Text = settings.Password;
                try { cmbSQLProvider.SelectedIndex = cmbSQLProvider.FindString(settings.ProviderName.ToString()); }
                catch (Exception expt) { tbResultShow.AppendLine(expt.ToString()); }
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

            TextBox tb = (sender as TextBox);

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

                if (!(int.TryParse(text, out int result)))
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
            if (e.KeyChar != 8 && (e.KeyChar < 48 || e.KeyChar > 57))
            {
                ShowMessage("Номер порта может содержать только цифры!");
                e.Handled = true;
            }
        }


    }
}