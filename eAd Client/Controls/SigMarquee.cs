using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ClientApp.Controls
{
    [Serializable]
    public class SigMarquee : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePChange(params string[] aparams)
        {
            if (PropertyChanged != null)
            {
                foreach (var p in aparams)
                {
                    if (p == "*")
                    {
                        //BindingFlags flag = BindingFlags.Public;
                        foreach (var pp in GetType().GetProperties())
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs(pp.Name));
                        }
                    }
                    else
                        PropertyChanged(this, new PropertyChangedEventArgs(p));
                }
            }
        }

        // get only logic property
        public string present { get { return IsLocal ? message : remoteUrl; } }
        public Brush foreBrush { get { return new SolidColorBrush(font.ForeColor); } }
        public Brush backBrush { get { return new SolidColorBrush(font.BackColor); } }

        public Guid ident = Guid.NewGuid();
        public string name { get; set; }
        public string message { get; set; }
        public double speed { get; set; }
        public TimeSpan interval { get; set; }
        public int repeat { get; set; }
        public FlowDirection dir { get; set; }
        public double rotate { get; set; }
        public bool IsVertical { get { return rotate == 90 || rotate == 270; } }
        public VerticalAlignment alignment { get; set; }
        public SigFont font { get; set; }
        public MarqueeEffect effect { get; set; }
        public bool IsLocal { get; set; }
        public bool IsRemote { get { return !IsLocal; } set { IsLocal = !value; } }
        public string remoteUrl { get; set; }
        public bool IsVerticalFlow { get; set; }

        public SigMarquee()
        {
            name = "marquee";
            message = @"Hello World!";
            speed = 1.0;
            rotate = 0.0;
            interval = TimeSpan.FromSeconds(8);
            dir = FlowDirection.RightToLeft;
            font = new SigFont();
            effect = MarqueeEffect.EvenPace;
            IsLocal = true;
            remoteUrl = @"http://feeds.finance.yahoo.com/rss/2.0/category-stocks?region=US&lang=en-US";
            IsVerticalFlow = false;
        }
    }
}