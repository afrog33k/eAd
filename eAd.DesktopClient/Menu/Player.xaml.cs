using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Serialization;
using eAd.DataViewModels;
using eAd.Utilities;
using Timer = System.Timers.Timer;

namespace DesktopClient.Menu
{
    public class Player : IPlayer
    {
        public MediaElement Control;
        public List<MediaListModel> Playlist;
      
   

    
        private int currentItem = 0;
        private System.Timers.Timer timer = new Timer();
        private double CurrentDuration = 0;


        public Player(List<MediaListModel> playlist,MediaElement control )
        {
            Playlist = playlist;
            Control = control;
        }
        public void Start()
        {
            Control.MediaOpened += MediaElementMediaOpened;
            ThreadPool.QueueUserWorkItem(
                (state) =>
                    {
                        Thread.Sleep(200);
                        Control.Dispatcher.BeginInvoke(

                            DispatcherPriority.Normal

                            , new DispatcherOperationCallback(delegate
                                                                  {



                                                                      Control.Source =
                                                                          new Uri(  System.IO.Path.GetDirectoryName(
                                                                              Assembly.GetExecutingAssembly().Location)+
                                                                                    "/DataCache/LoadVideo.m4v");
                                                                      Control.Play();
                                                                      //    PlayNextVideo();


                                                                      return null;

                                                                  }), null);

                    });
        }


        public void PlayNextVideo()
        {
            Control.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal
                , new DispatcherOperationCallback(delegate
                                                      {
                                                          lock (Playlist)
                                                          {
                                                              Thread.Sleep(200);
                                                              if (!_isPlaying)
                                                              {
                                                                  currentItem++;
                                                                  if (Playlist.Count > 0)
                                                                  {
                                                                      currentItem %= Playlist.Count;
                                                                      if (Control.Source.AbsolutePath !=
                                                                          Playlist[currentItem].Location)
                                                                      {
                                                                          Control.Position = TimeSpan.Zero;
                                                                          Control.Source =
                                                                              new Uri(Playlist[currentItem].Location);
                                                                          Control.Play();
                                                                      }
                                                                      //lastDuration = Player.NaturalDuration;
                                                                      //timer.Interval =lastDuration.TimeSpan.TotalMilliseconds;
                                                                      //timer.Enabled = true;
                                                                  }
                                                              }
                                                          }
                                                          return null;
                                                      }), null);
        }

        static Duration _lastDuration = new Duration(TimeSpan.Zero);
        private static volatile bool _isPlaying = false;
    

        private void MediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(200);
            lock (Playlist)
            {
                CurrentDuration = 0;
                _isPlaying = true;
                try
                {
                    _lastDuration = Playlist[currentItem].Duration;

                }
                catch (Exception)
                {

                    _lastDuration = new Duration(new TimeSpan(0));
                }

                if (Control.NaturalDuration != Duration.Automatic)
                {
                    _lastDuration = Control.NaturalDuration;
                }

                if (_lastDuration != new Duration(new TimeSpan(0)))
                {
                    timer.Enabled = false;
                    timer = new System.Timers.Timer(_lastDuration.TimeSpan.TotalMilliseconds);


                    timer.Elapsed += delegate
                                         {
                                             _isPlaying = false;
                                             PlayNextVideo();
                                         };
                    timer.Enabled = true;
                }
                else
                {
                    CurrentDuration += Constants.DefaultDuration;
                    //  timer = new System.Timers.Timer(Constants.DefaultDuration);
                    timer.Enabled = false;
                    timer = new System.Timers.Timer(Constants.DefaultDuration);
                    timer.Elapsed += delegate
                                         {
                                             _isPlaying = false;
                                             PlayNextVideo();
                                         };
                    timer.Enabled = true;
                }
                //    timer.Elapsed += _timer_Elapsed;
                //     timer.Enabled = true;
            }

        }

        public void UpdatePlayList()
        {
            lock (Playlist)
            {
                //var newPlayList = new List<FileInfo>();
                //newPlayList = PageSwitcher.Playlist.Select(d => new FileInfo(d)).ToList();

                //foreach (var player in Players)
                //{
                //    player.Playlist = PageSwitcher.Playlist;
                //}

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

                   
                        PlayNextVideo();
                    
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

        protected object FileLock = new object();
       
    }
}