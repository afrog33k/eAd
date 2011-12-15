using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DesktopClient.Menu;
using DesktopClient.eAdDataAccess;

namespace DesktopClient
{


    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PageSwitcher : Window
    {

        public static List<string> Playlist = new List<string>();
        public PageSwitcher()
        {
            InitializeComponent();
            Switcher.PageSwitcher = this;
            Switcher.Switch(MainWindow.Instance);

            // Keep Alive Thread
            ThreadPool.QueueUserWorkItem(
                (state) =>
                    {
                        while (true)
                        {
                            
                      
            try
            {

              
                {
                    ServiceClient myService = new ServiceClient();
                    //BootStrap WCF (otherwise slow)
                    myService.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                    myService.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                    var hi = myService.SayHi(Constants.MyStationID);
                    if (hi != "Hi there")
                    {
                        Console.WriteLine("Server Down");

                    }
                }
            }
            catch (Exception exception)
            {

                Console.WriteLine("Server Down" + exception.Message + exception.StackTrace);
            }
            Thread.Sleep(1000 * 10); 
                        }    
                    });

            //Messages Thread
            ThreadPool.QueueUserWorkItem((e) =>
                                             {

                                                 while (true)
                                                 {
                                                     try
                                                     {

                                                    
                                                     Thread.Sleep(Constants.MessageWaitTime);
                                                     ServiceClient myService2 = new ServiceClient();
                                                     //BootStrap WCF (otherwise slow)
                                                     myService2.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                                                     myService2.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                                                     var hi2 = myService2.DoIHaveUpdates(Constants.MyStationID);
                                                     if (hi2)
                                                     {
                                                         Switcher.PageSwitcher.Dispatcher.BeginInvoke(

  System.Windows.Threading.DispatcherPriority.Normal

  , new DispatcherOperationCallback(delegate
  {

      var messages = myService2.GetAllMyMessages(Constants.MyStationID);
      foreach (var message in messages)
      {
         
          if (message.Type == "Info")
          {


              MessageBoxResult result = MessageBox.Show(this,
                                                        message.Text,
                                                        message.Type,
                                                        MessageBoxButton.
                                                            OKCancel,
                                                        MessageBoxImage.
                                                            Warning);
              var status = myService2.MessageRead(message.ID);
          }

          if (message.Type == "Media")
          {
             var mediaList= myService2.GetMyMedia(Constants.MyStationID);
            
              ThreadPool.QueueUserWorkItem((e4) =>
                                               {
                                                   foreach (var item in mediaList)
                                                   {
                                                       WebClient client = new WebClient();
                                                       client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                                       client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

                                                       // Starts the download
                                                       var path =Constants.AppPath+ item.Location;

                                                       irio.utilities.File.FolderCreate(Path.GetDirectoryName(path));
                                                       var fileExists = false;
                                                       if(File.Exists(path)) // Todo: Check if currently playing and also compare properties to prevent unneeded redownloads
                                                       {
                                                           //File.Delete(path);
                                                           fileExists = true;
                                                       }

                                                       if(!fileExists)
                                                       client.DownloadFileAsync(new Uri((Constants.ServerUrl+"//"+ item.Location.Replace(@"\\",@"/"))), path);

                                                       if(!Playlist.Contains(path))
                                                       {
                                                           Playlist.Add(path);
                                                       }
                                                       MainWindow.Instance.UpdatePlayList();
                                                       //btnStartDownload.Text = "Download In Process";
                                                       //btnStartDownload.Enabled = false;
                                                      
                                                   }
                                                   ServiceClient myService3 = new ServiceClient();
                                                   //BootStrap WCF (otherwise slow)
                                                   myService3.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                                                   myService3.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                                                   var status = myService2.MessageRead(message.ID);
                                               });

          }

          if (message.Type == "Status")
          {
              switch (message.Command)
              {
                  case "Make UnAvailable":
                      {
                          CustomerPage.CurrentRFID = message.Text;
                          CustomerPage.Instance.Update();
                          Switcher.Switch(CustomerPage.Instance);
                      }
                      break;
                  case "Make Available":
                      {
                          Switcher.Switch(MainWindow.Instance);
                      }
                      break;

                  default:
                      MessageBoxResult result = MessageBox.Show(this,
                                                                message.Text,
                                                                message.Type,
                                                                MessageBoxButton.
                                                                    OKCancel,
                                                                MessageBoxImage.
                                                                    Warning);
                      break;
              }

              var status = myService2.MessageRead(message.ID); 
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


      return null;

  }), null);
                   
              
          
                                                    
                                                     }
                                                     }
                                                     catch (Exception exception)
                                                     {

                                                         Console.WriteLine("Server Down" + exception.Message + exception.StackTrace);
                                                     }
                                                 }
                                             });


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

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
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
         //   MessageBox.Show("Download Completed");

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

                   MainWindow.Instance.Update_Progress.Value = percentage;
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
