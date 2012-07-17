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
        public string present { get { return IsLocal ? Message : RemoteUrl; } }
        public Brush foreBrush { get { return new SolidColorBrush(Font.ForeColor); } }
        public Brush backBrush { get { return new SolidColorBrush(Font.BackColor); } }

        public Guid ident = Guid.NewGuid();
        public string Name { get; set; }
        public string Message { get; set; }
        public double Speed { get; set; }
        public TimeSpan Interval { get; set; }
        public int Repeat { get; set; }
        public FlowDirection Dir { get; set; }
        public double Rotate { get; set; }
        public bool IsVertical { get { return Rotate == 90 || Rotate == 270; } }
        public VerticalAlignment Alignment { get; set; }
        public SigFont Font { get; set; }
        public MarqueeEffect Effect { get; set; }
        public bool IsLocal { get; set; }
        public bool IsRemote { get { return !IsLocal; } set { IsLocal = !value; } }
        public string RemoteUrl { get; set; }
        public bool IsVerticalFlow { get; set; }

        public SigMarquee()
        {
            Name = "marquee";
            Message = @"Updating!";
            Speed = 1.0;
            Rotate = 0.0;
            Interval = TimeSpan.FromSeconds(8);
            Dir = FlowDirection.RightToLeft;
            Font = new SigFont();
            Effect = MarqueeEffect.EvenPace;
            IsLocal = true;
            RemoteUrl = @"http://feeds.finance.yahoo.com/rss/2.0/category-stocks?region=US&lang=en-US";
            IsVerticalFlow = false;
        }
    }
}