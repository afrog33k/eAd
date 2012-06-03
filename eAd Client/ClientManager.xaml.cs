using System.Net;
using System.Windows.Interop;
using ClientApp.Service;

namespace ClientApp
{
    using ClientApp.Core;
    using ClientApp.MainUI;
    using ClientApp.Properties;

    using eAd.DataViewModels;
    using eAd.Utilities;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.Threading;
    using System.Timers;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using System.Xml.Serialization;


    public partial class ClientManager : Window, IComponentConnector
    {
        private ClientApp.CacheManager _cacheManager;
        private bool _checkedForNewMessages;
  
        private static object _downLoadsLock = new object();
        private static ClientManager _instance;
        private static MessageViewModel _lastMessage = null;
        private static PositionViewModel[] _positions;
        private ClientApp.Schedule _schedule;
        private int _scheduleId;
        private System.Timers.Timer _schedulerTimer;
        private ClientApp.Core.Stat _stat;
        private ClientApp.Core.StatLog _statLog;
        public static List<DownloadProgressChangedEventArgs> Downloads = new List<DownloadProgressChangedEventArgs>();
      
        public static Mosaic MainMosaic;
      
        private static object MessageRetrievalLock = new object();
        public static RoutedCommand MyCommand = new RoutedCommand();
        public static List<MediaListModel> Playlist = new List<MediaListModel>();
        public static object PositionSaveLock = new object();
        private static bool preLoadedCustomerPage = false;
        private static volatile int runningDownloads = 0;
        private object ScheduleChangeLock;
        public static double TotalBytes = 0.0;
        public static double TotalBytesIn = 0.0;
        private static readonly List<MessageViewModel> UnProcessedMessages = new List<MessageViewModel>();

