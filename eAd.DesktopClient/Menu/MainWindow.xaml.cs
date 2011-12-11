using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

namespace DesktopClient.Menu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: UserControl
    {
        static MainWindow _instance;
        public static MainWindow Instance
        {
            get
            {
             
                if(_instance==null)
                {
                    _instance = new MainWindow();
                }
                return _instance;
            }
        }
        public MainWindow()
        {
         
            InitializeComponent();
        //   Player.Play();
           // ((Viewbox)Player.Parent).Width = ((Window)Player.Parent.Parent)
            StatusBox.Visibility = Visibility.Collapsed;
            PlayerViewBox.Width = this.Width;
            PlayerViewBox.Height = this.Height;
         
            Playlist = new DirectoryInfo(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory
,"Ads")).GetFiles();
            ThreadPool.QueueUserWorkItem(
                (state)=>
            {
                Thread.Sleep(200);
                Player.Dispatcher.BeginInvoke(

   System.Windows.Threading.DispatcherPriority.Normal

   , new DispatcherOperationCallback(delegate
                                         {


                                           
                                          
                                             Player.Play();

                                            
                                            
       return null;

   }), null);
               
            });
        }

        private FileInfo[] Playlist;
        private int currentItem = 0;
        private System.Timers.Timer timer;
         void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Player.Dispatcher.BeginInvoke(

System.Windows.Threading.DispatcherPriority.Normal

, new DispatcherOperationCallback(delegate
                                      {


                                          currentItem++;
                                          currentItem %=Playlist.Length;
    Player.Position = TimeSpan.Zero;
    Player.Source = new Uri(Playlist[currentItem].FullName);
    Player.Play();
    //lastDuration = Player.NaturalDuration;
    //timer.Interval =lastDuration.TimeSpan.TotalMilliseconds;
    //timer.Enabled = true;


    return null;

}), null);
         
        }

        static Duration _lastDuration = new Duration(TimeSpan.Zero);

        private void MediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
           _lastDuration = Player.NaturalDuration; 
            timer = new System.Timers.Timer(_lastDuration.TimeSpan.TotalMilliseconds);
            timer.Elapsed += _timer_Elapsed;
            timer.Enabled = true;
        }

        private void PlayerMediaEnded(object sender, RoutedEventArgs e)
        {
        }

        private void PlayerUnloaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void FormFadeOutCompleted(object sender, EventArgs e)
        {

        }
    }
}
