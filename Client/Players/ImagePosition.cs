using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ClientApp.Core;

namespace ClientApp
{
class ImagePosition : Media
{
    private readonly string _filePath;
    readonly MediaElement _pictureBox;

    public ImagePosition(RegionOptions options)
    : base(options.Width, options.Height, options.Top, options.Left)
    {
        _filePath = options.Uri;

        if (!System.IO.File.Exists(_filePath))
        {
            // Exit
            System.Diagnostics.Trace.WriteLine(new LogMessage("Image - Dispose", "Cannot Create image object. Invalid Filepath."), LogType.Error.ToString());
            return;
        }

        try
        {
            _pictureBox = new MediaElement();

            _pictureBox.Stretch = Stretch.UniformToFill;

            _pictureBox.Source = (new Uri(_filePath.Replace("\\", "/"), UriKind.Relative));
            // new Uri("pack://application:,,,/ApplicationName;component/"+_filePath);


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
            System.Diagnostics.Trace.WriteLine(new LogMessage("ImagePosition", String.Format("Cannot create Image Object with exception: {0}", ex.Message)), LogType.Error.ToString());
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
                    MediaCanvas.Children.Remove(_pictureBox);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(new LogMessage("Image - Dispose", String.Format("Cannot dispose Image Object with exception: {0}", ex.Message)), LogType.Error.ToString());
            }
        }));

        //     _pictureBox.Image.Dispose();
        //     _pictureBox.Dispose();



        base.Dispose();
    }
}
}
