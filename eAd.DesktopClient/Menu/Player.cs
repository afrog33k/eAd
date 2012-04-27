namespace DesktopClient.Menu
{

    using DesktopClient;

    using eAd.DataViewModels;

    using eAd.Utilities;

    using System;

    using System.Collections.Generic;

    using System.IO;

    using System.Reflection;

    using System.Runtime.CompilerServices;

    using System.Threading;

    using System.Timers;

    using System.Windows;

    using System.Windows.Controls;

    using System.Windows.Media.Animation;

    using System.Windows.Threading;

    using System.Xml.Serialization;



    public class Player : IPlayer
    {

        private static volatile bool _isPlaying = false;

        private static Duration _lastDuration = new Duration(TimeSpan.Zero);

        public MediaElement BackControl;

        public MediaElement Control;

        public MediaElement CurrentControl;

        private double CurrentDuration;

        private int currentItem;

        protected object FileLock = new object();

        public List<MediaListModel> Playlist;

        private System.Timers.Timer timer = new System.Timers.Timer();



        public Player(List<MediaListModel> playlist, MediaElement control)
        {

            this.Playlist = playlist;

            this.Control = control;

            this.BackControl = new MediaElement();

            this.BackControl.Width = this.Control.Width;

            this.BackControl.Height = this.Control.Height;

            this.BackControl.LoadedBehavior = this.Control.LoadedBehavior;

            this.BackControl.Stretch = this.Control.Stretch;

            this.BackControl.Margin = control.Margin;

            this.BackControl.Visibility = Visibility.Hidden;

            ((Canvas)control.Parent).Children.Add(this.BackControl);

        }



        private RoutedEventHandler CurrentControlOnMediaOpened(MediaElement lastControl)
        {

            return delegate(object j, RoutedEventArgs y)
            {

                this.CurrentControl.Visibility = Visibility.Visible;

                DoubleAnimation animation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(1300.0)));

                DoubleAnimation animation2 = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromMilliseconds(1300.0)));

                animation.AutoReverse = false;

                animation2.AutoReverse = false;

                animation2.Completed += delegate(object f, EventArgs k)
                {

                    lastControl.Stop();

                    lastControl.Visibility = Visibility.Hidden;

                };

                animation.Completed += delegate(object f, EventArgs k)
                {

                    lastControl.Stop();

                    lastControl.Visibility = Visibility.Hidden;

                };

                lastControl.BeginAnimation(UIElement.OpacityProperty, animation2);

                this.CurrentControl.BeginAnimation(UIElement.OpacityProperty, animation);

            };

        }



        private void MediaElementMediaOpened(object sender, RoutedEventArgs e)
        {

            ElapsedEventHandler handler = null;

            ElapsedEventHandler handler2 = null;

            Thread.Sleep(200);

            lock (this.Playlist)
            {

                this.CurrentDuration = 0.0;

                _isPlaying = true;

                try
                {

                    _lastDuration = this.Playlist[this.currentItem].Duration;

                }

                catch (Exception)
                {

                    _lastDuration = new Duration(new TimeSpan(0L));

                }

                if (this.CurrentControl.NaturalDuration != Duration.Automatic)
                {

                    _lastDuration = this.CurrentControl.NaturalDuration;

                }

                if (_lastDuration != new Duration(new TimeSpan(0L)))
                {

                    this.timer.Enabled = false;

                    this.timer = new System.Timers.Timer(_lastDuration.TimeSpan.TotalMilliseconds);

                    if (handler == null)
                    {

                        handler = delegate
                        {

                            _isPlaying = false;

                            this.PlayNextVideo();

                        };

                    }

                    this.timer.Elapsed += handler;

                    this.timer.Enabled = true;

                }

                else
                {

                    this.CurrentDuration += Constants.DefaultDuration;

                    this.timer.Enabled = false;

                    this.timer = new System.Timers.Timer(Constants.DefaultDuration);

                    if (handler2 == null)
                    {

                        handler2 = delegate
                        {

                            _isPlaying = false;

                            this.PlayNextVideo();

                        };

                    }

                    this.timer.Elapsed += handler2;

                    this.timer.Enabled = true;

                }

            }

        }



        public void PlayNextVideo()
        {

            this.Control.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(()=>
            {

                lock (this.Playlist)
                {

                    Thread.Sleep(10);

                    if (!_isPlaying)
                    {

                        do
                        {

                            this.currentItem++;

                            if (this.Playlist.Count <= 0)
                            {

                                return;

                            }

                            this.currentItem = this.currentItem % this.Playlist.Count;

                        }

                        while (!(this.CurrentControl.Source.AbsolutePath != this.Playlist[this.currentItem].DisplayLocation) || !File.Exists(this.Playlist[this.currentItem].DisplayLocation));

                        this.SwitchPlayers(new Uri(this.Playlist[this.currentItem].DisplayLocation));

                    

                    }

                }

            }));

        }



        public void Quit()
        {

            ((Canvas)this.Control.Parent).Children.Remove(this.BackControl);

            this.BackControl.Stop();

        }



        public void Start()
        {

            this.Control.MediaOpened += new RoutedEventHandler(this.MediaElementMediaOpened);

            ThreadPool.QueueUserWorkItem(delegate(object state)
            {

                Thread.Sleep(10);

                this.Control.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(()=> this.SwitchPlayers(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/DataCache/LoadVideo.m4v"))));

            });

        }



        public void Stop()
        {

            if (this.CurrentControl != null)
            {

                this.CurrentControl.Stop();

            }

        }



        private void SwitchPlayers(Uri videoPath)
        {

            MediaElement currentControl = this.CurrentControl;

            if (this.CurrentControl == null)
            {

                this.CurrentControl = this.Control;

                currentControl = this.BackControl;

            }

            else if (this.CurrentControl == this.Control)
            {

                this.CurrentControl = this.BackControl;

                currentControl = this.Control;

            }

            else if (this.CurrentControl == this.BackControl)
            {

                this.CurrentControl = this.Control;

                currentControl = this.BackControl;

            }

            this.CurrentControl.Source = videoPath;

            this.CurrentControl.Play();

            this.CurrentControl.MediaOpened -= this.CurrentControlOnMediaOpened(currentControl);

            this.CurrentControl.MediaOpened += this.CurrentControlOnMediaOpened(currentControl);

        }



        public void UpdatePlayList()
        {

            WaitCallback callBack = null;

            WaitCallback callback2 = null;

            lock (this.Playlist)
            {

                this.Playlist = PageSwitcher.Playlist;

                if (callBack == null)
                {

                    callBack = delegate(object state)
                    {

                        bool flag = false;

                        while (!flag)
                        {

                            Thread.Sleep(0x3e8);

                            flag = true;

                            foreach (MediaListModel model in PageSwitcher.Playlist)
                            {

                                if (!model.Downloaded)
                                {

                                    flag = false;

                                }

                            }

                        }

                        this.PlayNextVideo();

                    };

                }

                ThreadPool.QueueUserWorkItem(callBack);

                if (callback2 == null)
                {

                    callback2 = delegate(object r)
                    {

                        lock (this.FileLock)
                        {

                            XmlSerializer serializer = new XmlSerializer(typeof(List<MediaListModel>));

                            FileInfo info = new FileInfo(Constants.PlayListFile);

                            if (!File.Exists(Constants.PlayListFile))
                            {

                                StreamWriter writer = info.CreateText();

                                serializer.Serialize((TextWriter)writer, PageSwitcher.Playlist);

                                writer.Close();

                            }

                            else
                            {

                                info.Delete();

                                StreamWriter writer2 = info.CreateText();

                                serializer.Serialize((TextWriter)writer2, PageSwitcher.Playlist);

                                writer2.Close();

                            }

                        }

                    };

                }

                ThreadPool.QueueUserWorkItem(callback2);

            }

        }

    }

}



