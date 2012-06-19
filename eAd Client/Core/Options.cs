using System.IO;

namespace ClientApp.Core
{
    using ClientApp;
    using System;
    using System.Windows.Forms;

    internal static class Options
    {
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += ReportAndRestart;
         
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          OptionForm mainForm = new global::ClientApp.OptionForm();
            Application.Run(mainForm);
        }

        private static void ReportAndRestart(object sender, UnhandledExceptionEventArgs e)
        {
            string info = e.ExceptionObject.ToString();
            if (File.Exists("crash.log"))
                File.Delete("crash.log");
            File.Create("crash.log");
            File.WriteAllText("crash.log", "App Crashed: " + DateTime.Now.ToString() + " : " + info);
            System.Diagnostics.Process.Start(
                System.Reflection.Assembly.GetEntryAssembly().Location,
                string.Join(" ", Environment.GetCommandLineArgs()))
                ;
            Environment.Exit(1);
        }
    }
}

