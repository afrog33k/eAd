using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClientApp
{
    public static class Screenshot
    {
        /// <summary>
        /// Gets a JPG "screenshot" of the current UIElement
        /// </summary>
        /// <param name="source">UIElement to screenshot</param>
        /// <param name="scale">Scale to render the screenshot</param>
        /// <param name="quality">JPG Quality</param>
        /// <returns>Byte array of JPG data</returns>
        public static MemoryStream GetJpgImage(this UIElement source, double scale, int quality)
        {
            RenderTargetBitmap renderTarget = source.GetRenderTargetBitmap(scale);

            var jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.QualityLevel = quality;
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            //Byte[] _imageArray;

            using (var outputStream = new MemoryStream())
            {
                jpgEncoder.Save(outputStream);
                return outputStream;
            }
        }

        public static RenderTargetBitmap GetRenderTargetBitmap(this UIElement source, double scale)
        {
            if (source.RenderSize.Height == 0 || source.RenderSize.Width == 0)
                return null;

            double actualHeight = source.RenderSize.Height;
            double actualWidth = source.RenderSize.Width;

            double renderHeight = actualHeight*scale;
            double renderWidth = actualWidth*scale;

            var renderTarget = new RenderTargetBitmap((int) renderWidth, (int) renderHeight, 96, 96,
                                                      PixelFormats.Pbgra32);
            var sourceBrush = new VisualBrush(source);

            var drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null,
                                             new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);
            return renderTarget;
        }
    }
}