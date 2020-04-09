using System.Collections.Generic;
using System.Linq;

namespace AutoAnalysis
{
    public class CommandLineArguments
    {

        public delegate void InfoMessage(object sender, TextEventArgs e);
        public event InfoMessage EvntInfoMessage;


        char FindUsedDelimiter(string text)
        {
            char delimiter = '-';
            if (text.Trim().StartsWith("-"))
            {
                delimiter = '-';
            }
            else if (text.Trim().StartsWith("/"))
            {
                delimiter = '/';
            }

            return delimiter;
        }

        /// <summary>
        /// show Import Text File Button: -y  
        /// </summary>
        public IDictionary<string, string> CheckCommandLineArguments(string[] parameters)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            string[] arguments = null;
            //Get args
            //  string args = ToString(Environment.GetCommandLineArgs());

            string argumString = string.Empty;
            if (parameters?.Length > 0)
            {
                for (int i = 1; i < parameters?.Length; i++)
                {
                    argumString += parameters[i] + " ";
                }

                char delim = FindUsedDelimiter(argumString);

                arguments = argumString.Split(delim);
            }

            EvntInfoMessage.Invoke(this, new TextEventArgs(argumString));

            arguments.Any(x => x.StartsWith("a")); //admin

            arguments.Any(x => x.StartsWith("c")); //configuration db

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