namespace ClientApp.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Timers;
    using System.Windows.Controls;

    public class WindowAnimator
    {
        private Direction _dir;
        private bool _fadeOut;
        private float _step;
        private System.Timers.Timer _time;
        private readonly UserControl _window;
     

        public event FadeCompleteDelegate FadeComplete;

        public WindowAnimator(UserControl formToAnimate)
        {
            this._window = formToAnimate;
        }

        private void TimerTickIn(object sender, EventArgs e)
        {
            if (this._window.Opacity != 1.0)
            {
                this._window.Opacity += this._step;
            }
            else
            {
                this._time.Stop();
                try
                {
                    this.FadeComplete(this._window);
                }
                catch (Exception)
                {
                }
            }
        }

        private void TimerTickOut(object sender, EventArgs e)
        {
            if (this._window.Opacity != 0.0)
            {
                this._window.Opacity -= this._step;
            }
            else
            {
                this._time.Stop();
                this.FadeComplete(this._window);
            }
        }

        public void WindowFadeIn(int interval, float steps, Direction direction)
        {
            this._step = steps;
            this._time = new System.Timers.Timer();
            this._time.Interval = interval;
            if (direction == Direction.FadeIn)
            {
                this._time.Elapsed += new ElapsedEventHandler(this.TimerTickIn);
            }
            else
            {
                this._time.Elapsed += new ElapsedEventHandler(this.TimerTickOut);
            }
            this._time.Start();
        }

        public enum Direction
        {
            FadeIn,
            FadeOut
        }

        public delegate void FadeCompleteDelegate(UserControl f);
    }
}

