using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Animation;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientApp.Controls
{
    /// <summary>
    /// Interaction logic for MarqueeStrip.xaml
    /// </summary>
    public partial class MarqueeStrip : UserControl
    {
        SigMarquee marquee_;
        int delayedReset_ = 0;
        //System.IO.StreamWriter logFile_;

        class RssItem
        {
            public string title { get; set; }
            public string link { get; set; }
        }
        List<RssItem> items_ = new List<RssItem>();
        IEnumerator<RssItem> itr_;

        public SigMarquee Marquee { get { return marquee_; } set { marquee_ = value; } }

        public MarqueeStrip(SigMarquee amarquee)
        {
            InitializeComponent();
          
            AttachMarquee(amarquee == null ? new SigMarquee() : amarquee);
            CompositionTarget.Rendering += Animating;
            //logFile_ = new System.IO.StreamWriter(string.Format("marquee{0}.log", this.GetHashCode()));
        }

        void AttachMarquee(SigMarquee Amarquee = null)
        {
            if(Amarquee==null)
                Amarquee= new SigMarquee();
            Marquee = Amarquee;

            //TextBanner.FontFamily = Marquee.font.Family;
            TextBanner.FontSize = Marquee.Font.Size;
            //TextBanner.Foreground =  new SolidColorBrush(Marquee.font.ForeColor);
            TextBanner.FontWeight = Marquee.Font.Weight;
            //TextBanner.FontStyle = Marquee.font.Style;
            TextBanner.Text = Marquee.Message;
            
            TextBanner.Width = Marquee.IsVerticalFlow ? Marquee.Font.Size : double.NaN;

            Background = new SolidColorBrush(Marquee.Font.BackColor);
            if (Marquee.Font.IsGradient)
            {
                bkgPlane.Visibility = Visibility.Visible;
                if (marquee_.IsVerticalFlow)
                    bkgPlane.OpacityMask = Resources["brVBar"] as Brush;
                else
                    bkgPlane.OpacityMask = Resources["brHBar"] as Brush;
            }
            else
            {
         //       bkgPlane.Visibility = Visibility.Hidden;
           //     bkgPlane.OpacityMask = null;
            }

            if (Marquee.IsRemote)
            {
                OnLoopUpdate(null, null);
            }
        }

        DateTime lastUrlUpdate_ = DateTime.Now.AddYears(-10);
        void OnLoopUpdate(object sender, EventArgs e)
        {
            if ((DateTime.Now - lastUrlUpdate_) > TimeSpan.FromMinutes(10))
            {
                var doc = new XmlDocument();
                try
                {
                    doc.Load(Marquee.RemoteUrl);
                    var nodes = doc.GetElementsByTagName("item");
                    items_.Clear();
                    foreach (var elem in nodes.OfType<XmlElement>())
                    {
                        RssItem item = new RssItem();
                        item.title = elem.GetElementsByTagName("title")[0].InnerText;
                        item.link = elem.GetElementsByTagName("link")[0].InnerText;
                        items_.Add(item);
                    }
                    itr_ = items_.GetEnumerator();
                }
                catch (Exception ex)
                {
                    var msg = string.Format("url: {0} - {1}", Marquee.RemoteUrl, ex.Message);
                    System.Diagnostics.Debug.WriteLine(msg);
                }
                lastUrlUpdate_ = DateTime.Now;
            }

            ShowNext();
        }

        void ShowNext()
        {
            if (Marquee.IsRemote && itr_ != null)
            {
                if (!itr_.MoveNext())
                {
                    itr_.Reset();
                    itr_.MoveNext();
                }
                RssItem item = itr_.Current;
                if (item != null)
                {
                    // HINT: new text may longer and exceed the old boundary
                    TextBanner.Visibility = Visibility.Hidden;
                    TextBanner.Text = item.title;
                    delayedReset_ = 3;
                }
            }
        }

        void ResetAnimation()
        {
            double span;
            if (Marquee.IsVerticalFlow)
                span = ActualHeight + TextBanner.ActualHeight;
            else
                span = ActualWidth + TextBanner.ActualWidth;

            if (Marquee.Dir == FlowDirection.LeftToRight)
            {
                step_ = span / Marquee.Interval.TotalMilliseconds;
                halfSpan_ = span / 2;
            }
            else
            {
                step_ = -span / Marquee.Interval.TotalMilliseconds;
                halfSpan_ = -span / 2;
            }
        }

      

        void MarqueeStrip_Unloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= Animating;
            //logFile_.Close();
        }

        void MarqueeStrip_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetAnimation();
        }

        // animating
        DateTime lastFrameTime_ = DateTime.Now;
        DateTime beginTime_ = DateTime.Now;
        double halfSpan_ = 1500;
        private double step_;

        public MarqueeStrip():this(null)
        {
          
        }


        void Animating(object sender, EventArgs e)
        {
            TranslateTransform tt = (TextBanner.RenderTransform as TransformGroup).Children[0] as TranslateTransform;
            if (delayedReset_ > 0)
            {
                delayedReset_--;
                if (delayedReset_ == 0)
                {
                    ResetAnimation();
                    if (Marquee.IsVerticalFlow)
                        tt.Y = -halfSpan_;
                    else
                        tt.X = -halfSpan_;
                    TextBanner.Visibility = Visibility.Visible;
                }
                else
                    return;
            }

            double elapsed = (DateTime.Now - lastFrameTime_).TotalMilliseconds;
            //logFile_.WriteLine(elapsed);

            lastFrameTime_ = DateTime.Now;

            if (Marquee.IsVerticalFlow)
            {
                if (step_ > 0 ? tt.Y <= halfSpan_ : tt.Y >= halfSpan_)
                    tt.Y += elapsed * step_;
                else
                {
                    if (Marquee.IsRemote)
                        ShowNext();
                    else
                        tt.Y = -halfSpan_;
                }
            }
            else
            {
                if (step_ > 0 ? tt.X <= halfSpan_ : tt.X >= halfSpan_)
                    tt.X += elapsed * step_;
                else
                {
                    if (Marquee.IsRemote)
                        ShowNext();
                    else
                        tt.X = -halfSpan_;
                }
            }
        }
    }
}