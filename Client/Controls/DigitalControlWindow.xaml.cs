using System.Windows;

namespace Client.Controls
{
/// <summary>
/// Interaction logic for DigitalControlWindow.xaml
/// </summary>
public partial class DigitalClockWindow : Window
{
    public DigitalClockWindow()
    {
        InitializeComponent();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        clock.AutoUpdate = false;
    }
}
}
