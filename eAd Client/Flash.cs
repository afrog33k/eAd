using System.Windows.Threading;
using ClientApp.Players;

namespace ClientApp
{
    using ClientApp.Core;
    using System;
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Navigation;

    internal class Flash : Media
    {
        private string _backgroundColor;
        private string _backgroundImage;
        private string _backgroundLeft;
        private string _backgroundTop;
        private TemporaryHtml _tempHtml;
        private WebBrowser _webBrowser;

        public Flash(RegionOptions options) : base(options.Width, options.Height, options.Top, options.Left)
        {
            this._tempHtml = new TemporaryHtml();
            this._backgroundImage = options.backgroundImage;
            this._backgroundColor = options.backgroundColor;
            this._backgroundTop = options.BackgroundTop + "px";
            this._backgroundLeft = options.BackgroundLeft + "px";
            this.GenerateHeadHtml();
            string format = "\r\n                <object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' codebase='http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,0,0' Width='{2}' Height='{3}' id='analog_clock' align='middle'>\r\n                    <param name='allowScriptAccess' value='sameDomain' />\r\n                    <param name='movie' value='{1}' />\r\n                    <param name='quality' value='high' />\r\n                    <param name='bgcolor' value='#000' />\r\n                    <param name='WMODE' value='transparent' />\r\n                    <embed src='{1}' quality='high' wmode='transparent' bgcolor='#ffffff' Width='{2}' Height='{3}' name='analog_clock' align='middle' allowScriptAccess='sameDomain' type='application/x-shockwave-flash' pluginspage='http://www.macromedia.com/go/getflashplayer' />\r\n                </object>\r\n            ";
            this._tempHtml.BodyContent = string.Format(format, new object[] { options.Uri, options.Uri, options.Width.ToString(), options.Height.ToString() });
            this._webBrowser = new WebBrowser();
            this._webBrowser.RenderSize = base.RenderSize;
            this._webBrowser.LoadCompleted += new LoadCompletedEventHandler(this._webBrowser_DocumentCompleted);
            this._webBrowser.Navigate(this._tempHtml.Path);
        }

        private void _webBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            base.Show();
            this.AddChild(this._webBrowser);
            App.DoEvents();
        }

        public override void Dispose()
        {
            try
            {
                this._webBrowser.Dispose();
            }
            catch
            {
                Trace.WriteLine(new LogMessage("WebBrowser still in use.", string.Format("Dispose", new object[0])));
            }
            try
            {
                this._tempHtml.Dispose();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(new LogMessage("Dispose", string.Format("Unable to dispose TemporaryHtml with exception {0}", exception.Message)));
            }
            base.Dispose();
        }

        private void GenerateHeadHtml()
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
            this._tempHtml.HeadContent = "<style type='text/css'>body {" + str + " }</style>";
        }
    }
}

