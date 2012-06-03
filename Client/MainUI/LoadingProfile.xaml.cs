using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ClientApp.MainUI
{
    /// <summary>
    /// Interaction logic for LoadingProfile.xaml
    /// </summary>
    public partial class LoadingProfile : UserControl, ISwitchable
    {
        private static LoadingProfile _instance;

        public LoadingProfile()
        {
            Instance = this;
            InitializeComponent();
            var splash = new BitmapImage();
            splash.BeginInit();
            splash.UriSource = new Uri("Resources/greencar.jpg", UriKind.Relative);
            splash.EndInit();
            Background = new ImageBrush(splash);
        }

        public static LoadingProfile Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoadingProfile();
                return _instance;
            }
            set { _instance = value; }
        }

        public virtual void Pause()
        {

         
          
            var da2 =
                new DoubleAnimation
                    ();
            da2.From
                = 1;
            da2.To =
                0;
            da2.
                Duration
                =
                new Duration
                    (TimeSpan
                         .
                         FromMilliseconds
                         (400));
            da2.
                AutoReverse
                =
                false;
            //   da.RepeatBehavior = RepeatBehavior.Forever;
            //da.RepeatBehavior=new RepeatBehavior(3);
            this.
                BeginAnimation
                (OpacityProperty,
                 da2);
        }



        public void UnPause()
        {
            this.Opacity = 0;
            var da =
                new DoubleAnimation
                    ();
            da.From = 0;
            da.To = 1;
            da.Duration =
                new Duration
                    (TimeSpan
                         .
                         FromMilliseconds
                         (400));
            da.
                AutoReverse
                = false;
            //   da.RepeatBehavior = RepeatBehavior.Forever;
            //da.RepeatBehavior=new RepeatBehavior(3);
            this.
                BeginAnimation
                (OpacityProperty,
                 da);

            var par =
                                                                                                             this.
                                                                                                                 Parent
                                                                                                             as Grid;
            if (par !=
                null)
                par.
                    Children
                    .
                    Remove
                    (this);
         
        }
    }
}