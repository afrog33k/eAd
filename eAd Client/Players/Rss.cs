using System.Windows.Threading;
using ClientApp.Players;

namespace ClientApp
{
    using ClientApp.Core;
    using ClientApp.Properties;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.ServiceModel.Syndication;
    using System.Windows.Controls;
    using System.Xml;

    internal class Rss : Media
    {
        private string _backgroundColor;
        private string _backgroundImage;
        private string _backgroundLeft;
        private string _backgroundTop;
        private string _bodyText;
        private string _copyrightNotice;
        private string _direction;
        private string _documentTemplate;
        private string _documentText;
        private int _duration;
        private int _durationIsPerItem;
        private string _filePath;
        private string _headText;
        private int _layoutId;
        private string _mediaid;
        private int _numItems;
        private string _rssFilePath;
        private bool _rssReady;
        private double _scaleFactor;
        private int _scheduleId;
        private int _scrollSpeed;
        private string _takeItemsFrom;
        private TemporaryHtml _tempHtml;
        private int _updateInterval;
        private WebClient _wc;
        private WebBrowser _webBrowser;

        public Rss(RegionOptions options) : base(options.Width, options.Height, options.Top, options.Left)
        {
            if (string.IsNullOrEmpty(options.Uri))
            {
                throw new ArgumentNullException("Uri", "The Uri for the RSS feed can not be empty");
            }
            try
            {
                this._filePath = Uri.UnescapeDataString(options.Uri);
            }
            catch (Exception)
            {
                throw new ArgumentNullException("Uri", "The URI is invalid.");
            }
            this._direction = options.direction;
            this._backgroundImage = options.backgroundImage;
            this._backgroundColor = options.backgroundColor;
            this._copyrightNotice = options.copyrightNotice;
            this._mediaid = options.mediaid;
            this._scheduleId = options.scheduleId;
            this._layoutId = options.layoutId;
            this._scaleFactor = options.ScaleFactor;
            this._duration = options.Duration;
            this._updateInterval = options.updateInterval;
            this._scrollSpeed = options.scrollSpeed;
            this._numItems = Convert.ToInt32(options.Dictionary.Get("numItems", "0"));
            this._durationIsPerItem = Convert.ToInt32(options.Dictionary.Get("durationIsPerItem", "0"));
            this._takeItemsFrom = options.Dictionary.Get("takeItemsFrom", "start");
            this._tempHtml = new TemporaryHtml();
            this._backgroundTop = options.BackgroundTop + "px";
            this._backgroundLeft = options.BackgroundLeft + "px";
            this._documentText = options.text;
            this._documentTemplate = options.documentTemplate;
            this.GenerateHeadHtml();
            this.PrepareRSS();
            this._webBrowser = new WebBrowser();
            if (this._rssReady)
            {
                this.LoadRssIntoTempFile();
                this._webBrowser.Navigate(this._tempHtml.Path);
            }
        }

