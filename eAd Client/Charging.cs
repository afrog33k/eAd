

using ClientApp.Service;

namespace ClientApp
{
    using ClientApp.Core;
    using ClientApp.MainUI;
    using ClientApp.Properties;
    
    using ClientApp.Widgets;
    using eAd.DataViewModels;
    using eAd.Utilities;
    using mshtml;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Timers;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Threading;
    using System.Xml.Serialization;

   
    public partial class Charging : UserControl, IComponentConnector, IPausableControl
    {
        private System.Drawing.Size _clientSize;
      
        private static string _currentRFID;
        private System.Timers.Timer _dismissTimer;
        private static Charging _instance;
        private bool _isExpired;
        private double _layoutHeight;
        private int _layoutId;
        private double _layoutWidth;
        private static Collection<ClientApp.Core.Region> _regions;
        private double _scaleFactor;
        private BatteryInfo BatteryInfoWidget;
        private CarInfo CarInfoWidget;
        private static bool Cleared = false;
        private Profile CustomerProfile;
        private const string DisableScriptError = "function noError() {\r\n\r\n          return true;\r\n       }\r\n       window.onerror = noError;";
        private BitmapImage image;
        public Location LocationWidget;

        private static BitmapImage OfflineIcon = new BitmapImage(new Uri("pack://application:,,,/DataCache/Cute-Ball-Stop-icon32.png"));
        private static BitmapImage OnlineIcon = new BitmapImage(new Uri("pack://application:,,,/DataCache/Cute-Ball-Go-icon32.png"));
        private object UpdateLock = new object();

        public Charging()
        {
            Instance = this;
            this.InitializeComponent();
            if (Settings.Default.sizeX != 0M)
            {
                this._clientSize = new System.Drawing.Size((int) Settings.Default.sizeX, (int) Settings.Default.sizeY);
            }
            else
            {
                this._clientSize = new System.Drawing.Size((int) SystemParameters.PrimaryScreenWidth, (int) SystemParameters.PrimaryScreenHeight);
                base.Visibility = Visibility.Visible;
                base.Width = this._clientSize.Width;
                base.Height = this._clientSize.Height;
            }
            App.DoEvents();
        }

        public void ClearAllInfo()
        {
            Action method = null;
            Cleared = false;
            try
            {
                if (method == null)
                {
                    method =new Action(()=>{
                        if (((WidgetsFactory.Widgets["PersonalInfo"] != null) && (WidgetsFactory.Widgets["CarInfo"] != null)) && ((WidgetsFactory.Widgets["BatteryInfo"] != null) && (WidgetsFactory.Widgets["LocationInfo"] != null)))
                        {
                            this.CustomerProfile = WidgetsFactory.Widgets["PersonalInfo"] as Profile;
                            this.CarInfoWidget = WidgetsFactory.Widgets["CarInfo"] as CarInfo;
                            this.BatteryInfoWidget = WidgetsFactory.Widgets["BatteryInfo"] as BatteryInfo;
                            this.LocationWidget = WidgetsFactory.Widgets["LocationInfo"] as Location;
                            this.CustomerProfile.CustomerProfilePicture.Source = null;
                            this.CarInfoWidget.CarMake.Content = "";
                            this.CarInfoWidget.CarModel.Content = "";
                            this.CarInfoWidget.CarPlate.Content = "";
                            this.CustomerProfile.CustomerName.Content = "";
                            this.CustomerProfile.CustomerEmail.Content = "";
                            this.CustomerProfile.CustomerPhone.Content = "";
                            this.CustomerProfile.CustomerAddress.Text = "";
                            string str = "0";
                            this.BatteryInfoWidget.CurrentCharge.Content = str;
                            this.BatteryInfoWidget.BatteryAnimation.PercentCharged = Convert.ToInt32(str);
                            this.BatteryInfoWidget.LastCharged.Content = "";
                            this.BatteryInfoWidget.AccountBalance.Content = "";
                            Cleared = true;
                        }
                    });
                }
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
            }
            catch (Exception)
            {
            }
        }

