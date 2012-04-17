namespace DesktopClient
{

    using System;

    using System.Diagnostics;

    using System.Runtime.InteropServices;

    using System.Threading;

    using System.Windows;



    internal class App : Application
    {

        public static PageSwitcher Switcher;



        public App()
        {

            base.StartupUri = new Uri("PageSwitcher.xaml", UriKind.Relative);

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



        [STAThread]

        private static void Main(params string[] args)
        {

            Guid identifier = new Guid("{6EAE2E61-E7EE-42bf-8EBE-BAB890C5410F}");

            using (SingleInstance instance = new SingleInstance(identifier))
            {

                if (instance.IsFirstInstance)
                {

                    instance.ArgumentsReceived += new EventHandler<ArgumentsReceivedEventArgs>(App.singleInstance_ArgumentsReceived);

                    instance.ListenForArgumentsFromSuccessiveInstances();

                    RunApp(args);

                }

                else
                {

                    instance.PassArgumentsToFirstInstance(args);

                }

            }

        }



        private static void RunApp(string[] arguments)
        {

            bool createdNew = true;

            new Mutex(true, "UniqueApplicationName", out createdNew);

            if (createdNew)
            {

                Splasher.Splash = new DesktopClient.SplashScreen();

                Splasher.ShowSplash();

                new App();

            }

        }



        [return: MarshalAs(UnmanagedType.Bool)]

        [DllImport("user32.dll")]

        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private static void singleInstance_ArgumentsReceived(object sender, ArgumentsReceivedEventArgs e)
        {

            if (PageSwitcher.Instance != null)
            {

                PageSwitcher.Instance.RunCommands(e.Args);

            }

        }

    }

}



