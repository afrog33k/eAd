using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using ClientApp.Core;
using ClientApp.Properties;
using ClientApp.Service;
using eAd.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using MediaListModel = eAd.DataViewModels.MediaListModel;
using MessageViewModel = eAd.DataViewModels.MessageViewModel;
using PositionViewModel = eAd.DataViewModels.PositionViewModel;
using Timer = System.Timers.Timer;

namespace ClientApp
{
public partial class ClientManager : Window, IComponentConnector
{

    private CacheManager _cacheManager;

    private static PositionViewModel[] _positions;

    private bool _checkedForNewMessages;

    public static List<DownloadProgressChangedEventArgs> Downloads = new List<DownloadProgressChangedEventArgs>();

    private static object _downLoadsLock = new object();

    private static MessageViewModel _lastMessage = null;

    public static Mosaic MainMosaic;

    public static RoutedCommand MyCommand = new RoutedCommand();

    public static List<MediaListModel> Playlist = new List<MediaListModel>();

    public static object PositionSaveLock = new object();

    private static volatile int runningDownloads = 0;

    public static double TotalBytes = 0.0;

    public static double TotalBytesIn = 0.0;

    private static readonly List<MessageViewModel> UnProcessedMessages = new List<MessageViewModel>();
    private static ClientManager _instance;
    static object MessageRetrievalLock = new object();
    private  Stat _stat;
    private StatLog _statLog;
    private Schedule _schedule;
    private int _scheduleId;
    Timer _schedulerTimer = new Timer();

    private object ScheduleChangeLock = new object();

    public ClientManager()
    {
        Instance = this;

        InitializeComponent();

        // Check the directories exist
        if (!Directory.Exists(Settings.Default.LibraryPath) || !Directory.Exists(Settings.Default.LibraryPath + @"\backgrounds\"))
        {
            // Will handle the create of everything here
            Directory.CreateDirectory(Settings.Default.LibraryPath + @"\backgrounds");
            Directory.CreateDirectory(Settings.Default.LibraryPath + @"\Layouts");
            Directory.CreateDirectory(Settings.Default.LibraryPath + @"\Uploads\Media");
        }

        // Create a cachemanager
        SetCacheManager();

        ShowSplashScreen();

        // Change the default Proxy class
        OptionForm.SetGlobalProxy();

        Closed += delegate
        {
            Closing();
            Environment.Exit(0);

        };

        MyCommand.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));

        CommandBindings.Add(new CommandBinding(MyCommand, MyCommandExecuted));

        InitializeComponent();

        Switcher.ClientManager = this;


        Switcher.Switch(AdvertPlayer.Instance);

      var customerPage=  CustomerPage.Instance;

      // Setup the proxy information
      OptionForm.SetGlobalProxy();

      _statLog = new StatLog();

        this.StartKeepAliveThread();

        this.StartMessageReceiveThread();

        try
        {
            // Create the Schedule
            _schedule = new Schedule(App.UserAppDataPath + "\\" + Settings.Default.ScheduleFile, ClientManager.Instance.CacheManager);

            // Bind to the schedule change event - notifys of changes to the schedule
            _schedule.ScheduleChangeEvent += ScheduleScheduleChangeEvent;

            // Initialize the other schedule components
            _schedule.InitializeComponents();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message, LogType.Error.ToString());
            MessageBox.Show("Fatal Error initialising AdvertPlayer", "Fatal Error");

            Environment.Exit(0);
        }

    }

    /// <summary>
    /// Handles the ScheduleChange event
    /// </summary>
    /// <param name="layoutPath"></param>
    /// <param name="scheduleId"> </param>
    /// <param name="layoutId"> </param>
    void ScheduleScheduleChangeEvent(string layoutPath, int scheduleId, int layoutId)
    {
        lock (ScheduleChangeLock)
        {
            Debug.WriteLine(String.Format("Schedule Changing to {0}", layoutPath),
                            "AdvertPlayer - ScheduleChangeEvent");

            _scheduleId = scheduleId;
            AdvertPlayer.Instance.LayoutId = layoutId;
           

            if (Instance.Stat != null)
            {
                // Log the end of the currently running layout.
                Instance.Stat.ToDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Record this stat event in the statLog object
                ClientManager.Instance.StatLog.RecordStat(ClientManager.Instance.Stat);
            }


            try
            {
                AdvertPlayer.Instance.DestroyLayout();

                AdvertPlayer.Instance.IsExpired = false;

                AdvertPlayer.Instance.PrepareLayout(layoutPath);
              
                Charging.Instance.DestroyLayout();

                Charging.Instance.IsExpired = false;

                Charging.Instance.PrepareLayout(_schedule.ScheduleManager.ProfileLayout.LayoutFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                AdvertPlayer.Instance.IsExpired = true;
                Charging.Instance.IsExpired = true;
                   ShowSplashScreen();

                if (_schedulerTimer != null)
                    _schedulerTimer.Stop();

                _schedulerTimer = new Timer { Interval = ( 10000) };
                // In 10 seconds fire the next layout?
                _schedulerTimer.Elapsed += SplashScreenTimerTick;

                // Start the timer
                _schedulerTimer.Start();
            }
        }
    }

    void SplashScreenTimerTick(object sender, EventArgs e)
    {
        Debug.WriteLine(new LogMessage("timer_Tick", "Loading next layout after splashscreen"));

        Timer timer = (Timer)sender;
        timer.Stop();
        timer.Dispose();

        _schedule.NextLayout();
    }

    public StatLog StatLog
    {
        get { return _statLog; }
        set { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Shows the splash screen (set the background to the embedded resource)
    /// </summary>
    private void ShowSplashScreen()
    {
        // We are running with the Default.xml - meaning the schedule doesnt exist
        Debug.WriteLine("Showing Splash Screen");

        // Load into a stream and then into an Image
        try
        {
        Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
            {

                var splash = new BitmapImage();
                splash.BeginInit();
                splash.UriSource =new Uri("Resources/splash.png", UriKind.Relative);
                splash.EndInit();
                Background = new ImageBrush(splash);
            }));
        }
        catch (Exception ex)
        {
            // Log
            Debug.WriteLine("Failed Showing Splash Screen: " + ex.Message);
        }
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
                        this.RetrieveMessages();
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

        if ((UnProcessedMessages.Where(m => (m.Command == message.Command) && (m.Text == message.Text))).Any())
        {

            SendMessageReceipt(message, false);

            return false;

        }

        flag = true;

        UnProcessedMessages.Add(message);

        return flag;

    }












    private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        Closing();
        Environment.Exit(0);

    }



