using System.Windows.Threading;
using ClientApp.Service;

namespace ClientApp
{
    using ClientApp.Core;
    using ClientApp.Properties;

    using ClientApp.Update;
    using eAd.DataViewModels;
    using eAd.Utilities;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Timers;
    using System.Xml.Serialization;

    public class Schedule
    {
        private CacheManager _cacheManager;
        private int _currentLayout;
        private bool _forceChange;
        private HardwareKey _hardwareKey;
        private Collection<LayoutSchedule> _layoutSchedule;
        private string _scheduleLocation;
        private ClientApp.ScheduleManager _scheduleManager;
        private readonly UpdateMe _updater = new UpdateMe();
        private ServiceClient _xmds2;
        private bool _xmdsProcessing;
        private object CheckingForUpdate = new object();
        private object LayoutChangeLock = new object();
        protected bool LoadedAtleast1LayoutAlready;
        protected bool ShouldAlwaysRefresh1Layout;
        private static bool UpdateRunning;

        public event ScheduleChangeDelegate ScheduleChangeEvent;
        public Schedule(string scheduleLocation, CacheManager cacheManager)
        {
            this._scheduleLocation = scheduleLocation;
            this._layoutSchedule = new Collection<LayoutSchedule>();
            this._cacheManager = cacheManager;
            this._scheduleManager = new ClientApp.ScheduleManager(this._cacheManager, scheduleLocation);
            this._xmds2 = new ServiceClient();
        }

        private void fileCollector_MediaFileChanged(string path)
        {
        }

        private void FileCollectorCollectionComplete()
        {
            this._xmds2.ScheduleAsync(Settings.Default.ServerKey, this._hardwareKey.Key, Settings.Default.Version);
        }

