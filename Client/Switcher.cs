using System.Windows.Controls;
using Client;

namespace ClientApp
{
public static class Switcher
{
    public static ClientManager ClientManager;

    public static UserControl CurrentPage;

    public static void Switch(UserControl newPage)
    {
  //      newPage.Visibility = Visibility.Visible;
        ClientManager.Navigate(newPage);
        CurrentPage = newPage;
    }

    public static void Switch(UserControl newPage, object state)
    {
        ClientManager.Navigate(newPage, state);
    }
}
}
