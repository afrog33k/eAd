using System.Drawing.Imaging;
using System.Globalization;
using Client.Core;
using Client.Service;
using eAd.DataViewModels;
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
using Timer = System.Timers.Timer;

namespace Client
{
public partial class PageSwitcher : Window, IComponentConnector
{



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
    private static PageSwitcher _instance;
    static object MessageRetrievalLock = new object();



    public PageSwitcher()
    {
        Instance = this;

        InitializeComponent();


        Closed += delegate
        {

            Environment.Exit(0);

        };

        MyCommand.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));

        CommandBindings.Add(new CommandBinding(MyCommand, MyCommandExecuted));

        InitializeComponent();

        Switcher.PageSwitcher = this;


        Switcher.Switch(MainForm.Instance);

      var customerPage=  CustomerPage.Instance;


        this.StartKeepAliveThread();

        this.StartMessageReceiveThread();

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
                    lock (MessageRetrievalLock)
                        this.RetrieveMessages();

                }

            }

        }

        catch (Exception exception)
        {

            Console.WriteLine(Properties.Resources.PageSwitcher_CheckForNewMessages_Server_Down + exception.Message + exception.StackTrace);

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
                    Switcher.Switch(CustomerPage.Instance);
                    CustomerPage.Instance.Update(20000); //20000

                    goto SendReceipt;

                }

                if (command == "Added Theme")
                {

                    ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

                    long mosaicIDForStation = client.GetMosaicIDForStationKey(new HardwareKey().Key);

                    //    Positions = client.GetPositionsForMosaic(mosaicIDForStation);

                    //      MainForm.Instance.LoadPositions(null);

                    goto SendReceipt;

                }

                if (command == "Removed Theme")
                {

                    ServiceClient client2 = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

                    long mosaicID = client2.GetMosaicIDForStationKey(new HardwareKey().Key);

                    //      Positions = client2.GetPositionsForMosaic(mosaicID);

                    //    MainForm.Instance.LoadPositions(null);

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

                CustomerPage.CurrentRFID = message.Text;
                Switcher.Switch(CustomerPage.Instance);
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
                    Console.WriteLine(Properties.Resources.PageSwitcher_CheckForNewMessages_Server_Down);
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
                Console.WriteLine(Properties.Resources.PageSwitcher_CheckForNewMessages_Server_Down + exception3.Message + exception3.StackTrace);
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
                                             }
                                         });

    }


    [DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
    public static PageSwitcher Instance
    {
        get
        {
            if(_instance==null)
                _instance = new PageSwitcher();
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
}
}



