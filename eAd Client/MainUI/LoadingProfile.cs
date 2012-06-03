namespace ClientApp.MainUI
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    
    public partial class LoadingProfile : UserControl, IComponentConnector, IPausableControl
    {
     
        private static LoadingProfile _instance;
     

        public LoadingProfile()
        {
            Instance = this;
            this.InitializeComponent();
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("Resources/greencar.jpg", UriKind.Relative);
            image.EndInit();
            base.Background = new ImageBrush(image);
        }

    

       

        public static LoadingProfile Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoadingProfile();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public void Pause()
        {
            
        }

        public void UnPause()
        {
           
        }
    }
}