    public void Navigate(UserControl nextPage)
    {
        Instance.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action<UserControl>((page) =>
        {
            page.Visibility = Visibility.Visible;
            Instance.Content = page;
        }),nextPage);



    }



    public void Navigate(UserControl nextPage, object state)
    {

        base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
        {

            Content = nextPage;

            var switchable = nextPage as ISwitchable;

            if (switchable == null)
            {

                throw new ArgumentException("NextPage is not ISwitchable! " + nextPage.Name.ToString(CultureInfo.InvariantCulture));

            }

            switchable.UtilizeState(state);
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

        finally
        {

            if (client != null)
            {

                //  client.Dispose();

            }

        }

    }



    private void ProcessStatusMessage(MessageViewModel message)
    {

        string command = message.Command;

        if (command != null)
        {

            if (!(command == "Make UnAvailable"))
            {

                if (command == "Make Available")
                {
                   // Switcher.Switch(CustomerPage.Instance);
                    Charging.Instance.Update(20000); //20000
                    Switcher.Switch(Charging.Instance);
                  
                    goto SendReceipt;

                }

                if (command == "Added Theme")
                {

                    ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

                    long mosaicIDForStation = client.GetMosaicIDForStationKey(new HardwareKey().Key);

                    //    Positions = client.GetPositionsForMosaic(mosaicIDForStation);

                    //      AdvertPlayer.Instance.LoadPositions(null);

                    goto SendReceipt;

                }

                if (command == "Removed Theme")
                {

                    ServiceClient client2 = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

                    long mosaicID = client2.GetMosaicIDForStationKey(new HardwareKey().Key);

                    //      Positions = client2.GetPositionsForMosaic(mosaicID);

                    //    AdvertPlayer.Instance.LoadPositions(null);

                    goto SendReceipt;

                }

                if (command == "Screenshot")
                {

                    try
                    {

                        ThreadPool.QueueUserWorkItem(delegate
                        {

                            string filename = string.Concat(new object[]
                            { "Screenshot#", new HardwareKey().Key, "#", string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now), ".jpg"
                            });

                            new ScreenCapture().CaptureScreenToFile(filename, ImageFormat.Jpeg);

                            NameValueCollection nvc = new NameValueCollection
                            {
                                {"StationID", new HardwareKey().Key},
                                {"btn-submit-photo", "Upload"}
                            };

                            //   WebUpload.HttpUploadFile(Constants.ServerUrl + "/Stations/UploadScreenshot", filename, "file", "image/jpeg", nvc);

                        });

                    }

                    catch (Exception)
                    {

                    }

                    goto SendReceipt;

                }

            }

            else
            {

            //    CustomerPage.CurrentRFID = message.Text;
                Switcher.Switch(Charging.Instance);
                CustomerPage.Instance.Update(60000); //60000

                goto SendReceipt;

            }

        }

        MessageBox.Show(this, message.Text, message.Type, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);

        SendReceipt:
        SendMessageReceipt(message, true);

    }



    private void RetrieveMessages()
    {
        
            try
            {
                using (var client = new ServiceClient())
                {
                    var messages = client.GetAllMyMessagesKey(new HardwareKey().Key);

                    foreach (MessageViewModel model in messages)
                    {
                        if (model.Type == "Info")
                        {
                            if (!CheckMessage(model))
                            {
                                return;
                            }

                            this.ProcessInformationMessage(model);

                        }

                        if (model.Type == "Media")
                        {

                            if (!CheckMessage(model))
                            {
                                return;
                            }

                            //LoadMosaic(model);

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
                Console.WriteLine("Something Bad Happened ... :( Look Here: " + exception.Message + "\n" +
                                  exception.StackTrace);
            }

   //     }));

    }



    public void RunCommands(string[] arguments)
    {

        DispatcherOperationCallback method = null;

        DispatcherOperationCallback callback2 = null;

        DispatcherOperationCallback callback3 = null;

        foreach (string str in arguments)
        {

            switch (str)
            {

            case "Minimize":

                if (!Dispatcher.CheckAccess())
                {
                    if (method == null)
                    {

                        method = delegate
                        {
                            WindowState = WindowState.Minimized;
                            return null;
                        };

                    }
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, method, null);
                }
                else
                {
                    WindowState = WindowState.Minimized;
                }
                break;

            case "Maximize":
                if (!Dispatcher.CheckAccess())
                {
                    if (callback2 == null)
                    {
                        callback2 = delegate
                        {
                            base.WindowState = WindowState.Maximized;
                            return null;
                        };
                    }
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, callback2, null);
                }
                else
                {
                    WindowState = WindowState.Maximized;
                }
                break;

            case "Normal":
                if (!base.Dispatcher.CheckAccess())
                {
                    if (callback3 == null)
                    {
                        callback3 = delegate
                        {
                            base.WindowState = WindowState.Normal;
                            return null;
                        };
                    }
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, callback3, null);
                }
                else
                {
                    WindowState = WindowState.Normal;
                }
                break;

            case "Close":
                Environment.Exit(0);
                break;
            }

        }

    }



    //private static void SavePositions(List<PositionViewModel> positions)
    //{

    //    ThreadPool.QueueUserWorkItem(delegate(object r)
    //    {

    //        lock (PositionSaveLock)
    //        {

    //            string fileName = "positions.xml";

    //            XmlSerializer serializer = new XmlSerializer(typeof(List<PositionViewModel>));

    //            FileInfo info = new FileInfo(fileName);

    //            if (!File.Exists(fileName))
    //            {

    //                StreamWriter writer = info.CreateText();

    //                serializer.Serialize(writer, positions);

    //                writer.Close();

    //            }

    //            else
    //            {

    //                info.Delete();

    //                StreamWriter writer2 = info.CreateText();

    //                serializer.Serialize(writer2, positions);

    //                writer2.Close();

    //            }

    //        }

    //    });

    //}



    private static void SendMessageReceipt(MessageViewModel message, bool updateList = true)
    {

        var client = new ServiceClient();

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



    private void StartKeepAliveThread()
    {
        System.Timers.Timer timer = new Timer();

        timer.Interval = (10000);
        timer.Elapsed += delegate
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
                else if (!_checkedForNewMessages)
                {
                    //          this.GetMedia(null);
                    _checkedForNewMessages = true;
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
        };
        timer.Start();

    }




    private void StartMessageReceiveThread()
    {
        //System.Timers.Timer timer = new Timer();   // seems to have high contention rate

        //timer.Interval = (Constants.MessageWaitTime);
        //timer.Elapsed += delegate
        //{
        //    this.CheckForNewMessages();
        //};
        //timer.Start();

        ThreadPool.QueueUserWorkItem((state) =>
                                         {
                                             while (true)
                                             {
                                                 this.CheckForNewMessages();
                                                 if(App.ShutDownRequested)
                                                 {
                                                     break;
                                                 }
                                                 Thread.Sleep(Constants.MessageWaitTime);
                                               //  CheckForNewSchedule();
                                             }
                                         });

    }
    /// <summary>
    /// Sets the CacheManager
    /// </summary>
    private void SetCacheManager()
    {
        try
        {
            using (FileStream fileStream = File.Open(App.UserAppDataPath + "\\" + Settings.Default.CacheManagerFile, FileMode.Open))
            {
                var xmlSerializer = new XmlSerializer(typeof(CacheManager));

                _cacheManager = (CacheManager)xmlSerializer.Deserialize(fileStream);
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine(new LogMessage("AdvertPlayer - SetCacheManager", "Unable to reuse the Cache Manager because: " + ex.Message));

            // Create a new cache manager
            _cacheManager = new CacheManager();
        }

        try
        {
            _cacheManager.Regenerate();
        }
        catch (Exception ex)
        {
            Trace.WriteLine(new LogMessage("AdvertPlayer - SetCacheManager", "Regenerate failed because: " + ex.Message));
        }
    }

    public CacheManager CacheManager
    {
        get { return _cacheManager; }
    }

    private void Closing()
    {
        // We want to tidy up some stuff as this form closes.

        // Flush the stats
        _statLog.Flush();

        // Write the CacheManager to disk
        _cacheManager.WriteCacheManager();

        // Flush the logs
        Trace.Flush();
    }


    [DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
    public static ClientManager Instance
    {
        get
        {
            if(_instance==null)
                _instance = new ClientManager();
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public  Stat Stat
    {
        get {
            return _stat;
        }
        set {
            _stat = value;
        }
    }

    public Schedule Schedule
    {
        get {
            return _schedule;
        }
        set {
            _schedule = value;
        }
    }

    public int ScheduleId
    {
        get {
            return _scheduleId;
        }
        set {
            _scheduleId = value;
        }
    }
}
}



