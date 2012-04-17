
using System;
using System.Windows.Controls;
using System.Diagnostics;
using Client.Core;

namespace Client
{
    internal class Flash
        : Media
    {
        private TemporaryHtml _tempHtml;
        private WebBrowser _webBrowser;
        private string _backgroundImage;
        private string _backgroundColor;
        private string _backgroundTop;
        private string _backgroundLeft;

        public Flash(RegionOptions options)
            : base(options.Width, options.Height, options.Top, options.Left)
        {
            _tempHtml = new TemporaryHtml();

            _backgroundImage = options.backgroundImage;
            _backgroundColor = options.backgroundColor;
            _backgroundTop = options.BackgroundTop + "px";
            _backgroundLeft = options.BackgroundLeft + "px";

            // Create the HEAD of the document
            GenerateHeadHtml();

            // Set the body
            string html =
                @"
                <object classid='clsid:d27cdb6e-ae6d-11cf-96b8-444553540000' codebase='http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,0,0' Width='{2}' Height='{3}' id='analog_clock' align='middle'>
                    <param name='allowScriptAccess' value='sameDomain' />
                    <param name='movie' value='{1}' />
                    <param name='quality' value='high' />
                    <param name='bgcolor' value='#000' />
                    <param name='WMODE' value='transparent' />
                    <embed src='{1}' quality='high' wmode='transparent' bgcolor='#ffffff' Width='{2}' Height='{3}' name='analog_clock' align='middle' allowScriptAccess='sameDomain' type='application/x-shockwave-flash' pluginspage='http://www.macromedia.com/go/getflashplayer' />
                </object>
            ";

            _tempHtml.BodyContent = string.Format(html, options.Uri, options.Uri, options.Width.ToString(),
                                                  options.Height.ToString());

            // Fire up a webBrowser control to display the completed file.
            _webBrowser = new WebBrowser();
            _webBrowser.RenderSize = this.RenderSize;
         
      //      _webBrowser.Size = this.Size;
    //        _webBrowser.ScrollBarsEnabled = false;
        //    _webBrowser.ScriptErrorsSuppressed = true;
            _webBrowser.LoadCompleted +=  (_webBrowser_DocumentCompleted);

            // Navigate to temp file
            _webBrowser.Navigate(_tempHtml.Path);
        }

        /// <summary>
        /// Generates the Head Html for this Document
        /// </summary>
        private void GenerateHeadHtml()
        {
            // Handle the background
            String bodyStyle;

            if (_backgroundImage == null || _backgroundImage == "")
            {
                bodyStyle = "background-color:" + _backgroundColor + " ;";
            }
            else
            {
                bodyStyle = "background-image: url('" + _backgroundImage +
                            "'); background-attachment:fixed; background-color:" + _backgroundColor +
                            " background-repeat: no-repeat; background-position: " + _backgroundLeft + " " +
                            _backgroundTop + ";";
            }

            // Store the document text in the temporary HTML space
            _tempHtml.HeadContent = "<style type='text/css'>body {" + bodyStyle + " }</style>";
            ;
        }

        /// <summary>
        /// Web browser completed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _webBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            // We have navigated to the temporary file.
            Show();
            AddChild(_webBrowser);
            App.DoEvents();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        public override void Dispose()
        {

            // Remove the webbrowser control
            try
            {
                _webBrowser.Dispose();
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine(new LogMessage("WebBrowser still in use.", String.Format("Dispose")));
            }

            // Remove the temporary file we created
            try
            {
                _tempHtml.Dispose();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new LogMessage("Dispose",
                                               String.Format("Unable to dispose TemporaryHtml with exception {0}",
                                                             ex.Message)));
            }


            base.Dispose();

        }
    }
}
