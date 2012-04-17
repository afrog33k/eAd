using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Client.Core;
using Client.Properties;
using eAd.DataViewModels;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using MessageBox = System.Windows.MessageBox;
using Region = Client.Core.Region;
using Size = System.Drawing.Size;
using TextBox = System.Windows.Controls.TextBox;
using Timer = System.Timers.Timer;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainForm.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        public MainForm()
        {
            InitializeComponent();

            // Check the directories exist
            if (!Directory.Exists(Settings.Default.LibraryPath) || !Directory.Exists(Settings.Default.LibraryPath + @"\backgrounds\"))
            {
                // Will handle the create of everything here
                Directory.CreateDirectory(Settings.Default.LibraryPath + @"\backgrounds");
                Directory.CreateDirectory(Settings.Default.LibraryPath + @"\Layouts");
                Directory.CreateDirectory(Settings.Default.LibraryPath + @"\Uploads\Media");
            }

            // Hide the cursor
            //    Cursor.Position = new Point(_clientSize.Width, _clientSize.Height);

            //     if (!Settings.Default.EnableMouse)
            //  Cursor.Hide();

            ShowSplashScreen();

            // Change the default Proxy class
            OptionForm.SetGlobalProxy();

            // UserApp data
            Debug.WriteLine(new LogMessage("MainForm_Load", "User AppData Path: " + App.UserAppDataPath), LogType.Info.ToString());
   

            // Override the default size if necessary
            if (Settings.Default.sizeX != 0)
            {
                _clientSize = new System.Drawing.Size((int)Settings.Default.sizeX, (int)Settings.Default.sizeY);
           //     Size = _clientSize;
            //    WindowState = FormWindowState.Normal;
            //    Location = new System.Drawing.Point((int)Settings.Default.offsetX, (int)Settings.Default.offsetY);
            //    StartPosition = FormStartPosition.Manual;
            }
            else
            {
                _clientSize = new Size((int) SystemParameters.PrimaryScreenWidth, (int) SystemParameters.PrimaryScreenHeight);
                this.Visibility = Visibility.Visible;
                this.Width = _clientSize.Width ;
                this.Height = _clientSize.Height;
             

               
            }

            // Setup the proxy information
            OptionForm.SetGlobalProxy();

            _statLog = new StatLog();

            this.Closing  +=  (MainFormFormClosing);
            
            MainFormShown();
        }

         
        private Schedule _schedule;
        private Collection<Region> _regions;
        private bool _isExpired = false;
        private int _scheduleId;
        private int _layoutId;

        double _layoutWidth;
        double _layoutHeight;
        double _scaleFactor;
        private System.Drawing.Size _clientSize;

        private StatLog _statLog;
        private Stat _stat;
        private CacheManager _cacheManager;

      

        /// <summary>
        /// Called after the form has been shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainFormShown()
        {
          


                // Process any stuff that has happened during the loading process
                App.DoEvents();

                // Create a cachemanager
                SetCacheManager();

                try
                {
                    // Create the Schedule
                    _schedule = new Schedule(App.UserAppDataPath + "\\" + Settings.Default.ScheduleFile,
                                             ref _cacheManager);

                    // Bind to the schedule change event - notifys of changes to the schedule
                    _schedule.ScheduleChangeEvent += new Schedule.ScheduleChangeDelegate(schedule_ScheduleChangeEvent);

                    // Initialize the other schedule components
                    _schedule.InitializeComponents();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message, LogType.Error.ToString());
                    MessageBox.Show("Fatal Error initialising the application", "Fatal Error");
                    Close();
                  //  Dispose();
                }
            
        }

      

        private void MainFormFormClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            // We want to tidy up some stuff as this form closes.

            // Flush the stats
            _statLog.Flush();

            // Write the CacheManager to disk
            _cacheManager.WriteCacheManager();

            // Flush the logs
            System.Diagnostics.Trace.Flush();
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
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(CacheManager));

                    _cacheManager = (CacheManager)xmlSerializer.Deserialize(fileStream);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new LogMessage("MainForm - SetCacheManager", "Unable to reuse the Cache Manager because: " + ex.Message));

                // Create a new cache manager
                _cacheManager = new CacheManager();
            }

            try
            {
                _cacheManager.Regenerate();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new LogMessage("MainForm - SetCacheManager", "Regenerate failed because: " + ex.Message));
            }
        }

        /// <summary>
        /// Handles the ScheduleChange event
        /// </summary>
        /// <param name="layoutPath"></param>
        void schedule_ScheduleChangeEvent(string layoutPath, int scheduleId, int layoutId)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Schedule Changing to {0}", layoutPath), "MainForm - ScheduleChangeEvent");

            _scheduleId = scheduleId;
            _layoutId = layoutId;

            if (_stat != null)
            {
                // Log the end of the currently running layout.
                _stat.ToDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Record this stat event in the statLog object
                _statLog.RecordStat(_stat);
            }

            try
            {
                DestroyLayout();

                _isExpired = false;

                PrepareLayout(layoutPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _isExpired = true;

                ShowSplashScreen();

                // In 10 seconds fire the next layout?
                Timer timer = new Timer();
                timer.Interval = 10000;
                timer.Elapsed += new ElapsedEventHandler(splashScreenTimer_Tick);

                // Start the timer
                timer.Start();
            }
        }

        void splashScreenTimer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine(new LogMessage("timer_Tick", "Loading next layout after splashscreen"));

            Timer timer = (Timer)sender;
            timer.Stop();
            timer.Dispose();

            _schedule.NextLayout();
        }

        /// <summary>
        /// Prepares the Layout.. rendering all the necessary controls
        /// </summary>
        private void PrepareLayout(string layoutPath)
        {
            // Create a start record for this layout
            _stat = new Stat();
            _stat.FileType = StatType.Layout;
            _stat.ScheduleID = _scheduleId;
            _stat.LayoutID = _layoutId;
            _stat.FromDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            LayoutModel layout;

            // Default or not
            if (layoutPath == Settings.Default.LibraryPath + @"\Default.xml" || String.IsNullOrEmpty(layoutPath))
            {
                throw new Exception("Default layout");
            }
            else
            {
                try
                {

               
            // Get this layouts XML
            using (var file = File.Open(layoutPath, FileMode.Open, FileAccess.Read, FileShare.Write))
            {


                XmlSerializer serializer = new XmlSerializer(typeof (LayoutModel));
            layout=    (LayoutModel) serializer.Deserialize(file);

            }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Could not find the layout file {0}: {1}", layoutPath, ex.Message));
                    throw;
                }
            }

            // Attributes of the main layout node
       
         

            // Set the background and size of the form
            _layoutWidth = layout.Width;
            _layoutHeight =layout.Height;


            // Scaling factor, will be applied to all regions
            _scaleFactor = Math.Min(_clientSize.Width / _layoutWidth, _clientSize.Height / _layoutHeight);

            // Want to be able to center this shiv - therefore work out which one of these is going to have left overs
            int backgroundWidth = (int)(_layoutWidth * _scaleFactor);
            int backgroundHeight = (int)(_layoutHeight * _scaleFactor);

            double leftOverX;
            double leftOverY;

            try
            {
                leftOverX = Math.Abs(_clientSize.Width - backgroundWidth);
                leftOverY = Math.Abs(_clientSize.Height - backgroundHeight);

                if (leftOverX != 0) leftOverX = leftOverX / 2;
                if (leftOverY != 0) leftOverY = leftOverY / 2;
            }
            catch
            {
                leftOverX = 0;
                leftOverY = 0;
            }

            // New region and region options objects
            _regions = new Collection<Region>();
            RegionOptions options = new RegionOptions();

            // Deal with the color
            try
            {
                if (!String.IsNullOrEmpty(layout.Bgcolor))
                {
                  Dispatcher.BeginInvoke(DispatcherPriority.Normal,new ThreadStart(()=>
                                                                                       {
                                                                                           MediaCanvas.Background =
                                                                                               new SolidColorBrush(
                                                                                                   (Color)
                                                                                                   ColorConverter.
                                                                                                       ConvertFromString
                                                                                                       (layout.Bgcolor));
                                                                                       }));
                    options.backgroundColor = layout.Bgcolor;
                }
            }
            catch
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                {
                    MediaCanvas.Background = new SolidColorBrush(Colors.Black); // Default black
                }));

             
                options.backgroundColor = "#000000";
            }

            // Get the background
            try
            {
                if (layout.Background == null)
                {
                    // Assume there is no background image
                    MediaCanvas.Background = null;
                    options.backgroundImage = "";
                }
                else
                {

                    string bgFilePath = Settings.Default.LibraryPath + @"\backgrounds\" + backgroundWidth + "x" + backgroundHeight + "_" + layout.Background;
                    Utilities.CreateFolder(Path.GetDirectoryName(bgFilePath));
                    // Create a correctly sized background image in the temp folder
                    if (!File.Exists(bgFilePath))
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(Settings.Default.LibraryPath + @"\" + layout.Background);

                        Bitmap bmp = new Bitmap(img, backgroundWidth, backgroundHeight);
                        EncoderParameters encoderParameters = new EncoderParameters(1);
                        EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                        encoderParameters.Param[0] = qualityParam;

                        ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

                        bmp.Save(bgFilePath, jpegCodec, encoderParameters);

                        img.Dispose();
                        bmp.Dispose();
                    }

                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                          {

                                                                                              MediaCanvas.Background = new ImageBrush(
                                                                                                                                                                        new BitmapImage(
                                                                                                                                                                            new Uri(
                                                                                                                                                                                
                                                                                                                                                                               bgFilePath.Replace("\\","/"),UriKind.Relative)));
                    }));
                 
                    options.backgroundImage = bgFilePath;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to set background: " + ex.Message);

                // Assume there is no background image
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                {
                    MediaCanvas.Background = Brushes.Black;
                }));
             
                options.backgroundImage = "";
            }

            // Get it to paint the background now
            App.DoEvents();

            // Get the regions
            var listRegions = layout.Regions;
            var listMedia = layout.Regions.Select(r=>r.Media).ToList();

            // Check to see if there are any regions on this layout.
            if (listRegions.Count == 0 || listMedia.Count == 0)
            {
                Trace.WriteLine(new LogMessage("PrepareLayout",
                    string.Format("A layout with {0} regions and {1} media has been detected.", listRegions.Count.ToString(), listMedia.Count.ToString())),
                    LogType.Info.ToString());

                if (_schedule.ActiveLayouts == 1)
                {
                    Trace.WriteLine(new LogMessage("PrepareLayout", "Only 1 layout scheduled and it has nothing to show."), LogType.Info.ToString());

                    throw new Exception("Only 1 layout schduled and it has nothing to show");
                }
                else
                {
                    Trace.WriteLine(new LogMessage("PrepareLayout",
                        string.Format(string.Format("An empty layout detected, will show for {0} seconds.", Settings.Default.emptyLayoutDuration.ToString()))), LogType.Info.ToString());

                    // Put a small dummy region in place, with a small dummy media node - which expires in 10 seconds.
                    // Replace the list of regions (they mean nothing as they are empty)
                    listRegions = new List<LayoutRegion>()
                                      {
                                          new LayoutRegion()
                                              {
                                                  Id="blah", Width = 1, Height = 1, Top = 1,Left = 1,
                                                  Media = new List<LayoutRegionMedia>()
                                                              {
                                                                  new LayoutRegionMedia()
                                                                      {
                                                                          Id =   "blah",
                                                                          Type = "Text",
                                                                          Duration = 0,
                                                                          Raw = new LayoutRegionMediaRaw()
                                                                                    {
                                                                                        Text = ""
                                                                                    }
                                                                      }
                                                              }

                                              }
                                      };
                }
            }

            foreach (var region in listRegions)
            {
                // Is there any media
                if (region.Media.Count == 0)
                {
                    Debug.WriteLine("A region with no media detected");
                    continue;
                }

                //each region
           

                options.scheduleId = _scheduleId;
                options.layoutId = _layoutId;
                options.regionId = region.Id;
                options.Width = (int)(region.Width * _scaleFactor);
                options.Height = (int)(region.Height * _scaleFactor);
                options.Left = (int)(region.Left * _scaleFactor);
                options.Top = (int)(region.Top * _scaleFactor);
                options.ScaleFactor = _scaleFactor;

                // Set the backgrounds (used for Web content offsets)
                options.BackgroundLeft = options.Left * -1;
                options.BackgroundTop = options.Top * -1;

                //Account for scaling
                options.Left = options.Left + (int)leftOverX;
                options.Top = options.Top + (int)leftOverY;

                // All the media nodes for this region / layout combination
                options.mediaNodes = region.Media;

                Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(
                                                                    () =>
                                                                    {
                Region temp = new Region(ref _statLog, ref _cacheManager);
                temp.DurationElapsedEvent += new Region.DurationElapsedDelegate(TempDurationElapsedEvent);

                Debug.WriteLine("Created new region", "MainForm - Prepare Layout");

                // Dont be fooled, this innocent little statement kicks everything off
                temp.RegionOptions = options;

                _regions.Add(temp);

                                                                        temp.Opacity = 0;

                                                                        MediaCanvas.Children.Add(temp);

                                                                        temp.AnimateIn();
          
//          new TextBox(){
//Text                                                                        = "Hey",
//                                                                          Margin = new Thickness(options.left,options.top,0,0),
//                                                                          Height = options.Height,
//                                                                          Width = options.Width
//                                                                        })
                                                                        ;
   //   temp.Background = new SolidColorBrush(Colors.Coral);
                                                                          }));
          

                Debug.WriteLine("Adding region", "MainForm - Prepare Layout");

            
                App.DoEvents();
            }

            // Null stuff
            listRegions = null;
            listMedia = null;
        }

        /// <summary>
        /// Shows the splash screen (set the background to the embedded resource)
        /// </summary>
        private void ShowSplashScreen()
        {
            // We are running with the Default.xml - meaning the schedule doesnt exist
        //    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
         //   Stream resourceStream = assembly.GetManifestResourceStream("XiboClient.Resources.splash.jpg");

            Debug.WriteLine("Showing Splash Screen");

            // Load into a stream and then into an Image
            try
            {
           //     System.Drawing.Image bgSplash = System.Drawing.Image.FromStream(resourceStream);
                //ImageBrush myBrush = new ImageBrush();
                //Image image = new Image();
                //image.Source = new BitmapImage(
                //    new Uri(
                //       "pack://application:,,,/MyClassLibrary;component/Images/myImage.jpg"));
                //myBrush.ImageSource = image.Source;
                //Grid grid = new Grid();
                //grid.Background = myBrush;    
         //       Bitmap bmpSplash = new Bitmap(bgSplash, _clientSize);
                Dispatcher.Invoke(DispatcherPriority.Input, new ThreadStart(() =>
                {

                BitmapImage splash = new BitmapImage();
splash.BeginInit();
                splash.UriSource = //new Uri("Resources/w0000114.jpg", UriKind.Relative);
     new Uri("pack://application:,,,/Client;component/Resources/w0000114.jpg");
splash.EndInit();


    MediaCanvas.Background = new ImageBrush(splash);

}));

             

        //        bgSplash.Dispose();
            }
            catch (Exception ex)
            {
                // Log
                Debug.WriteLine("Failed Showing Splash Screen: " + ex.Message);
            }
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        }

        /// <summary>
        /// The duration of a Region has been reached
        /// </summary>
        void TempDurationElapsedEvent()
        {
            Debug.WriteLine("Region Elapsed", "MainForm - DurationElapsedEvent");

            _isExpired = true;
            
            // Check the other regions to see if they are also expired.
            foreach (Region temp in _regions)
            {
                if (!temp.hasExpired)
                {
                    _isExpired = false;
                }
            }

            if (_isExpired)
            {
                // Inform each region that the layout containing it has expired
                foreach (Region temp in _regions)
                {
                    temp.layoutExpired = true;
                }

                System.Diagnostics.Debug.WriteLine("Region Expired - Next Region.", "MainForm - DurationElapsedEvent");
                _schedule.NextLayout();
            }

            App.DoEvents();
        }

        /// <summary>
        /// Disposes Layout - removes the controls
        /// </summary>
        private void DestroyLayout() 
        {
            System.Diagnostics.Debug.WriteLine("Destroying Layout", "MainForm - DestoryLayout");

            App.DoEvents();

            if (_regions == null) return;

            for (int i = 0; i < _regions.Count; i++)
            {
                
            
                Region region = _regions[i];
                region.Clear();

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                {
                    MediaCanvas.Children.Remove(region);
                }));
         

                try
                {
                    System.Diagnostics.Debug.WriteLine("Calling Dispose Region", "MainForm - DestoryLayout");
                    region.Dispose();
                }
                catch (Exception e)
                {
                    //do nothing (perhaps write to some error xml somewhere?)
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            _regions.Clear();
            _regions = null;
        }

        /// <summary>
        /// Force a flush of the stats log
        /// </summary>
        public void FlushStats()
        {
            try
            {
                _statLog.Flush();
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine(new LogMessage("MainForm - FlushStats", "Unable to Flush Stats"), LogType.Error.ToString());
            }
        }
    }
    }

