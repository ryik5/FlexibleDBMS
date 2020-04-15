using System;
using System.Collections.Generic;
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
