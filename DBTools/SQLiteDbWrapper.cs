using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace FlexibleDBMS
{

    //SQLite
    public abstract class SQLiteDbAbstract : IDisposable
    {
        public delegate void Message(object sender, TextEventArgs e);

        public SQLiteConnection sqlConnection;
        public SQLiteCommand sqlCommand;
        private string _dbConnectionString;

        protected SQLiteDbAbstract(string dbConnectionString,string filePath)
        {
            _dbConnectionString = dbConnectionString;
            ConnectToDB(_dbConnectionString);
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
        public SqLiteDbWrapper(string dbConnectionString, string filePath) :
            base(dbConnectionString, filePath)
        {        }

        public event Message Status;

        public DataTable GetQueryResultAsTable(string query)
        {
            DataTable dt = new DataTable();

            if (CommonExtesions.CheckIsSqlQuery(query))
            {
                using (var sqlAdapter = new SQLiteDataAdapter(query, sqlConnection))
                {
                    Status?.Invoke(this, new TextEventArgs("query: " + query));
                    sqlAdapter.SelectCommand.CommandType = CommandType.Text;
                    sqlAdapter.Fill(dt);
                }
            }
            return dt;
        }

        public IModelEntityDB<DBFilterModel> MakeFilterCollection(string table, string column, string alias)
        {
            IModelEntityDB<DBFilterModel> modelDBColumn = new DBColumnModel();
            modelDBColumn.Name = column;
            modelDBColumn.Alias = alias;
            modelDBColumn.Collection = new List<DBFilterModel>();
            modelDBColumn.Collection.Add(new DBFilterModel() { Name = "Нет" });

            string q = $"SELECT distinct {column}, COUNT(*) as amount FROM {table} WHERE LENGTH(TRIM({column}))>1 GROUP BY {column} ORDER BY amount DESC";

            DataTable dt = GetQueryResultAsTable(q);

            foreach (DataRow r in dt.Rows)
            {
                modelDBColumn.Collection.Add(new DBFilterModel() { Name = r[column].ToString() });
            }
            return modelDBColumn;
        }

        public void Execute(string query)
        {
            if (CommonExtesions.CheckIsSqlQuery(query))
            {
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