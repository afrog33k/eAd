using System.Windows;
using System.Windows.Controls;
using Client.Properties;

namespace Client
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            label1.Content = App.ProductName;
            label2.Content = Settings.Default.ClientVersion;
        }

      

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

      
    }
}
