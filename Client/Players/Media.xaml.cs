using System;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Client.Properties;
using Control = System.Windows.Controls.Control;
using Timer = System.Timers.Timer;
using UserControl = System.Windows.Controls.UserControl;

namespace Client
{
    /// <summary>
    /// Interaction logic for Media.xaml
    /// </summary>
    public partial class Media : IDisposable
    {

        protected string Text { get; set; }
        protected FormBorderStyle FormBorderStyle { get; set; }
        protected Size ClientSize { get; set; }
        public Point Location { get; set; }
        protected AutoScaleMode AutoScaleMode { get; set; }
        protected Size AutoScaleDimensions { get; set; }

        public Media(int width, int height, int top, int left)
        {
            InitializeComponent();

            //  Hide();
            // Form properties
            //    this.TopLevel = false;
            //   this.BorderBrush = new LinearGradientBrush();
            //this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.BorderBrush = new RadialGradientBrush(Colors.Red, Colors.Purple);
            this.Width = width;
            this.Height = height;
            this.Margin = new Thickness(left, top, 0, 0); //new System.Drawing.Point(0, 0);

            // Transparency
            //  this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //    this.BackColor = System.Drawing.Color.Transparent;
            //   this.TransparencyKey = System.Drawing.Color.White;
            //   Background = Brushes.Transparent;
        //    Background = Brushes.Violet;
            if (Settings.Default.DoubleBuffering)
            {

                // SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                // SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            }
        }

        public void Hide()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                  {
                                                                                      //this.Visibility = Visibility.Hidden;
                                                                                  }));


        }
        object timerLock = new object();

        protected void StartTimer()
        {
            lock (timerLock)
            {
            //start the timer
            if (!timerStarted && duration != 0)
            {
                timer = new Timer();
                timer.Interval = (1000*duration);
                timer.Start();

                timer.Elapsed += new ElapsedEventHandler(timer_Tick);

                timerStarted = true;
            }
            }
        }

        /// <summary>
        /// Render Media call
        /// </summary>
        public virtual void RenderMedia()
        {
            // Start the timer for this media
            StartTimer();

            // Show the form
            Show();
        }



        public void AnimateIn(Canvas Parent,Media OldOne)
        {
         
         
            if (CheckAccess())
            {
                DoubleAnimation ani = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(1000)));


                if (OldOne != null)
                {
                    OldOne.AnimateAway(Parent);
                    OldOne = null;
                }
                ani.Completed += delegate
                {

                    App.DoEvents();
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                          {
                                                                                              Opacity = 1.0;
                                                                                          }));
                };
                this.BeginAnimation(OpacityProperty, ani);
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                {
                    DoubleAnimation ani = new DoubleAnimation(0.0, 1.0, new Duration(TimeSpan.FromMilliseconds(250)));

                    ani.Completed += delegate
                    {

                        App.DoEvents();
                        Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                        {


                            Opacity = 1.0;

                        }));
                    };
                    this.BeginAnimation(Control.OpacityProperty, ani);
                }));

            }
        }
        public void Show()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                  {
                                                                                        //    this.Visibility = Visibility.Visible;
                                                                                  }));


        }

        protected virtual void timer_Tick(object sender, EventArgs e)
        {
            // Once it has expired we might as well stop the timer?
            timer.Stop();

            SignalElapsedEvent();
        }

        /// <summary>
        /// Signals that an event is elapsed
        /// Will raise a DurationElapsedEvent
        /// </summary>
        public void SignalElapsedEvent()
        {
            this.hasExpired = true;

            System.Diagnostics.Debug.WriteLine("Media Complete", "Media - SignalElapsedEvent");

            DurationElapsedEvent();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    try
        //    {
        //        timer.Dispose();
        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        // Some things dont have a timer
        //        System.Diagnostics.Debug.WriteLine(ex.Message);
        //    }

        //    base.Dispose(disposing);
        //}


        public delegate void DurationElapsedDelegate();

        public event DurationElapsedDelegate DurationElapsedEvent;

        #region Properties

        /// <summary>
        /// Gets or Sets the duration of this media. Will be 0 if ""
        /// </summary>
        public int Duration
        {
            get { return this.duration; }
            set { duration = value; }
        }

        #endregion

        //Variables for size and position


        public bool hasExpired = false;

        //timer vars
        private Timer timer;
        private int duration;
        private bool timerStarted = false;

        public virtual void Dispose()
        {
            try
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }

            }
            catch (NullReferenceException ex)
            {
                // Some things dont have a timer
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }


        }

        public bool HasOnLoaded = false;
     
        public Media OldOne ;
        public virtual void OnLoaded()
        {
            AnimateIn((Canvas) Parent,OldOne);
        }

        public void AnimateAway(Canvas Parent)
        {
           

            if (CheckAccess())
            {
                DoubleAnimation ani = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromMilliseconds(1550)));

                ani.Completed += delegate
                {

                    App.DoEvents();
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                    {


                        this.Hide
                            ();
                        // Remove the controls
                        Parent
                            .
                            Children
                            .
                            Remove
                            (this);
                        this
                            .
                            Dispose
                            ();

                        //            RemoveVisualChild();

                    }));
                };
                this.BeginAnimation(OpacityProperty, ani);
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                      {
                                                                                          DoubleAnimation ani = new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromMilliseconds(250)));

                                                                                          ani.Completed += delegate
                                                                                          {

                                                                                              App.DoEvents();
                                                                                              Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                              {


                                                                                                  this.Hide
                                                                                                      ();
                                                                                                  // Remove the controls
                                                                                                  Parent
                                                                                                      .
                                                                                                      Children
                                                                                                      .
                                                                                                      Remove
                                                                                                      (this);
                                                                                                  this
                                                                                                      .
                                                                                                      Dispose
                                                                                                      ();

                                                                                                  //            RemoveVisualChild();

                                                                                              }));
                                                                                          };
                                                                                          this.BeginAnimation(Control.OpacityProperty, ani);
                                                                                      }));

            }
        }
    }
}
