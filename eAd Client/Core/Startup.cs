namespace ClientApp.Core
{
    using ClientApp;
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    internal class Startup
    {
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUnhandledException(e);
        }

        private static void HandleUnhandledException(object o)
        {
            Exception exception = o as Exception;
            Debug.WriteLine(new LogMessage("Main", "Unhandled Exception: " + exception.Message), LogType.Error.ToString());
            Debug.WriteLine(new LogMessage("Main", "Stack Trace: " + exception.StackTrace), LogType.Error.ToString());
            Environment.Exit(1);
        }

        [STAThread]
        private static void Main(string[] args)
        {
            Guid identifier = new Guid("{6EAE2E61-E7EE-42bf-8EBE-BAB890C5410F}");
            using (SingleInstance instance = new SingleInstance(identifier))
            {
                if (instance.IsFirstInstance)
                {
                    instance.ArgumentsReceived = (EventHandler<ArgumentsReceivedEventArgs>) Delegate.Combine(instance.ArgumentsReceived, new EventHandler<ArgumentsReceivedEventArgs>(App.SingleInstanceArgumentsReceived));
                    instance.ListenForArgumentsFromSuccessiveInstances();
                    RunApp(args);
                }
                else
                {
                    instance.PassArgumentsToFirstInstance(args);
                }
            }
        }

        private static void RunApp(string[] arg)
        {
            Process[] processesByName = Process.GetProcessesByName("eAd Client");
            if (processesByName.Length <= 1)
            {
                Trace.Listeners.Add(new ClientTraceListener());
                Trace.AutoFlush = false;
                try
                {
                    if (arg.GetLength(0) > 0)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Trace.WriteLine(new LogMessage("Main", "Options Started"), LogType.Info.ToString());
                        Form mainForm = new global::ClientApp.OptionForm();
                        Application.Run(mainForm);
                    }
                    else
                    {
                        bool createdNew = true;
                        new Mutex(true, "UniqueApplicationName", out createdNew);
                        if (createdNew)
                        {
                            Splasher.Splash = new SplashScreen();
                            Splasher.ShowSplash();
                            Trace.WriteLine(new LogMessage("Main", "Client Started"), LogType.Info.ToString());
                            new App().Run();
                        }
                    }
                }
                catch (Exception exception)
                {
                    HandleUnhandledException(exception);
                }
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Startup.CurrentDomain_UnhandledException);
                Application.ThreadException += new ThreadExceptionEventHandler(Startup.Application_ThreadException);
                Trace.WriteLine(new LogMessage("Main", "Application Finished"), LogType.Info.ToString());
                Trace.Flush();
            }
            else
            {
                ShowWindowAsync(processesByName[0].MainWindowHandle, 6);
                ShowWindowAsync(processesByName[0].MainWindowHandle, 9);
            }
        }

        [DllImport("User32.dll")]
        public static extern int ShowWindowAsync(IntPtr hWnd, int swCommand);
    }
}

