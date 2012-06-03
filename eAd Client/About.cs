namespace ClientApp
{
    using ClientApp.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

   
    public partial class About : Window, IComponentConnector
    {
     

        public About()
        {
            this.label1.Content = App.ProductName;
            this.label2.Content = Settings.Default.ClientVersion;
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

