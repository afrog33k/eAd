using ClientApp.Players;

namespace ClientApp
{
    using ClientApp.Core;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    internal class Picture : Media
    {
        private readonly string _filePath;
        private readonly MediaElement _pictureBox;

        public Picture(RegionOptions options) : base(options.Width, options.Height, options.Top, options.Left)
        {
            this._filePath = options.Uri;
            if (!File.Exists(this._filePath))
            {
                Trace.WriteLine(new LogMessage("Image - Dispose", "Cannot Create image object. Invalid Filepath."), LogType.Error.ToString());
            }
            else
            {
                try
                {
                    this._pictureBox = new MediaElement();
                    this._pictureBox.Stretch = Stretch.Fill;
                    Uri uriSource = new Uri(this._filePath.Replace(@"\", "/"), UriKind.Relative);
                    this._pictureBox.Source = uriSource;
                    base.MediaCanvas.Background = new ImageBrush(new BitmapImage(uriSource));
                    base.SnapShot = uriSource;
                    this._pictureBox.Width = base.Width;
                    this._pictureBox.Height = base.Height;
                    this._pictureBox.HorizontalAlignment = HorizontalAlignment.Center;
                    this._pictureBox.VerticalAlignment = VerticalAlignment.Center;
                    this._pictureBox.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
                    this._pictureBox.Loaded += new RoutedEventHandler(this.PictureBoxLoaded);
                    base.MediaCanvas.Children.Add(this._pictureBox);
                    base.HasOnLoaded = true;
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(new LogMessage("Picture", string.Format("Cannot create Image Object with exception: {0}", exception.Message)), LogType.Error.ToString());
                }
            }
        }

        public override void Dispose()
        {
            base.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action(()=>{
                try
                {
                    if (this._pictureBox != null)
                    {
                        base.MediaCanvas.Children.Remove(this._pictureBox);
                    }
                }
                catch (Exception exception)
                {
                    Trace.WriteLine(new LogMessage("Image - Dispose", string.Format("Cannot dispose Image Object with exception: {0}", exception.Message)), LogType.Error.ToString());
                }
            }));
            base.Dispose();
        }

        private void PictureBoxLoaded(object sender, RoutedEventArgs e)
        {
            this.OnLoaded();
        }
    }
}

