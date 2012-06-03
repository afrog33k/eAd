namespace ClientApp.Controls
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Timers;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Threading;

   
    public partial class BatteryRecharging : Canvas, IComponentConnector
    {
   
        private Timer _loadTimer;
        private Path _pthFiller;
        private Path _pthFillerReflection;
        private TextBlock _txtStatus;
    
        private const double c_FillBy = 1.0;
        private const double c_MaxFill = 314.0;
        private const double c_TimerSizeInPoints = 100.0;
     

        public BatteryRecharging()
        {
            this.InitializeComponent();
            this._pthFiller = this.pthFiller;
            this._pthFillerReflection = this.pthFiller1;
            this._txtStatus = this.txtStatus;
            this._pthFiller.Width = 0.0;
            this._pthFillerReflection.Width = 0.0;
            this._txtStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            base.Loaded += new RoutedEventHandler(this.PageLoaded);
        }

    

        private void PageLoaded(object sender, EventArgs args)
        {
        }

     

        private void TimerCompleted(object sender, EventArgs e)
        {
            this._loadTimer.Stop();
            this.pthFiller.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(()=> {
                double width = this._pthFiller.Width;
                if (width < 314.0)
                {
                    width++;
                    width = (width > 314.0) ? 314.0 : width;
                    this._pthFiller.Width = width;
                    this._pthFillerReflection.Width = width;
                    this._loadTimer.Start();
                }
               
            }));
        }

        public int PercentCharged
        {
            set
            {
                double num = 314.0 * (((float) value) / 100f);
                num = (num > 314.0) ? 314.0 : num;
                this._pthFiller.Width = num;
                this._pthFillerReflection.Width = num;
            }
        }
    }
}

