namespace DesktopClient
{

    using DesktopClient.eAdDataAccess;

    using DesktopClient.Menu;

    using eAd.DataViewModels;

    using eAd.Utilities;

    using irio.utilities;

    using System;

    using System.CodeDom.Compiler;

    using System.Collections.Generic;

    using System.Collections.Specialized;

    using System.ComponentModel;

    using System.Diagnostics;

    using System.Drawing.Imaging;

    using System.IO;

    using System.Linq;

    using System.Net;

    using System.Runtime.CompilerServices;

    using System.Runtime.InteropServices;

    using System.ServiceModel;

    using System.Threading;

    using System.Windows;

    using System.Windows.Controls;

    using System.Windows.Input;

    using System.Windows.Markup;

    using System.Windows.Threading;

    using System.Xml.Serialization;



 
     public partial class PageSwitcher : Window, IComponentConnector
    {

       

        private static PositionViewModel[] _positions;

        private bool CheckedForNewMessages;

        public static List<DownloadProgressChangedEventArgs> downloads = new List<DownloadProgressChangedEventArgs>();

        private static object DownLoadsLock = new object();
         
        private static MessageViewModel lastMessage = null;

        public static Mosaic MainMosaic;

        public static RoutedCommand MyCommand = new RoutedCommand();

        public static List<MediaListModel> Playlist = new List<MediaListModel>();

        public static object PositionSaveLock = new object();

        private static volatile int runningDownloads = 0;

        public static double TotalBytes = 0.0;

        public static double TotalBytesIn = 0.0;

        private static List<MessageViewModel> UnProcessedMessages = new List<MessageViewModel>();
         private static PageSwitcher _instance;


         public PageSwitcher()
        {
            InitializeComponent();

            Instance = this;

            base.Closed += delegate(object sender, EventArgs e)
            {

                Environment.Exit(0);

            };

            MyCommand.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));

            base.CommandBindings.Add(new CommandBinding(MyCommand, new ExecutedRoutedEventHandler(this.MyCommandExecuted)));

            this.InitializeComponent();

            Switcher.PageSwitcher = this;

            Switcher.Switch(MainWindow.Instance);

            this.StartKeepAliveThread();

