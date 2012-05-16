using System.Windows;
using System.Windows.Controls;
using ClientApp.Widgets;

namespace ClientApp.Core
{
    internal class WidgetsFactory
    {
        public static Media CreateFrom(RegionOptions regionOptions)
        {
            UserControl widget = null;
            Media Backing = new Media(regionOptions.Width,regionOptions.Height,regionOptions.Top,regionOptions.Left);
            switch (regionOptions.Name)
            {
                case "CarInfo": 
                    widget = new CarInfo();
                    break;
                case "BatteryInfo":
                    widget = new BatteryInfo();
                    break;
                case "LocationInfo":
                    widget = new Location();
                    break;
                case "PersonalInfo":
                    widget = new Profile();
                    break;
                default:
                    break;
            }

            widget.Width = regionOptions.Width;
            widget.Height = regionOptions.Height;
            //MediaGrid.Width = Width;
            //MediaGrid.Height = Height;

            widget.HorizontalAlignment = HorizontalAlignment.Center;
            widget.VerticalAlignment = VerticalAlignment.Center;
            widget.Margin = new Thickness(0, 0, 0, 0);
           Backing.MediaCanvas.Children.Add(widget);
           Backing.HasOnLoaded = true;
            return Backing;
        }
    }
}