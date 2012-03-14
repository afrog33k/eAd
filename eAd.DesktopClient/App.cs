using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;

namespace DesktopClient
{


    /// <summary>
    /// 
    /// </summary>
    internal class App : Application
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 
        /// </summary>
        [STAThread()]
        private static void Main()
        {
            Splasher.Splash = new SplashScreen();
            Splasher.ShowSplash();



            //for ( int i = 0; i < 5000; i++ )
            //{
            //MessageListener.Instance.ReceiveMessage ( string.Format ( "Load module {0}", "Database..." ) );
            //Thread.Sleep ( 500 );
            //MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "Database...Done"));
            //Thread.Sleep(500);
            //MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "AdThrower..."));
            //Thread.Sleep(500);
            //MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "AdThrower...Done"));
            //Thread.Sleep(500);
            //MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "Schedule Master..."));
            //Thread.Sleep(500);
            //MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "Schedule Master...Done"));
            //Thread.Sleep(500);
            //}

            new App();
        }

        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            StartupUri = new Uri("PageSwitcher.xaml", UriKind.Relative);


            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "eAd Advert Platform", out createdNew))
            {
                if (createdNew)
                {
                    Run();
                }
                else
                {
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }

            }
        }
    }
}
