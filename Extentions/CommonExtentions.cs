using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AutoAnalysis
{
    public static class CommonExtesions
    {

        public static string Translate(this string key, IDictionary<string, string> dic)
        {
            string result = dic.TryGetValue(key, out string name) == false ? key : name;

            return result;
        }

        public static SQLProvider GetSQLProvider(this string provider)
        {
            SQLProvider deffinedSQLProvider = SQLProvider.MS_SQL;
            switch (provider)
            {
                case "MS_SQL":
                    deffinedSQLProvider = SQLProvider.MS_SQL;
                    break;
                case "My_SQL":
                    deffinedSQLProvider = SQLProvider.My_SQL;
                    break;
                case "SQLite":
                    deffinedSQLProvider = SQLProvider.SQLite;
                    break;
                case "None":
                default:
                    deffinedSQLProvider = SQLProvider.None;
                    break;
            }

            return deffinedSQLProvider;
        }

        /// <summary>
        /// Check correctness of query
        /// </summary>
        /// <param name="query"> sql statement </param>
        public static bool IsSqlQuery(string query)
        {
            bool isQuery = true;
            if (string.IsNullOrEmpty(query))
            {
                isQuery = false;
               // Status?.Invoke(this, new TextEventArgs("Error. The query can not be empty or null!"));
               // new ArgumentNullException();
            }

            string[] word = query.Split(' ');
            if (word?.Length < 3 ||
               !word[0].ToLower().Equals("select") ||
               !word[0].ToLower().Equals("update") ||
               !word[0].ToLower().Equals("insert") ||
               !word[0].ToLower().Equals("create") ||
               !word[0].ToLower().Equals("delete") ||
               !word[0].ToLower().Equals("replace")||
               !word[0].ToLower().Equals("backup") ||
               !word[0].ToLower().Equals("restore")||
               !word[0].ToLower().Equals("repair") ||
               !word[0].ToLower().Equals("check"))
            {
                //   Status?.Invoke(this, new TextEventArgs(
                //       "Query is wrong! It does not start with appropriate commands. Query must start with any these commands: " +
                //       $"'SELECT, UPDATE, INSERT, CREATE, DELETE, REPLACE'\r\nYour query:  {query.ToUpper()}"));
                //   new ArgumentException();
                isQuery = false;
            }

            if (word.Where(x => x.ToLower().Equals("from")).Count() == 0)
            {
                isQuery = false;

                //      Status?.Invoke(this, new TextEventArgs(
                //          $"Query is wrong! It does not contain any query to a table. " +
                //          $"Check your expresion near 'FROM'\r\nYour query:  {query.ToUpper()}"));
                //     new ArgumentException();
            }

            return isQuery;
        }

        public static string OpenFileDialogReturnPath(this OpenFileDialog ofd) //Return its name 
        {
            ofd.FileName = @"";
            ofd.Filter = Properties.Resources.OpenDialogTextFiles;
            ofd.ShowDialog();
            string filePath = ofd.FileName;

            return filePath;
        }

        public static void AppendLine(this TextBox source, string value = "\r\n")
        {
            if (source?.Text?.Length == 0)
                source.Text = value;
            else
                source.AppendText($"{Environment.NewLine} {value}");
        }
       
    }
}
