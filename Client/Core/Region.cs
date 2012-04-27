using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Diagnostics;
using Client.Properties;
using eAd.DataViewModels;

namespace Client.Core
{
    //<summary>
    //A screen region control
    //</summary>
    class Region : Media, IDisposable
    {
        private BlackList blackList;
        public delegate void DurationElapsedDelegate();
        public event DurationElapsedDelegate DurationElapsedEvent;

        private Media media;
        private RegionOptions options;
        public bool hasExpired = false;
        public bool layoutExpired = false;
        private int currentSequence = -1;

        // Stat objects
        private StatLog _statLog;
        private Stat _stat;

        // Cache Manager
        private CacheManager _cacheManager;

        public Region(ref StatLog statLog, ref CacheManager cacheManager)
            : base(0, 0, 0, 0)
        {
            // Store the statLog
            _statLog = statLog;

            // Store the cache manager
            // TODO: What happens if the cachemanger changes during the lifecycle of this region?
            _cacheManager = cacheManager;

            //default options
            options.Width = 1024;
            options.Height = 768;
            options.Left = 0;
            options.Top = 0;
            options.Uri = null;

            this.Size = new Size(options.Width, options.Height);

            this.Location = new Point(options.Left, options.Top);



            //  this.Background = new SolidColorBrush(Colors.Red);

            if (Settings.Default.DoubleBuffering)
            {
                //   SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                //      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            }

            // Create a new BlackList for us to use
            blackList = new BlackList();
        }

        public RegionOptions RegionOptions
        {
            get
            {
                return this.options;
            }
            set
            {
                this.options = value;

                EvaluateOptions();
            }
        }

        ///<summary>
        /// Evaulates the change in options
        ///</summary>
        private void EvaluateOptions()
        {
            if (currentSequence == -1)
            {
                //evaluate the Width, etc
                this.Location = new Point(options.Left, options.Top);
                this.Size = new Size(options.Width, options.Height);
            }

            int temp = currentSequence;

            //set the next media node for this panel
            if (!SetNextMediaNode())
            {
                // For some reason we cannot set a media node... so we need this region to become invalid
                hasExpired = true;
                DurationElapsedEvent();
                return;
            }

            // If the sequence hasnt been changed, OR the layout has been expired
            if (currentSequence == temp || layoutExpired)
            {
                //there has been no change to the sequence, therefore the media we have already created is still valid
                //or this media has actually been destroyed and we are working out way out the call stack
                return;
            }
            var oldmedia = media;
            System.Diagnostics.Debug.WriteLine(String.Format("Creating new media: {0}, {1}", options.FileType, options.mediaid), "Region - EvaluateOptions");
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                {



                                                                                    try
                                                                                    {
                                                                                        switch (options.FileType)
                                                                                        {

                                                                                            case "Image":
                                                                                                options.Uri =
                                                                                                    Settings.Default.
                                                                                                        LibraryPath +
                                                                                                    @"\" + options.Uri;
                                                                                                media =
                                                                                                    new ImagePosition(
                                                                                                        options);
                                                                                                break;

                                                                                            case "Text":
                                                                                                media =
                                                                                                    new Text(options);
                                                                                                break;

                                                                                            case "Powerpoint":
                                                                                                options.Uri =
                                                                                                    Settings.Default.
                                                                                                        LibraryPath +
                                                                                                    @"\" + options.Uri;
                                                                                                media =
                                                                                                    new WebContent(
                                                                                                        options);
                                                                                                break;

                                                                                            case "Video":
                                                                                                options.Uri =
                                                                                                    Settings.Default.
                                                                                                        LibraryPath +
                                                                                                    @"\" + options.Uri;
                                                                                                media =
                                                                                                    new Video(options);
                                                                                                break;

                                                                                            case "Webpage":
                                                                                                media =
                                                                                                    new WebContent(
                                                                                                        options);
                                                                                                break;

                                                                                            case "Flash":
                                                                                                options.Uri =
                                                                                                    Settings.Default.
                                                                                                        LibraryPath +
                                                                                                    @"\" + options.Uri;
                                                                                                media =
                                                                                                    new Flash(options);
                                                                                                break;

                                                                                            case "Ticker":
                                                                                                media =
                                                                                                    new Rss(options);
                                                                                                break;

                                                                                            case "Embedded":
                                                                                                media =
                                                                                                    new Text(options);
                                                                                                break;

                                                                                            case "Datasetview":
                                                                                                media =
                                                                                                    new DataSetView(
                                                                                                        options);
                                                                                                break;

                                                                                            default:
                                                                                                //do nothing
                                                                                                SetNextMediaNode();
                                                                                                return;
                                                                                        }

                                                                                        //sets up the timer for this media
                                                                                        media.Duration =
                                                                                            options.Duration;

                                                                                        //add event handler
                                                                                        media.DurationElapsedEvent +=
                                                                                            new Media.
                                                                                                DurationElapsedDelegate
                                                                                                (media_DurationElapsedEvent);

                                                                                        Dispatcher.Invoke(
                                                                                            DispatcherPriority.Normal,
                                                                                            new ThreadStart(() =>
                                                                                                                {
                                                                                                                    media.Opacity = 0;
                                                                                                                    ;
                                                                                                                    media
                                                                                                                        .
                                                                                                                        Height
                                                                                                                        =
                                                                                                                        this
                                                                                                                            .
                                                                                                                            Height;
                                                                                                                    media
                                                                                                                        .
                                                                                                                        Width
                                                                                                                        =
                                                                                                                        this
                                                                                                                            .
                                                                                                                            Width;
                                                                                                                    media.Margin = new Thickness(0, 0, 0, 0);

                                                                                                                    media.RenderMedia();
                                                                                                                    media
                                                                                                                        .
                                                                                                                        OldOne
                                                                                                                        =
                                                                                                                        oldmedia;
                                                                                                                    if (!media.HasOnLoaded) //not so useful
                                                                                                                    {
                                                                                                                        media
                                                                                                                            .
                                                                                                                            AnimateIn
                                                                                                                            (MediaCanvas,
                                                                                                                            oldmedia);
                                                                                                                    }

                                                                                                                    //this
                                                                                                                    //    .
                                                                                                                    //    AddChild
                                                                                                                    //    (media);
                                                                                                                    MediaCanvas.Children.Add(media);
                                                                                                                }
                                                                                                ));
                                                                                        //any additional media specific render options (and starts the timer)

                                                                                    }
                                                                                    catch (Exception ex)
                                                                                    {
                                                                                        Trace.WriteLine(
                                                                                            new LogMessage(
                                                                                                "EvalOptions",
                                                                                                "Unable to start media. " +
                                                                                                ex.Message),
                                                                                            LogType.Error.ToString());
                                                                                        SetNextMediaNode();
                                                                                    }

                                                                                    // This media has started and is being replaced
                                                                                    _stat = new Stat();
                                                                                    _stat.FileType = StatType.Media;
                                                                                    _stat.FromDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                                                    _stat.ScheduleID = options.scheduleId;
                                                                                    _stat.LayoutID = options.layoutId;
                                                                                    _stat.MediaID = options.mediaid;
                                                                                }));
            System.Diagnostics.Debug.WriteLine("Showing new media", "Region - Eval Options");
        }

