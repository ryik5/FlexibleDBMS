using System;
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

        protected SQLiteDbAbstract(string dbConnectionString)
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
}