        public void DestroyLayout()
        {
            App.DoEvents();
            if (_regions != null)
            {
                if (_regions.Count == 0)
                {
                    _regions = null;
                }
                else
                {
                    for (int i = 0; i < _regions.Count; i++)
                    {
                        ClientApp.Core.Region region = _regions[i];
                        region.Clear();
                        base.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(()=>{
                            if (region != null)
                            {
                                this.MediaCanvas.Children.Remove(region);
                            }
                        }));
                        try
                        {
                            region.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
                    _regions.Clear();
                    _regions = null;
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T: DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if ((child != null) && (child is T))
                    {
                        yield return (T) child;
                    }
                    IEnumerator<T> enumerator = FindVisualChildren<T>(child).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        T current = enumerator.Current;
                        yield return current;
                    }
                }
            }
        }

        public void FlushStats()
        {
            try
            {
                ClientManager.Instance.StatLog.Flush();
            }
            catch
            {
                Trace.WriteLine(new LogMessage("AdvertPlayer - FlushStats", "Unable to Flush Stats"), LogType.Error.ToString());
            }
        }

        private void FormFadeOutCompleted(object sender, EventArgs e)
        {
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < imageEncoders.Length; i++)
            {
                if (imageEncoders[i].MimeType == mimeType)
                {
                    return imageEncoders[i];
                }
            }
            return null;
        }

        private void GoogleMapNavigated(object sender, NavigationEventArgs e)
        {
            this.SuppressScriptErrors(this.LocationWidget.GoogleMap, true);
            this.InjectDisableScript();
            Instance.Dispatcher.BeginInvoke(new Action(()=> {
                Instance.LocationWidget.Visibility = Visibility.Visible;
            }));
        }

     

        private void InjectDisableScript()
        {
            HTMLDocument document = this.LocationWidget.GoogleMap.Document as HTMLDocument;
            HTMLDocument document2 = this.LocationWidget.GoogleMap.Document as HTMLDocument;
            IHTMLScriptElement element = (IHTMLScriptElement) document2.createElement("SCRIPT");
            element.type = "text/javascript";
            element.text = "function noError() {\r\n\r\n          return true;\r\n       }\r\n       window.onerror = noError;";
            foreach (IHTMLElement element2 in document.getElementsByTagName("head"))
            {
                ((HTMLHeadElement) element2).appendChild((IHTMLDOMNode) element);
            }
        }

