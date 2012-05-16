using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using Client;
using ClientApp.Core;
using ClientApp.Properties;
using eAd.DataViewModels;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Region = ClientApp.Core.Region;
using Size = System.Drawing.Size;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for Charging.xaml
    /// </summary>
    public partial class Charging : UserControl
    {
        #region Fields (15)


        private Size _clientSize;
        private static Charging _instance;
        private bool _isExpired = false;
        double _layoutHeight;
        private int _layoutId;
        double _layoutWidth;
        private Collection<Region> _regions;
        double _scaleFactor;


        #endregion Fields

        #region Constructors (1)

        public Charging()
        {
            Instance = this;
            InitializeComponent();

            // UserApp data
            Debug.WriteLine(new LogMessage("Charging Load", "User AppData Path: " + App.UserAppDataPath), LogType.Info.ToString());


            // Override the default size if necessary
            if (Settings.Default.sizeX != 0)
            {
                _clientSize = new Size((int)Settings.Default.sizeX, (int)Settings.Default.sizeY);

            }
            else
            {
                _clientSize = new Size((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);
                this.Visibility = Visibility.Visible;
                this.Width = _clientSize.Width;
                this.Height = _clientSize.Height;
            }


            // Process any stuff that has happened during the loading process
            App.DoEvents();




        }

        #endregion Constructors

        #region Properties (1)

        public static Charging Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Charging();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public int LayoutId
        {
            get
            {
                return _layoutId;
            }
            set
            {
                _layoutId = value;
            }
        }

        public bool IsExpired
        {
            get
            {
                return _isExpired;
            }
            set
            {
                _isExpired = value;
            }
        }

        #endregion Properties

        #region Methods (9)

        // Public Methods (1) 

        /// <summary>
        /// Force a flush of the stats log
        /// </summary>
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
        // Private Methods (8) 

        /// <summary>
        /// Disposes Layout - removes the controls
        /// </summary>
        public void DestroyLayout()
        {
            Debug.WriteLine("Destroying Layout", "AdvertPlayer - DestoryLayout");

            App.DoEvents();

            if (_regions == null) return;

            if (_regions.Count == 0)
            {
                _regions = null;
                return;
            }

            for (int i = 0; i < _regions.Count; i++)
            {
                Region region = _regions[i];
                region.Clear();

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                {
                    if (region != null)
                        MediaCanvas.Children.Remove(region);
                }));


                try
                {
                    Debug.WriteLine("Calling Dispose Region", "AdvertPlayer - DestoryLayout");
                    region.Dispose();
                }
                catch (Exception e)
                {
                    //do nothing (perhaps write to some error xml somewhere?)
                    Debug.WriteLine(e.Message);
                }
            }

            _regions.Clear();
            _regions = null;
        }

        private void FormFadeOutCompleted(object sender, EventArgs e)
        {

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
        /// Prepares the Layout.. rendering all the necessary controls
        /// </summary>
        public void PrepareLayout(string layoutPath)
        {
            // Create a start record for this layout
          //  ClientManager.Instance.Stat = new Stat();
           // ClientManager.Instance.Stat.FileType = StatType.Layout;
         //   ClientManager.Instance.Stat.ScheduleID = ClientManager.Instance.ScheduleId;
        //    ClientManager.Instance.Stat.LayoutID = _layoutId;
         //   ClientManager.Instance.Stat.FromDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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


                        XmlSerializer serializer = new XmlSerializer(typeof(LayoutModel));
                        layout = (LayoutModel)serializer.Deserialize(file);

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
            _layoutHeight = layout.Height;


            //// Scaling factor, will be applied to all regions
            //_scaleFactor = Math.Max(_clientSize.Width / _layoutWidth, _clientSize.Height / _layoutHeight);

            //// Want to be able to center this shiv - therefore work out which one of these is going to have left overs
            int backgroundWidth = _clientSize.Width;// (int)(_layoutWidth * _scaleFactor);
            int backgroundHeight = _clientSize.Height;//(int)(_layoutHeight * _scaleFactor);

            //double leftOverX;
            //double leftOverY;

            //try
            //{
            //    leftOverX = Math.Abs(_clientSize.Width - backgroundWidth);
            //    leftOverY = Math.Abs(_clientSize.Height - backgroundHeight);

            //    if (leftOverX != 0) leftOverX = leftOverX / 2;
            //    if (leftOverY != 0) leftOverY = leftOverY / 2;
            //}
            //catch
            //{
            //    leftOverX = 0;
            //    leftOverY = 0;
            //}


            // New region and region options objects
            _regions = new Collection<Region>();
            RegionOptions options = new RegionOptions();

            // Deal with the color
            try
            {
                if (!String.IsNullOrEmpty(layout.Bgcolor))
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                    {
                        MediaCanvas.Background =
                        new SolidColorBrush((Color)ColorConverter.ConvertFromString(layout.Bgcolor));
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

                        MediaCanvas.Background = new ImageBrush(new BitmapImage(new Uri(bgFilePath.Replace("\\", "/"), UriKind.Relative)));
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
            var listMedia = layout.Regions.Select(r => r.Media).ToList();

            // Check to see if there are any regions on this layout.
            if (listRegions.Count == 0 || listMedia.Count == 0)
            {
                Trace.WriteLine(new LogMessage("PrepareLayout",
                                               string.Format("A layout with {0} regions and {1} media has been detected.", listRegions.Count.ToString(), listMedia.Count.ToString())),
                                LogType.Info.ToString());

                if (ClientManager.Instance.Schedule.ActiveLayouts == 1)
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
                    if(region.Type!="Widget")
                    {
                           Debug.WriteLine("A region with no media detected");
                           continue;
                    }
                 
                }

                //each region
                options.scheduleId = ClientManager.Instance.ScheduleId;
                options.layoutId = _layoutId;
                options.regionId = region.Id;
                options.Name = region.Name;
                options.FileType = region.Type;
                options.Width = (int)((region.Width + 30.0) / _layoutWidth * _clientSize.Width); //(int)((region.Width + 15.0) * _scaleFactor);
                options.Height = (int)((region.Height + 30) / _layoutHeight * _clientSize.Height);//(int)((region.Height + 15.0) * _scaleFactor);
                var left = region.Left - 15;
                if (left < 0)
                    left = 0;

                var top = region.Top - 15;
                if (top < 0)
                    top = 0;

                options.Left = (int)(left / _layoutWidth * _clientSize.Width);//(int)(region.Left * _scaleFactor);
                options.Top = (int)(top / _layoutHeight * _clientSize.Height); //(int)(region.Top * _scaleFactor);

                options.ScaleFactor = _scaleFactor;

                // Set the backgrounds (used for Web content offsets)
                options.BackgroundLeft = options.Left * -1;
                options.BackgroundTop = options.Top * -1;

                //Account for scaling
                //       options.Left = options.Left + (int)leftOverX;
                //        options.Top = options.Top + (int)leftOverY;

                // All the media nodes for this region / layout combination
                options.mediaNodes = region.Media;

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<RegionOptions>(
                                           (opts) =>
                                           {

                                               Region temp = new Region(ClientManager.Instance.StatLog, ClientManager.Instance.CacheManager);
                                               temp.DurationElapsedEvent += new Region.DurationElapsedDelegate(TempDurationElapsedEvent);

                                               Debug.WriteLine("Created new region", "Charging Player - Prepare Layout");

                                               // Dont be fooled, this innocent little statement kicks everything off
                                               temp.RegionOptions = opts;

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
                                           }), options);


                Debug.WriteLine("Adding region", "Charging - Prepare Layout");


                App.DoEvents();
            }

            // Null stuff
            listRegions = null;
            listMedia = null;
        }



        /// <summary>
        /// The duration of a Region has been reached
        /// </summary>
        void TempDurationElapsedEvent()
        {
            Debug.WriteLine("Region Elapsed", "Charging - DurationElapsedEvent");

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

                System.Diagnostics.Debug.WriteLine("Region Expired - Next Region.", "Charging - DurationElapsedEvent");
                ClientManager.Instance.Schedule.NextLayout();
            }

            App.DoEvents();
        }

        #endregion Methods
    }
}

