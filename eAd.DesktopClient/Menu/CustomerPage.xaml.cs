using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DesktopClient.eAdDataAccess;
using eAd.Utilities;

namespace DesktopClient.Menu
{


	/// <summary>
	/// Interaction logic for Login.xaml
	/// </summary>
	public partial class CustomerPage : UserControl, ISwitchable
	{

            public static  string Path
{
    get { return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName); }
}
		public CustomerPage()
		{
			this.InitializeComponent();
            Update();
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

	    public void Update()
        {
           
            ThreadPool.QueueUserWorkItem(
               (state) =>
               {
            //       Thread.Sleep(50);
                   try
                   {

                  
                   ServiceClient myService = new ServiceClient();
                   myService.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                   myService.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                   var customer = myService.GetCustomerByRFID(CurrentRFID);
                   var stations = myService.GetAllStations();
                //   var items = myService.GetRecords().Select(p => p.Name);
                  Instance.Dispatcher.BeginInvoke(

                       System.Windows.Threading.DispatcherPriority.Normal

                       , new DispatcherOperationCallback(delegate
                       {
                   try
                   {
                  
                  
                           // Switcher.Switch(new RFIDDetected());

                          // LocationOverview.ItemsSource = items;
                       CustomerProfilePicture.Source =
                           new BitmapImage( new Uri(
                               "pack://application:,,," + customer.Picture));
                       CarMake.Content = customer.CarMake;
                       CarModel.Content = customer.CarModel;
                       CarPlate .Content = customer.CarLicense;
                       CustomerName.Content =( customer).Name;
                       CustomerEmail.Content = customer.Email;
                       CustomerPhone.Content = customer.Phone;
                       CustomerAddress.Content = customer.Address;
                       CurrentCharge.Content = customer.ChargeRemaining;
                       BatteryAnimation.PercentCharged = Convert.ToInt32(customer.ChargeRemaining);
                       LastCharged.Content = customer.LastRechargeDate;
                       AccountBalance.Content = customer.AccountBalance;
                       StationOverview.ItemsSource = stations;
                       GoogleMaps.Locations = stations.ToArray();
                       // .Content = (customer).Name;
                       NavigatePage();
                   }
                   catch (Exception ex)
                   {

                       Console.WriteLine(ex.StackTrace + "\n" + ex.Message);
                   }
                   return null;
                       }), null);
                   }
                   catch (Exception ex)
                   {
                       CustomerName.Content = "Invalid Customer";
                       Console.WriteLine(ex.StackTrace + "\n" + ex.Message);
                   }
               });
        }

	    public static string CurrentRFID { get; set; }

	    private void NavigatePage()
        {
            Uri uri = new Uri(@"pack://application:,,,/Navigator.htm");
            //Stream source = n// Application.GetContentStream(uri).Stream;
            GoogleMap.NavigateToString(GoogleMaps.Webpage);
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
            //    wbNavigator.Dispatcher.Invoke((MyDelegate)delegate
            //    {
            //        wbNavigator.InvokeScript("setInstruction", new object[] { step.html_instructions });
            //    }, null);
            //    startLat = Convert.ToDouble(step.start_location[0].lat);
            //    startLng = Convert.ToDouble(step.start_location[0].lng);
            //    endLat = Convert.ToDouble(step.end_location[0].lat);
            //    endLng = Convert.ToDouble(step.end_location[0].lng);
            //    while (Math.Abs(startLat - endLat) > 0.00050 || Math.Abs(startLng - endLng) > 0.00050)
            //    {
            //        wbNavigator.Dispatcher.Invoke((MyDelegate)delegate
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

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new MainMenu());
		}

        #region ISwitchable Members

        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {

        }
    }
}