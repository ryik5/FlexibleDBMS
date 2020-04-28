using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public static class CommonExtesions
    {
        /// <summary>
        /// to look for a translation of the word 'key' from Dictionary 
        /// </summary>
        /// <param name="key">the foreign word for which to find a translation</param>
        /// <param name="translationDictionary">the dictionary with translation words</param>
        /// <returns></returns>
        public static string Translate(this string key, IDictionary<string, string> translationDictionary)
        {
            string result = translationDictionary.TryGetValue(key, out string name) == false ? key : name;

            return result;
        }

        public static IDictionary<string, string> Update(this IDictionary<string, string> oldDic, IDictionary<string, string> newDic)
        {
            foreach(var d in newDic)
            {
                 oldDic[d.Key] = d.Value;                
            }
            
            return oldDic;
        }

        public static IDictionary<string, string> GetObjectPropertiesValuesToString(this object obj, int maxAmountClassElements = 1000)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>(maxAmountClassElements);

            if (obj == null) return null;

            Type t = obj.GetType();

            PropertyInfo[] props = t.GetProperties();

            if (props?.Length > 0)
            {
                foreach (var prop in props)
                {
                    if (prop.GetIndexParameters().Length == 0)
                    { dic.Add(prop?.Name, prop?.GetValue(obj)?.ToString()); }
                    else
                    { dic.Add(prop?.Name, prop?.PropertyType?.Name); }
                }
            }

            return dic;
        }
        

        public static IDictionary<string, object> GetObjectPropertiesValuesToObject(this object obj, int maxAmountClassElements = 1000)
        {
            IDictionary<string, object> dic = new Dictionary<string, object>(maxAmountClassElements);

            if (obj == null) return null;

            Type t = obj.GetType();

            PropertyInfo[] props = t.GetProperties();

            if (props?.Length > 0)
            {
                foreach (var prop in props)
                {
                    if (prop.GetIndexParameters().Length == 0)
                    { dic.Add(prop?.Name, prop?.GetValue(obj)); }
                    else
                    { dic.Add(prop?.Name, prop?.PropertyType?.Name); }
                }
            }

            return dic;
        }


        /// <summary>
        /// Check correctness of query
        /// </summary>
        /// <param name="query"> sql statement </param>
        public static bool CheckIsSqlQuery(string query)
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


        public static string AsString(this IDictionary<string, string> dic)
        {
            string text = string.Empty;
            if (dic?.Count > 0)
            {
                foreach (var s in dic)
                {
                    text += $"{s.Key}:\t{s.Value}\r\n";
                }
            }

            return text;
        }


    }
}
