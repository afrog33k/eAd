namespace ClientApp
{
    using ClientApp.Core;
    using ClientApp.Properties;
    using eAd.DataViewModels;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using System.Xml.Serialization;


    public partial class AdvertPlayer : UserControl, IComponentConnector, IPausableControl
    {
        private System.Drawing.Size _clientSize;

        private static AdvertPlayer _instance;
        private bool _isExpired;
        private double _layoutHeight;
        private int _layoutId;
        private double _layoutWidth;
        private Collection<ClientApp.Core.Region> _regions;
        private double _scaleFactor;


        public AdvertPlayer()
        {
            Instance = this;
            this.InitializeComponent();
            if (Settings.Default.sizeX != 0M)
            {
                this._clientSize = new System.Drawing.Size((int)Settings.Default.sizeX, (int)Settings.Default.sizeY);
            }
            else
            {
                this._clientSize = new System.Drawing.Size((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);
                base.Visibility = Visibility.Visible;
                base.Width = this._clientSize.Width;
                base.Height = this._clientSize.Height;
            }
            App.DoEvents();
        }

        public void DestroyLayout()
        {
            App.DoEvents();

            base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                                                                                  {
                                                                                      //Remove Background
                                                                                      this.MediaCanvas.Background =
                                                                                          null;
                                                                                  }));

            if (this._regions != null)
            {
                if (this._regions.Count == 0)
                {
                    this._regions = null;
                }
                else
                {
                    for (int i = 0; i < this._regions.Count; i++)
                    {
                        ClientApp.Core.Region region = this._regions[i];
                        region.Clear();
                        base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                        {
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
                    this._regions.Clear();
                    this._regions = null;

                  
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



        public void PrepareLayout(string layoutPath)
        {
            Action method = null;
            Action start2 = null;
            Action start3 = null;
            Action<RegionOptions> action = null;
            LayoutModel layout;
            ClientManager.Instance.Stat = new ClientApp.Core.Stat();
            ClientManager.Instance.Stat.FileType = StatType.Layout;
            ClientManager.Instance.Stat.ScheduleID = ClientManager.Instance.ScheduleId;
            ClientManager.Instance.Stat.LayoutID = this._layoutId;
            ClientManager.Instance.Stat.FromDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if ((layoutPath == (Settings.Default.LibraryPath + @"\Default.xml")) || string.IsNullOrEmpty(layoutPath))
            {
                throw new Exception("Default layout");
            }
            try
            {
                using (FileStream stream = File.Open(layoutPath, FileMode.Open, FileAccess.Read, FileShare.Write))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LayoutModel));
                    layout = (LayoutModel)serializer.Deserialize(stream);
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
            this._regions = new Collection<ClientApp.Core.Region>();
            RegionOptions options = new RegionOptions();
            try
            {
                if (!string.IsNullOrEmpty(layout.Bgcolor))
                {
                    if (method == null)
                    {
                        method = new Action(() =>
                        {
                            this.MediaCanvas.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(layout.Bgcolor));
                        });
                    }
                    base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
                    options.backgroundColor = layout.Bgcolor;
                }
            }
            catch
            {
                if (start2 == null)
                {
                    start2 = new Action(() =>
                    {
                        this.MediaCanvas.Background = new SolidColorBrush(Colors.Black);
                    });
                }
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, start2);
                options.backgroundColor = "#000000";
            }
            try
            {
                if (layout.Background == null)
                {
                    this.MediaCanvas.Background = null;
                    options.backgroundImage = "";
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
                    base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        this.MediaCanvas.Background = new ImageBrush(new BitmapImage(new Uri(bgFilePath.Replace(@"\", "/"), UriKind.Relative)));
                    }));
                    options.backgroundImage = bgFilePath;
                }
            }
            catch (Exception)
            {
                if (start3 == null)
                {
                    start3 = new Action(() =>
                    {
                        this.MediaCanvas.Background = System.Windows.Media.Brushes.Black;
                    });
                }
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, start3);
                options.backgroundImage = "";
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
                LayoutRegion region = new LayoutRegion
                {
                    Id = "blah",
                    Width = 1,
                    Height = 1,
                    Top = 1,
                    Left = 1
                };
                List<LayoutRegionMedia> list4 = new List<LayoutRegionMedia>();
                LayoutRegionMedia media = new LayoutRegionMedia
                {
                    Id = "blah",
                    Type = "Text",
                    Duration = 0
                };
                LayoutRegionMediaRaw raw = new LayoutRegionMediaRaw
                {
                    Text = ""
                };
                media.Raw = raw;
                list4.Add(media);
                region.Media = list4;
                list3.Add(region);
                regions = list3;
            }
            else
            {// Keep Running region, preventing visual blank out;
                regions.Add(
                    new LayoutRegion()
                        {
                            Height = 0,Width = 0,Name = "Keep Alive Region", Media = new List<LayoutRegionMedia>()
                                                                                         {
                                                                                             new LayoutRegionMedia()
                                                                                                 {
                                                                                                     Duration = (int) TimeSpan.FromDays(10).TotalMilliseconds,
                                                                                                     Type = "KeepAlive"
                                                                                                     
                                                                                                 }
                                                                                         }
                        });
            }

            foreach (LayoutRegion region2 in regions)
            {
                if (region2.Media.Count != 0)
                {
                    options.scheduleId = ClientManager.Instance.ScheduleId;
                    options.layoutId = this._layoutId;
                    options.regionId = region2.Id;
                    options.Width = (int)((((double)(region2.Width + 14)) / this._layoutWidth) * this._clientSize.Width);
                    options.Height = (int)((((double)(region2.Height + 14)) / this._layoutHeight) * this._clientSize.Height);
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
                    options.Left = (int)((((double)left) / this._layoutWidth) * this._clientSize.Width);
                    options.Top = (int)((((double)top) / this._layoutHeight) * this._clientSize.Height);
                    options.ScaleFactor = this._scaleFactor;
                    options.BackgroundLeft = options.Left * -1;
                    options.BackgroundTop = options.Top * -1;
                    options.mediaNodes = region2.Media;
                    if (action == null)
                    {
                        action = delegate(RegionOptions opts)
                        {
                            Core.Region region = new ClientApp.Core.Region(ClientManager.Instance.StatLog, ClientManager.Instance.CacheManager);
                            region.DurationElapsedEvent += new ClientApp.Core.Region.DurationElapsedDelegate(this.TempDurationElapsedEvent);
                            if (opts.FileType != "Widget")
                            {
                                region.RegionOptions = opts;
                                this._regions.Add(region);
                                region.Opacity = 0.0;
                                this.MediaCanvas.Children.Add(region);
                                region.AnimateIn();
                            }
                            else if (!WidgetsFactory.Widgets.ContainsKey(opts.Name))
                            {
                                region.RegionOptions = opts;
                                region.Opacity = 0.0;
                                this.MediaCanvas.Children.Add(region);
                                region.AnimateIn();
                            }
                        };
                    }
                    base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action, options);
                    App.DoEvents();
                }
            }
            regions = null;
            list2 = null;
        }

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

                    if(region.CurrentMedia!=null)
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

        public  void UnPause()
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
                  if(region.CurrentMedia!=null)
                  region.CurrentMedia.UnPause();
                  region.EvaluateOptions();
              }
          }

      }


        private void TempDurationElapsedEvent()
        {
            try
            {

          
            this._isExpired = true;
            foreach (ClientApp.Core.Region region in this._regions)
            {
                if (!region.hasExpired)
                {
                    this._isExpired = false;
                }
            }
            if (this._isExpired)
            {
                foreach (ClientApp.Core.Region region2 in this._regions)
                {
                    region2.layoutExpired = true;
                }
                lock (Instance)
                {

                }
                lock (ScheduleLock)
                    ClientManager.Instance.Schedule.NextLayout("AdvertPlayer");
            }
            App.DoEvents();
            }
            catch (Exception)
            {

              
            }
        }

        object ScheduleLock = new object();

        public static AdvertPlayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AdvertPlayer();
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

