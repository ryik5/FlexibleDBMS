using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FlexibleDBMS
{

    public class SQLiteModelDBOperations : SqlAbstractConnector
    {
        public override event Message<TextEventArgs> EvntInfoMessage;
        public IDictionary<string, string> columnsAndAliases;

        string connString;
        ISQLConnectionSettings settings;

         public SQLiteModelDBOperations(ISQLConnectionSettings settings) 
        {
            SetConnection(settings);
        }

        public override void SetConnection(ISQLConnectionSettings settings)
        {
            this.settings = settings;
            EvntInfoMessage?.Invoke(this, new TextEventArgs($"Установлено новое подключение{Environment.NewLine}{settings.Database}"));
            connString = SetConnectionString(settings);

            if (SQLiteImportedDB.Check(settings.Database))
            {
                if (!(columnsAndAliases?.Count > 0))
                {
                    MakeNewDictionary();
                }
            }
        }

        private static string SetConnectionString(ISQLConnectionSettings settings)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
            {
                DataSource = settings.Database,
                PageSize = 4096,
                UseUTF16Encoding = true,
                Version = 3
            };
         //   string sqLiteConnectionString = builder.ConnectionString;// $"Data Source = {settings.Database}; Version=3;";
            return builder.ConnectionString;
        }
        public override ISQLConnectionSettings GetConnection()
        {
            return new SQLConnectionSettings(settings);
        }


        /// <summary>
        /// Check SQLite DB structure
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        private bool CheckUpDBStructure(string TableName)
        {
            bool isGood = false;

            DbSchema schemaDB = null;
            string errors = string.Empty;

            try
            {
                schemaDB = DbSchema.LoadDB(settings.Database);

                foreach (var table in schemaDB?.Tables)
                {
                    if (TableName != null && table.Value.TableName.Equals(TableName))
                    {
                        isGood = true;
                        break;
                    }

                    if (!(table.Value.Columns?.Count > 0))
                    { EvntInfoMessage?.Invoke(this, new TextEventArgs($"Ошибка в таблице: {table.Value.TableName} - отсутствуют колонки и структура данных в таблице.")); }
                }
            }
            catch (Exception err)
            { EvntInfoMessage?.Invoke(this, new TextEventArgs($"Ошибка БД - {err.Message}:{Environment.NewLine}{err.ToString()}")); }
            finally
            {
                if (!(schemaDB?.Tables?.Count > 0))
                {
                    EvntInfoMessage?.Invoke(this, new TextEventArgs(
                        $"Подключенная база данных пустая или же в ней отсутствуют какие-либо таблицы с данными!{Environment.NewLine}" +
                        $"Предварительно создайте базу данных, таблицы и импортируйте/добавьте в них данные..."));
                    isGood = false;
                }

                schemaDB = null;
            }

            if (isGood)
            {
                if (string.IsNullOrWhiteSpace(TableName))
                { EvntInfoMessage?.Invoke(this, new TextEventArgs($"В базе данных {settings.Database} со структурой все в порядке")); }
                else
                { EvntInfoMessage?.Invoke(this, new TextEventArgs($"В базе данных {settings.Database} со структурой таблицы '{TableName}' все в порядке")); }
            }

            return isGood;
        }


        private IDictionary<string, string> TryToSetColumnDictionary()
        {
            IDictionary<string, string> newDictionary = new Dictionary<string, string>();

            EvntInfoMessage?.Invoke(this, new TextEventArgs($"БД: {settings.Database}"));

            if (SQLiteImportedDB.Check(settings.Database))
            {
                string query = "SELECT ColumnName, ColumnAlias FROM ColumnNameAndAlias;";
                using SqLiteDbWrapper readData = new SqLiteDbWrapper(connString);
                using (DataTable dt = readData?.GetQueryResultAsTable(query))
                {
                    if (dt?.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt?.Rows)
                        {
                            newDictionary[r["ColumnName"]?.ToString()] = r["ColumnAlias"]?.ToString();
                        }
                    }
                }
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"Сгенерирован новый словарь алиасов: {newDictionary?.Count} слов"));
            }

            return newDictionary;
        }


        /// <summary>
        /// Make Columns and matched Aliases Dictionary<string,string> if the current DB has been made from imported text file in the application
        /// </summary>
        private void MakeNewDictionary()
        {
            columnsAndAliases = TryToSetColumnDictionary();
        }


        public  string ReplaceColumnByAlias(IDictionary<string ,string> dic, string query)
        {
            EvntInfoMessage?.Invoke(this, new TextEventArgs($"Ищу в словаре из {dic?.Count} слов замену для слов в запросе:" +
                $"{Environment.NewLine}'{query}"));
            if (!(CommonExtensions.CheckQueryToReplaceWords(query)))
            {
                return query;
            }

            if (!(dic?.Count > 0))
            {
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"Словарь замен пустой"));
                return query;
            }
            
            query = query.Replace($",", $" , ");
            query = query.Replace($"(", $" ( ");
            query = query.Replace($")", $" ) ");
            
            Regex regex = new Regex(@"\s+", RegexOptions.IgnoreCase);
            query = regex.Replace(query, @" ");
            
            query = query.Replace($" , ", $", ");
            query = query.Replace($"( ", $"(");
            query = query.Replace($" )", $")");

            foreach (var k in dic)
            {
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"{k.Key} - {query.IndexOf(k.Key)}"));

                
                query = ReplaceCaseInsensitive(query, $" {k.Key} from", $" {k.Key} as '{k.Key} ({k.Value})' from");
                query = ReplaceCaseInsensitive(query, $" {k.Key},", $" {k.Key} as '{k.Key} ({k.Value})',");
                query = ReplaceCaseInsensitive(query, $" {k.Key})", $" {k.Key} as '{k.Key} ({k.Value})')");
                query = ReplaceCaseInsensitive(query, $"({k.Key}, ", $"({k.Key} as '{k.Key} ({k.Value})', ");
                query = ReplaceCaseInsensitive(query, $"({k.Key}) ", $"({k.Key}) as ('{k.Key} ({k.Value})') ");
            }

            return query;
        }

        string ReplaceCaseInsensitive(string input, string search, string replacement)
        {
            string result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }

        public override DataTable GetTable(string query, int timeout = 3600)
        {
            DataTable dt = new DataTable();

            if (CheckUpDBStructure(settings.Table))
            {
                //убрать двойные пробелы из запроса
                Regex regex = new Regex(@"\s+", RegexOptions.IgnoreCase);
                query = regex.Replace(query, @" ");

                string newQuery = query;

                if (SQLiteImportedDB.Check(settings.Database))
                {
                    if (!(columnsAndAliases?.Count > 0))
                    { MakeNewDictionary(); }

                    if (query.IndexOf(CommonConst.QUERY_COMMON, StringComparison.OrdinalIgnoreCase) != -1)//.ToUpperInvariant().StartsWith(COMMONQUERY)
                    {
                        newQuery = "SELECT ";

                        using SqLiteDbWrapper readData1 = new SqLiteDbWrapper(connString);
                        using (DataTable dt1 = readData1?.GetQueryResultAsTable(CommonConst.QUERY_ALIAS))
                        {
                            if (dt1?.Rows.Count > 0)
                            {
                                foreach (DataRow r in dt1?.Rows)
                                {
                                    newQuery += $"{r.Field<string>("ColumnName")} as '{r.Field<string>("ColumnName")} ({r.Field<string>("ColumnAlias")})', ";
                                }
                            }
                        }

                        newQuery = $"{newQuery.TrimEnd(' ').TrimEnd(',')} FROM MAINDATA ";
                        newQuery = CommonExtensions.ReplaceCaseInsensitive(query, CommonConst.QUERY_COMMON, newQuery);

                        EvntInfoMessage?.Invoke(this, new TextEventArgs($"Произведена замена запроса на newQuery:{Environment.NewLine}'{query}'{Environment.NewLine}на: '{newQuery}'"));
                    }
                    else
                    {
                        newQuery = ReplaceColumnByAlias(columnsAndAliases, query); //replace words by new Dictionary
                    }
                }

                EvntInfoMessage?.Invoke(this, new TextEventArgs(
                    $"{Environment.NewLine}Запрос к БД" +
                    $"{Environment.NewLine} =>  '{settings.Database}'" +
                    $"{Environment.NewLine} =>  '{newQuery}'"));

                using SqLiteDbWrapper readData = new SqLiteDbWrapper(connString);
                dt = readData?.GetQueryResultAsTable(newQuery);
            }
            else
            { EvntInfoMessage?.Invoke(this, new TextEventArgs($"Со структурой таблицы '{settings.Table}' базы данных '{settings.Database}' проблема!")); }

            return dt;
        }


        public ModelCommonStringStore GetColumns(string table)
        {
            ModelCommonStringStore models = new ModelCommonStringStore();
            IModel model;
            
            if ( SQLiteImportedDB.Check(settings.Database)&& table.Equals("MainData"))
            {
                string query = "SELECT ColumnName, ColumnAlias FROM 'ColumnNameAndAlias';";
                using (DataTable dt = GetTable(query))
                {
                    if (dt?.Rows.Count > 0)
                    {
                        foreach (DataRow r in dt?.Rows)
                        {
                            model = new ModelCommon
                            {
                                Name = $"{r.Field<string>("ColumnName")}",
                                Alias = $"{r.Field<string>("ColumnAlias")}"
                            };
                            models.Add( model);
                        }
                    }
                }
            }
            else
            {
                DbSchema schemaDB = DbSchema.LoadDB(settings.Database);
                if (schemaDB?.Tables?.Count > 0)
                {
                    foreach (var _table in schemaDB.Tables)
                    {
                        if (_table.Value.TableName.Equals(table))
                        {
                            foreach (var column in _table.Value.Columns)
                            {
                                model = new ModelCommon
                                {
                                    Name = column.ColumnName,
                                    Alias = column.ColumnName
                                };
                                models.Add( model);
                            }
                        }
                    }
                }
            }

            return models;
        }

        /// <summary>
        /// get only simple query like 'SELECT DISTINCT name_column FROM name_table'
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public IModelEntityDB<DBColumnModel> GetFilterList(IDictionary<string, string> columns, string table)
        {
            EvntInfoMessage?.Invoke(this, new TextEventArgs("В таблице: " + table + " " + columns?.Keys?.Count + " колонок "));

            IModelEntityDB<DBColumnModel> _table = new DBTableModel();
            IModelEntityDB<DBFilterModel> result;
            _table.ColumnCollection = new List<DBColumnModel>();
            if (CheckUpDBStructure(table))
            {
                foreach (var column in columns)
                {
                    //SQLiteDBOperations dBOperations
                    using (SqLiteDbWrapper readData = new SqLiteDbWrapper(connString))
                    {
                        result = readData.MakeFilterCollection(table, column.Key, column.Value);
                    }

                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"Для фильтра отобрано {result.ColumnCollection.Count} строк"));

                    _table.ColumnCollection.Add((DBColumnModel)result);
                }
            }
            return _table;
        }

        public override void DoQuery(string query, bool isCommit = true)
        {
            if (!File.Exists(settings.Database))
            {
                SQLiteConnection.CreateFile(settings.Database);
            }
            EvntInfoMessage?.Invoke(this, new TextEventArgs($"БД: {settings.Database}{Environment.NewLine}" +
                $"sqLiteConnectionString: {connString}{Environment.NewLine}Выполняю запрос: {query}"));

            using (SqLiteDbWrapper dbWriter = new SqLiteDbWrapper(connString))
            {
                if (isCommit)
                { dbWriter.Execute("begin"); }
                dbWriter.Execute(query);
                if (isCommit)
                { dbWriter.Execute("end"); }
            }
        }

        public void PrepareTablesForCommonModel(IModels columnsAndAliases)
        {
            //create table MainData
            string query = "CREATE TABLE 'MainData' (";//'Id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
            IDictionary<int, IModel> models = columnsAndAliases.list;

            foreach (var k in models)
            { query += $"'{k.Value.Name}' TEXT, "; }
             query = query.TrimEnd(' ').TrimEnd(',') + ")"; //remove in the end - , "
            DoQuery(query);
            
            //Create table ColumnNameAndAlias
            query = "CREATE TABLE 'ColumnNameAndAlias' ('ColumnName' TEXT, 'ColumnAlias' TEXT);"; //'Id' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, 
            DoQuery(query);
            
            //fill table ColumnNameAndAlias
            query = "INSERT INTO 'ColumnNameAndAlias' ('ColumnName', 'ColumnAlias') VALUES (@ColumnName, @ColumnAlias);";

            if (SQLiteImportedDB.Check(settings.Database))
            {
             //   if (CheckUpDBStructure("ColumnNameAndAlias"))
           // {
                using (SqLiteDbWrapper dbWriter = new SqLiteDbWrapper(connString))
                {
                    dbWriter.Execute("begin");

                    foreach (var row in models)
                    {
                        using (SQLiteCommand sqlCommand = new SQLiteCommand(query, dbWriter.sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@ColumnName", DbType.String).Value = row.Value.Name;
                            sqlCommand.Parameters.Add("@ColumnAlias", DbType.String).Value = row.Value.Alias;

                            dbWriter.Execute(sqlCommand);
                        }
                    }

                    dbWriter.Execute("end");

                    EvntInfoMessage?.Invoke(this, new TextEventArgs("Запись имен колонок и к ним алиасов завершена"));
                }
            }
            else
            { EvntInfoMessage?.Invoke(this, new TextEventArgs($"Ошибка записи.{Environment.NewLine}Предварительно нужно проверить базу и таблицу 'ColumnNameAndAlias' в ней!")); }
        }

        public void WriteModelInTable(IList<IModels> models)
        {
            string  query = "INSERT OR REPLACE INTO 'MainData' (";
            IDictionary<int, IModel> firstModels = models[0].list;

            foreach (var k in firstModels)
            { query += $"{k.Value.Name}, "; }
            query = query.TrimEnd(' ').TrimEnd(',');
            query += ") VALUES (";

            foreach (var k in firstModels)
            { query += $"@{k.Value.Name}, "; }
            query = query.TrimEnd(' ').TrimEnd(',');
            query += ");";

            if (SQLiteImportedDB.Check(settings.Database))
            {
                //   if (CheckUpDBStructure("MainData"))
                //{
                using (SqLiteDbWrapper dbWriter = new SqLiteDbWrapper(connString))
                {
                    dbWriter.Execute("begin");

                    foreach (var row in models)
                    {
                        using (SQLiteCommand sqlCommand = new SQLiteCommand(query, dbWriter.sqlConnection))
                        {
                            foreach (var c in row.list)
                            { sqlCommand.Parameters.Add($"@{c.Value.Name}", DbType.String).Value = c.Value.Alias; }

                            dbWriter.Execute(sqlCommand);
                        }
                    }

                    dbWriter.Execute("end");

                    EvntInfoMessage?.Invoke(this, new TextEventArgs("Подготовленные данные импортированы в БД"));
                }
            }
            else
            {
                EvntInfoMessage?.Invoke(this, new TextEventArgs($"Ошибка записи.{Environment.NewLine}Предварительно нужно проверить базу и таблицу 'MainData' в ней!"));
            }

            FileReaderModels.evntWaitHandle.Set();
        }


        /// <summary>
        /// clear table from columns which don't store any data. 
        /// parameters - table which need to clear, columns which need to leave in the result table
        /// </summary>
        /// <param name="tableName"> table's name which need to clear</param>
        /// <param name="leaveColumns">as string 'column1, column2, column5'</param>
        public void ClearDataTable(string tableName, IList<string> leaveColumns)
        {
            string columns = leaveColumns.ToStringComa();
            DoQuery("BEGIN TRANSACTION;", false);
            DoQuery($"CREATE TABLE 't1_backup' ({columns});", false);
            DoQuery($"INSERT INTO 't1_backup' SELECT {columns} FROM '{tableName}';", false);
            DoQuery($"DROP TABLE '{tableName}';", false);
            DoQuery($"CREATE TABLE '{tableName}' ({columns});", false);
            DoQuery($"INSERT INTO '{tableName}' SELECT {columns} FROM t1_backup;", false);
            DoQuery($"DROP TABLE t1_backup;", false);
            DoQuery($"VACUUM;", false);
            DoQuery("COMMIT;", false);
        }

        /// <summary>
        /// clear table from columns which don't store any data. 
        /// parameters - table which need to clear, columns which need to leave in the result table
        /// </summary>
        /// <param name="columnsDelete">as string - 'column1, column2, column5'</param>
        /// <param name="aliasTable">table where columns' names matches aliases</param>
        /// <param name="NameColumnNames"></param>
        public void ClearDataTable(string aliasTable, string NameColumnNames, IList<string> columnsDelete)
        {
            string[] columnsDeleteInTable = columnsDelete.ToArray();

            DoQuery("BEGIN TRANSACTION;", false);
            foreach (var column in columnsDeleteInTable)
            {
                if (!(string.IsNullOrWhiteSpace(column)))
                {
                  string  query = $"DELETE FROM '{aliasTable}' WHERE {NameColumnNames} LIKE '{column.Trim()}'";
                    EvntInfoMessage?.Invoke(this, new TextEventArgs($"{query}"));

                    DoQuery(query, false);
                }
            }
            DoQuery("COMMIT;", false);
        }
    }
}