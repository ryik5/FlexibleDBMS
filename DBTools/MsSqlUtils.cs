using System;
using System.Data;

namespace FlexibleDBMS
{
    internal class MsSqlUtils : SqlAbstractConnector
    {
        ISQLConnectionSettings settings = null;
        public string connString = null;

        public override event Message<TextEventArgs> EvntInfoMessage;

        public MsSqlUtils(ISQLConnectionSettings settings)
        { SetConnection(settings); }

        public override ISQLConnectionSettings GetConnection()
        { return settings; }

        public override void SetConnection(ISQLConnectionSettings settings)
        {
            this.settings = settings;
            EvntInfoMessage?.Invoke(this, new TextEventArgs($"Установлено новое подключение{Environment.NewLine}{settings.Database}"));
            connString = SetConnectionString(settings);
        }


        public override DataTable GetTable(string query, int timeout = 3600)
        {
            if(timeout != 3600)
            {
                connString = SetConnectionString(settings, timeout);
            }
            DataTable dt = null;
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(connString))
            {
                connection.Open();
                using (System.Data.SqlClient.SqlCommand sqlCom = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    sqlCom.ExecuteNonQuery();
                    using (System.Data.SqlClient.SqlDataAdapter dataAdapter = new System.Data.SqlClient.SqlDataAdapter(sqlCom))
                    {
                        dt = new DataTable();
                        dataAdapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static string SetConnectionString(ISQLConnectionSettings settings, int timeout = 3600)
        {
            //string  = $"Data Source={server}\\SQLEXPRESS;Initial Catalog=intellect;Persist Security Info=True;User ID={user};Password={password};Connect Timeout=5";

            string connString = string.Empty;
            if (string.IsNullOrWhiteSpace(settings?.Host))
            {
                return null;
            }
            else
            {
                connString += "Data Source=" + settings.Host;
            }

            if (!string.IsNullOrWhiteSpace(settings?.Database))
            {
                connString += ";Initial Catalog=" + settings.Database;
            }

            connString += ";Persist Security Info=True;User ID=" + settings.Username + ";Password=" + settings.Password + $";Connect Timeout={timeout}";

            return connString;
        }

        public override void DoQuery(string query, bool isCommit = true)
        {
            throw new System.NotImplementedException();
        }
    }
}