        public ClientManager()
        {
      //      RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly; 

            //Listeners 

            TextWriterTraceListener [] listeners = new TextWriterTraceListener[]
                                                       {
                                                           new TextWriterTraceListener("log.txt"),
                                                           new TextWriterTraceListener(Console.Out), 
                                                       };

           
            Debug.Listeners.AddRange(listeners);

            EventHandler handler = null;
            this._schedulerTimer = new System.Timers.Timer();
            this.ScheduleChangeLock = new object();
            Instance = this;
            this.InitializeComponent();
            if (!Directory.Exists(Settings.Default.LibraryPath) || !Directory.Exists(Settings.Default.LibraryPath + @"\backgrounds\"))
            {
                Directory.CreateDirectory(Settings.Default.LibraryPath + @"\backgrounds");
                Directory.CreateDirectory(Settings.Default.LibraryPath + @"\Layouts");
                Directory.CreateDirectory(Settings.Default.LibraryPath + @"\Uploads\Media");
            }
            this.SetCacheManager();
            this.ShowSplashScreen();
            OptionForm.SetGlobalProxy();
            if (handler == null)
            {
                handler = new EventHandler((y, t) =>
                {
                    this.Closing();
                    Environment.Exit(0);
                });
            }
            base.Closed += handler;
            MyCommand.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));
            base.CommandBindings.Add(new CommandBinding(MyCommand, new ExecutedRoutedEventHandler(this.MyCommandExecuted)));
            this.InitializeComponent();
            Switcher.ClientManager = this;
            OptionForm.SetGlobalProxy();
            this._statLog = new ClientApp.Core.StatLog();

            Switcher.Switch(AdvertPlayer.Instance);
            this.StartMessageReceiveThread();
            try
            {
                this._schedule = new ClientApp.Schedule(App.UserAppDataPath + @"\" + Settings.Default.ScheduleFile, Instance.CacheManager);
                this._schedule.ScheduleChangeEvent += new ClientApp.Schedule.ScheduleChangeDelegate(this.ScheduleScheduleChangeEvent);
                this._schedule.InitializeComponents();
            }
            catch (Exception)
            {
                MessageBox.Show("Fatal Error initialising eAd", "Fatal Error");
                Environment.Exit(0);
            }

       
            Charging instance = Charging.Instance;
            LoadingProfile profile1 = LoadingProfile.Instance;

           
        }




        private void CheckForNewMessages()
        {
            try
            {
                using (ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress))
                {
                    if (client.ClientCredentials != null)
                    {
                        client.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                        client.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                    }
                    if (client.DoIHaveUpdatesKey(new HardwareKey().Key))
                    {
                        messageReceiveWorker.ReportProgress(10);
                    
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Server Down" + exception.Message + exception.StackTrace);
            }
        }

        private static bool CheckMessage(MessageViewModel message)
        {
            bool flag = false;
            if ((from m in UnProcessedMessages
                 where (m.Command == message.Command) && (m.Text == message.Text)
                 select m).Any<MessageViewModel>())
            {
                SendMessageReceipt(message, false);
                return false;
            }
            flag = true;
            UnProcessedMessages.Add(message);
            return flag;
        }

        private void Closing()
        {
            this._statLog.Flush();
            this._cacheManager.WriteCacheManager();
            Trace.Flush();
        }

      

        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Closing();
            Environment.Exit(0);
        }

        public void Navigate(IPausableControl nextPage)
        {
         Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<IPausableControl>
                ((p) =>
                     {
                         if (p != null)
                         {
                             Debug.WriteLine("Switching to new View " + p.GetType().Name);
                             var page = p as UserControl;
                             page.Opacity = 0.0;
                             DoubleAnimation animation = new DoubleAnimation
                                                             {
                                                                 From = 0.0,
                                                                 To = 1.0,
                                                                 Duration =
                                                                     new Duration(TimeSpan.FromMilliseconds(400.0)),
                                                                 AutoReverse = false,
                                                             };
                        
                            
                             //Grid parent = page.Parent as Grid;
                             //if (parent != null)
                             //{
                             //    parent.Children.Remove(page);
                             //}
                           
                             MediaGrid.Children.Remove(page);
                            
                             p.UnPause();
                             this.MediaGrid.Children.Add(page);
                            page.BeginAnimation(UIElement.OpacityProperty, animation);
                         }

                         if (Switcher.LastPage != null && Switcher.LastPage!=p)
                         {
                             Debug.WriteLine("Removing LastPAge Added" + Switcher.LastPage.GetType().Name);
                             Charging lastPage = Switcher.LastPage as Charging;
                             if (lastPage != null)
                             {
                                 lastPage.LocationWidget.Visibility = Visibility.Hidden;
                             }
                             DoubleAnimation animation2 = new DoubleAnimation
                             {
                                 From = 1.0,
                                 To = 0.0,
                                 Duration = new Duration(TimeSpan.FromMilliseconds(400.0)),
                                 AutoReverse = false
                             };
                             var lp = Switcher.LastPage as UserControl;
                           
                             Debug.WriteLine("Removing LastPAge BeginAni");
                             MediaGrid.Children.Remove(lp);
                             Switcher.LastPage.Pause();
                             Debug.WriteLine("Pased LastPAge ");
                             lp.BeginAnimation(UIElement.OpacityProperty, animation2);
                         }



                     }), nextPage);
        }

