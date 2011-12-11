using System;
using System.Windows;
using System.Threading;

namespace DesktopClient
{
    /// <summary>
    /// 
    /// </summary>
    class App: Application
    {
        /// <summary>
        /// 
        /// </summary>
        [STAThread ( )]
        static void Main ( )
        {
            Splasher.Splash = new SplashScreen ( );
            Splasher.ShowSplash ( );

            //for ( int i = 0; i < 5000; i++ )
            //{
                MessageListener.Instance.ReceiveMessage ( string.Format ( "Load module {0}", "Database..." ) );
                Thread.Sleep ( 500 );
                MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "Database...Done"));
                Thread.Sleep(500);
                MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "AdThrower..."));
                Thread.Sleep(500);
                MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "AdThrower...Done"));
                Thread.Sleep(500);
                MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "Schedule Master..."));
                Thread.Sleep(500);
                MessageListener.Instance.ReceiveMessage(string.Format("Load module {0}", "Schedule Master...Done"));
                Thread.Sleep(500);
            //}

            new App ( );
        }
         /// <summary>
        /// 
        /// </summary>
        public App ( )
        {         
            StartupUri = new Uri ( "PageSwitcher.xaml", UriKind.Relative );
      
            Run ( );            
        }
    }
}
