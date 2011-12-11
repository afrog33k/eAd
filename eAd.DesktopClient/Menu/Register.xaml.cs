using System.Windows.Controls;

namespace DesktopClient.Menu
{
	/// <summary>
	/// Interaction logic for Register.xaml
	/// </summary>
	public partial class Register : UserControl
	{
		public Register()
		{
			this.InitializeComponent();
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(new MainMenu());
		}
	}
}