        protected Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        protected Point Location
        {
            get
            {
                return new Point(Margin.Left, Margin.Top);
            }
            set
            {
                this.Margin = new Thickness(value.X, value.Y, 0, 0);
            }
        }

        /// <summary>
        /// Sets the next media node. Should be used either from a mediaComplete event, or an options reset from 
        /// the parent.
        /// </summary>
        private bool SetNextMediaNode()
        {
            int playingSequence = currentSequence;

            // What if there are no media nodes?
            if (options.mediaNodes.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No media nodes to display", "Region - SetNextMediaNode");
                hasExpired = true;
                return false;
            }

            if (options.mediaNodes.Count == 1 && currentSequence != -1)
            {
                //dont bother discarding this media, keep all the same details, but still trigger an expired event
                System.Diagnostics.Debug.WriteLine("Media Expired:" + options.ToString() + " . Nothing else to show", "Region - SetNextMediaNode");
                hasExpired = true;

                DurationElapsedEvent();
                return true;
            }

            // Move the sequence on
            currentSequence++;

            if (currentSequence >= options.mediaNodes.Count)
            {
                currentSequence = 0; //zero it

                hasExpired = true; //we have expired (want to raise an expired event to the parent)

                System.Diagnostics.Debug.WriteLine("Media Expired:" + options.ToString() + " . Reached the end of the sequence. Starting from the beginning.", "Region - SetNextMediaNode");

                DurationElapsedEvent();

                // We want to continue on to show the next media (unless the duration elapsed event triggers a region change)
                if (layoutExpired) return true;
            }

            //Zero out the options that are persisted
            options.text = "";
            options.documentTemplate = "";
            options.copyrightNotice = "";
            options.scrollSpeed = 30;
            options.updateInterval = 6;
            options.Uri = "";
            options.direction = "none";
            options.javaScript = "";
            options.Dictionary = new MediaDictionary();

            // Get a media node
            bool validNode = false;
            int numAttempts = 0;

            while (!validNode)
            {
                numAttempts++;

                // Get the media node for this sequence
                LayoutRegionMedia medium = options.mediaNodes[currentSequence];



                if (medium.Id != null) options.mediaid = medium.Id;

                // Check isnt blacklisted
                if (blackList.BlackListed(options.mediaid))
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("The File [{0}] has been blacklisted", options.mediaid), "Region - SetNextMediaNode");

                    // Increment and Loop
                    currentSequence++;

                    if (currentSequence >= options.mediaNodes.Count)
                    {
                        currentSequence = 0; //zero it

                        hasExpired = true; //we have expired (want to raise an expired event to the parent)

                        System.Diagnostics.Debug.WriteLine("Media Expired:" + options.ToString() + " . Reached the end of the sequence. Starting from the beginning.", "Region - SetNextMediaNode");

                        DurationElapsedEvent();

                        // We want to continue on to show the next media (unless the duration elapsed event triggers a region change)
                        if (layoutExpired) return true;
                    }
                }
                else
                {
                    validNode = true;

                    // New version has a different schema - the right way to do it would be to pass the <options> and <raw> nodes to 
                    // the relevant media class - however I dont feel like engineering such a change so the alternative is to
                    // parse all the possible media type nodes here.

                    // Type and Duration will always be on the media node
                    options.FileType = medium.Type;

                    //TODO: Check the type of node we have, and make sure it is supported.

                    if (medium.Duration != -1)
                    {
                        options.Duration = medium.Duration;
                    }
                    else
                    {
                        options.Duration = 60;
                        System.Diagnostics.Trace.WriteLine("Duration is Empty, using a default of 60.",
                                                           "Region - SetNextMediaNode");
                    }

                    // We cannot have a 0 duration here... not sure why we would... but
                    if (options.Duration == 0 && options.FileType != "Video")
                    {
                        int emptyLayoutDuration = int.Parse(Settings.Default.emptyLayoutDuration.ToString());
                        options.Duration = (emptyLayoutDuration == 0) ? 10 : emptyLayoutDuration;
                    }

                    // There will be some stuff on option nodes

                    options.direction = medium.Options.Direction;
                    // Add this to the options object
                    options.Dictionary.Add("direction", medium.Options.Direction);
                    options.Uri = medium.Options.Uri;
                    options.Dictionary.Add("Uri", medium.Options.Uri);
                    options.copyrightNotice = medium.Options.Copyright;
                    options.Dictionary.Add("copyrightNotice", medium.Options.Copyright);
                    options.scrollSpeed = medium.Options.ScrollSpeed;
                    options.Dictionary.Add("ScrollSpeed", (medium.Options.ScrollSpeed).ToString());
                    if (medium.Options.ScrollSpeed == -1)
                    {
                        System.Diagnostics.Trace.WriteLine("Non integer scrollSpeed in XLF", "Region - SetNextMediaNode");
                    }
                    options.updateInterval = medium.Options.UpdateInterval;
                    if (medium.Options.UpdateInterval == -1)
                    {
                        System.Diagnostics.Trace.WriteLine("Non integer updateInterval in XLF",
                                                           "Region - SetNextMediaNode");
                    }

                    // And some stuff on Raw nodes

                    if (medium.Raw != null)
                    {
                        if (!String.IsNullOrEmpty(medium.Raw.Text))
                        {
                            options.text = medium.Raw.Text;
                        }

                    if (!String.IsNullOrEmpty(medium.Raw.Text))
                    {
                        options.documentTemplate = medium.Raw.DocumentTemplate;
                    }
                    if (!String.IsNullOrEmpty(medium.Raw.EmbedHtml))
                    {
                        options.text = medium.Raw.EmbedHtml;
                    }

                    if (!String.IsNullOrEmpty(medium.Raw.EmbedScript))
                    {
                        options.javaScript = medium.Raw.EmbedScript;
                    }

                }
                // Is this a file based media node?
                    if (options.FileType == "Video" || options.FileType == "flash" || options.FileType == "Image" || options.FileType == "powerpoint")
                    {
                        // Use the cache manager to determine if the file is valid
                        validNode = _cacheManager.IsValidPath(options.Uri);
                    }
                }

