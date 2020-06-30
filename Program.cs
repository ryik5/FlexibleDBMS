using System;
using System.Windows.Forms;

namespace FlexibleDBMS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //load libraries from this assembly
            AssemblyLoader.RegisterAssemblyLoader();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // get GIUD application
            string appGuid =
            ((System.Runtime.InteropServices.GuidAttribute)System.Reflection.Assembly.GetExecutingAssembly().
            GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false).GetValue(0)).Value;

            CommonExtesions.Logger(LogTypes.Info, "");
            CommonExtesions.Logger(LogTypes.Info, $"{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}");
            CommonExtesions.Logger(LogTypes.Info, "");
            CommonExtesions.Logger(LogTypes.Info, "");
            CommonExtesions.Logger(LogTypes.Info, "-= Загрузка ПО =-");
            CommonExtesions.Logger(LogTypes.Info, "");
            //Блок проверки уровня настройки логгирования
            CommonExtesions.Logger(LogTypes.Info,"Test Info message");
            CommonExtesions.Logger(LogTypes.Info, "Test1 Trace message");
            CommonExtesions.Logger(LogTypes.Info, "Test2 Debug message");
            CommonExtesions.Logger(LogTypes.Info, "Test3 Warn message");
            CommonExtesions.Logger(LogTypes.Info, "Test4 Error message");
            CommonExtesions.Logger(LogTypes.Info, "Test5 Fatal message");

            //using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, "Global\\" + appGuid))
            //{
            //    if (!mutex.WaitOne(0, false))
            //    {
            //        //writing info about attempt to run another copy of the application
            //        logger.Warn("Попытка запуска второй копии программы");
            //        System.Threading.Tasks.Task.Run(() => MessageBox.Show("Программа уже запущена. Попытка запуска второй копии программы"));
            //        System.Threading.Thread.Sleep(5000);
            //        return;
            //    }

            //    //Running jnly one copy. Try to run the main application's form
            //    Application.Run(new MainForm());
            //}


            Application.Run(new MainForm());
        }
    }
}