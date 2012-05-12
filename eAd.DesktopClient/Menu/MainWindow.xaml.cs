namespace DesktopClient.Menu

{

using DesktopClient;

using DesktopClient.Controls;

using DesktopClient.eAdDataAccess;

using eAd.DataViewModels;

using eAd.Utilities;

using System;

using System.CodeDom.Compiler;

using System.Collections.Generic;

using System.ComponentModel;

using System.Diagnostics;

using System.IO;

using System.Linq;

using System.Runtime.CompilerServices;

using System.Runtime.InteropServices;

using System.Threading;

using System.Timers;

using System.Windows;

using System.Windows.Controls;

using System.Windows.Markup;

using System.Windows.Media;

using System.Windows.Media.Animation;

using System.Windows.Threading;

using System.Xml.Serialization;

public partial class MainWindow : UserControl, IComponentConnector

{



    private static MainWindow _instance;

    private static volatile bool _isPlaying = false;

    private static Duration _lastDuration = new Duration(TimeSpan.Zero);

    private List<MediaElement> _mediaElements = new List<MediaElement>();



    private double CurrentDuration;

    private int currentItem;

    internal Grid DropDownBar;



    public static volatile object FileLock = new object();





    private List<Player> Players = new List<Player>();

    private List<MediaListModel> Playlist;



    private System.Timers.Timer timer = new System.Timers.Timer();



    public MainWindow()

    {

        Instance = this;

        this.InitializeComponent();

        this.StatusBox.Visibility = Visibility.Collapsed;

        this.MediaGrid.Width = SystemParameters.PrimaryScreenWidth;

        this.MediaGrid.Height = SystemParameters.PrimaryScreenHeight;

        this.MediaCanvas.Width = this.MediaGrid.Width;

        this.MediaCanvas.Height = this.MediaGrid.Height;

        this.eAdWindow.Width = this.MediaGrid.Width;

        this.eAdWindow.Height = this.MediaGrid.Height;

    }



    [DebuggerNonUserCode]




    private void _timer_Elapsed(object sender, ElapsedEventArgs e)

    {

        this.timer.Enabled = false;

    }



    private void FormFadeOutCompleted(object sender, EventArgs e)

    {

    }



    private static string GetPositionFileName(PositionViewModel position)

    {

        return string.Concat(new object[] { Path.GetFileNameWithoutExtension(Constants.PlayListFile), "mosaic", position.MosaicID, "posn", position.PositionID, ".xml" });

    }



    [DebuggerNonUserCode]





    private void LoadPosition(PositionViewModel position)

    {

        Func<long, MediaListModel> selector = null;

        PositionViewModel model = position;

        MediaElement element = new MediaElement
        {

            LoadedBehavior = MediaState.Manual,

            Stretch = Stretch.Fill

        };

        if (model.Width.HasValue)

        {

            double? nullable4 = model.Width + 15.0;

            element.Width = (nullable4.Value / 768.0) * SystemParameters.PrimaryScreenWidth;

        }

        if (model.Height.HasValue)

        {

            double? nullable8 = model.Height + 15.0;

            element.Height = (nullable8.Value / 1366.0) * SystemParameters.PrimaryScreenHeight;

        }

        if (model.X.HasValue && model.Y.HasValue)

        {

            element.Margin = new Thickness((model.X.Value / 768.0) * SystemParameters.PrimaryScreenWidth, (model.Y.Value / 1366.0) * SystemParameters.PrimaryScreenHeight, 0.0, 0.0);

        }

        this.Playlist = new List<MediaListModel>();

        ServiceClient myService1 = new ServiceClient("BasicHttpBinding_IService", Constants.ServerAddress);

        try

        {

            if (selector == null)

            {

                selector = media => new MediaListModel { MediaID = media, DisplayLocation = myService1.GetMediaLocation(media), Duration = myService1.GetMediaDuration(media) };

            }

            List<MediaListModel> list = position.Media; // position.Media.Select<long, MediaListModel>(selector).ToList<MediaListModel>();

            myService1.Close();

            List<MediaListModel> playlist = (from m in list select new MediaListModel { MediaID = m.MediaID, DisplayLocation = m.DisplayLocation, Duration = m.Duration }).ToList<MediaListModel>();

            this.SavePositionMedia(position, playlist);

            this.MediaCanvas.Children.Add(element);

            Player player = new Player(this.Playlist, element);

            foreach (MediaListModel model2 in list.ToArray())

            {

                PageSwitcher.DownloadMedium(model2, this.Playlist, player);

            }

            this.Players.Add(player);

            player.Start();

        }

        catch (Exception exception)

        {

            Console.WriteLine(exception.StackTrace + "\n\n" + exception.Message);

        }

    }



    private IEnumerable<MediaListModel> LoadPositionMedia(PositionViewModel position)

    {

        List<MediaListModel> list = new List<MediaListModel>();

        try

        {

            string positionFileName = GetPositionFileName(position);

            XmlSerializer serializer = new XmlSerializer(typeof(List<MediaListModel>));

            StreamReader textReader = File.OpenText(positionFileName);

            list = serializer.Deserialize(textReader) as List<MediaListModel>;

            textReader.Close();

        }

        catch (Exception)

        {

            list = new List<MediaListModel>();

        }

        return list;

    }



    public void LoadPositions(PositionViewModel[] positions = null)

    {

        ThreadPool.QueueUserWorkItem( (e) =>
        {

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(()=>
            {

                List<Player> players = Players;

                this.Players =
                new List<Player>();

                if (positions == null)

                {

                    foreach (
                        PositionViewModel
                        model in
                        PageSwitcher.
                        Positions)

                    {

                        this.LoadPosition(
                            model);

                    }

                }

                else

                {

                    foreach (
                        PositionViewModel
                        model2 in
                        positions)

                    {

                        this.LoadSavedPosition
                        (model2);

                    }

                }

                try

                {

                    foreach (
                        Player player in
                        players)

                    {

                        player.Stop();

                        player.Quit();

                        this.MediaCanvas.
                        Children.Remove(
                            player.Control);

                    }

                }

                catch (Exception)

                {

                }

            }));

        });

    }



    private void LoadSavedPosition(PositionViewModel position)

    {

        PositionViewModel model = position;

        MediaElement element = new MediaElement
        {

            LoadedBehavior = MediaState.Manual,

            Stretch = Stretch.Fill

        };

        if (model.Width.HasValue)

        {

            double? nullable4 = model.Width + 15.0;

            element.Width = (nullable4.Value / 768.0) * SystemParameters.PrimaryScreenWidth;

        }

        if (model.Height.HasValue)

        {

            double? nullable8 = model.Height + 15.0;

            element.Height = (nullable8.Value / 1366.0) * SystemParameters.PrimaryScreenHeight;

        }

        if (model.X.HasValue && model.Y.HasValue)

        {

            element.Margin = new Thickness((model.X.Value / 768.0) * SystemParameters.PrimaryScreenWidth, (model.Y.Value / 1366.0) * SystemParameters.PrimaryScreenHeight, 0.0, 0.0);

        }

        this.Playlist = new List<MediaListModel>();

        IEnumerable<MediaListModel> enumerable = this.LoadPositionMedia(position);

        this.MediaCanvas.Children.Add(element);

        Player player = new Player(this.Playlist, element);

        foreach (MediaListModel model2 in enumerable)

        {

            PageSwitcher.DownloadMedium(model2, this.Playlist, player);

        }

        this.Players.Add(player);

        player.Start();

    }



    private void PlayerMediaEnded(object sender, RoutedEventArgs e)

    {

    }



    private void PlayerUnloaded(object sender, RoutedEventArgs e)

    {

    }



    private void SavePositionMedia(PositionViewModel position, List<MediaListModel> playlist)

    {

        List<MediaListModel> thisPlaylist = playlist;

        ThreadPool.QueueUserWorkItem(delegate (object r)
        {

            lock (FileLock)

            {

                string fileName = GetPositionFileName(position);

                XmlSerializer serializer = new XmlSerializer(typeof(List<MediaListModel>));

                FileInfo info = new FileInfo(fileName);

                if (!File.Exists(fileName))

                {

                    StreamWriter writer = info.CreateText();

                    serializer.Serialize((TextWriter) writer, thisPlaylist);

                    writer.Close();

                }

                else

                {

                    info.Delete();

                    StreamWriter writer2 = info.CreateText();

                    serializer.Serialize((TextWriter) writer2, thisPlaylist);

                    writer2.Close();

                }

            }

        });

    }



    private void StatusBox_TextChanged(object sender, TextChangedEventArgs e)

    {

    }



    [DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]




    public void UpdatePlayList()

    {

        WaitCallback callBack = null;

        lock (this.Playlist)

        {

            foreach (Player player in this.Players)

            {

                player.Playlist = PageSwitcher.Playlist;

            }

            this.Playlist = PageSwitcher.Playlist;

            if (callBack == null)

            {

                callBack = delegate (object state)
                {

                    bool flag = false;

                    while (!flag)

                    {

                        Thread.Sleep(0x3e8);

                        flag = true;

                        foreach (MediaListModel model in PageSwitcher.Playlist)

                        {

                            if (!model.Downloaded)

                            {

                                flag = false;

                            }

                        }

                    }

                    foreach (Player player in this.Players)

                    {

                        player.PlayNextVideo();

                    }

                };

            }

            ThreadPool.QueueUserWorkItem(callBack);

            ThreadPool.QueueUserWorkItem(delegate (object r)
            {

                lock (FileLock)

                {

                    XmlSerializer serializer = new XmlSerializer(typeof(List<MediaListModel>));

                    FileInfo info = new FileInfo(Constants.PlayListFile);

                    if (!File.Exists(Constants.PlayListFile))

                    {

                        StreamWriter writer = info.CreateText();

                        serializer.Serialize((TextWriter) writer, PageSwitcher.Playlist);

                        writer.Close();

                    }

                    else

                    {

                        info.Delete();

                        StreamWriter writer2 = info.CreateText();

                        serializer.Serialize((TextWriter) writer2, PageSwitcher.Playlist);

                        writer2.Close();

                    }

                }

            });

        }

    }



    public static MainWindow Instance

    {

        get

        {

            if (_instance == null)

            {

                _instance = new MainWindow();

            }

            return _instance;

        }

        set

        {

            _instance = value;

        }

    }

}

}



