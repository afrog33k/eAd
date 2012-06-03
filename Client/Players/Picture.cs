using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ClientApp.Core;

namespace ClientApp
{
    internal class Picture : Media
    {
        private readonly string _filePath;
        private readonly MediaElement _pictureBox;

        public Picture(RegionOptions options)
            : base(options.Width, options.Height, options.Top, options.Left)
        {
            _filePath = options.Uri;

            if (!File.Exists(_filePath))
            {
                // Exit
                Trace.WriteLine(new LogMessage("Image - Dispose", "Cannot Create image object. Invalid Filepath."),
                                LogType.Error.ToString());
                return;
            }

            try
            {
                _pictureBox = new MediaElement();

                _pictureBox.Stretch = Stretch.UniformToFill;
                var uri = new Uri(_filePath.Replace("\\", "/"), UriKind.Relative);
                _pictureBox.Source = (uri);
                MediaCanvas.Background = new ImageBrush(new BitmapImage(uri));
                // new Uri("pack://application:,,,/ApplicationName;component/"+_filePath);

                SnapShot = uri;

                _pictureBox.Width = Width;
                _pictureBox.Height = Height;
                //MediaGrid.Width = Width;
                //MediaGrid.Height = Height;

                _pictureBox.HorizontalAlignment = HorizontalAlignment.Center;
                _pictureBox.VerticalAlignment = VerticalAlignment.Center;
                _pictureBox.Margin = new Thickness(0, 0, 0, 0);

                //     _pictureBox.BorderStyle = BorderStyle.None;
                //     _pictureBox.BackColor = Color.Transparent;
                _pictureBox.Loaded += PictureBoxLoaded;
                MediaCanvas.Children.Add(_pictureBox);
                HasOnLoaded = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(
                    new LogMessage("Picture",
                                   String.Format("Cannot create Image Object with exception: {0}", ex.Message)),
                    LogType.Error.ToString());
            }
        }

        private void PictureBoxLoaded(object sender, RoutedEventArgs e)
        {
            OnLoaded();
        }

        public override void Dispose()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() =>
                                                                                  {
                                                                                      try
                                                                                      {
                                                                                          if (_pictureBox != null)
                                                                                              MediaCanvas.Children.
                                                                                                  Remove(_pictureBox);
                                                                                      }
                                                                                      catch (Exception ex)
                                                                                      {
                                                                                          Trace.WriteLine(
                                                                                              new LogMessage(
                                                                                                  "Image - Dispose",
                                                                                                  String.Format(
                                                                                                      "Cannot dispose Image Object with exception: {0}",
                                                                                                      ex.Message)),
                                                                                              LogType.Error.ToString());
                                                                                      }
                                                                                  }));

            //     _pictureBox.Image.Dispose();
            //     _pictureBox.Dispose();


            base.Dispose();
        }
    }
}