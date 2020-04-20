using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace AutoAnalysis
{
    public class MySQLUtils:ISqlDbConnector
    {
        //https://mysqlconnector.net/
        //https://mysqlconnector.net/overview/use-with-orms/
        //https://mysqlconnector.net/tutorials/basic-api/
        //https://andreyex.ru/bazy-dannyx/uchebnoe-posobie-po-sql/14-naibolee-chasto-ispolzuemyx-zaprosov-sql-vopros-otvet/

        ISQLConnectionSettings settings = null;
        string connString = null;

        public MySQLUtils(ISQLConnectionSettings settings)
        { SetSQLConnectionString(settings); }

        public void SetSQLConnectionSettings(ISQLConnectionSettings settings)
        { SetSQLConnectionString(settings); }

        private void SetSQLConnectionString(ISQLConnectionSettings settings)
        {
            this.settings = settings;
            connString = SetConnectionString(settings);
        }

        public DataTable GetTable(string query)
        {
            DataTable dt = null;

            using (MySqlConnection connection = new MySqlConnection(connString))
            {
                using (MySqlCommand sqlCom = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    sqlCom.ExecuteNonQuery();
                    using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom))
                    {
                        dt = new DataTable();
                        dataAdapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable GetData(string query, ISQLConnectionSettings _settings)
        {
            SetSQLConnectionString(_settings);

            using (var connection = new MySqlConnection(connString))
            {
                connection.Open();

                DataTable dt = null;
                // Insert some data
                //using (var cmd = new MySqlCommand())
                //{
                //    cmd.Connection = conn;
                //    cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                //    cmd.Parameters.AddWithValue("p", "Hello world");
                //    await cmd.ExecuteNonQueryAsync();
                //}

                // Retrieve all rows
                using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection))
                {
                    dataAdapter.SelectCommand.CommandType = CommandType.Text;

                    dataAdapter.Fill(dt);
                }
                return dt;
            }
        }

        public void TestMySql()
        {
            string query = "show databases";

            try
            {
                using (MySqlConnection dbCon = new MySqlConnection(connString))
                {
                    dbCon.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, dbCon))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader.GetString(0) != null && reader.GetString(0).Length > 0)
                                    reader.GetString(0);
                            }
                        }
                    }
                    dbCon.Close();
                }
            }
            catch (Exception excpt) { MessageBox.Show(excpt.ToString()); }
        }

        public static string SetConnectionString(ISQLConnectionSettings settings)
        {
            string connString = string.Empty;
            if (string.IsNullOrWhiteSpace(settings?.Host))
            {
                return null;
            }
            else
            {
                connString += "Server=" + settings.Host;
            }

            if (!string.IsNullOrWhiteSpace(settings?.Database))
            {
                connString += ";Database=" + settings.Database;
            }

            if (settings?.Port > 0)
            {
                connString += ";Port=" + settings.Port;
            }

            connString += ";User Id=" + settings.Username + ";Password=" + settings.Password+ ";Connection Timeout=120";

            return connString;
        }
    }
}

