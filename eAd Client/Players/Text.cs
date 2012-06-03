using System.IO;
using System.Windows.Media;
using ClientApp.Controls;
using ClientApp.Core;
using ClientApp.Properties;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace ClientApp.Players
{
    internal class Text : Media
    {
        private string _backgroundColor;
        private string _backgroundImage;
        private string _backgroundLeft;
        private string _backgroundTop;
        private string _direction;
        private string _documentText;
        private string _filePath;
        private string _headJavaScript;
        private string _headText;
        private double _scaleFactor;
        private int _scrollSpeed;
        private TemporaryHtml _tempHtml;
        private WebBrowser _webBrowser;
        private MarqueeText marquee;

        public Text(RegionOptions options) : base(options.Width, options.Height, options.Top, options.Left)
        {
            //NavigatedEventHandler handler = null;
            this._filePath = options.Uri;
            this._direction = options.direction??"left";
            this._backgroundImage = options.backgroundImage;
            this._backgroundColor = options.backgroundColor;
            //this._scaleFactor = options.ScaleFactor==0?3:options.ScaleFactor;
            //this._backgroundTop = options.BackgroundTop + "px";
            //this._backgroundLeft = options.BackgroundLeft + "px";
            //this._documentText = File.ReadAllText(_filePath);//options.text;
            //this._scrollSpeed = 50;//options.scrollSpeed;
            //this._headJavaScript = options.javaScript;
            //this._tempHtml = new TemporaryHtml();
            //this.GenerateHeadHtml();
            //this.GenerateBodyHtml();
            //this._webBrowser = new WebBrowser();
            //this._webBrowser.Height = options.Height;
            //this._webBrowser.Width = options.Width;
            //this._webBrowser.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
            //this._webBrowser.LoadCompleted += new LoadCompletedEventHandler(this.WebBrowserDocumentCompleted);
            //if (handler == null)
            //{
            //    handler =new NavigatedEventHandler((t,r)=>{
            //        this.HideScriptErrors(this._webBrowser, true);
            //    });
            //}
            //this._webBrowser.Navigated += handler;
            //this._webBrowser.Navigate(this._tempHtml.Path);
            //base.MediaCanvas.Children.Add(this._webBrowser);

            marquee = new MarqueeText();
            marquee.Background =System.Windows.Media.Brushes.Red;
            marquee.MarqueeTimeInSeconds = options.Duration;
            marquee.Foreground = System.Windows.Media.Brushes.Black;
            marquee.Height = options.Height;
            marquee.Width = options.Width;
            marquee.MarqueeType = MarqueeType.RightToLeft; ;
            marquee.MarqueeContent = File.ReadAllText(_filePath);
            base.MediaCanvas.Children.Add(this.marquee);
        }

        public override void Dispose()
        {
            Action method = null;
            try
            {
                if (method == null)
                {
                    method =new Action(()=>{
                        base.MediaCanvas.Children.Remove(this._webBrowser);
                   //     this._webBrowser.Dispose();
                    });
                }
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
            }
            catch
            {
                Trace.WriteLine(new LogMessage("WebBrowser still in use.", string.Format("Dispose", new object[0])));
            }
            try
            {
           //     this._tempHtml.Dispose();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(new LogMessage("Dispose", string.Format("Unable to dispose TemporaryHtml with exception {0}", exception.Message)));
            }
            base.Dispose();
        }

        private void GenerateBodyHtml()
        {
            string str = "left";
            if (this._direction == "right")
            {
                str = "right";
            }
            if (this._direction == "none")
            {
                this._tempHtml.BodyContent = this._documentText;
            }
            else
            {
                string str2 = "";
                string str3 = "";
                if ((this._direction == "left") || (this._direction == "right"))
                {
                    str3 = "white-space: nowrap";
                }
                str2 = str2 + string.Format("<div id='text' style='position:relative;overflow:hidden;Width:{0}px; Height:{1}px;'>", base.Width - 10.0, base.Height) + string.Format("<div id='innerText' style='position:absolute; {3}: 0px; top: 0px; Width:{2}px; {0}'>{1}</div></div>", new object[] { str3, this._documentText, base.Width - 10.0, str });
                this._tempHtml.BodyContent = str2;
            }
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
            string str2 = "";
            if (this._direction != "none")
            {
                str2 = "\n<script type='text/javascript'>\nfunction init() \n{ \n    tr = new TextRender('text', 'innerText', '" + this._direction + "', " + Settings.Default.scrollStepAmount.ToString() + ");\n\n    var timer = 0;\n    timer = setInterval('tr.TimerTick()', " + this._scrollSpeed.ToString() + ");\n}\n</script>";
            }
            this._headText = this._headJavaScript + str2 + "<style type='text/css'>body {" + str + " font-size:" + this._scaleFactor.ToString() + "em; }</style>";
            this._tempHtml.HeadContent = this._headText;
        }

        public void HideScriptErrors(WebBrowser wb, bool hide)
        {
            FieldInfo field = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                object target = field.GetValue(wb);
                if (target != null)
                {
                    target.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, target, new object[] { hide });
                }
            }
        }

        public override void RenderMedia()
        {
            base.StartTimer();
        }

        private void WebBrowserDocumentCompleted(object sender, NavigationEventArgs navigationEventArgs)
        {
            base.Show();
            App.DoEvents();
        }
    }
}

