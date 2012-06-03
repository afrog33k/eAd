namespace ClientApp
{
    using System;
    using System.Windows.Controls;

    public static class Switcher
    {
        public static ClientApp.ClientManager ClientManager;
        public static IPausableControl CurrentPage;
        public static IPausableControl LastPage;

        public static void Switch(IPausableControl newPage)
        {
            LastPage = CurrentPage;
            ClientManager.Navigate(newPage);
            CurrentPage = newPage;
        }

        public static void Switch(UserControl newPage, object state)
        {
            ClientManager.Navigate(newPage, state);
        }
    }
}

