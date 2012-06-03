using System.Collections.Generic;
using ClientApp.Players;

namespace ClientApp.Core
{
    using ClientApp;
    using ClientApp.Properties;
    using eAd.DataViewModels;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;

    internal class Region : Media, IDisposable
    {
        private CacheManager _cacheManager;
        private ClientApp.Core.Stat _stat;
        private StatLog _statLog;
        private BlackList blackList;
        private int currentSequence;
      
        public bool hasExpired;
        public bool layoutExpired;
        public Media CurrentMedia;
        private ClientApp.Core.RegionOptions options;
        public bool Paused = false;

        public event DurationElapsedDelegate DurationElapsedEvent;

        public Region(StatLog statLog, CacheManager cacheManager) : base(0, 0, 0, 0)
        {
            this.currentSequence = -1;
            this._statLog = statLog;
            this._cacheManager = cacheManager;
            this.options.Width = 0x400;
            this.options.Height = 0x300;
            this.options.Left = 0;
            this.options.Top = 0;
            this.options.Uri = null;
            this.Size = new System.Windows.Size((double) this.options.Width, (double) this.options.Height);
            this.Location = new Point((double) this.options.Left, (double) this.options.Top);
            base.Background = new SolidColorBrush(Colors.Transparent);
            bool doubleBuffering = Settings.Default.DoubleBuffering;
            this.blackList = new BlackList();
        }

    

