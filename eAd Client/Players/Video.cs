using ClientApp.Players;

namespace ClientApp
{
    using ClientApp.Core;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    internal class Video : Media
    {
        private int duration;
        private string filePath;
        private VideoPlayer videoPlayer;

        public Video(RegionOptions options) : base(options.Width, options.Height, options.Top, options.Left)
        {
            this.filePath = options.Uri;
            this.duration = options.Duration;
            this.videoPlayer = new VideoPlayer();
            this.videoPlayer.Width = options.Width;
            this.videoPlayer.Height = options.Height;
            videoPlayer.SetSize(new Size(options.Width,options.Height));
            this.videoPlayer.Location = new Point(0.0, 0.0);
            base.MediaCanvas.Children.Add(this.videoPlayer);
        }

        public override void Dispose()
        {
            Action method = null;
            try
            {
                this.videoPlayer.Hide();
                if (method == null)
                {
                    method =new Action(()=>{
                        base.MediaCanvas.Children.Remove(this.videoPlayer);
                    });
                }
                base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
                this.videoPlayer.Dispose();
            }
            catch
            {
            }
            base.Dispose();
        }

        public override void Pause()
        {
            this.videoPlayer.Pause();
        }

        public override void UnPause()
    {
        this.videoPlayer.UnPause();
    }

        public override void RenderMedia()
        {
            if (this.duration == 0)
            {
                base.Duration = 1;
            }
            base.RenderMedia();
            this.videoPlayer.Show();
            try
            {
                this.videoPlayer.StartPlayer(this.filePath);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception.Message);
                base.TimerTick(null, null);
            }
        }

        

        protected override void TimerTick(object sender, EventArgs e)
        {
            if (this.duration == 0)
            {
                if (this.videoPlayer.FinishedPlaying)
                {
                    base.TimerTick(sender, e);
                }
            }
            else
            {
                base.TimerTick(sender, e);
            }
        }
    }
}

