using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ClientApp.Core;

namespace ClientApp
{
class Video : Media
{
    public Video(RegionOptions options)
    : base(options.Width, options.Height, options.Top, options.Left)
    {
        this.filePath = options.Uri;
        this.duration = options.Duration;

        videoPlayer = new VideoPlayer();
        videoPlayer.Width = options.Width;
        videoPlayer.Height = options.Height;
        videoPlayer.Location = new Point(0, 0);

        MediaCanvas.Children.Add(videoPlayer);
    }

    public override void RenderMedia()
    {
        if (duration == 0)
        {
            // Determine the end time ourselves
            base.Duration = 1; //check every second
        }

        base.RenderMedia();

        videoPlayer.Show();

        try
        {
            videoPlayer.StartPlayer(filePath);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(ex.Message);

            // Unable to start video - expire this media immediately
            base.timer_Tick(null, null);
        }
    }

    protected override void timer_Tick(object sender, EventArgs e)
    {
        if (duration == 0)
        {
            // Has the video finished playing
            if (videoPlayer.FinishedPlaying)
            {
                // Raise the expired tick which will clear this media
                base.timer_Tick(sender, e);
            }
        }
        else
        {
            // Our user defined timer duration has expired - so raise the base timer tick which will clear this media
            base.timer_Tick(sender, e);
        }

        return;
    }

    public override void Dispose()
    {


        try
        {
            videoPlayer.Hide();
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
            {
                MediaCanvas.Children.Remove(videoPlayer);
            }));

            videoPlayer.Dispose();
        }
        catch
        {

        }

        base.Dispose();
    }

    string filePath;
    VideoPlayer videoPlayer;
    private int duration;
}
}
