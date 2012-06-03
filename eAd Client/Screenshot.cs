namespace ClientApp
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public static class Screenshot
    {
        public static MemoryStream GetJpgImage(this UIElement source, double scale, int quality)
        {
            RenderTargetBitmap renderTargetBitmap = source.GetRenderTargetBitmap(scale);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder {
                QualityLevel = quality
            };
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream;
            }
        }

        public static RenderTargetBitmap GetRenderTargetBitmap(this UIElement source, double scale)
        {
            if ((source.RenderSize.Height == 0.0) || (source.RenderSize.Width == 0.0))
            {
                return null;
            }
            double height = source.RenderSize.Height;
            double width = source.RenderSize.Width;
            double num3 = height * scale;
            double num4 = width * scale;
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int) num4, (int) num3, 96.0, 96.0, PixelFormats.Pbgra32);
            VisualBrush brush = new VisualBrush(source);
            DrawingVisual visual = new DrawingVisual();
            DrawingContext context = visual.RenderOpen();
            using (context)
            {
                context.PushTransform(new ScaleTransform(scale, scale));
                context.DrawRectangle(brush, null, new Rect(new Point(0.0, 0.0), new Point(width, height)));
            }
            bitmap.Render(visual);
            return bitmap;
        }
    }
}

