using System.Windows.Controls;

namespace ClientApp.Controls
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

