using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using ClientApp.Core;
using ClientApp.Properties;

namespace ClientApp
{
class WebContent : Media
{
    int scheduleId;
    int layoutId;
    string mediaId;
    string type;

    public WebContent(RegionOptions options)
    : base(options.Width, options.Height, options.Top, options.Left)
    {
        duration        = options.Duration;
        scheduleId      = options.scheduleId;
        layoutId        = options.layoutId;
        mediaId         = options.mediaid;
        type = options.FileType;

        webBrowser = new WebBrowser();

        webBrowser.Height = options.Height;
        webBrowser.Width = options.Width;

        //webBrowser.ScrollBarsEnabled = false;
        //webBrowser.ScriptErrorsSuppressed = true;

        // Attach event
        webBrowser.LoadCompleted +=  (WebBrowserDocumentCompleted);

        if (!Settings.Default.powerpointEnabled && options.FileType == "powerpoint")
        {
            webBrowser.Source = new Uri("<html><body><h1>Powerpoint not enabled on this display</h1></body></html>");
            System.Diagnostics.Trace.WriteLine(String.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", "Powerpoint is not enabled on this display", scheduleId, layoutId, mediaId));
        }
        else
        {
            try
            {
                // Try to make a URI out of the file path
                try
                {
                    this.filePath = Uri.UnescapeDataString(options.Uri);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message, "WebContent");
                }

                // Navigate
                webBrowser.Navigate(this.filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(String.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", ex.Message, scheduleId, layoutId, mediaId));

                webBrowser.Source = new Uri("<html><body><h1>Unable to show this web location - invalid address.</h1></body></html>");

                System.Diagnostics.Trace.WriteLine(String.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", "Unable to show the powerpoint, cannot be located", scheduleId, layoutId, mediaId));
            }
        }
    }

    public override void RenderMedia()
    {
        //do nothing
        return;
    }

    void WebBrowserDocumentCompleted(object sender, NavigationEventArgs navigationEventArgs)
    {
        base.Duration = duration;
        base.RenderMedia();

        // Get ready to show the control
        Show();
        App.DoEvents();
        AddChild(webBrowser);
    }

    public override void Dispose()
    {
        System.Diagnostics.Debug.WriteLine(String.Format("Disposing {0}", filePath));

        try
        {
            RemoveVisualChild(webBrowser);
            webBrowser.Dispose();
            GC.Collect();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Unable to dispose {0} because {1}", filePath, ex.Message));
        }

        base.Dispose();

        System.Diagnostics.Debug.WriteLine(String.Format("Disposed {0}", filePath));
    }

    string filePath;
    WebBrowser webBrowser;
    int duration;
}
}