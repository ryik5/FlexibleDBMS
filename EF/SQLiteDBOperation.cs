using System;
using System.Data;
using System.Data.SQLite;

namespace AutoAnalyse
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
            CheckDB(_dbConnectionString, dbFileInfo);
            ConnectToDB(_dbConnectionString);
        }

        public string GetConnectionString()
        {
            return _dbConnectionString;
        }

        private void CheckDB(string dbConnectionString, System.IO.FileInfo dbFileInfo)
        {
            if (!(dbFileInfo?.Name?.Length > 0))
                throw new System.ArgumentException("dbFileInfo can not be null!");

            if (!dbFileInfo.Exists)
                throw new System.ArgumentException("dbFileInfo is not exist");

            if (!(dbConnectionString?.Trim()?.Length > 0))
                throw new System.ArgumentException("dbConnectionString string can not be Empty or short");
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

    internal class SqLiteDbWrapper : SQLiteDbAbstract, IDisposable
    {
        public SqLiteDbWrapper(string dbConnectionString, System.IO.FileInfo dbFileInfo) :
            base(dbConnectionString, dbFileInfo)
        { }

        public event Message Status;

        //Read Data
        public SQLiteDataReader GetDataReader(string query)
        {
            Status?.Invoke(this, new TextEventArgs("query: " + query));
            using (var _sqlCommand = new SQLiteCommand(query, sqlConnection))
            { return _sqlCommand.ExecuteReader(); }
        }

        public DataTable GetTable(string query)
        {
            DataTable dt = new DataTable();

            using (var sqlAdapter = new SQLiteDataAdapter(query, sqlConnection))
            {
                Status?.Invoke(this, new TextEventArgs("query: " + query));
                sqlAdapter.SelectCommand.CommandType = CommandType.Text;
                sqlAdapter.Fill(dt);
            }
            return dt;
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
            if (query == null)
            {
                Status?.Invoke(this, new TextEventArgs("Error. The query can not be empty or null!"));
                new ArgumentNullException();
            }

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
                Status?.Invoke(this, new TextEventArgs("Execute ExecuteBulk - Ok"));
            }
            catch (Exception expt)
            { Status?.Invoke(this, new TextEventArgs("Execute -> Error! " + expt.ToString())); }
        }
    }

}
