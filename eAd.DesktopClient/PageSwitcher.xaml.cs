using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DesktopClient.Menu;
using DesktopClient.eAdDataAccess;
using eAd.DataViewModels;
using eAd.Utilities;

namespace DesktopClient
{


    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PageSwitcher : Window
    {
        static volatile int runningDownloads = 0;
        /// <summary>
        public static List<MediaListModel> Playlist = new List<MediaListModel>();

        private object DownLoadsLock = new object();
        bool CheckedForNewMessages = false;
        public PageSwitcher()
        {
            InitializeComponent();
            Switcher.PageSwitcher = this;
            Switcher.Switch(MainWindow.Instance);

            StartKeepAliveThread();

            StartMessageReceiveThread();


            //          ThreadPool.QueueUserWorkItem(
            //              (state) =>
            //              {
            //                  Thread.Sleep(5000);
            //                  Switcher.PageSwitcher.Dispatcher.BeginInvoke(

            //System.Windows.Threading.DispatcherPriority.Normal

            //, new DispatcherOperationCallback(delegate
            //{




            //   // Switcher.Switch(new RFIDDetected());

            //    Switcher.Switch(CustomerPage.Instance);

            //    return null;

            //}), null);


            //              });

        }

        private void StartMessageReceiveThread()
        {
            //Messages Thread
            ThreadPool.QueueUserWorkItem((e) =>
                                             {
                                                 while (true)
                                                 {
                                                     Thread.Sleep(Constants.MessageWaitTime);
                                                     CheckForNewMessages();
                                                 }
                                             });
        }

        private void CheckForNewMessages()
        {
            try
            {
                using (ServiceClient myService2 = new ServiceClient())
                {
                    //BootStrap WCF (otherwise slow)
                    myService2.ClientCredentials.Windows.ClientCredential.UserName
                        = "admin";
                    myService2.ClientCredentials.Windows.ClientCredential.Password
                        = "Green2o11";

                    var hi2 = myService2.DoIHaveUpdates(Constants.MyStationID);
                    if (hi2)
                    {
                        RetrieveMessages();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Server Down" + exception.Message +
                                  exception.StackTrace);
            }
        }

        private void RetrieveMessages()
        {
            this.Dispatcher.BeginInvoke(
                 DispatcherPriority.Normal
                 , new DispatcherOperationCallback(delegate
                                                       {
                                                           try
                                                           {
                                                               using (
                                                                   ServiceClient
                                                                       myService3
                                                                           =
                                                                           new ServiceClient
                                                                               ()
                                                                   )
                                                               {
                                                                   var
                                                                       messages
                                                                           =
                                                                           myService3
                                                                               .
                                                                               GetAllMyMessages
                                                                               (Constants
                                                                                    .
                                                                                    MyStationID);

                                                                   foreach (
                                                                       var
                                                                           message
                                                                           in
                                                                           messages
                                                                       )
                                                                   {
                                                                       if (
                                                                           message
                                                                               .
                                                                               Type ==
                                                                           "Info")
                                                                       {
                                                                           ProcessInformationMessage(message);
                                                                       }

                                                                       if (
                                                                           message
                                                                               .
                                                                               Type ==
                                                                           "Media")
                                                                       {
                                                                           GetMedia
                                                                               (message);
                                                                       }

                                                                       if (
                                                                           message
                                                                               .
                                                                               Type ==
                                                                           "Status")
                                                                       {
                                                                           ProcessStatusMessage(message);
                                                                       }
                                                                   }


                                                                   //if (result == MessageBoxResult.OK)
                                                                   //{
                                                                   //    // Yes code here
                                                                   //}
                                                                   //else
                                                                   //{
                                                                   //    // No code here
                                                                   //}
                                                               }
                                                           }

                                                           catch (
                                                               Exception
                                                                   exception
                                                               )
                                                           {
                                                               Console.
                                                                   WriteLine
                                                                   ("Something Bad Happened ... :( Look Here: " +
                                                                    exception
                                                                        .
                                                                        Message +
                                                                    "\n" +
                                                                    exception
                                                                        .
                                                                        StackTrace);
                                                           }
                                                           return null;
                                                       }),
                 null)
                 ;
        }

        private void ProcessInformationMessage(MessageViewModel message)
        {
            MessageBoxResult
                result
                    =
                    MessageBox
                        .
                        Show
                        (this,
                         message
                             .
                             Text,
                         message
                             .
                             Type,
                         MessageBoxButton
                             .
                             OKCancel,
                         MessageBoxImage
                             .
                             Warning);

            using
                (
                ServiceClient
                    myService4
                        =
                        new ServiceClient
                            ()
                )
            {
                try
                {
                    var
                        status
                            =
                            myService4
                                .
                                MessageRead
                                (message
                                     .
                                     ID);
                }
                catch (TimeoutException exception)
                {
                    Console.WriteLine("Got {0}", exception.GetType());
                    myService4.Abort();
                }
                catch (CommunicationException exception)
                {
                    Console.WriteLine("Got {0}", exception.GetType());
                    myService4.Abort();
                }
            }
        }

        private void ProcessStatusMessage(MessageViewModel message)
        {
            switch
                (
                message
                    .
                    Command
                )
            {
                case
                    "Make UnAvailable"
                    :
                    {
                        CustomerPage
                            .
                            CurrentRFID
                            =
                            message
                                .
                                Text;
                        CustomerPage
                            .
                            Instance
                            .
                            Update
                            ();
                        Switcher
                            .
                            Switch
                            (CustomerPage
                                 .
                                 Instance);
                    }
                    break;
                case
                    "Make Available"
                    :
                    {
                        Switcher
                            .
                            Switch
                            (MainWindow
                                 .
                                 Instance);
                    }
                    break;
                case "Screenshot":
                    var fileName = "Screenshot#" + Constants.MyStationID + "#" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".jpg";
                    new ScreenCapture().CaptureScreenToFile(fileName,ImageFormat.Jpeg);
                    Snapshot.UploadFile(fileName,FileTypeEnum.Generic);
                    break;
                default
                    :
                    MessageBoxResult
                        result
                            =
                            MessageBox
                                .
                                Show
                                (this,
                                 message
                                     .
                                     Text,
                                 message
                                     .
                                     Type,
                                 MessageBoxButton
                                     .
                                     OKCancel,
                                 MessageBoxImage
                                     .
                                     Warning);
                    break;
            }


            var
                service
                    =
                    new ServiceClient
                        ();


            try
            {
                service.MessageRead
                        (message
                             .
                             ID);
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine("Got {0}", exception.GetType());
                service.Abort();
            }
            catch (CommunicationException exception)
            {
                Console.WriteLine("Got {0}", exception.GetType());
                service.Abort();
            }
        }

        private void StartKeepAliveThread()
        {
            // Keep Alive Thread
            ThreadPool.QueueUserWorkItem(
                (state) =>
                {
                    while (true)
                    {
                        ServiceClient myService = new ServiceClient();
                        try
                        {
                            //BootStrap WCF (otherwise slow)
                            myService.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                            myService.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                            var hi = myService.SayHi(Constants.MyStationID);
                            if (hi != "Hi there")
                            {
                                Console.WriteLine("Server Down");
                            }
                            else
                            {
                                if (!CheckedForNewMessages)
                                {
                                    GetMedia();
                                    CheckedForNewMessages = true;
                                }
                            }
                        }
                        catch (TimeoutException exception)
                        {
                            Console.WriteLine("Got {0}", exception.GetType());
                            myService.Abort();
                        }
                        catch (CommunicationException exception)
                        {
                            Console.WriteLine("Got {0}", exception.GetType());
                            myService.Abort();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Server Down" + exception.Message + exception.StackTrace);
                        }
                        Thread.Sleep(1000 * 10);
                    }
                });
        }

        private void GetMedia(MessageViewModel message = null)
        {
            using (ServiceClient myService5 = new ServiceClient())
            {
                try
                {
                    var
                        mediaList
                            =
                            myService5
                                .
                                GetMyMedia
                                (Constants
                                     .
                                     MyStationID);

                    //Playlist= new List<MediaListModel>();
                    if (message != null)
                    {
                        var
                            status
                                =
                                myService5
                                    .
                                    MessageRead
                                    (message
                                         .
                                         ID);
                    }
                    ThreadPool
                        .
                        QueueUserWorkItem
                        ((
                            e4)
                         =>
                             {
                                 foreach
                                     (
                                     var
                                         item
                                         in
                                         mediaList
                                     )
                                 {


                                     var
                                         path
                                             =
                                             Constants
                                                 .
                                                 AppPath +
                                             item
                                                 .
                                                 Location;
                                     var urlPath = item
                                         .
                                         Location;

                                     var
                                         fileExists
                                             =
                                             false;
                                     if
                                         (
                                         File
                                             .
                                             Exists
                                             (path) && new FileInfo(path).Length > 0)
                                     // Todo: Check if currently playing and also compare properties to prevent unneeded redownloads
                                     {
                                         //File.Delete(path);
                                         fileExists
                                             =
                                             true;
                                     }
                                     if
                                         (
                                         Playlist
                                             .
                                             Where
                                             (
                                                 i
                                                 =>
                                                 i
                                                     .
                                                     Location ==
                                                 path)
                                             .
                                             Count
                                             () <=
                                         0)
                                     {
                                         var
                                             model
                                                 =
                                                 item;
                                         model
                                             .
                                             Location
                                             =
                                             path;
                                         if
                                             (
                                             fileExists)
                                         {
                                             model
                                                 .
                                                 Downloaded
                                                 =
                                                 true;
                                         }

                                         Playlist
                                             .
                                             Add
                                             (model);
                                         if
                                             (
                                             fileExists)
                                             return;

                                     }
                                     irio
                                         .
                                         utilities
                                         .
                                         FileUtilities
                                         .
                                         FolderCreate
                                         (
                                             Path
                                                 .
                                                 GetDirectoryName
                                                 (path));

                                     using
                                         (
                                         WebClient
                                             client
                                                 =
                                                 new WebClient
                                                     ()
                                         )
                                     {
                                         try
                                         {
                                             client
                                                 .
                                                 DownloadProgressChanged
                                                 +=
                                                 client_DownloadProgressChanged;

                                             client
                                                 .
                                                 DownloadFileCompleted
                                                 += delegate(object sender, AsyncCompletedEventArgs args)
                                                        {
                                                            lock (DownLoadsLock)
                                                            {
                                                                runningDownloads--;
                                                            }
                                                            HideProgressBar();
                                                            var pitem =
                                                                Playlist.Where(i => i.Location == path).FirstOrDefault();

                                                            if (pitem != null)
                                                            {
                                                                pitem.Downloaded =
                                                                    true;

                                                                if (!File.Exists(path) || new FileInfo(path).Length <= 0)
                                                                {
                                                                    Playlist.Remove(pitem);
                                                                    Console.WriteLine("Downloaded Corrupted File");
                                                                }
                                                            }


                                                            if (args.Error == null)
                                                            {
                                                                lock (DownLoadsLock)
                                                                    Thread.Sleep(200);
                                                                if (runningDownloads == 0)
                                                                {
                                                                    MainWindow.Instance
                                                                        .UpdatePlayList
                                                                        ();
                                                                }
                                                            }




                                                        };
                                             //ClientDownloadFileCompleted;



                                             if
                                                 (
                                                 !fileExists)
                                             {
                                                 // Starts the download
                                                 lock
                                                     (
                                                     DownLoadsLock
                                                     )
                                                     runningDownloads
                                                         ++;

                                                 var
                                                     uri
                                                         = new Uri(Constants.ServerUrl + urlPath.Replace("\\", @"/"));
                                                 client.DownloadFileAsync(uri, path);
                                             }

                                         }
                                         catch (TimeoutException exception)
                                         {
                                             Console.WriteLine("Got {0}", exception.GetType());

                                         }
                                         catch (CommunicationException exception)
                                         {
                                             Console.WriteLine("Got {0}", exception.GetType());

                                         }
                                         //btnStartDownload.Text = "Download In Process";
                                         //btnStartDownload.Enabled = false;

                                     }
                                     //ServiceClient myService3 = new ServiceClient();
                                     ////BootStrap WCF (otherwise slow)
                                     //myService3.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                                     //myService3.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                                 }


                             });
                }
                catch (TimeoutException exception)
                {
                    Console.WriteLine("Got {0}", exception.GetType());
                    myService5.Abort();
                }
                catch (CommunicationException exception)
                {
                    Console.WriteLine("Got {0}", exception.GetType());
                    myService5.Abort();
                }
            }
        }

        private void ClientDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {

            lock (DownLoadsLock)
            {
                runningDownloads--;
            }
            HideProgressBar();
            //Playlist.Where(i => i.Location == path).First().Downloaded =
            //    true;
            if (e.Error == null)
            {
                lock (DownLoadsLock)
                    if (runningDownloads == 0)
                    {
                        MainWindow.Instance
                            .UpdatePlayList
                            ();
                    }
            }

            //   MessageBox.Show("Download Completed");

        }

        private static void HideProgressBar()
        {
            Switcher.PageSwitcher.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal
                , new DispatcherOperationCallback(delegate
                                                      {
                                                          if (MainWindow.Instance.Update_Progress.Visibility ==
                                                              Visibility.Visible)
                                                              MainWindow.Instance.Update_Progress.Visibility =
                                                                  Visibility.Hidden;
                                                          return null;
                                                      }), null);
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {


            Switcher.PageSwitcher.Dispatcher.BeginInvoke(

               System.Windows.Threading.DispatcherPriority.Normal

               , new DispatcherOperationCallback(delegate
               {
                   if (MainWindow.Instance.Update_Progress.Visibility == Visibility.Hidden)
                       MainWindow.Instance.Update_Progress.Visibility = Visibility.Visible;


                   double bytesIn = double.Parse(e.BytesReceived.ToString());
                   double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                   double percentage = bytesIn / totalBytes * 100;

                   MainWindow.Instance.Update_Progress.Value = ((percentage + MainWindow.Instance.Update_Progress.Value) / 2);
                   return null;
               }), null);


        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Splasher.CloseSplash();
        }
        public void Navigate(UserControl nextPage)
        {
            this.Content = nextPage;
        }

        public void Navigate(UserControl nextPage, object state)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable! "
                  + nextPage.Name.ToString());
        }
    }
}
