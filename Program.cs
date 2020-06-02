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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

            // get GIUD application
            string appGuid =
            ((System.Runtime.InteropServices.GuidAttribute)System.Reflection.Assembly.GetExecutingAssembly().
            GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false).GetValue(0)).Value;

            logger.Info("");
            logger.Info($"{Properties.Resources.SosSlashSymbols}{Properties.Resources.SosSlashSymbols}");
            logger.Info("");
            logger.Info("");
            logger.Info("-= Загрузка ПО =-");
            logger.Info("");
            //Блок проверки уровня настройки логгирования
            logger.Info("Test Info message");
            logger.Trace("Test1 Trace message");
            logger.Debug("Test2 Debug message");
            logger.Warn("Test3 Warn message");
            logger.Error("Test4 Error message");
            logger.Fatal("Test5 Fatal message");

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