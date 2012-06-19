using System.IO;

namespace ClientApp
{
    using ClientApp.Core;
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;

   
    public partial class App : Application
    {


        private static string _productName;

        public App()
        {
            base.StartupUri = new Uri("ClientManager.xaml", UriKind.Relative);
            base.ShutdownMode = ShutdownMode.OnMainWindowClose;
            bool createdNew = true;
            using (new Mutex(true, "eAd Advert Platform", out createdNew))
            {
                if (createdNew)
                {
                    base.Run();
                }
                else
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    foreach (Process process2 in Process.GetProcessesByName(currentProcess.ProcessName))
                    {
                        if (process2.Id != currentProcess.Id)
                        {
                            SetForegroundWindow(process2.MainWindowHandle);
                            return;
                        }
                    }
                }
            }
        }

        public static void Close()
        {
            ShutDownRequested = true;
            Application.Current.Shutdown();
        }

        public static void DoEvents()
        {
        }

      
        [STAThread, DebuggerNonUserCode]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException+=ReportAndRestart;
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        private static void ReportAndRestart(object sender, UnhandledExceptionEventArgs e)
        {
            string info = e.ExceptionObject.ToString();
            if(File.Exists("crash.log")) 
                File.Delete("crash.log");
            File.Create("crash.log");
            File.WriteAllText("crash.log","App Crashed: " + DateTime.Now.ToString() + " : " + info);
            System.Diagnostics.Process.Start(
                System.Reflection.Assembly.GetEntryAssembly().Location,
                string.Join(" ",  Environment.GetCommandLineArgs()))
                ;
            Environment.Exit(1);
        }

        private static void RunApp(string[] arguments)
        {
            bool createdNew = true;
            new Mutex(true, "UniqueApplicationName", out createdNew);
            if (createdNew)
            {
                Splasher.Splash = new ClientApp.SplashScreen();
                Splasher.ShowSplash();
                new App();
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        public static void SingleInstanceArgumentsReceived(object sender, ArgumentsReceivedEventArgs e)
        {
        }

        public static string ProductName
        {
            get
            {
                return "eAd Desktop";
            }
            set
            {
                _productName = value;
            }
        }

        public static bool ShutDownRequested { get; set; }
        public static string UserAppDataPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
    }
}

