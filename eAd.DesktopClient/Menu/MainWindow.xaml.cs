using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using eAd.DataViewModels;
using eAd.Utilities;
using Timer = System.Timers.Timer;

namespace DesktopClient.Menu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl//, IPlayer
    {
        static MainWindow _instance;
        public static MainWindow Instance
        {
            get
            {

                if (_instance == null)
                {
                    _instance = new MainWindow();
                }
                return _instance;
            }
            set { _instance = value; }
        }
        List<Player> Players = new List<Player>();


        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
            //   Player.Play();
            // ((Viewbox)Player.Parent).Width = ((Window)Player.Parent.Parent)
            StatusBox.Visibility = Visibility.Collapsed;
            MediaGrid.Width = SystemParameters.PrimaryScreenWidth;
         MediaGrid.Height = SystemParameters.PrimaryScreenHeight;
         MediaCanvas.Width = MediaGrid.Width;
         MediaCanvas.Height = MediaGrid.Height;
         eAdWindow.Width = MediaGrid.Width;
         eAdWindow.Height = MediaGrid.Height;
            try
            {


                XmlSerializer serializer = new XmlSerializer(typeof(List<MediaListModel>));
                StreamReader reader = File.OpenText(Constants.PlayListFile);


                Playlist = (serializer.Deserialize(reader) as List<MediaListModel>);

                reader.Close();

            }
            catch (Exception)
            {

                Playlist = new List<MediaListModel>();
            }



         

            
         
        }


        private List<MediaElement> _mediaElements = new List<MediaElement>(); 

        public void LoadPositions()
        {

            eAdWindow.Dispatcher.BeginInvoke(

    DispatcherPriority.Normal

    , new DispatcherOperationCallback(delegate
                                          {
                                              foreach (var position in PageSwitcher.MainMosaic.Positions)
         {
             var posn = position;
             var newBtn = new MediaElement();
             newBtn.LoadedBehavior = MediaState.Manual;
             
             newBtn.Stretch = Stretch.UniformToFill;

             newBtn.Width = ((double)posn.Width / 768) * SystemParameters.PrimaryScreenWidth;

             newBtn.Height = ((double)posn.Height / 1366) * SystemParameters.PrimaryScreenHeight; 
             newBtn.Margin = new Thickness(((double) posn.X/ 768) * SystemParameters.PrimaryScreenWidth,( (double) posn.Y / 1366) * SystemParameters.PrimaryScreenHeight, 0, 0);

             //newBtn.Source =
             //    new Uri(System.IO.Path.GetDirectoryName(
             //        Assembly.GetExecutingAssembly().Location) +
             //            "/DataCache/LoadVideo.m4v");
             //newBtn.Play();

                                                  Playlist = new List<MediaListModel>();
           var pList=  position.Media.Select(
                 media =>
                 new MediaListModel()
                     {MediaID = media.MediaID, Location = media.Location, Duration = (TimeSpan) media.Duration}).ToList();

           var player = new Player(Playlist, newBtn);

           foreach (var mediaListModel in pList)
             {
                 PageSwitcher.DownloadMedium(mediaListModel,Playlist,player);
             }
          

          
             Players.Add(player);
             MediaCanvas.Children.Add(newBtn);
                                                  player.Start();
            
         }
                                              return null;
                                          }), null);


        //        ThreadPool.QueueUserWorkItem(
        //         (state) =>
        //         {


        //             Thread.Sleep(200);
        //             Player.Dispatcher.BeginInvoke(

        //DispatcherPriority.Normal

        //, new DispatcherOperationCallback(delegate
        //{


        //    foreach (var mediaelement in _mediaElements)
        //    {


        //        mediaelement.Source =
        //            new Uri(System.IO.Path.GetDirectoryName(
        //                Assembly.GetExecutingAssembly().Location) +
        //                    "/DataCache/LoadVideo.m4v");
        //        mediaelement.Play();
        //    }
        //    //    PlayNextVideo();


        //    return null;

        //}), null);

        //         });
        }
        private List<MediaListModel> Playlist;
        private int currentItem = 0;
        private System.Timers.Timer timer = new Timer();
        private double CurrentDuration = 0;
       
        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //             Duration playerDuration = new Duration();
            //            Player.Dispatcher.BeginInvoke(

            //System.Windows.Threading.DispatcherPriority.Normal

            //, new DispatcherOperationCallback(delegate
            //                                      {


            //                                 playerDuration=         Player.NaturalDuration;

            //    return null;

            //}), null);



            timer.Enabled = false;

            //if (Playlist[currentItem].Duration==TimeSpan.Zero)
            //{
            //    CurrentDuration += Constants.DefaultDuration;
            //        //Not yet
            //        timer =
            //        new System.Timers.Timer(Constants.DefaultDuration);

            //}
            //else
            //{
            //      //Not yet
            //        timer =
            //        new System.Timers.Timer(Playlist[currentItem].Duration.TotalMilliseconds);
            //}
            //timer.Elapsed += _timer_Elapsed;
            //       timer.Enabled = true;
            //   return;

         //   PlayNextVideo();

        }


        //private void PlayNextVideo()
        //{
        //    Instance.Player.Dispatcher.BeginInvoke(
        //        DispatcherPriority.Normal
        //        , new DispatcherOperationCallback(delegate
        //                                              {
        //                                                  lock (Playlist)
        //                                                  {
        //                                                      Thread.Sleep(200);
        //                                                      if (!_isPlaying)
        //                                                      {
        //                                                          currentItem++;
        //                                                          if (Playlist.Count > 0)
        //                                                          {
        //                                                              currentItem %= Playlist.Count;
        //                                                              if (Instance.Player.Source.AbsolutePath !=
        //                                                                  Playlist[currentItem].Location)
        //                                                              {


        //                                                                  foreach (var mediaelement in _mediaElements)
        //                                                                  {

        //                                                                      mediaelement.Position = TimeSpan.Zero;
        //                                                                      mediaelement.Source =
        //                                                                         new Uri(Playlist[currentItem].Location);
        //                                                                      mediaelement.Play();
        //                                                                  }


        //                                                                  //Instance.Player.Position = TimeSpan.Zero;
        //                                                                  //Instance.Player.Source =
        //                                                                  //    new Uri(Playlist[currentItem].Location);
        //                                                                  //Instance.Player.Play();
        //                                                              }
        //                                                              //lastDuration = Player.NaturalDuration;
        //                                                              //timer.Interval =lastDuration.TimeSpan.TotalMilliseconds;
        //                                                              //timer.Enabled = true;
        //                                                          }
        //                                                      }
        //                                                  }
        //                                                  return null;
        //                                              }), null);
        //}

        static  Duration _lastDuration = new Duration(TimeSpan.Zero);
        private static volatile bool _isPlaying = false;

        //private void MediaElementMediaOpened(object sender, RoutedEventArgs e)
        //{
        //    Thread.Sleep(200);
        //    lock (Playlist)
        //    {
        //        CurrentDuration = 0;
        //        _isPlaying = true;
        //        try
        //        {
        //            _lastDuration = Playlist[currentItem].Duration;
             
        //        }
        //        catch (Exception)
        //        {
                    
        //            _lastDuration =new Duration(new TimeSpan(0));
        //        }

        //        if(   Player.NaturalDuration!=Duration.Automatic)
        //        {
        //            _lastDuration = Player.NaturalDuration;
        //        }
              
        //        if (_lastDuration != new Duration(new TimeSpan(0)))
        //        {
        //            timer.Enabled = false;
        //            timer = new System.Timers.Timer(_lastDuration.TimeSpan.TotalMilliseconds);


        //            timer.Elapsed += delegate
        //            {
        //                _isPlaying = false;
        //                PlayNextVideo();
        //            };
        //            timer.Enabled = true;
        //        }
        //        else
        //        {
        //            CurrentDuration += Constants.DefaultDuration;
        //            //  timer = new System.Timers.Timer(Constants.DefaultDuration);
        //            timer.Enabled = false;
        //            timer = new System.Timers.Timer(Constants.DefaultDuration);
        //            timer.Elapsed += delegate
        //                                          {
        //                                              _isPlaying = false;
        //                                              PlayNextVideo();
        //                                          };
        //            timer.Enabled = true;
        //        }
        //        //    timer.Elapsed += _timer_Elapsed;
        //        //     timer.Enabled = true;
        //    }
           
        //}

        private void PlayerMediaEnded(object sender, RoutedEventArgs e)
        {
     //       
        //    PlayNextVideo();
        }

        private void PlayerUnloaded(object sender, RoutedEventArgs e)
        {

        }

        private void FormFadeOutCompleted(object sender, EventArgs e)
        {

        }
        public static volatile Object FileLock = new object();

        public void UpdatePlayList()
        {
            lock (Playlist)
            {
                //var newPlayList = new List<FileInfo>();
                //newPlayList = PageSwitcher.Playlist.Select(d => new FileInfo(d)).ToList();

                foreach (var player in Players)
                {
                    player.Playlist = PageSwitcher.Playlist;
                }

                this.Playlist = PageSwitcher.Playlist;
                
                ThreadPool.QueueUserWorkItem((state) =>
                                                 {
                                                     bool alldownloaded = false;
                                                     while (!alldownloaded)
                                                     {
                                                         Thread.Sleep(1000);
                                                         alldownloaded = true;
                                                         foreach (var mediaListModel in PageSwitcher.Playlist)
                                                         {
                                                             if (!mediaListModel.Downloaded )
                                                             {
                                                                 alldownloaded = false;
                                                             }
                                                         }

                                                     }

                                                     foreach (var player in Players)
                                                     {
                                                         player.PlayNextVideo();
                                                     }
                                                //     Instance.PlayNextVideo();
                                                 });

                ThreadPool.QueueUserWorkItem((r) =>
                                                 {
                                                     lock (FileLock)
                                                     {
                                                         XmlSerializer serializer = new XmlSerializer(typeof(List<MediaListModel>));
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
