using System;
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
                    var hi = myService.SayHi(1);
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

                                                    
                                                     Thread.Sleep(1000);
                                                     ServiceClient myService2 = new ServiceClient();
                                                     //BootStrap WCF (otherwise slow)
                                                     myService2.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                                                     myService2.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                                                     var hi2 = myService2.DoIHaveUpdates(1);
                                                     if (hi2)
                                                     {
                                                         Switcher.PageSwitcher.Dispatcher.BeginInvoke(

  System.Windows.Threading.DispatcherPriority.Normal

  , new DispatcherOperationCallback(delegate
  {

      var messages = myService2.GetAllMyMessages(1);
      foreach (var message in messages)
      {
          var status = myService2.MessageRead(message.ID);
          if (message.Type == "Info")
          {


              MessageBoxResult result = MessageBox.Show(this,
                                                        message.Text,
                                                        message.Type,
                                                        MessageBoxButton.
                                                            OKCancel,
                                                        MessageBoxImage.
                                                            Warning);
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
