using System.Windows.Threading;
using ClientApp.Players;
using ClientApp.Service;

namespace ClientApp
{
    using ClientApp.Core;
    using ClientApp.Properties;
    
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Controls;
    using System.Windows.Navigation;

    internal class DataSetView : Media
    {
        private string _backgroundColor;
        private string _backgroundImage;
        private string _backgroundLeft;
        private string _backgroundTop;
        private int _duration;
        private int _layoutId;
        private string _mediaId;
        private string _regionId;
        private double _scaleFactor;
        private int _updateInterval;
        private WebBrowser _webBrowser;

        public DataSetView(RegionOptions options) : base(options.Width, options.Height, options.Top, options.Left)
        {
            this._layoutId = options.layoutId;
            this._regionId = options.regionId;
            this._mediaId = options.mediaid;
            this._duration = options.Duration;
            this._scaleFactor = options.ScaleFactor;
            this._updateInterval = Convert.ToInt32(options.Dictionary.Get("updateInterval"));
            this._backgroundImage = options.backgroundImage;
            this._backgroundColor = options.backgroundColor;
            this._backgroundTop = options.BackgroundTop + "px";
            this._backgroundLeft = options.BackgroundLeft + "px";
            this._webBrowser = new WebBrowser();
            this._webBrowser.LoadCompleted += new LoadCompletedEventHandler(this.webBrowser_DocumentCompleted);
            if (this.HtmlReady())
            {
                string source = Settings.Default.LibraryPath + @"\" + this._mediaId + ".htm";
                this._webBrowser.Navigate(source);
            }
            else
            {
                this.RefreshLocalHtml();
            }
        }

        private bool HtmlReady()
        {
            string path = Settings.Default.LibraryPath + @"\" + this._mediaId + ".htm";
            if (!File.Exists(path) || (this._updateInterval == 0))
            {
                return false;
            }
            DateTime lastWriteTime = File.GetLastWriteTime(path);
            if (DateTime.Now.CompareTo(lastWriteTime.AddHours((this._updateInterval * 1.0) / 60.0)) > 0)
            {
                return false;
            }
            return true;
        }

        private void RefreshLocalHtml()
        {
            ServiceClient client = new ServiceClient();
            client.GetResourceCompleted += new EventHandler<GetResourceCompletedEventArgs>(this.xmds_GetResourceCompleted);
            client.GetResourceAsync(Settings.Default.ServerKey, Settings.Default.hardwareKey, this._layoutId, this._regionId, this._mediaId, Settings.Default.Version);
        }

        private void webBrowser_DocumentCompleted(object sender, NavigationEventArgs navigationEventArgs)
        {
            base.Show();
            this.AddChild(this._webBrowser);
            App.DoEvents();
        }

        private void xmds_GetResourceCompleted(object sender, GetResourceCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Trace.WriteLine(new LogMessage("xmds_GetResource", "Unable to get Resource: " + e.Error.Message), LogType.Error.ToString());
            }
            else
            {
                string str;
                if ((this._backgroundImage == null) || (this._backgroundImage == ""))
                {
                    str = "background-color:" + this._backgroundColor + " ;";
                }
                else
                {
                    str = "background-image: url('" + this._backgroundImage + "'); background-attachment:fixed; background-color:" + this._backgroundColor + " background-repeat: no-repeat; background-position: " + this._backgroundLeft + " " + this._backgroundTop + ";";
                }
                string str2 = e.Result.Replace("</head>", "<style type='text/css'>body {" + str + " font-size:" + this._scaleFactor.ToString() + "em; }</style></head>");
                string path = Settings.Default.LibraryPath + @"\" + this._mediaId + ".htm";
                using (StreamWriter writer = new StreamWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read)))
                {
                    writer.Write(str2);
                    writer.Close();
                }
                this._webBrowser.Navigate(path);
            }
        }
    }
}

