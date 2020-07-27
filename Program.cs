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

            CommonExtensions.Logger(LogTypes.Info, "");
            CommonExtensions.Logger(LogTypes.Info, $"{Properties.Resources.SymbolsSosSlash}{Properties.Resources.SymbolsSosSlash}");
            CommonExtensions.Logger(LogTypes.Info, "");
            CommonExtensions.Logger(LogTypes.Info, "");
            CommonExtensions.Logger(LogTypes.Info, "-= Загрузка ПО =-");
            CommonExtensions.Logger(LogTypes.Info, "");
            //Блок проверки уровня настройки логгирования
            CommonExtensions.Logger(LogTypes.Info,"Test Info message");
            CommonExtensions.Logger(LogTypes.Info, "Test1 Trace message");
            CommonExtensions.Logger(LogTypes.Info, "Test2 Debug message");
            CommonExtensions.Logger(LogTypes.Info, "Test3 Warn message");
            CommonExtensions.Logger(LogTypes.Info, "Test4 Error message");
            CommonExtensions.Logger(LogTypes.Info, "Test5 Fatal message");

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

            //    //Running only one copy. Try to run the main application's form
            //    Application.Run(new MainForm());
            //}

            Application.Run(new MainForm());
        }
    }
}