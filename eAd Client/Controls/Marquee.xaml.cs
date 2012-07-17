using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ClientApp.Controls
{
	/// <summary>
	/// Interaction logic for Marquee.xaml
	/// </summary>
	public partial class Marquee : UserControl
	{

        public Marquee()
		{
			InitializeComponent();
			_bd.DataContext = this;
			
		}
        private double _wid;
        public double DesiredWidth
        {
            get 
            { 
                return ((TextBlock)GetValue(MarqueeContentProperty)).Width; 
            }
            set 
            { 
                   _wid = value;
                   TextBlock textBlock1 = ((TextBlock)GetValue(MarqueeContentProperty));
                   if (textBlock1 != null)
                   {
                       System.Globalization.CultureInfo enUsCultureInfo;
                       Typeface fontTF;
                       FormattedText frmmtText;
                       double stringSize;
                       if (textBlock1.Text.Length > 0)
                       {
                           enUsCultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-us");
                           fontTF = new Typeface(textBlock1.FontFamily, textBlock1.FontStyle, textBlock1.FontWeight, textBlock1.FontStretch);
                           frmmtText = new FormattedText(textBlock1.Text, enUsCultureInfo, FlowDirection.LeftToRight, fontTF, textBlock1.FontSize, textBlock1.Foreground);
                           stringSize = frmmtText.WidthIncludingTrailingWhitespace;

                           while (stringSize < (value - 16))
                           {
                               textBlock1.Text += " ";
                               frmmtText = new FormattedText(textBlock1.Text, enUsCultureInfo, FlowDirection.LeftToRight, fontTF, textBlock1.FontSize, textBlock1.Foreground);
                               stringSize = frmmtText.WidthIncludingTrailingWhitespace;
                           }
                       }
                   }
            }
        }

        public TextBlock MarqueeContent
        {
            get { return (TextBlock)GetValue(MarqueeContentProperty); }
            set
            {
                //value.Width = _bd.ActualWidth;
                SetValue(MarqueeContentProperty, value);
                TextBlock textBlock1 = ((TextBlock)GetValue(MarqueeContentProperty));
                System.Globalization.CultureInfo enUsCultureInfo;   
            Typeface fontTF;   
            FormattedText frmmtText;
                double stringSize;
                if (textBlock1.Text.Length > 0)
                {
                    enUsCultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-us");
                    fontTF = new Typeface(textBlock1.FontFamily, textBlock1.FontStyle, textBlock1.FontWeight, textBlock1.FontStretch);
                    frmmtText = new FormattedText(textBlock1.Text, enUsCultureInfo, FlowDirection.LeftToRight, fontTF, textBlock1.FontSize, textBlock1.Foreground);

                    stringSize = frmmtText.WidthIncludingTrailingWhitespace;

                    while (stringSize < _wid)
                    {
                        textBlock1.Text += " ";
                        frmmtText = new FormattedText(textBlock1.Text, enUsCultureInfo, FlowDirection.LeftToRight, fontTF, textBlock1.FontSize, textBlock1.Foreground);
                        stringSize = frmmtText.WidthIncludingTrailingWhitespace;
                    }
                }
            }
        }

	    public bool LeftToRight
	    {
	        set
	        {
                if (value)
                {

                    _storyboard.Stop();
                    var fade = new DoubleAnimation()
                                   {
                                       From = 0,
                                       To = Width,
                                       Duration =   _storyboard.Duration, 
                                       RepeatBehavior = RepeatBehavior.Forever,
                                       AutoReverse = true
                                    };

                    _storyboard.Children.Clear();
                    _storyboard.Children.Add(fade);

                    _storyboard.Duration = (Duration) Duration;
                    _storyboard.RepeatBehavior = RepeatBehavior.Forever;
                    _storyboard.AutoReverse =true;
                    _storyboard.Begin();
                }
	        }
	    }

        public static readonly DependencyProperty MarqueeContentProperty =
            DependencyProperty.Register("MarqueeContent", typeof(TextBlock), typeof(Marquee), new UIPropertyMetadata(null));

        public Duration Duration
		{
            get { return (Duration)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		public static readonly DependencyProperty DurationProperty =
			DependencyProperty.Register("Duration", typeof(Duration), typeof(Marquee), new UIPropertyMetadata(new Duration(new TimeSpan(0, 0, 1)), DurationPropertyChanged));

		private static void DurationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			Marquee marquee = sender as Marquee;
			if (marquee != null)
			{
				//marquee._storyboard.Duration = (Duration)e.NewValue;
             
				marquee._storyboard.Stop();
				marquee._storyboard.Begin();
			}
		}
	}
}
