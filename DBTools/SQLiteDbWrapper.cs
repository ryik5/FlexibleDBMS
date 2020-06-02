using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace FlexibleDBMS
{
    public class SqLiteDbWrapper : SQLiteDbAbstract, IDisposable
    {
        public SqLiteDbWrapper(string dbConnectionString) :
            base(dbConnectionString)
        {        }

        public event Message Status;

        public DataTable GetQueryResultAsTable(string query)
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

        public IModelEntityDB<DBFilterModel> MakeFilterCollection(string table, string column, string alias)
        {
            IModelEntityDB<DBFilterModel> modelDBColumn = new DBColumnModel();
            modelDBColumn.Name = column;
            modelDBColumn.Alias = alias;
            modelDBColumn.ColumnCollection = new List<DBFilterModel>();
            modelDBColumn.ColumnCollection.Add(new DBFilterModel() { Name = "Нет" });

            string q = $"SELECT distinct {column}, COUNT(*) as amount FROM {table} WHERE LENGTH(TRIM({column}))>1 GROUP BY {column} ORDER BY amount DESC";

            DataTable dt = GetQueryResultAsTable(q);

            foreach (DataRow r in dt.Rows)
            {
                modelDBColumn.ColumnCollection.Add(new DBFilterModel() { Name = r[column].ToString() });
            }
            return modelDBColumn;
        }

        public void Execute(string query)
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
        

        /// <summary>
        /// To use with transaction keywords - "begin" and "end"
        /// </summary>
        /// <param name="sqlCommand"></param>
        public void Execute(SQLiteCommand sqlCommand)
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