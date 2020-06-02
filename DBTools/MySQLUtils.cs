using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public class MySQLUtils : SqlAbstractConnector
    {
        /*
1) mysql -uroot -p
2) use newpost
3.1) CHECK TABLE person;
3.2) repair table person;

restart mysql server

---------------fixed DB:
update person set phone = replace(phone,',80',',380') 				--   fix 0504596966,805041999 ->  0504596966,3805041999
update person set phone = replace(phone,';',',')
select * from person where phone like '0%' limit 100
UPDATE `person` SET `phone` = CONCAT( '38', `phone` ) where phone like '0%'    --- fix 380504596966,05041999 ->  0504596966,05041999
update person set phone = replace(phone,',0',',380') where phone like '%,0%'   --   fix 0504596966,05041999 ->  0504596966,3805041999
             */
        //https://mysqlconnector.net/
        //https://mysqlconnector.net/overview/use-with-orms/
        //https://mysqlconnector.net/tutorials/basic-api/
        //https://andreyex.ru/bazy-dannyx/uchebnoe-posobie-po-sql/14-naibolee-chasto-ispolzuemyx-zaprosov-sql-vopros-otvet/

        public override event Message<TextEventArgs> EvntInfoMessage;

        ISQLConnectionSettings settings = null;
      public  string connString = null;

        public MySQLUtils(ISQLConnectionSettings settings)
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
            if (timeout != 3600)
            {
                connString = SetConnectionString(settings, timeout);
            }

            DataTable dt = new DataTable();
            try
            {
                using MySqlConnection connection = new MySqlConnection(connString);
                using (MySqlCommand sqlCom = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    sqlCom.ExecuteNonQuery();
                    using MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
                    dt = new DataTable();
                    dataAdapter.Fill(dt);
                }
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"Получено {dt.Rows.Count} строк(и)"));
            }
            catch(Exception err)
            {
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"Ошибка получения данных {err.Message}"));
            }
            return dt;
        }

        public MenuAbstractStore GetTables()
        {
            string query = $"SHOW TABLES FROM {settings.Database}";
            MenuAbstractStore models = new MenuItemStore();

            using (DataTable dt = GetTable(query))
            {
                foreach (DataRow r in dt?.Rows)
                {
                    ToolStripMenuItem model = new ToolStripMenuItem
                    {
                        Text = r[0].ToString(),
                        Tag = r[0].ToString()
                    };
                    models.Add(model);
                }
            }
            EvntInfoMessage?.Invoke(this, new TextEventArgs($"Создана модель содержащая {models?.GetAllItems()?.Count} элемента(ов)"));

            return models;
        }


        public static string SetConnectionString(ISQLConnectionSettings settings, int timeout = 3600)
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

            connString += ";User Id=" + settings.Username + ";Password=" + settings.Password + $";Default Command Timeout={timeout};charset=utf8";

            return connString;
        }

        public override void DoQuery(string query, bool isCommit = true)
        {
            throw new System.NotImplementedException();
        }
    }

  }

