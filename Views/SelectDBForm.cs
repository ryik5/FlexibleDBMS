using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AutoAnalysis
{
    public partial class SelectDBForm : Form
    {

        public ISQLConnectionSettings settings { get; set; }
        string selectedDB;

        ToolTip tp = new ToolTip();

        public SelectDBForm()
        {
            InitializeComponent();

            if (settings?.Database == null)
            {
                selectedDB = "mysql";
            }
            btnCheck.Click += btnCheck_Click;
            btnSave.Enabled = false;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += BtnCancel_Click;
            cmbDataBases.SelectedIndexChanged += CmbDataBases_SelectedIndexChanged;
            cmbSQLProvider.SelectedIndexChanged += CmbSQLProvider_SelectedIndexChanged;

            SetStartedControls();
        }


        private void CmbSQLProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                    settings = new SQLConnectionSettings();
                    tbName.Text = settings.Name;
                    tbHost.Text=settings.Host ;
                    tbPort.Text = "0";
                    foreach (TextBox tb in this.Controls.OfType<TextBox>())
                    {
                        tb.Enabled = false;
                    }
                    tp.SetToolTip(tbPort, "Port doesn't set");
                    break;
                case SQLProvider.None:
                default:
                    tbPort.Text = "0";
                    tp.SetToolTip(tbPort, "Port doesn't set");
                    break;
            }
        }

        private void CmbDataBases_SelectedIndexChanged(object sender, EventArgs e)
        { ChangeDB(); }

        private void ChangeDB()
        {
            textBox1.Clear();
            cmbTables.Enabled = false;

            if (!string.IsNullOrWhiteSpace(tbHost?.Text))
            {
                settings = new SQLConnectionSettings();
                settings.Name = tbName?.Text;
                settings.Host = tbHost?.Text;
                settings.Port = int.TryParse(tbPort?.Text, out int port) ? port : 0;
                settings.Database = cmbDataBases?.Items?.Count > 0 ? cmbDataBases?.SelectedItem?.ToString() : selectedDB;
                settings.Username = tbUserName?.Text;
                settings.Password = tbPassword?.Text;
            }
            else
            {
                textBox1.AppendLine("Check the correctness of the entered data:");
                textBox1.AppendLine("Host: " + tbHost?.Text);
                return;
            }

            MySQLUtils mySQL = new MySQLUtils(settings);

            try
            {
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
                    cmbTables.SelectedIndex = 0;

                    settings.Table = cmbTables?.Items?.Count > 0 ? cmbTables?.SelectedItem?.ToString() : null;

                    textBox1.Clear();

                    textBox1.AppendLine($"DB exists and table {settings.Table} are selected.");
                    textBox1.AppendLine("Please will select needed DataBase!");

                    btnSave.Enabled = true;
                }
            }
            catch (MySqlException excpt)
            {
                textBox1.AppendLine(excpt.Message + ":");
                textBox1.AppendLine(excpt.ToString());
            }
            catch (Exception excpt)
            {
                textBox1.AppendLine(excpt.Message + ":");
                textBox1.AppendLine(excpt.ToString());
            }
        }


        private void CheckDB()
        {
            SQLProvider selectedProvider=cmbSQLProvider.SelectedItem.ToString().GetSQLProvider();
 
            switch (selectedProvider)
            {
                case (SQLProvider.My_SQL):
                    {
                        if (!string.IsNullOrWhiteSpace(tbHost?.Text))
                        {
                            settings = new SQLConnectionSettings();
                            settings.Name = tbName?.Text;
                            settings.Host = tbHost?.Text;
                            settings.Port = int.TryParse(tbPort?.Text, out int port) ? port : 0;
                            settings.Database = cmbDataBases?.Items?.Count > 0 ? cmbDataBases?.SelectedItem?.ToString() : selectedDB;
                            settings.Table = cmbTables?.Items?.Count > 0 ? cmbTables?.SelectedItem?.ToString() : null;
                            settings.ProviderName = selectedProvider;
                            settings.Username = tbUserName?.Text;
                            settings.Password = tbPassword?.Text;
                        }
                        else
                        {
                            textBox1.AppendLine("Check Host name: " + tbHost?.Text);
                            return;
                        }

                        CheckMySQLDB(settings);
                        break;
                    }
                case SQLProvider.SQLite:
                    {
                        settings = new SQLConnectionSettings();

                        settings.Database = CheckSQLiteDB();
                        settings.Table = cmbTables?.Items?.Count > 0 ? cmbTables?.SelectedItem?.ToString() : null;
                        settings.ProviderName = selectedProvider;
                        cmbTables.Enabled = cmbTables?.Items?.Count > 1;
                        break;
                    }
            }            
        }

        private void CheckMySQLDB(ISQLConnectionSettings connectionSettings)
        {
            MySQLUtils mySQL = new MySQLUtils(connectionSettings);

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
                        cmbDataBases.SelectedIndex = 0;
                        textBox1.Clear();

                        textBox1.AppendLine("The inputed data are correct.");

                        btnSave.Enabled = true;
                    }
                }
            }
            catch (MySqlException excpt)
            {
                textBox1.AppendLine("My SQL error - " + excpt.Message + ":");
                textBox1.AppendLine(excpt.ToString());
            }
            catch (Exception excpt)
            {
                textBox1.AppendLine(excpt.Message + ":");
                textBox1.AppendLine(excpt.ToString());
            }
        }

        private string CheckSQLiteDB()
        {
            string filePath = string.Empty;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                filePath = ofd.OpenFileDialogReturnPath();
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    CheckUpSelectedSQLiteDB(filePath);
                }
            }
            return filePath;
        }

        private void CheckUpSelectedSQLiteDB(string filePath)
        {
            DbSchema schemaDB = null;
            IList<string> tablesDB;
            ModelDB db = new ModelDB();
            db.Name = "currentDB";
            db.Collection = new List<ModelDBTable>();
            db.FilePath = filePath;
            db.SqlConnectionString = $"Data Source = {filePath}; Version=3;";

            try
            {
                schemaDB = DbSchema.LoadDB(filePath);
                tablesDB = new List<string>();

                foreach (var tbl in schemaDB.Tables)
                {
                    tablesDB.Add(tbl.Value.TableName);
                }
                
                cmbDataBases.DataSource = new List<string>() { filePath };
                cmbTables.DataSource = tablesDB;
                cmbDataBases.SelectedIndex = 0;
                btnSave.Enabled = true;

                textBox1.Clear();

                textBox1.AppendLine("The inputed data are correct.");
            }
            catch (Exception e)
            {
                textBox1.AppendLine($"Ошибка в БД: {e.Message}");
                textBox1.AppendLine($"Ошибка в БД: {e.Message}");
                textBox1.AppendLine($"{e.ToString()}");
            }
            finally
            {
                if (schemaDB?.Tables?.Count == 0)
                {
                    textBox1.AppendLine("Подключенная база данных пустая или же в ней отсутствуют какие-либо таблицы с данными!");
                    textBox1.AppendLine("Предварительно создайте базу данных, таблицы и импортируйте/добавьте в них данные...");
                    textBox1.AppendLine("Заблокирован функционал по получению данных из таблиц...");
                }
                schemaDB = null;
            }
        }



        private void SetStartedControls()
        {
            cmbSQLProvider.DataSource = SQLProviderManager.GetSQLProvider();
            tp.SetToolTip(cmbSQLProvider, "Select SQL's Provider");

            tp.SetToolTip(cmbDataBases, "Select DataBase");
            cmbDataBases.Enabled = false;
            cmbTables.Enabled = false;

            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                SetDefaultFontTextBox(tb);
                tb.Click += Reset_Click;
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
        }


        private void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }





        private void BtnCancel_Click(object sender, EventArgs e)
        { ActiveForm.Close(); }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbName?.Text) && !string.IsNullOrWhiteSpace(tbHost?.Text))
            {
                settings = new SQLConnectionSettings();
                settings.Name = tbName?.Text;
                settings.ProviderName = cmbSQLProvider.SelectedItem.ToString().GetSQLProvider();
                settings.Host = tbHost?.Text;
                settings.Port = int.TryParse(tbPort?.Text, out int port) ? port : 0;
                settings.Username = tbUserName?.Text;
                settings.Password = tbPassword?.Text;
                settings.Database = cmbDataBases?.Items?.Count > 0 ? cmbDataBases?.SelectedItem?.ToString() : null;
                settings.Table = cmbTables?.Items?.Count > 0 ? cmbTables?.SelectedItem?.ToString() : null;

                ActiveForm.Close();
            }
            else
            {
                textBox1.AppendLine("Check corectness inputed data:");
                textBox1.AppendLine("Name: " + tbName?.Text);
                textBox1.AppendLine("Host: " + tbHost?.Text);
                textBox1.AppendLine("DataBase: " + cmbDataBases?.SelectedItem?.ToString());
                return;
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            CheckDB();
        }


        private void Reset_Click(object sender, EventArgs e)
        {
            TextBox tb = (sender as TextBox);

            if (tb.Name == nameof(tbPort))
            { tb.Text = "0"; }
            else { tb.Clear(); }

            tb.Font = new Font("Arial", 9, FontStyle.Bold);
            tb.ForeColor = Color.Black;
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
            int length = (sender as TextBox).Text.Trim().Length;

            if (e.KeyChar != 8 && (e.KeyChar < 48 || e.KeyChar > 57))
            {
                ShowMessage("Номер порта может содержать только цифры!");
                e.Handled = true;
            }
        }
    }
}
