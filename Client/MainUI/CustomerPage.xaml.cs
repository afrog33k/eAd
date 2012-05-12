using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Reflection;
using Client.Service;
using eAd.Utilities;
using mshtml;

namespace Client
{
/// <summary>
/// Interaction logic for CustomerPage.xaml
/// </summary>
public partial class CustomerPage : UserControl, ISwitchable
{

    public static string Path
    {
        get
        {
            return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }
    }

    static CustomerPage()
    {
        var instance = Instance;
    }

    public CustomerPage(bool update = false)
    {
        this.InitializeComponent();
        //    GoogleMap.NavigateToString("maps.google.com");

        if (update)
            Update(1000);
    }

    static CustomerPage _instance;
    public static CustomerPage Instance
    {
        get
        {

            if (_instance == null)
            {
            
                                                                                                            _instance = new CustomerPage();
                                                                                         
               
            }
            return _instance;
        }
    }

    object UpdateLock = new object();
    private static BitmapImage image;
    private Thread DismissThread;

    public void Update(int milliseconds)
    {
        new Thread(
            () =>
        {
            //       Thread.Sleep(50);

            lock (UpdateLock)
            {
                try
                {

                    var sleepTime = milliseconds;

                    //   var items = myService.GetRecords().Select(p => p.Name);

                    try
                    {
                        var sleepTime2 = sleepTime;
                        using (
                            ServiceClient myService2 = new ServiceClient("BasicHttpBinding_IService",
                                                                         Constants.ServerAddress))
                        {
                            myService2.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                            myService2.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                            var customer = myService2.GetCustomerByRFID(CurrentRFID);
                            var stations = myService2.GetAllStations();
                            // LocationOverview.ItemsSource = items;
                            
                           
                            GoogleMaps.Locations = stations.ToArray();
                          
                           this.Dispatcher.BeginInvoke(
                                DispatcherPriority.Normal
                                , new Action(() =>
                                                              {
                                                                  try
                                                                  {

                                                                      image = null;
                                                                      int BytesToRead = 100;

                                                                      WebRequest request = WebRequest.Create(new Uri(customer.Picture, UriKind.Absolute));
                                                                      request.Timeout = -1;
                                                                      WebResponse response = request.GetResponse();
                                                                      Stream responseStream = response.GetResponseStream();
                                                                      if (responseStream != null)
                                                                      {
                                                                          var reader = new BinaryReader(responseStream);
                                                                          var memoryStream = new MemoryStream();

                                                                          var bytebuffer = new byte[BytesToRead];
                                                                          int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                                                                          while (bytesRead > 0)
                                                                          {
                                                                              memoryStream.Write(bytebuffer, 0, bytesRead);
                                                                              bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                                                                          }
                                                                          image = new BitmapImage();
                                                                          image.BeginInit();
                                                                          memoryStream.Seek(0, SeekOrigin.Begin);
                                                                          image.StreamSource = memoryStream;
                                                                      }
                                                                      image.EndInit();

                                                                  }
                                                                  catch (Exception)
                                                                  {

                                                                      //Default no pic pic
                                                                  }

                                                                  if (image != null && image.StreamSource.CanRead)
                                                                     CustomerProfile.CustomerProfilePicture.Source = image;

                                                                  CarInfoWidget.CarMake.Content = customer.CarMake;
                                                                  CarInfoWidget.CarModel.Content = customer.CarModel;
                                                                   CarInfoWidget. CarPlate.Content = customer.CarLicense;
                                                                  CustomerProfile.CustomerName.Content = (customer).Name;
                                                                  CustomerProfile.CustomerEmail.Content = customer.Email;
                                                                  CustomerProfile.CustomerPhone.Content = customer.Phone;
                                                                  CustomerProfile.CustomerAddress.Text = customer.Address;
                                                                  try
                                                                  {


                                                                      var charge = customer.ChargeRemaining;
                                                                      if (charge == null)
                                                                          charge = "0";
                                                                   BatteryInfoWidget.   CurrentCharge.Content = charge;
                                                                   BatteryInfoWidget.BatteryAnimation.PercentCharged = Convert.ToInt32(charge);
                                                                   BatteryInfoWidget.LastCharged.Content = customer.LastRechargeDate;
                                                                   BatteryInfoWidget.AccountBalance.Content = customer.AccountBalance;
                                                                  }
                                                                  catch (Exception ex)
                                                                  {
                                                                  }
                                                                  Dictionary<string,BitmapImage> stationlist= new Dictionary<string, BitmapImage>();
                                                                  int count = 0;
                                                                  foreach (var stationViewModel in stations)
                                                                  {
                                                                      if(stationlist.Count>=6)
                                                                          break;

                                                                      if (stationlist.ContainsKey(stationViewModel.Name))
                                                                          stationViewModel.Name =
                                                                              stationViewModel.Name + count;
                                                                      stationlist.Add(stationViewModel.Name,stationViewModel.Available?OnlineIcon:OfflineIcon);
                                                                  }

                                                                  LocationWidget.StationList.Images.ItemsSource = stationlist;

                                                                  // .Content = (customer).Name;
                                                                  NavigatePage();
                                                              }));



                           if (_dismissTimer != null)
                           {
                               _dismissTimer.Stop();
                               _dismissTimer = null;
                           }

                           _dismissTimer = new System.Timers.Timer();

                           _dismissTimer.Elapsed += delegate
                                                        {

                                                            Switcher.Switch(MainForm.Instance);
                                                            _dismissTimer.Stop();


                                                        };

                           _dismissTimer.Interval = (milliseconds);
                           _dismissTimer.Start();




                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.StackTrace + "\n" + ex.Message);
                    }


                }
                catch (Exception ex)
                {
                    Instance.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                                                                                       {
                                                                                         CustomerProfile.  CustomerName.Content =
                                                                                               "Invalid Customer";

                                                                                           return null;
                                                                                       }));
                    Console.WriteLine(ex.StackTrace + "\n" + ex.Message);
                    ;
                }
            }
        }).Start();
    }

    static System.Timers.Timer _dismissTimer = new System.Timers.Timer();

    private static BitmapImage OnlineIcon = new BitmapImage(new Uri("pack://application:,,,/DataCache/Cute-Ball-Go-icon32.png"));
    private static BitmapImage OfflineIcon = new BitmapImage(new Uri("pack://application:,,,/DataCache/Cute-Ball-Stop-icon32.png"));

    public static string CurrentRFID
    {
        get;
        set;
    }

    private void NavigatePage()
    {
        // Uri uri = new Uri(@"pack://application:,,,/Navigator.htm");
        //Stream source = n// Application.GetContentStream(uri).Stream;
     LocationWidget.GoogleMap.NavigateToString(GoogleMaps.Webpage);
     LocationWidget.GoogleMap.Navigated += GoogleMapNavigated;
    }
    private const string DisableScriptError =
        @"function noError() {

          return true;
       }
       window.onerror = noError;";

    private void GoogleMapNavigated(object sender, NavigationEventArgs e)
    {
        SuppressScriptErrors(LocationWidget.GoogleMap, true);
        InjectDisableScript();
    }
    private void InjectDisableScript()
    {

        HTMLDocument doc = LocationWidget.GoogleMap.Document as HTMLDocument;

        HTMLDocument doc2 = LocationWidget.GoogleMap.Document as HTMLDocument;


        //Questo crea lo script per la soprressione degli errori

        IHTMLScriptElement scriptErrorSuppressed = (IHTMLScriptElement)doc2.createElement("SCRIPT");
        scriptErrorSuppressed.type = "text/javascript";

        scriptErrorSuppressed.text = DisableScriptError;



        IHTMLElementCollection nodes = doc.getElementsByTagName("head");



        foreach (IHTMLElement elem in nodes)
        {

            //Appendo lo script all'head cosi è attivo



            HTMLHeadElement head = (HTMLHeadElement)elem;

            head.appendChild((IHTMLDOMNode)scriptErrorSuppressed);

        }

    }

    /// <summary>
    /// Supress Script Errors
    /// </summary>
    /// <param name="wb"></param>
    /// <param name="hide"></param>
    public void SuppressScriptErrors(WebBrowser wb, bool hide)
    {
        FieldInfo fi = typeof(WebBrowser).GetField(
                           "_axIWebBrowser2",
                           BindingFlags.Instance | BindingFlags.NonPublic);

        if (fi != null)
        {
            object browser = fi.GetValue(wb);

            if (browser != null)
            {
                browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty,
                                               null, browser, new object[] { hide });

            }
        }
    }
    private void btnSubmit_Click(object sender, RoutedEventArgs e)
    {
        //wbNavigator.InvokeScript("setDirections", new object[] { tbFrom.Text, tbTo.Text, "en" });
        //    _LoadDirectionFromGoogle(_queryBuilder(tbFrom.Text, tbTo.Text));

        Thread gpsSignal = new Thread(new ThreadStart(_gpsSignal));
        gpsSignal.Start();
    }

    public delegate void MyDelegate();
    private void _gpsSignal()
    {
        //double startLat = Convert.ToDouble(_directionResponse.route[0].leg[0].step[0].start_location[0].lat);
        //double startLng = Convert.ToDouble(_directionResponse.route[0].leg[0].step[0].start_location[0].lng);
        //double endLat, endLng;
        //foreach (DirectionsResponseRouteLegStep step in _directionResponse.route[0].leg[0].step)
        //{
        //    wbNavigator.Dispatcher.BeginInvoke((MyDelegate)delegate
        //    {
        //        wbNavigator.InvokeScript("setInstruction", new object[] { step.html_instructions });
        //    }, null);
        //    startLat = Convert.ToDouble(step.start_location[0].lat);
        //    startLng = Convert.ToDouble(step.start_location[0].lng);
        //    endLat = Convert.ToDouble(step.end_location[0].lat);
        //    endLng = Convert.ToDouble(step.end_location[0].lng);
        //    while (Math.Abs(startLat - endLat) > 0.00050 || Math.Abs(startLng - endLng) > 0.00050)
        //    {
        //        wbNavigator.Dispatcher.BeginInvoke((MyDelegate)delegate
        //        {
        //            wbNavigator.InvokeScript("setCenter", new object[] { startLat.ToString(), startLng.ToString() });
        //        }, null);
        //        if (endLat > startLat) startLat = startLat + 0.00009; else startLat = startLat - 0.00009;
        //        if (endLng > startLng) startLng = startLng + 0.00009; else startLng = startLng - 0.00009;
        //        Thread.Sleep(20);
        //    }
        //}
    }

    private void _LoadDirectionFromGoogle(string url)
    {
        ////string url = "http://maps.google.com/maps/api/directions/xml?origin=Chicago,IL&destination=Los+Angeles,CA&waypoints=Joplin,MO|Oklahoma+City,OK&sensor=false";
        //HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
        //IWebProxy proxy = WebRequest.GetSystemWebProxy(); proxy.Credentials = CredentialCache.DefaultCredentials;
        //httpRequest.Proxy = proxy;
        //httpRequest.Method = "GET";
        ////httpRequest.UserAgent = "Mozilla/4.0+(compatible;+MSIE+6.0;+Windows+NT+5.1;+.NET+CLR+1.1.4322)";
        //httpRequest.Timeout = -1;
        //HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
        //Stream resStream = httpResponse.GetResponseStream();

        //System.Xml.Serialization.XmlSerializer deserializer = new System.Xml.Serialization.XmlSerializer(typeof(DirectionsResponse));
        //_directionResponse = (DirectionsResponse)deserializer.Deserialize(resStream);

    }

    private string _queryBuilder(string from, string to)
    {
        string url = "http://maps.google.com/maps/api/directions/xml";
        url = url + "?origin=" + from.Trim() + "&destination=" + to.Trim() + "&sensor=false";
        return url;
    }


    public void UtilizeState(object state)
    {
        throw new NotImplementedException();
    }

    private void FormFadeOut_Completed(object sender, EventArgs e)
    {

    }
}
}