using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Controls
{
    /// <summary>
    /// Interaction logic for ImageListBox.xaml
    /// </summary>
    public partial class ImageListBox : UserControl
    {
        public ImageListBox()
        {
            InitializeComponent();
       //   Images.ItemsSource = RobotImageLoader.LoadImages();
		}
	}

    //public static class RobotImageLoader
    //{
    //    public static Dictionary<string, BitmapImage> LoadImages()
    //    {

    //        Dictionary<string,BitmapImage> robotImages = new Dictionary<string, BitmapImage>();
    //        DirectoryInfo robotImageDir = new DirectoryInfo( "C:\\Robots" );
    //        foreach( FileInfo robotImageFile in robotImageDir.GetFiles( "*.jpg" ) )
    //        {
    //            Uri uri = new Uri( robotImageFile.FullName );
    //            robotImages.Add(robotImageFile.Name, new BitmapImage( uri ) );
    //        }
    //        return robotImages;
    //    }
    //}
    }

