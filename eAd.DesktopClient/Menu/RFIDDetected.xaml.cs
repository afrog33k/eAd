using System;
using System.Windows.Controls;

namespace DesktopClient.Menu
{
	public partial class RFIDDetected : UserControl, ISwitchable
	{
		public RFIDDetected()
		{
			InitializeComponent();
		}

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	Switcher.Switch(MainWindow.Instance);
        }
        #endregion

        private void FormFadeOut_Completed(object sender, EventArgs e)
        {

        }
    }
}