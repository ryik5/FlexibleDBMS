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

        /// <summary>
        /// Search words in Cyrilic looks like - '"SELECT * from таблица WHERE CustomLike(столбец, 'текст')"'
        /// </summary>
        [SQLiteFunction(Name = "CustomLike", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class CustomLike : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                if (args.Length != 2 || args[0] == null || args[1] == null) return null;
                string val1 = args[0].ToString();
                string val2 = args[1].ToString();
                return val1.IndexOf(val2, StringComparison.OrdinalIgnoreCase) >= 0;
            }
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