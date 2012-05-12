using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DesktopClient.Controls
{
public partial class BatteryRecharging : Canvas
{
    private System.Timers.Timer _loadTimer;
    private const double c_TimerSizeInPoints = 100;

    private Path _pthFiller;
    private Path _pthFillerReflection;
    private TextBlock _txtStatus;

    private const double c_MaxFill = 314d;
    private const double c_FillBy = 1d;

    public BatteryRecharging()
    {
        InitializeComponent();
        this.Loaded += this.PageLoaded;
    }

    private void PageLoaded(object sender, EventArgs args)
    {
        #region Initialize Fillers
        this._pthFiller = pthFiller; //(Path)this.FindName("pthFiller");
        this._pthFillerReflection = pthFiller1;// (Path)this.FindName("pthFiller1");
        this._txtStatus = txtStatus; //(TextBlock)this.FindName("txtStatus");

        this._pthFiller.Width = 0;
        this._pthFillerReflection.Width = 0;
        this._txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        #endregion

        #region Initialise and start Load Timer
        //_loadTimer = new Timer();
        //_loadTimer.Interval = 100;
        //_loadTimer.Elapsed += TimerCompleted;
        //_loadTimer.Start();
        #endregion
    }

    public int PercentCharged
    {
        set
        {
            double newFill = c_MaxFill * (value/(float)100);

            newFill = newFill > c_MaxFill ? c_MaxFill : newFill;

            this._pthFiller.Width = newFill;
            this._pthFillerReflection.Width = newFill;

        }
    }


    private void TimerCompleted(object sender, EventArgs e)
    {
        _loadTimer.Stop();
        pthFiller.Dispatcher.BeginInvoke(

            System.Windows.Threading.DispatcherPriority.Normal

            , new DispatcherOperationCallback(delegate
        {
            double newFill = this._pthFiller.Width;

            if (newFill < c_MaxFill)
            {
                newFill += c_FillBy;
                newFill = newFill > c_MaxFill ? c_MaxFill : newFill;

                this._pthFiller.Width = newFill;
                this._pthFillerReflection.Width = newFill;


                _loadTimer.Start();
            }
            return null;

        }), null);






    }
}



}