        public void AnimateIn()
        {
            EventHandler handler = null;
            Action method = null;
            if (base.CheckAccess())
            {
                DoubleAnimation animation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(1000.0)));
                if (handler == null)
                {
                    handler =new EventHandler((r,t)=>{
                        base.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(()=>{
                            base.Opacity = 1.0;
                        }));
                    });
                }
                animation.Completed += handler;
                base.BeginAnimation(UIElement.OpacityProperty, animation);
            }
            else
            {
                if (method == null)
                {
                    method =new Action(()=>{
                        DoubleAnimation animation = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(250.0)));
                        animation.Completed +=new EventHandler((a,b)=>{
                                                                       base.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(()=>{
                                                                                                                                                base.Opacity = 1.0;
                                                                       }));
                        });
                        base.BeginAnimation(UIElement.OpacityProperty, animation);
                    });
                }
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
            }
        }

        public void Clear()
        {
            try
            {
                if ((this._stat != null) && string.IsNullOrEmpty(this._stat.ToDate))
                {
                    this._stat.ToDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    this._statLog.RecordStat(this._stat);
                }
            }
            catch
            {
                Trace.WriteLine(new LogMessage("Region - Clear", "Error closing off stat record"), LogType.Error.ToString());
            }
        }

        public void Dispose()
        {
            try
            {
                if (this.CurrentMedia != null)
                {
                    this.CurrentMedia.Dispose();
                    this.CurrentMedia = null;
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (this.CurrentMedia != null)
                {
                    this.CurrentMedia = null;
                }
            }
        }

        public  Media OldMedia;

        public void EvaluateOptions()
        {
            if (!Paused)
            {
                Action start = null;
                OldMedia = CurrentMedia;
                if (this.currentSequence == -1)
                {
                    this.Location = new Point((double) this.options.Left, (double) this.options.Top);
                    this.Size = new System.Windows.Size((double) this.options.Width, (double) this.options.Height);
                }
                int currentSequence = this.currentSequence;
                if ((this.options.FileType != "Widget") && !this.SetNextMediaNode())
                {
                    this.hasExpired = true;
                    this.DurationElapsedEvent();
                }

                else if (((this.currentSequence != currentSequence) && !this.layoutExpired) ||
                         (this.options.FileType == "Widget"))
                {
                    try
                    {
                        if (start == null)
                        {
                            start = new Action(() =>
                                                   {
                                                       Action<Media> method = null;
                                                       ClientApp.Core.RegionOptions options = this.RegionOptions;
                                                       Console.WriteLine("type: " + options.FileType);
                                                       switch (options.FileType)
                                                       {
                                                           case "Image":
                                                               options.Uri = Settings.Default.LibraryPath + @"\" +
                                                                             options.Uri;
                                                               this.CurrentMedia = new Picture(options);
                                                               break;

                                                           case "Marquee":
                                                               options.Uri = Settings.Default.LibraryPath + @"\" +
                                                                             options.Uri;
                                                               this.CurrentMedia = new Text(options);
                                                               break;

                                                           case "Powerpoint":
                                                               options.Uri = Settings.Default.LibraryPath + @"\" +
                                                                             options.Uri;
                                                               this.CurrentMedia = new WebContent(options);
                                                               break;

                                                           case "Video":
                                                               options.Uri = Settings.Default.LibraryPath + @"\" +
                                                                             options.Uri;
                                                               this.CurrentMedia = new Video(options);
                                                               break;

                                                           case "Webpage":
                                                               this.CurrentMedia = new WebContent(options);
                                                               break;

                                                           case "Flash":
                                                               options.Uri = Settings.Default.LibraryPath + @"\" +
                                                                             options.Uri;
                                                               this.CurrentMedia = new Flash(options);
                                                               break;

                                                           case "Ticker":
                                                               this.CurrentMedia = new Rss(options);
                                                               break;

                                                           case "Embedded":
                                                               this.CurrentMedia = new Text(options);
                                                               break;

                                                           case "Datasetview":
                                                               this.CurrentMedia = new DataSetView(options);
                                                               break;

                                                           case "Widget":
                                                               this.CurrentMedia = WidgetsFactory.CreateFrom(options);
                                                               break;

                                                           case "KeepAlive":
                                                               this.CurrentMedia = new KeepAlive(options);
                                                               break;

                                                           default:
                                                               this.SetNextMediaNode();
                                                               return;
                                                       }
                                                       if (this.CurrentMedia != null)
                                                       {
                                                           this.CurrentMedia.Duration = options.Duration;
                                                           this.CurrentMedia.DurationElapsedEvent +=
                                                               new Media.DurationElapsedDelegate(
                                                                   this.MediaDurationElapsedEvent);
                                                           this.CurrentMedia.Height = base.Height;
                                                           this.CurrentMedia.Width = base.Width;
                                                           this.CurrentMedia.Margin = new Thickness(0.0, 0.0, 0.0,
                                                                                                    0.0);
                                                           this.CurrentMedia.RenderMedia();
                                                           if (method == null)
                                                           {
                                                               method = delegate(Media med)
                                                                            {
                                                                                med.AnimateIn(this.MediaCanvas,
                                                                                              OldMedia);
                                                                                if (med != null)
                                                                                {
                                                                                    base.MediaCanvas.Children.Add(
                                                                                        med);
                                                                                    if (med.SnapShot != null)
                                                                                    {
                                                                                        base.MediaCanvas.Background
                                                                                            =
                                                                                            new ImageBrush(
                                                                                                med.
                                                                                                    GetRenderTargetBitmap
                                                                                                    (1.0));
                                                                                    }
                                                                                }
                                                                                if (OldMedia != null)
                                                                                {

                                                                                    OldMedia.Dispose();
                                                                                    OldMedia = null;
                                                                                }
                                                                            };
                                                           }
                                                           base.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                                                       method, this.CurrentMedia);
                                                       }
                                                       else
                                                       {
                                                           Trace.WriteLine(new LogMessage("EvalOptions",
                                                                                          "Unable to start media. media == null" +
                                                                                          LogType.Error.ToString()));
                                                           this.SetNextMediaNode();
                                                       }
                                                   });
                        }
                        base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, start);
                    }
                    catch (Exception exception)
                    {
                        Trace.WriteLine(
                            new LogMessage("EvalOptions", "Unable to start media. " + exception.Message),
                            LogType.Error.ToString());
                        this.SetNextMediaNode();
                    }
                    ClientApp.Core.Stat stat = new ClientApp.Core.Stat
                                                   {
                                                       FileType = StatType.Media,
                                                       FromDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                                       ScheduleID = this.options.scheduleId,
                                                       LayoutID = this.options.layoutId,
                                                       MediaID = this.options.mediaid
                                                   };
                    this._stat = stat;
                }
            }
        }

        private void MediaDurationElapsedEvent()
        {
           
            this.EvaluateOptions();
        }

        private bool SetNextMediaNode()
        {
            WaitCallback callBack = null;
            int currentSequence = this.currentSequence;
            if (this.options.mediaNodes.Count == 0)
            {
                this.hasExpired = true;
                return false;
            }
            if ((this.options.mediaNodes.Count == 1) && (this.currentSequence != -1))
            {
                this.hasExpired = true;
                this.DurationElapsedEvent();
                return true;
            }
            this.currentSequence++;
            if (this.currentSequence >= this.options.mediaNodes.Count)
            {
                this.currentSequence = 0;
                this.hasExpired = true;
                this.DurationElapsedEvent();
                if (this.layoutExpired)
                {
                    return true;
                }
            }
            this.options.text = "";
            this.options.documentTemplate = "";
            this.options.copyrightNotice = "";
            this.options.scrollSpeed = 30;
            this.options.updateInterval = 6;
            this.options.Uri = "";
            this.options.direction = "none";
            this.options.javaScript = "";
            this.options.Dictionary = new MediaDictionary();
            bool flag = false;
            int num2 = 0;
            while (!flag)
            {
                num2++;
                LayoutRegionMedia media = this.options.mediaNodes[this.currentSequence];
                if (media.Id != null)
                {
                    this.options.mediaid = media.Id;
                }
                if (this.blackList.BlackListed(this.options.mediaid))
                {
                    this.currentSequence++;
                    if (this.currentSequence >= this.options.mediaNodes.Count)
                    {
                        this.currentSequence = 0;
                        this.hasExpired = true;
                        this.DurationElapsedEvent();
                        if (this.layoutExpired)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    flag = true;
                    this.options.FileType = media.Type;
                    if (media.Duration != -1)
                    {
                        this.options.Duration = media.Duration;
                    }
                    else
                    {
                        this.options.Duration = 60;
                        Trace.WriteLine("Duration is Empty, using a default of 60.", "Region - SetNextMediaNode");
                    }
                    if ((this.options.Duration == 0) && (this.options.FileType != "Video"))
                    {
                        int num3 = int.Parse(Settings.Default.emptyLayoutDuration.ToString());
                        this.options.Duration = (num3 == 0) ? 10 : num3;
                    }
                    if (media.Options != null)
                    {
                        this.options.direction = media.Options.Direction;
                        this.options.Dictionary.Add("direction", media.Options.Direction);
                        this.options.Uri = media.Options.Uri;
                        this.options.Dictionary.Add("Uri", media.Options.Uri);
                        this.options.copyrightNotice = media.Options.Copyright;
                        this.options.Dictionary.Add("copyrightNotice", media.Options.Copyright);
                        this.options.scrollSpeed = media.Options.ScrollSpeed;
                        if (media.Options != null)
                        {
                            this.options.Dictionary.Add("ScrollSpeed", media.Options.ScrollSpeed.ToString());
                        }
                        if (media.Options.ScrollSpeed == -1)
                        {
                            Trace.WriteLine("Non integer scrollSpeed in XLF", "Region - SetNextMediaNode");
                        }
                        this.options.updateInterval = media.Options.UpdateInterval;
                        if (media.Options.UpdateInterval == -1)
                        {
                            Trace.WriteLine("Non integer updateInterval in XLF", "Region - SetNextMediaNode");
                        }
                    }
                    if (media.Raw != null)
                    {
                        if (!string.IsNullOrEmpty(media.Raw.Text))
                        {
                            this.options.text = media.Raw.Text;
                        }
                        if (!string.IsNullOrEmpty(media.Raw.Text))
                        {
                            this.options.documentTemplate = media.Raw.DocumentTemplate;
                        }
                        if (!string.IsNullOrEmpty(media.Raw.EmbedHtml))
                        {
                            this.options.text = media.Raw.EmbedHtml;
                        }
                        if (!string.IsNullOrEmpty(media.Raw.EmbedScript))
                        {
                            this.options.javaScript = media.Raw.EmbedScript;
                        }
                    }
                    if (((this.options.FileType == "Video") || (this.options.FileType == "flash")) || ((this.options.FileType == "Image") || (this.options.FileType == "powerpoint")))
                    {
                        flag = this._cacheManager.IsValidPath(this.options.Uri);
                    }
                }
                if (num2 > this.options.mediaNodes.Count)
                {
                    Trace.WriteLine("No Valid media nodes to display", "Region - SetNextMediaNode");
                    this.hasExpired = true;
                    return false;
                }
            }
            if ((flag && (currentSequence != -1)) && (currentSequence != this.currentSequence))
            {
                try
                {
                    if (callBack == null)
                    {
                        callBack = delegate (object r) {
                      
                        };
                    }
                    ThreadPool.QueueUserWorkItem(callBack);
                }
                catch (Exception)
                {
                }
                try
                {
                    this._stat.ToDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    this._statLog.RecordStat(this._stat);
                }
                catch
                {
                    Trace.WriteLine("No Stat record when one was expected", LogType.Error.ToString());
                }
            }
            return true;
        }

        protected Point Location
        {
            get
            {
                return new Point(base.Margin.Left, base.Margin.Top);
            }
            set
            {
                base.Margin = new Thickness(value.X, value.Y, 0.0, 0.0);
            }
        }

        public ClientApp.Core.RegionOptions RegionOptions
        {
            get
            {
                return this.options;
            }
            set
            {
                this.options = value;
                this.EvaluateOptions();
            }
        }

        protected System.Windows.Size Size
        {
            get
            {
                return new System.Windows.Size(base.Width, base.Height);
            }
            set
            {
                base.Width = value.Width;
                base.Height = value.Height;
            }
        }

        public delegate void DurationElapsedDelegate();
    }
}