                if (numAttempts > options.mediaNodes.Count)
                {
                    // There are no valid nodes in this region, so just signify that the region is ending, and show nothing.
                    System.Diagnostics.Trace.WriteLine("No Valid media nodes to display", "Region - SetNextMediaNode");

                    hasExpired = true;
                    return false;
                }
            }

            System.Diagnostics.Debug.WriteLine("New media detected " + options.FileType, "Region - SetNextMediaNode");

            // Remove the old one if we have found a valid node - otherwise keep it
            if ((validNode && playingSequence != -1) && playingSequence != currentSequence)
            {
                System.Diagnostics.Debug.WriteLine("Trying to dispose of the current media", "Region - SetNextMediaNode");
                // Dispose of the current media
                try
                {
                    //       media.AnimateAway(MediaCanvas);
                    ThreadPool.QueueUserWorkItem((r) =>
                                                        {
                                                            Thread.Sleep(2500);
                                                            if (media!=null &&media.Parent != null)
                                                            {
                                                                if (media.OldOne != null)
                                                                    media.OldOne.AnimateAway(MediaCanvas);
                                                            }
                                                            else
                                                            {
                                                                if (media != null&&media.OldOne != null)
                                                                    media.OldOne.AnimateAway(MediaCanvas);
                                                                if (media != null) 
                                                                    media.AnimateAway(MediaCanvas);
                                                            }
                                                        });

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("No media to remove");
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                try
                {
                    // Here we say that this media is expired
                    _stat.ToDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    // Record this stat event in the statLog object
                    _statLog.RecordStat(_stat);
                }
                catch
                {
                    System.Diagnostics.Trace.WriteLine("No Stat record when one was expected", LogType.Error.ToString());
                }
            }

            return true;
        }

        public void AnimateIn()
        {
            if (CheckAccess())
            {
                DoubleAnimation ani = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(1000)));



                ani.Completed += delegate
                {


                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                    {
                        Opacity = 1.0;
                    }));
                };
                this.BeginAnimation(OpacityProperty, ani);
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                {
                    DoubleAnimation ani = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(250)));

                    ani.Completed += delegate
                    {


                        Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                        {


                            Opacity = 1.0;

                        }));
                    };
                    this.BeginAnimation(Control.OpacityProperty, ani);
                }));

            }
        }

        /// <summary>
        /// The media has elapsed
        /// </summary>
        void media_DurationElapsedEvent()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Media Elapsed: {0}", options.Uri), "Region - DurationElapsedEvent");

            // make some decisions about what to do next
            EvaluateOptions();
        }

        /// <summary>
        /// Clears the Region of anything that it shouldnt still have... 
        /// </summary>
        public void Clear()
        {
            try
            {
                // What happens if we are disposing this region but we have not yet completed the stat event?
                if (String.IsNullOrEmpty(_stat.ToDate))
                {
                    // Say that this media has ended
                    _stat.ToDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    // Record this stat event in the statLog object
                    _statLog.RecordStat(_stat);
                }
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine(new LogMessage("Region - Clear", "Error closing off stat record"), LogType.Error.ToString());
            }
        }

        /// <summary>
        /// Performs the disposal.
        /// </summary>
        public void Dispose()
        {

            {
                try
                {
                    if (media != null)
                    {
                           media.Dispose();
                    media = null;
                    }
                     

                    System.Diagnostics.Debug.WriteLine("Media Disposed by Region", "Region - Dispose");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine("There was no media to dispose", "Region - Dispose");
                }
                finally
                {
                    if (media != null) 
                        media = null;
                }
            }


        }
    }
}