        public void Navigate(UserControl nextPage, object state)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.Content = nextPage;
                ISwitchable switchable1 = nextPage as ISwitchable;
                if (switchable1 == null)
                {
                    throw new ArgumentException("NextPage is not ISwitchable! " + nextPage.Name.ToString(CultureInfo.InvariantCulture));
                }
                switchable1.UtilizeState(state);
            }));
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Splasher.CloseSplash();
        }

        private void ProcessInformationMessage(MessageViewModel message)
        {
            MessageBox.Show(this, message.Text, message.Type, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
            ServiceClient client = new ServiceClient();
            try
            {
                try
                {
                    client.MessageRead(message.ID);
                }
                catch (TimeoutException exception)
                {
                    Console.WriteLine("Got {0}", exception.GetType());
                    client.Abort();
                }
                catch (CommunicationException exception2)
                {
                    Console.WriteLine("Got {0}", exception2.GetType());
                    client.Abort();
                }
            }
            finally
            {
            }
        }

        private void ProcessStatusMessage(MessageViewModel message)
        {
            string command = message.Command;
            switch (command)
            {
                case null:
                    break;

                case "Make UnAvailable":
                    Charging.CurrentRFID = message.Text;
                    Switcher.Switch(LoadingProfile.Instance);
                    Charging.Instance.Update(60000);
                    goto Label_011C;

                default:
                    if (command == "Make Available")
                    {
                        Charging.Instance.Update(20000);
                        Switcher.Switch(LoadingProfile.Instance);
                    }
                    else if (command == "Added Theme")
                    {
                        new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress).GetMosaicIDForStationKey(new HardwareKey().Key);
                    }
                    else if (command == "Removed Theme")
                    {
                        new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress).GetMosaicIDForStationKey(new HardwareKey().Key);
                    }
                    else
                    {
                        if (!(command == "Screenshot"))
                        {
                            break;
                        }
                        try
                        {
                            ThreadPool.QueueUserWorkItem(delegate(object k)
                            {
                                string filename = string.Concat(new object[] { "Screenshot#", new HardwareKey().Key, "#", string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now), ".jpg" });
                                new ScreenCapture().CaptureScreenToFile(filename, ImageFormat.Jpeg);
                                NameValueCollection values = new NameValueCollection();
                                values.Add("StationID", new HardwareKey().Key);
                                values.Add("btn-submit-photo", "Upload");
                            });
                        }
                        catch (Exception)
                        {
                        }
                    }
                    goto Label_011C;
            }
            MessageBox.Show(this, message.Text, message.Type, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
        Label_011C:
            SendMessageReceipt(message, true);
        }

        private void RetrieveMessages()
        {
            try
            {
                using (ServiceClient client = new ServiceClient())
                {
                    foreach (MessageViewModel model in client.GetAllMyMessagesKey(new HardwareKey().Key))
                    {
                        if (model.Type == "Info")
                        {
                            if (!CheckMessage(model))
                            {
                                return;
                            }
                            this.ProcessInformationMessage(model);
                        }
                        if ((model.Type == "Media") && !CheckMessage(model))
                        {
                            return;
                        }
                        if (model.Type == "Status")
                        {
                            if (!CheckMessage(model))
                            {
                                return;
                            }
                            this.ProcessStatusMessage(model);
                        }
                        if (model.Type == "Group")
                        {
                            if (!CheckMessage(model))
                            {
                                return;
                            }
                            SendMessageReceipt(model, true);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Something Bad Happened ... :( Look Here: " + exception.Message + "\n" + exception.StackTrace);
            }
        }




     
        public void RunCommands(string[] arguments)
        {
            Action method = null;
            Action callback2 = null;
            Action callback3 = null;
            Action callback4 = null;
            Action callback5 = null;
            Action callback6 = null;
            foreach (string str in arguments)
            {
                string str2 = str;
                if (str2 != null)
                {
                    if (!(str2 == "Minimize"))
                    {
                        if (str2 == "Maximize")
                        {
                            goto Label_00B2;
                        }
                        if (str2 == "Normal")
                        {
                            goto Label_00F2;
                        }
                        if (str2 == "Close")
                        {
                            goto Label_0132;
                        }
                    }
                    else if (!base.Dispatcher.CheckAccess())
                    {
                        if (method == null)
                        {
                            if (callback4 == null)
                            {
                                callback4 = new Action(() =>
                                {
                                    base.WindowState = WindowState.Minimized;

                                });
                            }
                            method = callback4;
                        }
                        Dispatcher.BeginInvoke(DispatcherPriority.Normal, method, null);
                    }
                    else
                    {
                        base.WindowState = WindowState.Minimized;
                    }
                }
                goto Label_0138;
            Label_00B2:
                if (!base.Dispatcher.CheckAccess())
                {
                    if (callback2 == null)
                    {
                        if (callback5 == null)
                        {
                            callback5 = new Action(() =>
                            {
                                base.WindowState = WindowState.Maximized;

                            });
                        }
                        callback2 = callback5;
                    }
                    base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, callback2, null);
                }
                else
                {
                    base.WindowState = WindowState.Maximized;
                }
                goto Label_0138;
            Label_00F2:
                if (!base.Dispatcher.CheckAccess())
                {
                    if (callback3 == null)
                    {
                        if (callback6 == null)
                        {
                            callback6 = new Action(() =>
                            {
                                base.WindowState = WindowState.Normal;

                            });
                        }
                        callback3 = callback6;
                    }
                    base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, callback3, null);
                }
                else
                {
                    base.WindowState = WindowState.Normal;
                }
                goto Label_0138;
            Label_0132:
                ShutdownApplication();
            Label_0138: ;
            }
        }

        private static void ShutdownApplication()
        {
          
            App.Current.Shutdown();
            Environment.Exit(0);
        }

        private void ScheduleScheduleChangeEvent(string layoutPath, int scheduleId, int layoutId, string player)
        {
            lock (this.ScheduleChangeLock)
            {
                this._scheduleId = scheduleId;
                AdvertPlayer.Instance.LayoutId = layoutId;
                if (Instance.Stat != null)
                {
                    Instance.Stat.ToDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    Instance.StatLog.RecordStat(Instance.Stat);
                }
                try
                {
                    if (string.IsNullOrEmpty(player))
                    {
                       
                        AdvertPlayer.Instance.DestroyLayout();
                        AdvertPlayer.Instance.IsExpired = false;
                        AdvertPlayer.Instance.PrepareLayout(layoutPath);
                        Charging.Instance.DestroyLayout();
                        Charging.Instance.IsExpired = false;
                        Charging.Instance.PrepareLayout(this._schedule.ScheduleManager.ProfileLayout.LayoutFile);
                    }
                    else if (player == "AdvertPlayer")
                    {
                        AdvertPlayer.Instance.DestroyLayout();
                        AdvertPlayer.Instance.IsExpired = false;
                        AdvertPlayer.Instance.PrepareLayout(layoutPath);
                    }
                    else if (player == "Charging")
                    {
                        Charging.Instance.DestroyLayout();
                        Charging.Instance.IsExpired = false;
                        Charging.Instance.PrepareLayout(this._schedule.ScheduleManager.ProfileLayout.LayoutFile);
                    }
                    try
                    {
                        if (!preLoadedCustomerPage)
                        {
                            preLoadedCustomerPage = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (Exception)
                {
                    AdvertPlayer.Instance.IsExpired = true;
                    Charging.Instance.IsExpired = true;
                    this.ShowSplashScreen();
                    if (this._schedulerTimer != null)
                    {
                        this._schedulerTimer.Stop();
                    }
                    System.Timers.Timer timer = new System.Timers.Timer
                    {
                        Interval = 10000.0
                    };
                    this._schedulerTimer = timer;
                    this._schedulerTimer.Elapsed += new ElapsedEventHandler(this.SplashScreenTimerTick);
                    this._schedulerTimer.Start();
                }
                Monitor.Pulse(ScheduleChangeLock);
            }
        }

        private static void SendMessageReceipt(MessageViewModel message, bool updateList = true)
        {
            ServiceClient client = new ServiceClient();
            try
            {
                client.MessageRead(message.ID);
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine("Got {0}", exception.GetType());
                client.Abort();
            }
            catch (CommunicationException exception2)
            {
                Console.WriteLine("Got {0}", exception2.GetType());
                client.Abort();
            }
            if (updateList)
            {
                UnProcessedMessages.Remove(message);
            }
        }

        private void SetCacheManager()
        {
            try
            {
                using (FileStream stream = File.Open(App.UserAppDataPath + @"\" + Settings.Default.CacheManagerFile, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ClientApp.CacheManager));
                    this._cacheManager = (ClientApp.CacheManager)serializer.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(new LogMessage("AdvertPlayer - SetCacheManager", "Unable to reuse the Cache Manager because: " + exception.Message));
                this._cacheManager = new ClientApp.CacheManager();
            }
            try
            {
                this._cacheManager.Regenerate();
            }
            catch (Exception exception2)
            {
                Trace.WriteLine(new LogMessage("AdvertPlayer - SetCacheManager", "Regenerate failed because: " + exception2.Message));
            }
        }

        private void ShowSplashScreen()
        {
            Action method = null;
            try
            {
                if (method == null)
                {
                    method = new Action(() =>
                    {
                        
                        base.Background = new ImageBrush(new BitmapImage(new Uri("Resources/splash.png", UriKind.Relative)));
                    //    Switcher.Switch(null);
                    });
                }
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
            }
            catch (Exception)
            {
            }
        }

        private void SplashScreenTimerTick(object sender, EventArgs e)
        {
            System.Timers.Timer timer = (System.Timers.Timer)sender;
            timer.Stop();
            timer.Dispose();
            this._schedule.NextLayout("");
            Switcher.Switch(AdvertPlayer.Instance);
        }

        private void StartKeepAliveThread()
        {
            System.Timers.Timer timer = new System.Timers.Timer
            {
                Interval = 10000.0
            };
            timer.Elapsed += (t, n) =>
                                 {
                                     SendKeepAliveMessage();
                                 };
            timer.Start();
        }

        private void SendKeepAliveMessage()
        {
            ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);
            try
            {
                if (client.ClientCredentials != null)
                {
                    client.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                    client.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                }
                if (client.SayHiKey(new HardwareKey().Key) != "Hi there")
                {
                    Console.WriteLine("Server_Down");
                }
                else if (!this._checkedForNewMessages)
                {
                    this._checkedForNewMessages = true;
                }
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine("Got {0}", exception.GetType());
                client.Abort();
            }
            catch (CommunicationException exception2)
            {
                Console.WriteLine("Got {0}", exception2.GetType());
                client.Abort();
            }
            catch (Exception exception3)
            {
                Console.WriteLine("Server Down" + exception3.Message + exception3.StackTrace);
            }
        }

        private static BackgroundWorker messageReceiveWorker;

        private static int runs = 0;
        private void StartMessageReceiveThread()
        {
            messageReceiveWorker = new BackgroundWorker();
            messageReceiveWorker.WorkerReportsProgress = true;
            messageReceiveWorker.DoWork += MessageReceiveWorkerOnDoWork;
           messageReceiveWorker.ProgressChanged += MessageReceiveWorkerOnProgressChanged;
            messageReceiveWorker.RunWorkerAsync();

        }

        private void MessageReceiveWorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            this.RetrieveMessages();
        }

        private void MessageReceiveWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
         //
            while (true)
            {
                 
                    this.CheckForNewMessages();
                    if (App.ShutDownRequested)
                    {
                        return;
                    }
                    Thread.Sleep(Constants.MessageWaitTime);

                    if(runs%10==0)
                    {
                        SendKeepAliveMessage();
                    }

                    runs++;
                }
            
        }


        public ClientApp.CacheManager CacheManager
        {
            get
            {
                return this._cacheManager;
            }
        }

        [DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
        public static ClientManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClientManager();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public ClientApp.Schedule Schedule
        {
            get
            {
                return this._schedule;
            }
            set
            {
                this._schedule = value;
            }
        }

        public int ScheduleId
        {
            get
            {
                return this._scheduleId;
            }
            set
            {
                this._scheduleId = value;
            }
        }

        public ClientApp.Core.Stat Stat
        {
            get
            {
                return this._stat;
            }
            set
            {
                this._stat = value;
            }
        }

        public ClientApp.Core.StatLog StatLog
        {
            get
            {
                return this._statLog;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