        private void NavigatePage()
        {
            Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(()=>{
                this.LocationWidget.GoogleMap.NavigateToString(GoogleMaps.Webpage);
                this.LocationWidget.GoogleMap.Navigated += new NavigatedEventHandler(this.GoogleMapNavigated);
            }));
        }

        public void PrepareLayout(string layoutPath)
        {
            Action method = null;
            Action start2 = null;
            Action start3 = null;
            Action<RegionOptions> action = null;
            LayoutModel layout;
            if ((layoutPath == (Settings.Default.LibraryPath + @"\Default.xml")) || string.IsNullOrEmpty(layoutPath))
            {
                throw new Exception("Default layout");
            }
            try
            {
                using (FileStream stream = File.Open(layoutPath, FileMode.Open, FileAccess.Read, FileShare.Write))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LayoutModel));
                    layout = (LayoutModel) serializer.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("Could not find the layout file {0}: {1}", layoutPath, exception.Message));
                throw;
            }
            this._layoutWidth = layout.Width;
            this._layoutHeight = layout.Height;
            int width = this._clientSize.Width;
            int height = this._clientSize.Height;
            _regions = new Collection<ClientApp.Core.Region>();
            RegionOptions arg = new RegionOptions();
            try
            {
                if (!string.IsNullOrEmpty(layout.Bgcolor))
                {
                    if (method == null)
                    {
                        method =new Action(()=>{
                            MediaCanvas.Background = new SolidColorBrush((System.Windows.Media.Color) System.Windows.Media.ColorConverter.ConvertFromString(layout.Bgcolor));
                        });
                    }
                    base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
                    arg.backgroundColor = layout.Bgcolor;
                }
            }
            catch
            {
                if (start2 == null)
                {
                    start2 =new Action(()=>{
                        this.MediaCanvas.Background = new SolidColorBrush(Colors.Black);
                    });
                }
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, start2);
                arg.backgroundColor = "#000000";
            }
            try
            {
                if (layout.Background == null)
                {
                    this.MediaCanvas.Background = null;
                    arg.backgroundImage = "";
                }
                else
                {
                    string bgFilePath = string.Concat(new object[] { Settings.Default.LibraryPath, @"\backgrounds\", width, "x", height, "_", layout.Background });
                    Utilities.CreateFolder(Path.GetDirectoryName(bgFilePath));
                    if (!File.Exists(bgFilePath))
                    {
                        System.Drawing.Image original = System.Drawing.Image.FromFile(Settings.Default.LibraryPath + @"\" + layout.Background);
                        Bitmap bitmap = new Bitmap(original, width, height);
                        EncoderParameters encoderParams = new EncoderParameters(1);
                        EncoderParameter parameter = new EncoderParameter(Encoder.Quality, 90L);
                        encoderParams.Param[0] = parameter;
                        ImageCodecInfo encoderInfo = GetEncoderInfo("image/jpeg");
                        bitmap.Save(bgFilePath, encoderInfo, encoderParams);
                        original.Dispose();
                        bitmap.Dispose();
                    }
                    base.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(()=>{
                        this.MediaCanvas.Background = new ImageBrush(new BitmapImage(new Uri(bgFilePath.Replace(@"\", "/"), UriKind.Relative)));
                    }));
                    arg.backgroundImage = bgFilePath;
                }
            }
            catch (Exception)
            {
                if (start3 == null)
                {
                    start3 =new Action(()=>{
                        this.MediaCanvas.Background = System.Windows.Media.Brushes.Black;
                    });
                }
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, start3);
                arg.backgroundImage = "";
            }
            App.DoEvents();
            List<LayoutRegion> regions = layout.Regions;
            List<List<LayoutRegionMedia>> list2 = (from r in layout.Regions select r.Media).ToList<List<LayoutRegionMedia>>();
            if ((regions.Count == 0) || (list2.Count == 0))
            {
                Trace.WriteLine(new LogMessage("PrepareLayout", string.Format("A layout with {0} regions and {1} media has been detected.", regions.Count.ToString(), list2.Count.ToString())), LogType.Info.ToString());
                if (ClientManager.Instance.Schedule.ActiveLayouts == 1)
                {
                    Trace.WriteLine(new LogMessage("PrepareLayout", "Only 1 layout scheduled and it has nothing to show."), LogType.Info.ToString());
                    throw new Exception("Only 1 layout schduled and it has nothing to show");
                }
                Trace.WriteLine(new LogMessage("PrepareLayout", string.Format(string.Format("An empty layout detected, will show for {0} seconds.", Settings.Default.emptyLayoutDuration.ToString()), new object[0])), LogType.Info.ToString());
                List<LayoutRegion> list3 = new List<LayoutRegion>();
                LayoutRegion region = new LayoutRegion {
                    Id = "blah",
                    Width = 1,
                    Height = 1,
                    Top = 1,
                    Left = 1
                };
                List<LayoutRegionMedia> list4 = new List<LayoutRegionMedia>();
                LayoutRegionMedia media = new LayoutRegionMedia {
                    Id = "blah",
                    Type = "Text",
                    Duration = 0
                };
                LayoutRegionMediaRaw raw = new LayoutRegionMediaRaw {
                    Text = ""
                };
                media.Raw = raw;
                list4.Add(media);
                region.Media = list4;
                list3.Add(region);
                regions = list3;
            }
            foreach (LayoutRegion region2 in regions)
            {
                if ((region2.Media.Count != 0) || (region2.Type == "Widget"))
                {
                    arg.scheduleId = ClientManager.Instance.ScheduleId;
                    arg.layoutId = this._layoutId;
                    arg.regionId = region2.Id;
                    arg.Name = region2.Name;
                    arg.FileType = region2.Type;
                    arg.Width = (int) ((((double) (region2.Width + 14)) / this._layoutWidth) * this._clientSize.Width);
                    arg.Height = (int) ((((double) (region2.Height + 14)) / this._layoutHeight) * this._clientSize.Height);
                    int left = region2.Left;
                    if (left < 0)
                    {
                        left = 0;
                    }
                    int top = region2.Top;
                    if (top < 0)
                    {
                        top = 0;
                    }
                    arg.Left = (int) ((((double) left) / this._layoutWidth) * this._clientSize.Width);
                    arg.Top = (int) ((((double) top) / this._layoutHeight) * this._clientSize.Height);
                    arg.ScaleFactor = this._scaleFactor;
                    arg.BackgroundLeft = arg.Left * -1;
                    arg.BackgroundTop = arg.Top * -1;
                    arg.mediaNodes = region2.Media;
                    if (action == null)
                    {
                        action = delegate (RegionOptions opts) {
                            ClientApp.Core.Region item = new ClientApp.Core.Region(ClientManager.Instance.StatLog, ClientManager.Instance.CacheManager);
                            item.DurationElapsedEvent += new ClientApp.Core.Region.DurationElapsedDelegate(this.TempDurationElapsedEvent);
                            item.RegionOptions = opts;
                            _regions.Add(item);
                            item.Opacity = 0.0;
                            this.MediaCanvas.Children.Add(item);
                            item.AnimateIn();
                        };
                    }
                    base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action, arg);
                    App.DoEvents();
                }
            }
            regions = null;
            list2 = null;
        }

        public void SuppressScriptErrors(WebBrowser wb, bool hide)
        {
            FieldInfo field = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                object target = field.GetValue(wb);
                if (target != null)
                {
                    target.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, target, new object[] { hide });
                }
            }
        }

        

        private void TempDurationElapsedEvent()
        {
            this._isExpired = true;
            foreach (ClientApp.Core.Region region in _regions)
            {
                if (!region.hasExpired)
                {
                    this._isExpired = false;
                }
            }
            if (this._isExpired)
            {
                foreach (ClientApp.Core.Region region2 in _regions)
                {
                    region2.layoutExpired = true;
                }
                ClientManager.Instance.Schedule.NextLayout("Charging");
            }
            App.DoEvents();
        }


        private static BackgroundWorker ClientUpdateWorker;

        private static int runs = 0;

        public void Pause()
        {
            if (_regions != null)
            {
                var regionsToUpdate = new List<Core.Region>();

                foreach (var region in _regions)
                {
                    regionsToUpdate.Add(region);

                }

                foreach (var region in regionsToUpdate)
                {
                    region.Paused = true;
                    if (region.CurrentMedia != null)
                        region.CurrentMedia.Pause();
                    if (region.OldMedia != region.CurrentMedia)
                    {
                        if (region.OldMedia != null)
                        {
                            region.OldMedia.Dispose();
                        }
                    }
                }
            }
        }

        public void UnPause()
        {
            if (_regions != null)
            {
                var regionsToUpdate = new List<Core.Region>();

                foreach (var region in _regions)
                {
                    regionsToUpdate.Add(region);

                }

                foreach (var region in regionsToUpdate)
                {
                    region.Paused = false;
                    if (region.CurrentMedia != null)
                        region.CurrentMedia.UnPause();
                    region.EvaluateOptions();
                }
            }
        }



        public void Update(int milliseconds)
        {

            //ClientUpdateWorker = new BackgroundWorker();
            //ClientUpdateWorker.WorkerReportsProgress = true;
            //ClientUpdateWorker.DoWork += ClientUpdateWorkerOnDoWork;
            //ClientUpdateWorker.ProgressChanged += ;
            //ClientUpdateWorker.RunWorkerAsync();

            ThreadPool.QueueUserWorkItem(delegate (object hg) {
                ElapsedEventHandler handler = null;
                Action method = null;
                lock (this.UpdateLock)
                {
                    try
                    {
                        while (!Cleared)
                        {
                        }
                        int num1 = milliseconds;
                        try
                        {
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(()=>{
                                LoadingProfile.Instance.progressBar.Value = 10.0;
                            }));
                            using (ServiceClient client = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress))
                            {
                                client.ClientCredentials.Windows.ClientCredential.UserName = "admin";
                                client.ClientCredentials.Windows.ClientCredential.Password = "Green2o11";
                                CustomerViewModel customer = client.GetCustomerByRFID(CurrentRFID);
                                StationViewModel[] stations = client.GetAllStations();
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(()=>{
                                    LoadingProfile.Instance.progressBar.Value = 20.0;
                                }));
                                GoogleMaps.Locations = stations.ToArray<StationViewModel>();
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(()=>{
                                    if (customer.Picture != null)
                                    {
                                        this.CustomerProfile.CustomerProfilePicture.Source = new BitmapImage(new Uri(customer.Picture, UriKind.Absolute));
                                    }
                                    LoadingProfile.Instance.progressBar.Value += 10.0;
                                    this.CarInfoWidget.CarMake.Content = customer.CarMake;
                                    LoadingProfile.Instance.progressBar.Value += 10.0;
                                    this.CarInfoWidget.CarModel.Content = customer.CarModel;
                                    LoadingProfile.Instance.progressBar.Value += 10.0;
                                    this.CarInfoWidget.CarPlate.Content = customer.CarLicense;
                                    LoadingProfile.Instance.progressBar.Value += 10.0;
                                    this.CustomerProfile.CustomerName.Content = customer.Name;
                                    LoadingProfile.Instance.progressBar.Value += 10.0;
                                    this.CustomerProfile.CustomerEmail.Content = customer.Email;
                                    LoadingProfile.Instance.progressBar.Value += 10.0;
                                    this.CustomerProfile.CustomerPhone.Content = customer.Phone;
                                    this.CustomerProfile.CustomerAddress.Text = customer.Address;
                                    LoadingProfile.Instance.progressBar.Value += 10.0;
                                    try
                                    {
                                        string chargeRemaining = customer.ChargeRemaining;
                                        if (chargeRemaining == null)
                                        {
                                            chargeRemaining = "0";
                                        }
                                        this.BatteryInfoWidget.CurrentCharge.Content = chargeRemaining;
                                        this.BatteryInfoWidget.BatteryAnimation.PercentCharged = Convert.ToInt32(chargeRemaining);
                                        this.BatteryInfoWidget.LastCharged.Content = customer.LastRechargeDate;
                                        this.BatteryInfoWidget.AccountBalance.Content = customer.AccountBalance;
                                        LoadingProfile.Instance.progressBar.Value += 10.0;
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    Dictionary<string, BitmapImage> dictionary = new Dictionary<string, BitmapImage>();
                                    int num = 0;
                                    foreach (StationViewModel model in stations)
                                    {
                                        if (dictionary.Count >= 6)
                                        {
                                            break;
                                        }
                                        if (dictionary.ContainsKey(model.Name))
                                        {
                                            model.Name = model.Name + num;
                                        }
                                        dictionary.Add(model.Name, model.Available ? OnlineIcon : OfflineIcon);
                                    }
                                    this.LocationWidget.StationList.Images.ItemsSource = dictionary;
                                    this.NavigatePage();
                                    LoadingProfile.Instance.progressBar.Value += 10.0;
                                    Instance.LocationWidget.Visibility = Visibility.Hidden;
                                    Thread.Sleep(100);
                                    Switcher.Switch(Charging.Instance);
                                }));
                                if (this._dismissTimer != null)
                                {
                                    this._dismissTimer.Stop();
                                    this._dismissTimer = null;
                                }
                                this._dismissTimer = new System.Timers.Timer();
                                if (handler == null)
                                {
                                    handler = ((t,k)=>{
                                        Switcher.Switch(AdvertPlayer.Instance);
                                        Instance.Dispatcher.BeginInvoke(new Action(()=>
                                        {
                                            Instance.LocationWidget.Visibility = Visibility.Hidden;
                                        }));
                                        this._dismissTimer.Stop();
                                    });
                                }
                                this._dismissTimer.Elapsed += handler;
                                this._dismissTimer.Interval = milliseconds;
                                this._dismissTimer.Start();
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.StackTrace + "\n" + exception.Message);
                        }
                    }
                    catch (Exception exception2)
                    {
                        if (method == null)
                        {
                            method =new Action(()=>{
                                this.CustomerProfile.CustomerName.Content = "Invalid Customer";
                               
                            });
                        }
                        Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
                        Console.WriteLine(exception2.StackTrace + "\n" + exception2.Message);
                    }
                }
            });
        }

        private void ClientUpdateWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            
        }

        public static string CurrentRFID
        {
            get
            {
                return _currentRFID;
            }
            set
            {
                _currentRFID = value;
                if (Instance != null)
                {
                    Instance.ClearAllInfo();
                }
            }
        }

        public static Charging Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Charging();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public bool IsExpired
        {
            get
            {
                return this._isExpired;
            }
            set
            {
                this._isExpired = value;
            }
        }

        public int LayoutId
        {
            get
            {
                return this._layoutId;
            }
            set
            {
                this._layoutId = value;
            }
        }

    }
}