        public override void Dispose()
        {
            try
            {
                this._webBrowser.Dispose();
            }
            catch
            {
            }
            try
            {
                this._wc.Dispose();
            }
            catch
            {
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
            string str2 = "";
            if (this._direction == "single")
            {
                str2 = "\n<script type='text/javascript'>\nfunction init() \n{\n    var totalDuration = " + this._duration.ToString() + " * 1000;\n    var itemCount = $('.RssItem').Size();\n    var durationIsPerItem = " + this._durationIsPerItem.ToString() + "\n    \n    if (durationIsPerItem == 0)\n        var itemTime = totalDuration / itemCount;\n    else\n        var itemTime = totalDuration;\n\n    if (itemTime < 2000) itemTime = 2000;\n\n   // Try to get the itemTime from an element we expect to be in the HTML \n   $('#text').cycle({fx: 'fade', timeout:itemTime}));\n}\n</script>";
            }
            else if (this._direction != "none")
            {
                str2 = "\n<script type='text/javascript'>\nfunction init() \n{ \n    tr = new TextRender('text', 'innerText', '" + this._direction + "', " + Settings.Default.scrollStepAmount.ToString() + ");\n\n    var timer = 0;\n    timer = setInterval('tr.TimerTick()', " + this._scrollSpeed.ToString() + ");\n}\n</script>";
            }
            this._headText = str2 + "<style type='text/css'>body {" + str + " font-size:" + this._scaleFactor.ToString() + "em; }</style>";
            this._tempHtml.HeadContent = this._headText;
        }

        private void LoadRssIntoTempFile()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(Settings.Default.LibraryPath + @"\" + this._mediaid + ".xml"))
                {
                    SyndicationFeed feed = SyndicationFeed.Load(reader);
                    int num = 0;
                    int num2 = 0;
                    using (IEnumerator<SyndicationItem> enumerator = feed.Items.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            SyndicationItem current = enumerator.Current;
                            num++;
                        }
                    }
                    if (this._numItems == 0)
                    {
                        this._numItems = num;
                    }
                    if (this._numItems > num)
                    {
                        this._numItems = num;
                    }
                    foreach (SyndicationItem item in feed.Items)
                    {
                        string str2;
                        string text;
                        num2++;
                        if (this._takeItemsFrom == "end")
                        {
                            if (num2 >= this._numItems)
                            {
                                goto Label_00D3;
                            }
                            continue;
                        }
                        if (num2 > this._numItems)
                        {
                            continue;
                        }
                    Label_00D3:
                        str2 = this._documentTemplate.Replace("[Title]", item.Title.Text.ToString());
                        if (item.Summary == null)
                        {
                            text = item.ElementExtensions.ReadElementExtensions<string>("encoded", "http://purl.org/rss/1.0/modules/content/")[0].ToString();
                        }
                        else
                        {
                            text = item.Summary.Text;
                        }
                        str2 = str2.Replace("[Description]", text).Replace("[Date]", item.PublishDate.ToString("F"));
                        if (item.Links.Count > 0)
                        {
                            str2 = str2.Replace("[Link]", item.Links[0].Uri.ToString());
                        }
                        if ((this._direction == "left") || (this._direction == "right"))
                        {
                            str2 = str2.Replace("<p>", "").Replace("</p>", "");
                            this._documentText = this._documentText + string.Format("<span class='article' style='padding-left:4px;'>{0}</span>", str2);
                            continue;
                        }
                        this._documentText = this._documentText + string.Format("<div class='RssItem' style='display:block;padding:4px;Width:{1}'>{0}</div>", str2, base.Width - 10.0);
                    }
                    this._documentText = this._documentText + this.CopyrightNotice;
                    if (this._direction == "none")
                    {
                        this._bodyText = this._documentText;
                    }
                    else
                    {
                        string str4 = "";
                        string str5 = "";
                        if ((this._direction == "left") || (this._direction == "right"))
                        {
                            str5 = "white-space: nowrap";
                            this._documentText = string.Format("<nobr>{0}</nobr>", this._documentText);
                        }
                        else
                        {
                            str5 = string.Format("Width: {0}px;", base.Width - 50.0);
                        }
                        if (this._direction == "single")
                        {
                            str4 = str4 + string.Format("<div id='text'>{0}</div>", this._documentText);
                        }
                        else
                        {
                            string str6 = "left";
                            if (this._direction == "right")
                            {
                                str6 = "right";
                            }
                            str4 = str4 + string.Format("<div id='text' style='position:relative;overflow:hidden;Width:{0}px; Height:{1}px;'>", base.Width - 10.0, base.Height) + string.Format("<div id='innerText' style='position:absolute; {2}: 0px; top: 0px; {0}'>{1}</div></div>", str5, this._documentText, str6);
                        }
                        this._bodyText = str4;
                    }
                    this._tempHtml.BodyContent = this._bodyText;
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", new object[] { exception.Message, this._scheduleId, this._layoutId, this._mediaid }));
                this._bodyText = "<h1>Unable to load feed</h1>";
                this._tempHtml.BodyContent = this._bodyText;
                System.IO.File.Delete(Settings.Default.LibraryPath + @"\" + this._mediaid + ".xml");
            }
        }

        private void PrepareRSS()
        {
            this._rssReady = false;
            this._rssFilePath = Settings.Default.LibraryPath + @"\" + this._mediaid + ".xml";
            if (!System.IO.File.Exists(this._rssFilePath) || (this._updateInterval == 0))
            {
                this.RefreshLocalRss();
            }
            else
            {
                DateTime lastWriteTime = System.IO.File.GetLastWriteTime(this._rssFilePath);
                if (DateTime.Now.CompareTo(lastWriteTime.AddHours((this._updateInterval * 1.0) / 60.0)) > 0)
                {
                    this.RefreshLocalRss();
                }
                else
                {
                    this._rssReady = true;
                }
            }
        }

        private void RefreshLocalRss()
        {
            try
            {
                this._wc = new WebClient();
                this._wc.UseDefaultCredentials = true;
                this._wc.OpenReadCompleted += new OpenReadCompletedEventHandler(this.wc_OpenReadCompleted);
                this._wc.OpenReadAsync(new Uri(this._filePath));
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", new object[] { exception.Message, this._scheduleId, this._layoutId, this._mediaid }));
            }
        }

        public override void RenderMedia()
        {
            if (this._durationIsPerItem == 1)
            {
                base.Duration *= this._numItems;
            }
            base.StartTimer();
        }

        private void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            using (WebClient client = (WebClient) sender)
            {
                if (e.Error != null)
                {
                    Trace.WriteLine(string.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", new object[] { e.Error, this._scheduleId, this._layoutId, this._mediaid }));
                    return;
                }
                try
                {
                    using (StreamReader reader = new StreamReader(e.Result, client.Encoding))
                    {
                        using (StreamWriter writer = new StreamWriter(System.IO.File.Open(this._rssFilePath, FileMode.Create, FileAccess.Write, FileShare.Read), client.Encoding))
                        {
                            writer.Write(reader.ReadToEnd());
                        }
                        this._rssReady = true;
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(string.Format("[*]ScheduleID:{1},LayoutID:{2},MediaID:{3},Message:{0}", new object[] { exception.Message, this._scheduleId, this._layoutId, this._mediaid }));
                }
            }
            try
            {
                if (this._rssReady)
                {
                    this.LoadRssIntoTempFile();
                    this._webBrowser.Navigate(this._tempHtml.Path);
                }
            }
            catch (Exception)
            {
            }
        }

        private void webBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            base.Show();
            this.AddChild(this._webBrowser);
            App.DoEvents();
        }

        private string CopyrightNotice
        {
            get
            {
                if (!(this._direction == "left") && !(this._direction == "right"))
                {
                    return string.Format("<div style='display:block;font-family: Arial; font-size: 8px;'>{0}</div>", this._copyrightNotice);
                }
                return string.Format("<span style='font-family: Arial; font-size: 8px;'>{0}</span>", this._copyrightNotice);
            }
        }
    }
}

