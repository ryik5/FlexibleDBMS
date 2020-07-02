using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    public static class CommonExtensions
    {
        /// <summary>
        ///  Convert: "39270"  => "07/07/2007" OR "39456" => "01/09/2008"
        /// </summary>
        /// <param name="date">date as string in 5 digits</param>
        /// <returns>dd/MM/yyyy</returns>
        public static string FromOQDateToRealDate(this string date)
        {
            double d = double.Parse(date);
            DateTime conv = DateTime.FromOADate(d);
            return conv.ToString("dd/MM/yyyy");
        }

       static readonly object obj = new object();
        public static void Logger(LogTypes typo, string Event)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string pathToLogDir, pathToLog;
            try
            {
                pathToLogDir = Path.Combine(Path.GetDirectoryName(path), $"logs");
                if (!Directory.Exists(pathToLogDir))
                    Directory.CreateDirectory(pathToLogDir);

                pathToLog = Path.Combine(pathToLogDir, $"{DateTime.Now.ToString("yyyy-MM-dd")}.log");
                lock (obj)
                {
                    using (StreamWriter writer = new StreamWriter(pathToLog, true))
                    {

                        writer.WriteLine($"{DateTime.Now.ToString("yyyy.MM.dd|hh:mm:ss")}|{typo}|{Event}");
                        writer.Flush();
                    }
                }
            }
            catch (Exception err)
            {
                pathToLog = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".log");
                lock (obj)
                {
                    using (StreamWriter writer = new StreamWriter(pathToLog, true))
                    {
                        writer.WriteLine($"{DateTime.Now.ToString("yyyy.MM.dd|hh:mm:ss")}|{err.ToString()}");
                        writer.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Replace substring of case insensetive in the text
        /// </summary>
        /// <param name="input">inputed text</param>
        /// <param name="search">substring for replace</param>
        /// <param name="replacement">new substring</param>
        /// <returns>new string where old substring replaced by new one</returns>
        public static string ReplaceCaseInsensitive(string input, string search, string replacement)
        {
            string result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );
            return result;
        }

        public static bool CheckQueryToReplaceWords(string query)
        {
            string[] words = query?.Split(',', ' ', ')', '(', ';');

            if (!(words?.Length > 0))
            { return false; }

            if
              (!(words?.Count(x => x.ToLower().Equals("select")) > 0 &&
                words?.Count(x => x.ToLower().Equals("from")) > 0 &&
                words?.Count(x => x.ToLower().Equals("maindata")) > 0 &&
                (words?.Count(x => x.ToLower().StartsWith("column")) > 0 ))) //|| words?.Count(x => x.Equals("*"))>0 
            { return false; }


            return true;
        }
        
        /// <summary>
        /// Search translation of the word 'key' from the prepared dictionary 
        /// </summary>
        /// <param name="key">the foreign word for which need to find a translation</param>
        /// <param name="translationDictionary">the dictionary with translation words</param>
        /// <returns></returns>
        public static string Translate(this string key, IDictionary<string, string> translationDictionary)
        {
            return translationDictionary.TryGetValue(key, out string name) == false ? key : name; 
        }
        
        public static IDictionary<string, string> DoObjectPropertiesAsStringDictionary(this object obj, int maxAmountClassElements = 1000)
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
        
        public static IDictionary<string, object> DoObjectPropertiesAsObjectDictionary(this object obj, int maxAmountClassElements = 1000)
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

        ///// <summary>
        ///// Check correctness of query
        ///// </summary>
        ///// <param name="query"> sql statement </param>
        //public static bool CheckIsSqlQuery(string query)
        //{
        //    bool isQuery = true;
        //    if (string.IsNullOrEmpty(query))
        //    {
        //        isQuery = false;
        //       // Status?.Invoke(this, new TextEventArgs("Error. The query can not be empty or null!"));
        //       // new ArgumentNullException();
        //    }

        //    string[] word = query.Split(' ');
        //    if (word?.Length < 3 ||
        //       !word[0].ToLower().Equals("select") ||
        //       !word[0].ToLower().Equals("update") ||
        //       !word[0].ToLower().Equals("insert") ||
        //       !word[0].ToLower().Equals("create") ||
        //       !word[0].ToLower().Equals("delete") ||
        //       !word[0].ToLower().Equals("replace")||
        //       !word[0].ToLower().Equals("backup") ||
        //       !word[0].ToLower().Equals("restore")||
        //       !word[0].ToLower().Equals("repair") ||
        //       !word[0].ToLower().Equals("check"))
        //    {
        //        //   Status?.Invoke(this, new TextEventArgs(
        //        //       "Query is wrong! It does not start with appropriate commands. Query must start with any these commands: " +
        //        //       $"'SELECT, UPDATE, INSERT, CREATE, DELETE, REPLACE'\r\nYour query:  {query.ToUpper()}"));
        //        //   new ArgumentException();
        //        isQuery = false;
        //    }

        //    if (word.Where(x => x.ToLower().Equals("from")).Count() == 0)
        //    {
        //        isQuery = false;

        //        //      Status?.Invoke(this, new TextEventArgs(
        //        //          $"Query is wrong! It does not contain any query to a table. " +
        //        //          $"Check your expresion near 'FROM'\r\nYour query:  {query.ToUpper()}"));
        //        //     new ArgumentException();
        //    }

        //    return isQuery;
        //}

        public static string OpenFileDialogReturnPath(this OpenFileDialog ofd, string fileFilter, string title) //Return its name 
        {
            ofd.FileName = @"";
            ofd.Title = title;
            ofd.Filter = fileFilter;
            ofd.ShowDialog();
            string filePath = ofd.FileName;

            return filePath;
        }
        public static string SaveFileDialogReturnPath(this SaveFileDialog sfd,string fileName, string fileFilter, string title) //Return its name 
        {
            sfd.FileName = fileName??@"";
            sfd.Title = title;
            sfd.Filter = fileFilter;
            sfd.ShowDialog();
            string filePath = sfd.FileName;

            return filePath;
        }

        public static void AppendLine(this TextBox source, string value = "\r\n")
        {
            if (source != null)
            {
                if (source?.Text?.Length == 0 && value != null && value?.Length > 0)
                    source.Text = value;
                else if (value != null && value?.Length > 0)
                    source.AppendText($"{Environment.NewLine}{value}");
                else
                    source.AppendText($"{Environment.NewLine}");
            }
        }

        public static string AsString(this IDictionary<string, string> dic)
        {
            string text = string.Empty;
            if (dic?.Count > 0)
            {
                foreach (var s in dic)
                {
                    text += $"{s.Key}:\t{s.Value}{Environment.NewLine}";
                }
            }

            return text;
        }

        public static string ToStringNewLine(this IList<string> dic)
        {
            string text = string.Join(Environment.NewLine, dic.ToArray());
            return text;
        }

        public static string ToStringComa(this IList<string> dic)
        {
            string text = string.Join(",", dic.ToArray());
            return text;
        }
    }
}
