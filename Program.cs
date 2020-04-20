using System;
using System.Windows.Forms;

namespace AutoAnalysis
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    internal static class SQLConnection
    {
        public static SQLConnectionSettings Settings { get; set; }
    }
}
