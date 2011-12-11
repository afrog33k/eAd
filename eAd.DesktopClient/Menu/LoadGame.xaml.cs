using System;
using System.Windows.Controls;

namespace DesktopClient.Menu
{
	public partial class LoadGame : UserControl, ISwitchable
	{
		public LoadGame()
		{
			// Required to initialize variables
			InitializeComponent();
		}

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	Switcher.Switch(new MainMenu());
        }
        #endregion
	}
}