        private void FileCollectorLayoutFileChanged(string layoutPath)
        {
            if (Settings.Default.expireModifiedLayouts)
            {
                try
                {
                    if (this._layoutSchedule[this._currentLayout].LayoutFile == (Settings.Default.LibraryPath + @"\" + layoutPath))
                    {
                        this._forceChange = true;
                        this.NextLayout("");
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(new LogMessage("fileCollector_LayoutFileChanged", string.Format("Unable to determine current layout with exception {0}", exception.Message)), LogType.Error.ToString());
                }
            }
        }
        
        public void InitializeComponents()
        {
            this._hardwareKey = new HardwareKey();
            this._xmdsProcessing = false;
            this._xmds2.RequiredFilesCompleted += new EventHandler<RequiredFilesCompletedEventArgs>(this.Xmds2RequiredFilesCompleted);
            this._xmds2.ScheduleCompleted += new EventHandler<ScheduleCompletedEventArgs>(this.xmds2_ScheduleCompleted);
            Trace.WriteLine(string.Format("Collection Interval: {0}", Settings.Default.collectInterval), "Schedule - InitializeComponents");
           new Thread(() =>
                                             {
                                                 while(true)
                                                 {
                                                     XmdsTimerTick(null,null);
                                                     Thread.Sleep(1000);
                                                 }
                                             }).Start();

            //System.Timers.Timer timer = new System.Timers.Timer
            //{
            //    Interval = 1000.0
            //};
            //timer.Elapsed += new ElapsedEventHandler(this.XmdsTimerTick);
            //timer.Start();

           new Thread(() =>
           {
               while (true)
               {
                   ScheduleTimerTick(null, null);
                   Thread.Sleep(10000);
               }
           }).Start();

            //System.Timers.Timer timer2 = new System.Timers.Timer
            //{
            //    Interval = 10000.0
            //};
            //timer2.Elapsed += new ElapsedEventHandler(this.ScheduleTimerTick);
            //timer2.Start();
            this._xmdsProcessing = true;
            this.UpdateLayoutSchedule(true);
            if (!CollectingFiles)
            {
                CollectingFiles = true;  
                this._xmds2.RequiredFilesAsync(Settings.Default.ServerKey, this._hardwareKey.Key, Settings.Default.Version);
           
            }
          
        }

        public void NextLayout(string player)
        {
            //    lock (this.LayoutChangeLock)
            {
                this._currentLayout++;
                if (this._currentLayout >= this._layoutSchedule.Count)
                {
                    this._currentLayout = 0;
                }
                if (this._layoutSchedule.Count == 1)
                {
                    bool flag1 = this._forceChange;
                }
                this._forceChange = false;
                this.ScheduleChangeEvent(this._layoutSchedule[this._currentLayout].LayoutFile, this._layoutSchedule[this._currentLayout].Scheduleid, this._layoutSchedule[this._currentLayout].ID, player);
                this.LoadedAtleast1LayoutAlready = true;
                //     Monitor.Pulse(LayoutChangeLock);
            }
        }

        private void ScheduleTimerTick(object sender, EventArgs e)
        {
            this.UpdateLayoutSchedule(this._scheduleManager.NewScheduleAvailable);
        }

        private void UpdateLayoutSchedule(bool forceChange)
        {
            this._layoutSchedule = this._scheduleManager.CurrentSchedule;
            if (forceChange)
            {
                this._currentLayout = 0;
                this.ScheduleChangeEvent(this._layoutSchedule[0].LayoutFile, this._layoutSchedule[0].Scheduleid, this._layoutSchedule[0].ID, "");
            }
        }

        private void xmds2_ScheduleCompleted(object sender, ScheduleCompletedEventArgs e)
        {
            this._xmdsProcessing = false;
            if (e.Error != null)
            {
                Trace.WriteLine(e.Error.Message);
            }
            else
            {
                lock (ScheduleWriteLock)
                {
                    string str = "";
                    Settings.Default.XmdsLastConnection = DateTime.Now;
                    XmlSerializer serializer = new XmlSerializer(typeof (ScheduleModel));
                    MemoryStream stream = new MemoryStream();
                    serializer.Serialize((Stream) stream, e.Result);
                    string str2 = Hashes.MD5(Encoding.UTF8.GetString(stream.ToArray()));
                    try
                    {
                        StreamReader reader =
                            new StreamReader(File.Open(this._scheduleLocation, FileMode.Open, FileAccess.ReadWrite,
                                                       FileShare.ReadWrite));
                        str = Hashes.MD5(reader.ReadToEnd());
                        reader.Close();
                        if (str == str2)
                        {
                            return;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    FileStream stream2 = File.OpenWrite(this._scheduleLocation);
                    byte[] buffer = stream.ToArray();
                    stream2.Write(buffer, 0, buffer.Length);
                    stream2.Close();
                    stream.Close();
                    this._scheduleManager.RefreshSchedule = true;
                }
            }
        }

        private bool CollectingFiles = false;
        private void Xmds2RequiredFilesCompleted(object sender, RequiredFilesCompletedEventArgs e)
        {
          
                if (e.Error != null)
                {
                    Trace.WriteLine(new LogMessage("Schedule - RequiredFilesCompleted", e.Error.Message),
                                    LogType.Error.ToString());
                    if (e.Error.Message == "This display client is not licensed")
                    {
                        Settings.Default.licensed = 0;
                    }
                    this._xmdsProcessing = false;
                }
                else
                {
                    Settings.Default.XmdsLastConnection = DateTime.Now;
                    if (Settings.Default.licensed == 0)
                    {
                        Settings.Default.licensed = 1;
                    }
                    try
                    {
                        FileCollector collector = new FileCollector(this._cacheManager, e.Result);
                        collector.LayoutFileChanged +=
                            this.FileCollectorLayoutFileChanged;
                        collector.CollectionComplete +=
                            this.FileCollectorCollectionComplete;
                        collector.MediaFileChanged +=
                            this.fileCollector_MediaFileChanged;
                        collector.CompareAndCollect();
                    }
                    catch (Exception exception)
                    {
                        this._xmdsProcessing = false;
                        Trace.WriteLine(
                            new LogMessage("Schedule - RequiredFilesCompleted",
                                           "Error Comparing and Collecting: " + exception.Message),
                            LogType.Error.ToString());
                    }
                    this._cacheManager.WriteCacheManager();
                }
                CollectingFiles = false;
               
        }

        private FileCollector collector;
        private object ScheduleWriteLock= new object();

        private void XmdsTimerTick(object sender, EventArgs e)
        {
            DateTime xmdsLastConnection = Settings.Default.XmdsLastConnection;
            int xmdsResetTimeout = Settings.Default.xmdsResetTimeout;
            if (xmdsLastConnection < DateTime.Now.AddSeconds((double)(-1 * xmdsResetTimeout)))
            {
                Trace.WriteLine(new LogMessage("xmdsTimer_Tick", string.Format("XMDS reset, last connection was at {0}", xmdsLastConnection.ToString())), LogType.Error.ToString());
                this._xmdsProcessing = false;
            }
            if (!this._xmdsProcessing)
            {
                App.DoEvents();
                this._xmdsProcessing = true;
                if(!CollectingFiles)
                {
                         this._xmds2.RequiredFilesAsync(Settings.Default.ServerKey, this._hardwareKey.Key, Settings.Default.Version);
                }
           
                ThreadPool.QueueUserWorkItem(delegate(object p)
                {
                    try
                    {
                        lock (this.CheckingForUpdate)
                        {
                            if (!UpdateRunning)
                            {
                                UpdateRunning = true;
                                this._updater.CheckForUpdate();
                                UpdateRunning = false;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                });
                Trace.Flush();
            }
        }

        public int ActiveLayouts
        {
            get
            {
                return this._layoutSchedule.Count;
            }
        }

        public ClientApp.ScheduleManager ScheduleManager
        {
            get
            {
                return this._scheduleManager;
            }
        }

        public delegate void ScheduleChangeDelegate(string layoutPath, int scheduleId, int layoutId, string player);
    }
}

