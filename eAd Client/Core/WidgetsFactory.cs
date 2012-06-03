using ClientApp.Players;

namespace ClientApp.Core
{
    using ClientApp;
    using ClientApp.Widgets;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    public static class WidgetsFactory
    {
        public static Dictionary<string, UserControl> Widgets = new Dictionary<string, UserControl>();

        static WidgetsFactory()
        {
            Widgets["PersonalInfo"] = new Profile();
            Widgets["CarInfo"] = new CarInfo();
            Widgets["BatteryInfo"] = new BatteryInfo();
            Location location = new Location {
                Visibility = Visibility.Hidden
            };
            Widgets["LocationInfo"] = location;
        }

        public static Media CreateFrom(RegionOptions regionOptions)
        {
            UserControl element = null;
            Media media = new Media(regionOptions.Width, regionOptions.Height, regionOptions.Top, regionOptions.Left);
            if (Widgets.ContainsKey(regionOptions.Name))
            {
                element = Widgets[regionOptions.Name];
            }
            else
            {
                string name = regionOptions.Name;
                if (name != null)
                {
                    if (!(name == "CarInfo"))
                    {
                        if (name == "BatteryInfo")
                        {
                            element = new BatteryInfo();
                        }
                        else if (name == "LocationInfo")
                        {
                            element = new Location();
                        }
                        else if (name == "PersonalInfo")
                        {
                            element = new Profile();
                        }
                    }
                    else
                    {
                        element = new CarInfo();
                    }
                }
            }
            element.Width = regionOptions.Width;
            element.Height = regionOptions.Height;
            element.HorizontalAlignment = HorizontalAlignment.Stretch;
            element.VerticalAlignment = VerticalAlignment.Stretch;
            element.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
            Canvas parent = element.Parent as Canvas;
            if (parent != null)
            {
                parent.Children.Remove(element);
            }
            media.MediaCanvas.Children.Add(element);
            media.HasOnLoaded = true;
            try
            {
                Widgets.Add(regionOptions.Name, element);
            }
            catch (Exception)
            {
            }
            return media;
        }
    }
}

