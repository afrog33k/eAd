﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Serialization;
using eAd.DataViewModels;

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


                XmlSerializer serializer = new XmlSerializer(typeof(List<MediaListModel>));
            StreamReader reader = File.OpenText(Constants.PlayListFile);


            Playlist = (serializer.Deserialize(reader) as List<MediaListModel>);

               reader.Close();

            }
            catch (Exception)
            {

               Playlist = new List<MediaListModel>();
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

        private List<MediaListModel> Playlist;
        private int currentItem = 0;
        private System.Timers.Timer timer;
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
                                                                  if (Player.Source.AbsolutePath != Playlist[currentItem].Location)
                                                                  {
                                                                      
                                                                
                                                                  Player.Position = TimeSpan.Zero;
                                                                  Player.Source = new Uri(Playlist[currentItem].Location);
                                                                  Player.Play();
                                                                  }
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
                //var newPlayList = new List<FileInfo>();
                //newPlayList = PageSwitcher.Playlist.Select(d => new FileInfo(d)).ToList();
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
                                                             if (!mediaListModel.Downloaded)
                                                             {
                                                                 alldownloaded = false;
                                                             }
                                                         }
                                                       
                                                     }
                                                    Instance.PlayNextVideo();
                                                 });
                ThreadPool.QueueUserWorkItem((r) =>
                                                 {
                                                     lock (FileLock)
                                                     {
                                                     XmlSerializer serializer = new XmlSerializer(typeof (List<MediaListModel>));
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
