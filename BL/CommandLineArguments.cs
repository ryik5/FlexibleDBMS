using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAnalyse
{
 public   class CommandLineArguments
    {

        public delegate void InfoMessage(object sender, TextEventArgs e);
        public event InfoMessage EvntInfoMessage;


       char FindUsedDelimiter(string text)
        {
            char d='-';
            if (text.Trim().StartsWith("-"))
            {
                d = '-';
            }
            else if (text.Trim().StartsWith("/"))
            {
                d = '/';
            }

            return d;
        }

        /// <summary>
        /// show Import Text File Button: -y  
        /// </summary>
        public IDictionary<string, string> CheckCommandLineArguments(string[] parameters)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();

            //Get args
            //  string args = ToString(Environment.GetCommandLineArgs());
            
            char delim = '-';
            string argumString = string.Empty;
            if (parameters?.Length > 0)
            {
                for (int i = 1; i < parameters?.Length; i++)
                {
                    argumString += parameters[i] + " ";                    
                }
            }

            string[] arguments = argumString.Split(delim);



            EvntInfoMessage.Invoke(this, new TextEventArgs(argumString));

            //if (args?.Length > 1)
            //{
            //    //remove delimiters
            //    string envParameter = args[1]?.Trim()?.TrimStart('-', '/')?.ToLower();
            //    if (envParameter.StartsWith("y"))
            //    {
            //        administratorMenu.Enabled = true;
            //    }
            //    else if (envParameter.StartsWith("config"))
            //    {
            //        appDbPath = envParameter.Trim('\\', '/', ':', ';', '|', ' ').Replace("config", "");
            //    }
            //    else if (envParameter.StartsWith("n"))
            //    {
            //        administratorMenu.Enabled = false;
            //    }
            //}
            //else
            //{
            //    administratorMenu.Enabled = false;
            //}

            //sqLiteConnectionString = $"Data Source = {appDbPath}; Version=3;";

            return dic;
        }

        string ToString(string[] array)
        {
            string text = string.Empty;

            foreach (var s in array)
            {
                text += $"{s.ToString()} ";
            }
            return text;
        }

    }
}
