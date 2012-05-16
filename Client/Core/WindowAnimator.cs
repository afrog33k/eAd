using System;
using System.Timers;
using System.Windows.Controls;

namespace ClientApp.Core
{
public class WindowAnimator
{
    readonly UserControl _window;
    float _step;
    Timer _time;
    Direction _dir;

    public enum Direction
    {
        FadeIn, FadeOut
    }

    bool _fadeOut = false;

    public WindowAnimator(UserControl formToAnimate)
    {
        _window = formToAnimate;
    }

    public void WindowFadeIn(int interval, float steps, Direction direction)
    {
        //Save steps
        _step = steps;

        //Create Timer
        _time = new Timer();
        _time.Interval = interval;
        if (direction == Direction.FadeIn)
        {
            _time.Elapsed += new ElapsedEventHandler(TimerTickIn);
        }
        else
        {
            _time.Elapsed += new ElapsedEventHandler(TimerTickOut);
        }

        _time.Start();
    }

    private void TimerTickIn(object sender, EventArgs e)
    {
        //Check the Opacity of the form
        if (_window.Opacity != 1.0)
        {
            //Lower then 1, increment opacity
            _window.Opacity += _step;
        }
        else
        {
            //We´re finished, stop the timer
            _time.Stop();

            try
            {
                FadeComplete(_window);
            }
            catch (Exception)
            {
                // There might not be an event handler
            }
        }
    }

    private void TimerTickOut(object sender, EventArgs e)
    {
        //Check the Opacity of the form
        if (_window.Opacity != 0.0)
        {
            //Lower then 1, increment opacity
            _window.Opacity -= _step;
        }
        else
        {
            //We´re finished, stop the timer
            _time.Stop();

            FadeComplete(_window);
        }
    }

    public delegate void FadeCompleteDelegate(UserControl f);
    public event FadeCompleteDelegate FadeComplete;
}
}