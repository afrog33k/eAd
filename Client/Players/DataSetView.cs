using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using ClientApp.Core;
using ClientApp.Properties;
using ClientApp.Service;

namespace ClientApp
{
class DataSetView :
    Media
{
    private int _layoutId;
    private string _regionId;
    private string _mediaId;
    private int _updateInterval;
    private double _scaleFactor;
    private int _duration;
    private string _backgroundImage;
    private string _backgroundColor;
    private string _backgroundTop;
    private string _backgroundLeft;

    private WebBrowser _webBrowser;

    public DataSetView(RegionOptions options)
    : base(options.Width, options.Height, options.Top, options.Left)
    {
        _layoutId = options.layoutId;
        _regionId = options.regionId;
        _mediaId = options.mediaid;
        _duration = options.Duration;
        _scaleFactor = options.ScaleFactor;

        _updateInterval = Convert.ToInt32(options.Dictionary.Get("updateInterval"));

        _backgroundImage = options.backgroundImage;
        _backgroundColor = options.backgroundColor;

        // Set up the backgrounds
        _backgroundTop = options.BackgroundTop + "px";
        _backgroundLeft = options.BackgroundLeft + "px";

        // Create a webbrowser to take the temp file loc
        _webBrowser = new WebBrowser();
        //_webBrowser.ScriptErrorsSuppressed = true;
        //_webBrowser.Size = this.Size;
        //_webBrowser.ScrollBarsEnabled = false;
        _webBrowser.LoadCompleted +=  (webBrowser_DocumentCompleted);

        if (HtmlReady())
        {
            // Navigate to temp file
            string filePath = Settings.Default.LibraryPath + @"\" + _mediaId + ".htm";
            _webBrowser.Navigate(filePath);
        }
        else
        {
            RefreshLocalHtml();
        }
    }

    /// <summary>
    /// Handles the document completed event.
    /// Resets the Background color to be options.backgroundImage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void webBrowser_DocumentCompleted(object sender, NavigationEventArgs navigationEventArgs)
    {
        // Show the control
        Show();
        AddChild(_webBrowser);
        App.DoEvents();
    }

    private bool HtmlReady()
    {
        // Pull the RSS feed, and put it in a temporary file cache
        // We want to check the file exists first
        string filePath = Settings.Default.LibraryPath + @"\" + _mediaId + ".htm";

        if (!System.IO.File.Exists(filePath) || _updateInterval == 0)
            return false;

        // It exists - therefore we want to get the last time it was updated
        DateTime lastWriteDate = System.IO.File.GetLastWriteTime(filePath);

        if (DateTime.Now.CompareTo(lastWriteDate.AddHours(_updateInterval * 1.0 / 60.0)) > 0)
            return false;
        else
            return true;
    }

    private void RefreshLocalHtml()
    {
        ServiceClient xmds = new ServiceClient();
        xmds.GetResourceCompleted +=  (xmds_GetResourceCompleted);

        xmds.GetResourceAsync(Settings.Default.ServerKey, Settings.Default.hardwareKey, _layoutId, _regionId, _mediaId, Settings.Default.Version);
    }

    void xmds_GetResourceCompleted(object sender, GetResourceCompletedEventArgs e)
    {
        // Success / Failure
        if (e.Error != null)
        {
            Trace.WriteLine(new LogMessage("xmds_GetResource", "Unable to get Resource: " + e.Error.Message), LogType.Error.ToString());
        }
        else
        {
            // Handle the background
            String bodyStyle;

            if (_backgroundImage == null || _backgroundImage == "")
            {
                bodyStyle = "background-color:" + _backgroundColor + " ;";
            }
            else
            {
                bodyStyle = "background-image: url('" + _backgroundImage + "'); background-attachment:fixed; background-color:" + _backgroundColor + " background-repeat: no-repeat; background-position: " + _backgroundLeft + " " + _backgroundTop + ";";
            }

            string html = e.Result.Replace("</head>", "<style type='text/css'>body {" + bodyStyle + " font-size:" + _scaleFactor.ToString() + "em; }</style></head>");

            string fullPath = Settings.Default.LibraryPath + @"\" + _mediaId + ".htm";

            using (StreamWriter sw = new StreamWriter(File.Open(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                sw.Write(html);
                sw.Close();
            }

            _webBrowser.Navigate(fullPath);
        }
    }
}
}
