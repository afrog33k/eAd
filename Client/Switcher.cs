using System.Windows;
using System.Windows.Controls;

namespace Client
{
public static class Switcher
{
    public static PageSwitcher PageSwitcher;

    public static UserControl CurrentPage;

    public static void Switch(UserControl newPage)
    {
  //      newPage.Visibility = Visibility.Visible;
        PageSwitcher.Navigate(newPage);
        CurrentPage = newPage;
    }

    public static void Switch(UserControl newPage, object state)
    {
        PageSwitcher.Navigate(newPage, state);
    }
}
}
