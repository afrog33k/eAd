using System;
using System.Windows.Controls;
using Client;

namespace ClientApp
{
public partial class Option : ISwitchable
{
    private static UserControl _instance;

    public Option()
    {
        // Required to initialize variables
        InitializeComponent();
    }

    public static UserControl Instance
    {
        get
        {
            if(_instance==null)
                _instance= new UserControl();
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    #region ISwitchable Members
    public void UtilizeState(object state)
    {
        throw new NotImplementedException();
    }

    private void ButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        Switcher.Switch( CustomerPage.Instance);
    }
    #endregion
}
}