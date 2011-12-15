using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Serialization;

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

            try
            {

          
            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            StreamReader reader = File.OpenText(Constants.PlayListFile);


            Playlist = (serializer.Deserialize(reader) as List<string>).Select(i=>new FileInfo(i)).ToList();

               reader.Close();

            }
            catch (Exception)
            {

               Playlist = new List<FileInfo>();
            }
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

        private List<FileInfo> Playlist;
        private int currentItem = 0;
        private System.Timers.Timer timer;
        private double CurrentDuration = 0;
         void _timer_Elapsed(object sender, ElapsedEventArgs e)
         {
             Duration playerDuration = new Duration();
            Player.Dispatcher.BeginInvoke(

System.Windows.Threading.DispatcherPriority.Normal

, new DispatcherOperationCallback(delegate
                                      {


                                 playerDuration=         Player.NaturalDuration;

    return null;

}), null);

            if ( _lastDuration == Duration.Automatic)
            {
               
                    timer.Enabled = false;
                    _lastDuration = playerDuration;
                    if (playerDuration.HasTimeSpan)
                    {
                        //Finally got the length
                        timer =
                            new System.Timers.Timer(_lastDuration.TimeSpan.TotalMilliseconds - CurrentDuration);
                    }
                    else
                    {
                        CurrentDuration += Constants.DefaultDuration;
                            //Not yet
                            timer =
                            new System.Timers.Timer(Constants.DefaultDuration);
                      
                    }
                timer.Elapsed += _timer_Elapsed;
                    timer.Enabled = true;
                return;
            }
             
                 
             
         

         


            PlayNextVideo();
         
        }

        private void PlayNextVideo()
        {
            Player.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal
                , new DispatcherOperationCallback(delegate
                                                      {
                                                          lock (Playlist)
                                                          {
                                                              currentItem++;
                                                              if (Playlist.Count > 0)
                                                              {
                                                                  currentItem %= Playlist.Count;
                                                                  Player.Position = TimeSpan.Zero;
                                                                  Player.Source = new Uri(Playlist[currentItem].FullName);
                                                                  Player.Play();
                                                                  //lastDuration = Player.NaturalDuration;
                                                                  //timer.Interval =lastDuration.TimeSpan.TotalMilliseconds;
                                                                  //timer.Enabled = true;
                                                              }
                                                          }
                                                          return null;
                                                      }), null);
        }

        static Duration _lastDuration = new Duration(TimeSpan.Zero);

        private void MediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
            lock (Playlist)
            {
                CurrentDuration = 0;
                _lastDuration = Player.NaturalDuration;
                if (_lastDuration != Duration.Automatic)
                {
                    timer = new System.Timers.Timer(_lastDuration.TimeSpan.TotalMilliseconds);
                }
                else
                {
                    CurrentDuration += Constants.DefaultDuration;
                  //  timer = new System.Timers.Timer(Constants.DefaultDuration);
                    var exitImageTimer = new System.Timers.Timer(Constants.DefaultDuration);
                    exitImageTimer.Elapsed +=  delegate {
                                                               PlayNextVideo();
                    };
                    exitImageTimer.Enabled = true;
                }
            //    timer.Elapsed += _timer_Elapsed;
           //     timer.Enabled = true;
            }
        }

        private void PlayerMediaEnded(object sender, RoutedEventArgs e)
        {
           // _timer_Elapsed(null, (ElapsedEventArgs) new EventArgs());
            PlayNextVideo();
        }

        private void PlayerUnloaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void FormFadeOutCompleted(object sender, EventArgs e)
        {

        }
        public  static  volatile Object FileLock = new object();
        public void  UpdatePlayList()
        {
            lock (Playlist)
            {
                var newPlayList = new List<FileInfo>();
                newPlayList = PageSwitcher.Playlist.Select(d => new FileInfo(d)).ToList();
                this.Playlist =newPlayList;
                PlayNextVideo();
                ThreadPool.QueueUserWorkItem((r) =>
                                                 {
                                                     lock (FileLock)
                                                     {
                                                     XmlSerializer serializer = new XmlSerializer(typeof (List<string>));
                                                     FileInfo file = new FileInfo(Constants.PlayListFile);
                                                     if (!File.Exists(Constants.PlayListFile))
                                                     {
                                                         StreamWriter writer = file.CreateText();
                                                         serializer.Serialize(writer, PageSwitcher.Playlist);
                                                         writer.Close();
                                                     }
                                                     else
                                                     {
                                                         file.Delete();
                                                         StreamWriter writer = file.CreateText();
                                                         serializer.Serialize(writer, PageSwitcher.Playlist);
                                                         writer.Close();
                                                     }
                                                      }
                                                 });
            }
        }
    }
}