            this.StartMessageReceiveThread();

        }



        private void CheckForNewMessages()
        {

            try
            {

                using (ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress))
                {

                    client.ClientCredentials.Windows.ClientCredential.UserName = "admin";

                    client.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";

                    if (client.DoIHaveUpdates(Constants.MyStationID))
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

            if ((from m in UnProcessedMessages

                 where (m.Command == message.Command) && (m.Text == message.Text)

                 select m).Count<MessageViewModel>() > 0)
            {

                SendMessageReceipt(message, false);

                return false;

            }

            flag = true;

            UnProcessedMessages.Add(message);

            return flag;

        }



        private static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            Switcher.PageSwitcher.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                                        {

                                                                                                            if (
                                                                                                                MainWindow
                                                                                                                    .
                                                                                                                    Instance
                                                                                                                    .
                                                                                                                    Update_Progress
                                                                                                                    .
                                                                                                                    Visibility ==
                                                                                                                Visibility
                                                                                                                    .
                                                                                                                    Hidden)
                                                                                                            {

                                                                                                                MainWindow
                                                                                                                    .
                                                                                                                    Instance
                                                                                                                    .
                                                                                                                    Update_Progress
                                                                                                                    .
                                                                                                                    Visibility
                                                                                                                    =
                                                                                                                    Visibility
                                                                                                                        .
                                                                                                                        Visible;

                                                                                                            }

                                                                                                            double num =
                                                                                                                double.
                                                                                                                    Parse
                                                                                                                    (e.
                                                                                                                         BytesReceived
                                                                                                                         .
                                                                                                                         ToString
                                                                                                                         ());

                                                                                                            double num2
                                                                                                                =
                                                                                                                double.
                                                                                                                    Parse
                                                                                                                    (e.
                                                                                                                         TotalBytesToReceive
                                                                                                                         .
                                                                                                                         ToString
                                                                                                                         ());

                                                                                                            if (
                                                                                                                !downloads
                                                                                                                     .
                                                                                                                     Contains
                                                                                                                     (e))
                                                                                                            {

                                                                                                                downloads
                                                                                                                    .Add
                                                                                                                    (e);

                                                                                                                TotalBytes
                                                                                                                    +=
                                                                                                                    num2;

                                                                                                            }

                                                                                                            TotalBytesIn
                                                                                                                += num;

                                                                                                            double num3
                                                                                                                =
                                                                                                                (TotalBytesIn/
                                                                                                                 TotalBytes)*
                                                                                                                100.0;

                                                                                                            MainWindow.
                                                                                                                Instance
                                                                                                                .
                                                                                                                Update_Progress
                                                                                                                .Value =
                                                                                                                (num3 +
                                                                                                                 MainWindow
                                                                                                                     .
                                                                                                                     Instance
                                                                                                                     .
                                                                                                                     Update_Progress
                                                                                                                     .
                                                                                                                     Value)/
                                                                                                                2.0;

                                                                                                            if (
                                                                                                                MainWindow
                                                                                                                    .
                                                                                                                    Instance
                                                                                                                    .
                                                                                                                    Update_Progress
                                                                                                                    .
                                                                                                                    Visibility ==
                                                                                                                Visibility
                                                                                                                    .
                                                                                                                    Hidden)
                                                                                                            {

                                                                                                                MainWindow
                                                                                                                    .
                                                                                                                    Instance
                                                                                                                    .
                                                                                                                    Update_Progress
                                                                                                                    .
                                                                                                                    Visibility
                                                                                                                    =
                                                                                                                    Visibility
                                                                                                                        .
                                                                                                                        Visible;

                                                                                                            }



                                                                                                        }));
     

        }



        private void ClientDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            lock (DownLoadsLock)
            {

                runningDownloads--;

            }

            HideProgressBar();

            if (e.Error == null)
            {

                lock (DownLoadsLock)
                {

                    if (runningDownloads == 0)
                    {

                        MainWindow.Instance.UpdatePlayList();

                    }

                }

            }

        }



        public static bool DownloadMedium(MediaListModel item, List<MediaListModel> playlist, IPlayer player)
        {

            AsyncCompletedEventHandler handler = null;

            string path = Constants.AppPath + item.DisplayLocation;

            string location = item.DisplayLocation;

            bool flag = false;

            if (System.IO.File.Exists(path) && (new FileInfo(path).Length > 0L))
            {

                flag = true;

            }

            if ((from i in playlist

                 where i.DisplayLocation == path

                 select i).Count<MediaListModel>() <= 0)
            {

                MediaListModel model = item;

                model.DisplayLocation = path;

                if (flag)
                {

                    model.Downloaded = true;

                }

                if (playlist.Contains(item))
                {

                    playlist.Remove(item);

                }

                playlist.Add(model);

                if (flag)
                {

                    return true;

                }

            }

            FileUtilities.FolderCreate(Path.GetDirectoryName(path));

            WebClient client = new WebClient();

            try
            {

                client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e)
                                                      {

                                                          Switcher.PageSwitcher.Dispatcher.BeginInvoke(
                                                              DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                                             {

                                                                                                                 if (
                                                                                                                     MainWindow
                                                                                                                         .
                                                                                                                         Instance
                                                                                                                         .
                                                                                                                         Update_Progress
                                                                                                                         .
                                                                                                                         Visibility ==
                                                                                                                     Visibility
                                                                                                                         .
                                                                                                                         Hidden)
                                                                                                                 {

                                                                                                                     MainWindow
                                                                                                                         .
                                                                                                                         Instance
                                                                                                                         .
                                                                                                                         Update_Progress
                                                                                                                         .
                                                                                                                         Visibility
                                                                                                                         =
                                                                                                                         Visibility
                                                                                                                             .
                                                                                                                             Visible;

                                                                                                                 }

                                                                                                                 double
                                                                                                                     num
                                                                                                                         =
                                                                                                                         double
                                                                                                                             .
                                                                                                                             Parse
                                                                                                                             (e
                                                                                                                                  .
                                                                                                                                  BytesReceived
                                                                                                                                  .
                                                                                                                                  ToString
                                                                                                                                  ());

                                                                                                                 double
                                                                                                                     num2
                                                                                                                         =
                                                                                                                         double
                                                                                                                             .
                                                                                                                             Parse
                                                                                                                             (e
                                                                                                                                  .
                                                                                                                                  TotalBytesToReceive
                                                                                                                                  .
                                                                                                                                  ToString
                                                                                                                                  ());

                                                                                                                 if (
                                                                                                                     !downloads
                                                                                                                          .
                                                                                                                          Contains
                                                                                                                          (e))
                                                                                                                 {

                                                                                                                     downloads
                                                                                                                         .
                                                                                                                         Add
                                                                                                                         (e);

                                                                                                                     TotalBytes
                                                                                                                         +=
                                                                                                                         num2;

                                                                                                                 }

                                                                                                                 TotalBytesIn
                                                                                                                     +=
                                                                                                                     num;

                                                                                                                 double
                                                                                                                     num3
                                                                                                                         =
                                                                                                                         (TotalBytesIn/
                                                                                                                          TotalBytes)*
                                                                                                                         100.0;

                                                                                                                 MainWindow
                                                                                                                     .
                                                                                                                     Instance
                                                                                                                     .
                                                                                                                     Update_Progress
                                                                                                                     .
                                                                                                                     Value
                                                                                                                     =
                                                                                                                     (num3 +
                                                                                                                      MainWindow
                                                                                                                          .
                                                                                                                          Instance
                                                                                                                          .
                                                                                                                          Update_Progress
                                                                                                                          .
                                                                                                                          Value)/
                                                                                                                     2.0;



                                                                                                             }));

                };

                if (handler == null)
                {

                    handler = delegate(object sender, AsyncCompletedEventArgs args)
                    {

                        lock (DownLoadsLock)
                        {

                            runningDownloads--;

                        }

                        HideProgressBar();

                        item = (from i in playlist

                                where i.DisplayLocation == path

                                select i).FirstOrDefault<MediaListModel>();

                        if (item != null)
                        {

                            item.Downloaded = true;

                            if (!System.IO.File.Exists(path) || (new FileInfo(path).Length <= 0L))
                            {

                                playlist.Remove(item);

                                Console.WriteLine("Downloaded Corrupted File");

                            }

                        }

                        if (args.Error == null)
                        {

                            lock (DownLoadsLock)
                            {

                                Thread.Sleep(200);

                            }

                            if (runningDownloads == 0)
                            {

                                player.UpdatePlayList();

                            }

                        }

                    };

                }

                client.DownloadFileCompleted += handler;

                if (!flag)
                {

                    lock (DownLoadsLock)
                    {

                        runningDownloads++;

                    }

                    Uri address = new Uri(Constants.ServerUrl + location.Replace(@"\", "/"));

                    client.DownloadFileAsync(address, path);

                }

            }

            catch (TimeoutException exception)
            {

                Console.WriteLine("Got {0}", exception.GetType());

            }

            catch (CommunicationException exception2)
            {

                Console.WriteLine("Got {0}", exception2.GetType());

            }

            finally
            {

                if (client != null)
                {

                    client.Dispose();

                }

            }

            return false;

        }



        private void GetMedia(MessageViewModel message = null)
        {

            ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

            try
            {

                MediaListModel[] mediaList = client.GetMyMedia(Constants.MyStationID);

                if (message != null)
                {

                    client.MessageRead(message.ID);

                }

                ThreadPool.QueueUserWorkItem(delegate(object e4)
                {

                    MediaListModel[] modelArray1 = mediaList;

                    for (int j = 0; j < modelArray1.Length; j++)
                    {

                        MediaListModel model1 = modelArray1[j];

                    }

                });

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

                 //   client.Dispose();

                }

            }

        }



        private static void HideProgressBar()
        {

            Switcher.PageSwitcher.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                                        {

                                                                                                            if (
                                                                                                                MainWindow
                                                                                                                    .
                                                                                                                    Instance
                                                                                                                    .
                                                                                                                    Update_Progress
                                                                                                                    .
                                                                                                                    Visibility ==
                                                                                                                Visibility
                                                                                                                    .
                                                                                                                    Visible)
                                                                                                            {

                                                                                                                MainWindow
                                                                                                                    .
                                                                                                                    Instance
                                                                                                                    .
                                                                                                                    Update_Progress
                                                                                                                    .
                                                                                                                    Visibility
                                                                                                                    =
                                                                                                                    Visibility
                                                                                                                        .
                                                                                                                        Hidden;

                                                                                                            }
                                                                                                        }));

        }



      



        private static void LoadMosaic(MessageViewModel message)
        {

            ThreadPool.QueueUserWorkItem(delegate(object state)
            {

                try
                {

                    ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

                    long mosaicID = client.GetMosaicIDForStation(Constants.MyStationID);

                    Constants.CurrentClientConfiguration.CurrentMosaic=mosaicID;

                    Constants.SaveDefaults();

                    Positions = client.GetPositionsForMosaic(mosaicID);

                    SavePositions(new List<PositionViewModel>(Positions));

                    MainWindow.Instance.LoadPositions(null);

                    client.SetStationStatus(Constants.MyStationID, "Positions Loaded, Now Playing");

                }

                catch (Exception)
                {

                }

                ServiceClient client2 = new ServiceClient();

                try
                {

                    if (message != null)
                    {

                        client2.MessageRead(message.ID);

                    }

                }

                catch (TimeoutException exception)
                {

                    Console.WriteLine("Got {0}", exception.GetType());

                    client2.Abort();

                }

                catch (CommunicationException exception2)
                {

                    Console.WriteLine("Got {0}", exception2.GetType());

                    client2.Abort();

                }

            });

        }



        private List<PositionViewModel> LoadPositions()
        {

            List<PositionViewModel> list = new List<PositionViewModel>();

            try
            {

                string path = "positions.xml";

                XmlSerializer serializer = new XmlSerializer(typeof(List<PositionViewModel>));

                StreamReader textReader = System.IO.File.OpenText(path);

                list = serializer.Deserialize(textReader) as List<PositionViewModel>;

                textReader.Close();

            }

            catch (Exception)
            {

                list = new List<PositionViewModel>();

            }

            return list;

        }



        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            Environment.Exit(0);

        }



        public void Navigate(UserControl nextPage)
        {

            base.Content = nextPage;

        }



        public void Navigate(UserControl nextPage, object state)
        {

            base.Content = nextPage;

            ISwitchable switchable = nextPage as ISwitchable;

            if (switchable == null)
            {

                throw new ArgumentException("NextPage is not ISwitchable! " + nextPage.Name.ToString());

            }

            switchable.UtilizeState(state);

        }



        protected override void OnActivated(EventArgs e)
        {

            base.OnActivated(e);

            Splasher.CloseSplash();

        }



        private void PlayCurrentMosaic()
        {

            if (Constants.CurrentMosaic != -1L)
            {

                PositionViewModel[] positions = this.LoadPositions().ToArray();

                MainWindow.Instance.LoadPositions(positions);

            }

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

                        CustomerPage.Instance.Update(0x4e20);

                        Switcher.Switch(CustomerPage.Instance);

                        goto Label_014E;

                    }

                    if (command == "Added Theme")
                    {

                        ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

                        long mosaicIDForStation = client.GetMosaicIDForStation(Constants.MyStationID);

                        Positions = client.GetPositionsForMosaic(mosaicIDForStation);

                        MainWindow.Instance.LoadPositions(null);

                        goto Label_014E;

                    }

                    if (command == "Removed Theme")
                    {

                        ServiceClient client2 = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

                        long mosaicID = client2.GetMosaicIDForStation(Constants.MyStationID);

                        Positions = client2.GetPositionsForMosaic(mosaicID);

                        MainWindow.Instance.LoadPositions(null);

                        goto Label_014E;

                    }

                    if (command == "Screenshot")
                    {

                        try
                        {

                            ThreadPool.QueueUserWorkItem(delegate(object hjhj)
                            {

                                string filename = string.Concat(new object[] { "Screenshot#", Constants.MyStationID, "#", string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now), ".jpg" });

                                new ScreenCapture().CaptureScreenToFile(filename, ImageFormat.Jpeg);

                                NameValueCollection nvc = new NameValueCollection();

                                nvc.Add("StationID", Constants.MyStationID.ToString());

                                nvc.Add("btn-submit-photo", "Upload");

                                WebUpload.HttpUploadFile(Constants.ServerUrl + "/Stations/UploadScreenshot", filename, "file", "image/jpeg", nvc);

                            });

                        }

                        catch (Exception)
                        {

                        }

                        goto Label_014E;

                    }

                }

                else
                {

                    CustomerPage.CurrentRFID = message.Text;

                    CustomerPage.Instance.Update(0xea60);

                    Switcher.Switch(CustomerPage.Instance);

                    goto Label_014E;

                }

            }

            MessageBox.Show(this, message.Text, message.Type, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);

        Label_014E:

            SendMessageReceipt(message, true);

        }



        private void RetrieveMessages()
        {

            base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                       {

                                                                                           try
                                                                                           {

                                                                                               using (
                                                                                                   ServiceClient client
                                                                                                       =
                                                                                                       new ServiceClient
                                                                                                           ())
                                                                                               {

                                                                                                   foreach (
                                                                                                       MessageViewModel
                                                                                                           model in
                                                                                                           client.
                                                                                                               GetAllMyMessages
                                                                                                               (Constants
                                                                                                                    .
                                                                                                                    MyStationID)
                                                                                                       )
                                                                                                   {

                                                                                                       if (model.Type ==
                                                                                                           "Info")
                                                                                                       {

                                                                                                           if (
                                                                                                               !CheckMessage
                                                                                                                    (model))
                                                                                                           {

                                                                                                               return;

                                                                                                           }

                                                                                                           this.
                                                                                                               ProcessInformationMessage
                                                                                                               (model);

                                                                                                       }

                                                                                                       if (model.Type ==
                                                                                                           "Media")
                                                                                                       {

                                                                                                           if (
                                                                                                               !CheckMessage
                                                                                                                    (model))
                                                                                                           {

                                                                                                               return;

                                                                                                           }

                                                                                                           LoadMosaic(
                                                                                                               model);

                                                                                                       }

                                                                                                       if (model.Type ==
                                                                                                           "Status")
                                                                                                       {

                                                                                                           if (
                                                                                                               !CheckMessage
                                                                                                                    (model))
                                                                                                           {

                                                                                                               return;

                                                                                                           }

                                                                                                           this.
                                                                                                               ProcessStatusMessage
                                                                                                               (model);

                                                                                                       }

                                                                                                       if (model.Type ==
                                                                                                           "Group")
                                                                                                       {

                                                                                                           if (
                                                                                                               !CheckMessage
                                                                                                                    (model))
                                                                                                           {

                                                                                                               return;

                                                                                                           }

                                                                                                           SendMessageReceipt
                                                                                                               (model,
                                                                                                                true);

                                                                                                       }

                                                                                                   }

                                                                                               }

                                                                                           }

                                                                                           catch (Exception exception)
                                                                                           {

                                                                                               Console.WriteLine(
                                                                                                   "Something Bad Happened ... :( Look Here: " +
                                                                                                   exception.Message +
                                                                                                   "\n" +
                                                                                                   exception.StackTrace);

                                                                                           }

                                                                                       }));

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

                                    base.WindowState = WindowState.Minimized;

                                    return null;

                                };

                            }

                            Dispatcher.BeginInvoke(DispatcherPriority.Send, method, null);

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

                            Dispatcher.BeginInvoke(DispatcherPriority.Send, callback2, null);

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

                            Dispatcher.BeginInvoke(DispatcherPriority.Send, callback3, null);

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



        private static void SavePositions(List<PositionViewModel> positions)
        {

            ThreadPool.QueueUserWorkItem(delegate(object r)
            {

                lock (PositionSaveLock)
                {

                    string fileName = "positions.xml";

                    XmlSerializer serializer = new XmlSerializer(typeof(List<PositionViewModel>));

                    FileInfo info = new FileInfo(fileName);

                    if (!File.Exists(fileName))
                    {

                        StreamWriter writer = info.CreateText();

                        serializer.Serialize(writer, positions);

                        writer.Close();

                    }

                    else
                    {

                        info.Delete();

                        StreamWriter writer2 = info.CreateText();

                        serializer.Serialize(writer2, positions);

                        writer2.Close();

                    }

                }

            });

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



        private void StartKeepAliveThread()
        {

            ThreadPool.QueueUserWorkItem(delegate(object state)
            {

                this.PlayCurrentMosaic();

                ThreadPool.QueueUserWorkItem(delegate(object e)
                {

                    LoadMosaic(null);

                });

                while (true)
                {

                    ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

                    try
                    {

                        client.ClientCredentials.Windows.ClientCredential.UserName = "admin";

                        client.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";

                        if (client.SayHi(Constants.MyStationID) != "Hi there")
                        {

                            Console.WriteLine("Server Down");

                        }

                        else if (!this.CheckedForNewMessages)
                        {

                            this.GetMedia(null);

                            this.CheckedForNewMessages = true;

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

                    Thread.Sleep(0x2710);

                }

            });

        }



        private void StartMessageReceiveThread()
        {

            ThreadPool.QueueUserWorkItem(delegate(object e)
            {

                while (true)
                {

                    Thread.Sleep(Constants.MessageWaitTime);

                    this.CheckForNewMessages();

                }

            });

        }



        [DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]

      


        public static PageSwitcher Instance
        {

      

            get

            {

                return _instance;

            }

          

            set

            {

                _instance = value;

            }

        }



        public static PositionViewModel[] Positions
        {

            get
            {

                return _positions;

            }

            set
            {

                _positions = value;

            }

        }

    }

}



