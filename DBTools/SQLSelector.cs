using System;
using System.Data;

namespace FlexibleDBMS
{
    public static class SQLSelector
    {
        /// <summary>
        /// will return current SQL Connector was connected to MS SQL or My SQL or SQLite DB
        /// </summary>
        /// <param name="settings">ISQLConnectionSettings</param>
        public static SqlAbstractConnector SetConnector(ISQLConnectionSettings settings)
        {
            SqlAbstractConnector sqlConnector = null;
            switch (settings.ProviderName)
            {
                case SQLProvider.SQLite:
                    {
                        sqlConnector = new SQLiteModelDBOperations(settings);
                        break;
                    }
                case SQLProvider.My_SQL:
                    {
                        sqlConnector = new MySQLUtils(settings);
                        break;
                    }
                case SQLProvider.MS_SQL:
                    {
                        sqlConnector = new MsSqlUtils(settings);
                        break;
                    }
            }
            return sqlConnector;
        }


        public static ModelCommonStringStore GetTables(ISQLConnectionSettings tmpSettings)
        {
            ModelCommonStringStore models = null;

            switch (tmpSettings?.ProviderName)
            {
                case SQLProvider.SQLite:
                    {
                        models = SQLite_Tables(tmpSettings);
                        break;
                    }
                case SQLProvider.My_SQL:
                    {
                        models = My_SQL_Tables(tmpSettings);
                        break;
                    }
                case SQLProvider.MS_SQL:
                    {
                        models = MS_SQL_Tables(tmpSettings);
                        break;
                    }
                default:
                    break;
            }

            return models;
        }
        static ModelCommonStringStore MS_SQL_Tables(ISQLConnectionSettings tmpSettings)
        {
            string query = $"SELECT * FROM sys.objects WHERE type in (N'U')";

            ModelCommonStringStore models = GetListForModelStore(new MsSqlUtils(tmpSettings), query);

            return models;
        }

        static ModelCommonStringStore My_SQL_Tables(ISQLConnectionSettings tmpSettings)
        {
            string query = $"SHOW TABLES FROM {tmpSettings.Database}";

            ModelCommonStringStore models = GetListForModelStore(new MySQLUtils(tmpSettings), query);

            return models;
        }

        static ModelCommonStringStore SQLite_Tables(ISQLConnectionSettings tmpSettings)
        {
            string query = $"SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";

            ModelCommonStringStore models = GetListForModelStore(new SQLiteModelDBOperations(tmpSettings), query);

            return models;
        }



        public static ModelCommonStringStore GetColumns(ISQLConnectionSettings tmpSettings)
        {
            ModelCommonStringStore models = null;

            switch (tmpSettings.ProviderName)
            {
                case SQLProvider.SQLite:
                    {
                        models = SQLite_Columns(tmpSettings);
                        break;
                    }
                case SQLProvider.My_SQL:
                    {
                        models = My_SQL_Columns(tmpSettings);
                        break;
                    }
                case SQLProvider.MS_SQL:
                    {
                        models = MS_SQL_Columns(tmpSettings);
                        break;
                    }
                default:
                    break;
            }

            return models;
        }

        static ModelCommonStringStore MS_SQL_Columns(ISQLConnectionSettings tmpSettings)
        {
            string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS " +
                $"WHERE TABLE_NAME= '{tmpSettings.Table}';";

            ModelCommonStringStore models = GetListForModelStore(new MsSqlUtils(tmpSettings), query);

            return models;
        }

        static ModelCommonStringStore My_SQL_Columns(ISQLConnectionSettings tmpSettings)
        {
            string query = $"SELECT `COLUMN_NAME` FROM `INFORMATION_SCHEMA`.`COLUMNS` " +
                $"WHERE `TABLE_SCHEMA`= '{tmpSettings.Database}' AND `TABLE_NAME`= '{tmpSettings.Table}';";

            ModelCommonStringStore models = GetListForModelStore(new MySQLUtils(tmpSettings), query);

            return models;
        }

        static ModelCommonStringStore SQLite_Columns(ISQLConnectionSettings tmpSettings)
        {
            SqlAbstractConnector connector = new SQLiteModelDBOperations(tmpSettings);

            ModelCommonStringStore models = (connector as SQLiteModelDBOperations).GetColumns(tmpSettings.Table);

            return models;
        }



        public static ModelCommonStringStore GetDataSample(ISQLConnectionSettings tmpSettings, string column)
        {
            ModelCommonStringStore models = null;

            string query = $"SELECT DISTINCT {column} FROM {tmpSettings.Table} WHERE LENGTH(TRIM({column})) > 0 LIMIT 20;"; //GROUP BY {column} 

            switch (tmpSettings.ProviderName)
            {
                case SQLProvider.SQLite:
                    {
                        models = GetListForModelStore(new SQLiteModelDBOperations(tmpSettings), query);
                        break;
                    }
                case SQLProvider.My_SQL:
                    {
                        models = GetListForModelStore(new MySQLUtils(tmpSettings), query);
                        break;
                    }
                case SQLProvider.MS_SQL:
                    {
                        query = $"SELECT TOP 20 {column} FROM {tmpSettings.Table} WHERE LEN({column}) > 0 GROUP BY {column};";
                        models = GetListForModelStore(new MsSqlUtils(tmpSettings), query);
                        break;
                    }
                default:
                    break;
            }

            return models;
        }
        static ModelCommonStringStore GetListForModelStore(SqlAbstractConnector sqlConnector, string query)
        {
            ModelCommonStringStore models = new ModelCommonStringStore();
            IModel model;

            try
            {
                using DataTable dt = sqlConnector.GetTable(query);
                foreach (DataRow r in dt?.Rows)
                {
                    model = new ModelCommon
                    {
                        Name = r[0].ToString(),
                        Alias = r[0].ToString()
                    };
                    models.Add(model);
                }
                dt?.Dispose();
            }
            catch (Exception err) { }

            return models;
        }



        public static DataTableStore GetDataTableStore(ISQLConnectionSettings tmpSettings, string query)
        {
            SqlAbstractConnector sqlConnector = SetConnector(tmpSettings);
            sqlConnector.EvntInfoMessage += SqlConnector_EvntInfoMessage;

            DataTableStore dataTableStore = GetDataTableStore(sqlConnector, query);

            sqlConnector.EvntInfoMessage -= SqlConnector_EvntInfoMessage;
            return dataTableStore;
        }

        static DataTableStore GetDataTableStore(SqlAbstractConnector sqlConnector, string query)
        {
            DataTableStore data = new DataTableStore();
            try
            {
                using DataTable dt = sqlConnector.GetTable(query);
                data.Set(dt);
                dt?.Dispose();
            }
            catch (Exception err) { data.Errors = err.Message + " " + message; }

            return data;
        }
        static string message { get; set; }
        private static void SqlConnector_EvntInfoMessage(object sender, TextEventArgs e)
        {
            message = e.Message;
        }
    }
}