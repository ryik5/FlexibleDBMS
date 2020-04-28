using MySql.Data.MySqlClient;
using System.Data;

namespace FlexibleDBMS
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

        public ISQLConnectionSettings GetSettings()
        {
            return settings;
        }
        private void SetSQLConnectionString(ISQLConnectionSettings settings)
        {
            this.settings = settings;
            connString = SetConnectionString(settings);
        }
               
        public DataTable GetTable(string query)
        {
            DataTable dt = null;
        //    if (CommonExtesions.IsSqlQuery(query))
            {
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
            }
            return dt;
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
            ;

            connString += ";User Id=" + settings.Username + ";Password=" + settings.Password+ ";Default Command Timeout=3600";

            return connString;
        }
        
    }
}

