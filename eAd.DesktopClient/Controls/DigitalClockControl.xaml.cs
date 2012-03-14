using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;

namespace DesktopClient.Controls
{
    /// <summary>
    /// Interaction logic for DigitalClock.xaml
    /// </summary>
    public partial class DigitalClockControl : UserControl
    {
        protected delegate void RefreshDelegate();
        private Thread updateThread = null;
        private DateTime currentTime = DateTime.Now;

        public DigitalClockControl()
        {
            InitializeComponent();

            this.AutoUpdate = true;
        }

        #region styling
        public Brush DigitBrush
        {
            set
            {
                p0.RenderBrush = value;
                p1.RenderBrush = value;
                p3.RenderBrush = value;
                p4.RenderBrush = value;
                p6.RenderBrush = value;
                p7.RenderBrush = value;
            }
        }

        public Brush DotBrush
        {
            set
            {
                p2.RenderBrush = value;
                p5.RenderBrush = value;
            }
        }

        public Brush ClockBackground
        {
            get
            {
                return masterBorder.Background;
            }
            set
            {
                masterBorder.Background = value;
            }
        }
        #endregion

        public DateTime CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;

                #region hours
                if (value.Hour > 9)
                {
                    p0.Value = int.Parse(value.Hour.ToString()[0].ToString());
                    p1.Value = int.Parse(value.Hour.ToString()[1].ToString());
                }
                else
                {
                    p0.Value = 0;
                    p1.Value = int.Parse(value.Hour.ToString()[0].ToString());

                }
                #endregion

                #region minutes
                if (value.Minute > 9)
                {
                    p3.Value = int.Parse(value.Minute.ToString()[0].ToString());
                    p4.Value = int.Parse(value.Minute.ToString()[1].ToString());
                }
                else
                {
                    p3.Value = 0;
                    p4.Value = int.Parse(value.Minute.ToString()[0].ToString());

                }
                #endregion

                #region seconds
                if (value.Second > 9)
                {
                    p6.Value = int.Parse(value.Second.ToString()[0].ToString());
                    p7.Value = int.Parse(value.Second.ToString()[1].ToString());
                }
                else
                {
                    p6.Value = 0;
                    p7.Value = int.Parse(value.Second.ToString()[0].ToString());

                }
                #endregion
            }
        }

        public bool AutoUpdate
        {
            get
            {
                return updateThread != null;
            }
            set
            {
                if (updateThread != null)
                {
                    try
                    {
                        updateThread.Abort();
                    }
                    catch (Exception)
                    { }
                }

                if (value)
                {
                    updateThread = new Thread(delegate()
                    {
                        try
                        {
                            while (true)
                            {
                                DateTime d = DateTime.Now;

                                bool complete = false;
                                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new RefreshDelegate(delegate()
                                {
                                    this.CurrentTime = d;

                                    complete = true;
                                }));

                                #region pause
                                try
                                {
                                    do
                                    {
                                        Thread.Sleep(900);
                                    }
                                    while (!complete);
                                }
                                catch (Exception)
                                {
                                }
                                #endregion
                            }
                        }
                        catch (Exception)
                        {
                        }
                    });
                    updateThread.Name = "Clock Thread";
                    updateThread.Start();
                }
            }
        }
    }
}
