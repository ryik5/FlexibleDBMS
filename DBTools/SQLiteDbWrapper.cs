using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace AutoAnalysis
{

    //SQLite
    public abstract class SQLiteDbAbstract : IDisposable
    {
        public delegate void Message(object sender, TextEventArgs e);

        public SQLiteConnection sqlConnection;
        public SQLiteCommand sqlCommand;
        private string _dbConnectionString;

        protected SQLiteDbAbstract(string dbConnectionString, System.IO.FileInfo dbFileInfo)
        {
            _dbConnectionString = dbConnectionString;
            CheckUpDB(_dbConnectionString, dbFileInfo);
            ConnectToDB(_dbConnectionString);
        }

        public string GetConnectionString()
        {
            return _dbConnectionString;
        }

        public void CheckUpDB(string dbConnectionString, System.IO.FileInfo dbFileInfo)
        {
            if (!(dbFileInfo?.Name?.Length > 0))
            { throw new System.ArgumentNullException("dbFileInfo can not be null!"); }

            if (!dbFileInfo.Exists)
            { throw new System.Exception("dbFileInfo is not exist"); }

            if (!(dbConnectionString?.Trim()?.Length > 0))
            { throw new System.ArgumentNullException("dbConnectionString string can not be Empty or short"); }
        }

        private void ConnectToDB(string dbConnectionString)
        {
            if (sqlConnection != null)
            {
                Dispose();
            }

            sqlConnection = new SQLiteConnection(dbConnectionString);
            sqlConnection.Open();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (sqlCommand != null)
                    {
                        sqlCommand?.Dispose();
                    }
                    if (sqlConnection != null)
                    {
                        try
                        {
                            sqlConnection?.Close();
                            sqlConnection?.Dispose();
                        }
                        catch { }
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion IDisposable Support
    }

    public class SqLiteDbWrapper : SQLiteDbAbstract, IDisposable
    {
        public SqLiteDbWrapper(string dbConnectionString, System.IO.FileInfo dbFileInfo) :
            base(dbConnectionString, dbFileInfo)
        {        }

        public event Message Status;

        //Read Data
        public SQLiteDataReader GetQueryResultAsDataReader(string query)
        {
            CheckFormatQuery(query);

            Status?.Invoke(this, new TextEventArgs("query: " + query));
            using (var _sqlCommand = new SQLiteCommand(query, sqlConnection))
            { return _sqlCommand.ExecuteReader(); }
        }

        public DataTable GetQueryResultAsTable(string query)
        {
            CheckFormatQuery(query);

            DataTable dt = new DataTable();

            using (var sqlAdapter = new SQLiteDataAdapter(query, sqlConnection))
            {
                Status?.Invoke(this, new TextEventArgs("query: " + query));
                sqlAdapter.SelectCommand.CommandType = CommandType.Text;
                sqlAdapter.Fill(dt);
            }
            return dt;
        }

        public IModelDBable<ModelDBFilter> GetColumnUniqueValuesList(string table, string column, string alias)
        {
            IModelDBable<ModelDBFilter> modelDBColumn = new ModelDBColumn();
            modelDBColumn.Name = column;
            modelDBColumn.Alias = alias;
            modelDBColumn.Collection = new List<ModelDBFilter>();
            modelDBColumn.Collection.Add(new ModelDBFilter() { Name = "Нет" });

            string q = $"SELECT distinct {column}, COUNT(*) as amount FROM {table} WHERE LENGTH(TRIM({column}))>1 GROUP BY {column} ORDER BY amount DESC";

            DataTable dt = GetQueryResultAsTable(q);

            foreach (DataRow r in dt.Rows)
            {
                modelDBColumn.Collection.Add(new ModelDBFilter() { Name = r[column].ToString() });
            }
            return modelDBColumn;
        }

        //Write Data or Execute query
        public void Execute(SQLiteCommand sqlCommand)
        {
            if (sqlCommand == null)
            {
                Status?.Invoke(this, new TextEventArgs("Error. The SQLCommand can not be empty or null!"));
                new ArgumentNullException();
            }

            using (var sqlCommand1 = new SQLiteCommand("begin", sqlConnection))
            { sqlCommand1.ExecuteNonQuery(); }

            try
            {
                sqlCommand.ExecuteNonQuery();
                Status?.Invoke(this, new TextEventArgs("Execute sqlCommand - Ok"));
            }
            catch (Exception expt)
            { Status?.Invoke(this, new TextEventArgs("Error! " + expt.ToString())); }

            using (var sqlCommand1 = new SQLiteCommand("end", sqlConnection))
            { sqlCommand1.ExecuteNonQuery(); }
        }

        public void Execute(string query)
        {
            CheckFormatQuery(query);

            using (var sqlCommand = new SQLiteCommand(query, sqlConnection))
            {
                try
                {
                    sqlCommand.ExecuteNonQuery();
                    Status?.Invoke(this, new TextEventArgs("Execute query: " + query + " - ok"));
                }
                catch (Exception expt)
                { Status?.Invoke(this, new TextEventArgs("query: " + query + " ->Error! " + expt.ToString())); }
            }
        }

        /// <summary>
        /// Check correctness of query
        /// </summary>
        /// <param name="query"> sql statement </param>
        public void CheckFormatQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                Status?.Invoke(this, new TextEventArgs("Error. The query can not be empty or null!"));
                new ArgumentNullException();
            }

            string[] word = query.Split(' ');
            if (
               !word[0].ToLower().Equals("select") ||
               !word[0].ToLower().Equals("update") ||
               !word[0].ToLower().Equals("insert") ||
               !word[0].ToLower().Equals("create") ||
               !word[0].ToLower().Equals("delete") ||
               !word[0].ToLower().Equals("replace"))
            {
                Status?.Invoke(this, new TextEventArgs(
                    "Query is wrong! It does not start with appropriate commands. Query must start with any these commands: " +
                    $"'SELECT, UPDATE, INSERT, CREATE, DELETE, REPLACE'\r\nYour query:  {query.ToUpper()}"));
                new ArgumentException();
            }

            if (word.Where(x => x.ToLower().Equals("from")).Count() == 0)
            {
                Status?.Invoke(this, new TextEventArgs(
                    $"Query is wrong! It does not contain any query to a table. " +
                    $"Check your expresion near 'FROM'\r\nYour query:  {query.ToUpper()}"));
                new ArgumentException();
            }
        }

        /// <summary>
        /// To use with transaction keywords - "begin" and "end"
        /// </summary>
        /// <param name="sqlCommand"></param>
        public void ExecuteBulk(SQLiteCommand sqlCommand)
        {
            if (sqlCommand == null)
            {
                Status?.Invoke(this, new TextEventArgs("Error. The SQLCommand can not be empty or null!"));
                new ArgumentNullException();
            }

            try
            {
                sqlCommand.ExecuteNonQuery();
                Status?.Invoke(this, new TextEventArgs("ExecuteBulk - Ok"));
            }
            catch (Exception expt)
            { Status?.Invoke(this, new TextEventArgs("ExecuteBulk -> Error! " + expt.ToString())); }
        }
    }
}