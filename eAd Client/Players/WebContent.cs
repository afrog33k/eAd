using System.Windows;
using ClientApp.Core;
using ClientApp.Properties;
using System;
using System.Diagnostics;
using System.Windows.Navigation;
using Application = System.Windows.Forms.Application;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace ClientApp.Players
{
    public class WebContent : Media
    {
        private int duration;
        private string filePath;
        private int layoutId;
        private string mediaId;
        private int scheduleId;
        private string type;
        private WebBrowser webBrowser;

        public WebContent(RegionOptions options) : base(options.Width, options.Height, options.Top, options.Left)
        {
            this.duration = options.Duration;
            this.scheduleId = options.scheduleId;
            this.layoutId = options.layoutId;
            this.mediaId = options.mediaid;
            this.type = options.FileType;
            this.webBrowser = new WebBrowser();
            this.webBrowser.Height = options.Height;
            this.webBrowser.Width = options.Width;
            this.webBrowser.LoadCompleted += new LoadCompletedEventHandler(this.WebBrowserDocumentCompleted);
            //if (/*!Settings.Default.powerpointEnabled &&*/ (options.FileType == "Powerpoint"))
            //{
            //    this.webBrowser.Source = new Uri("<html><body><h1>Powerpoint not enabled on this display</h1></body></html>");
            //    Trace.WriteLine(string.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", new object[] { "Powerpoint is not enabled on this display", this.scheduleId, this.layoutId, this.mediaId }));
            //}
            //else
            {
                try
                {
                    try
                    {
                        this.filePath = Uri.UnescapeDataString(options.Uri);
                    }
                    catch (Exception)
                    {
                    }
                    this.webBrowser.Navigate(Application.StartupPath+"\\"+ this.filePath.Replace("\\\\","\\"));
                    MediaCanvas.Children.Add(webBrowser);

                }
                catch (Exception exception)
                {
                    Trace.WriteLine(string.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", new object[] { exception.Message, this.scheduleId, this.layoutId, this.mediaId }));
                    this.webBrowser.NavigateToString("<html><body><h1>Unable to show this web location - invalid address.</h1></body></html>");
                    Trace.WriteLine(string.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", new object[] { "Unable to show the powerpoint, cannot be located", this.scheduleId, this.layoutId, this.mediaId }));
                }
            }
        }

        public override void Dispose()
        {
            try
            {
                MediaCanvas.Children.Remove(this.webBrowser);
                this.webBrowser.Dispose();
              //  GC.Collect();
            }
            catch (Exception)
            {
            }
            base.Dispose();
        }

        public override void RenderMedia()
        {
        }

        private void WebBrowserDocumentCompleted(object sender, NavigationEventArgs navigationEventArgs)
        {
            base.Duration = this.duration;
            base.RenderMedia();
            base.Show();
            App.DoEvents();
            webBrowser.Visibility = Visibility.Visible;
            ;
        }
    }
}

