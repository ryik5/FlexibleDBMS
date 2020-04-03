using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoAnalysis
{
  public static  class CommonExtesions
    {

        public static string ToString(this string [] array)
        {
            string text = string.Empty;

            foreach (var s in array)
            {
                text += $"{s.ToString()}\r\n";
            }
            return text;
        }

        public static string ToString(this IList<string> list)
        {
            string text = string.Empty;

            foreach (var s in list)
            {
                text += $"{s.ToString()}\r\n";
            }
            return text;
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
