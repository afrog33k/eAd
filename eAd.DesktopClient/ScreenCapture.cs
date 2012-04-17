namespace DesktopClient
{

    using System;

    using System.Drawing;

    using System.Drawing.Imaging;

    using System.Runtime.InteropServices;



    public class ScreenCapture
    {

        public Image CaptureScreen()
        {

            return this.CaptureWindow(User32.GetDesktopWindow());

        }



        public void CaptureScreenToFile(string filename, ImageFormat format)
        {

            this.CaptureScreen().Save(filename, format);

        }



        public Image CaptureWindow(IntPtr handle)
        {

            IntPtr windowDC = User32.GetWindowDC(handle);

            User32.RECT rect = new User32.RECT();

            User32.GetWindowRect(handle, ref rect);

            int nWidth = rect.right - rect.left;

            int nHeight = rect.bottom - rect.top;

            IntPtr hDC = GDI32.CreateCompatibleDC(windowDC);

            IntPtr hObject = GDI32.CreateCompatibleBitmap(windowDC, nWidth, nHeight);

            IntPtr ptr4 = GDI32.SelectObject(hDC, hObject);

            GDI32.BitBlt(hDC, 0, 0, nWidth, nHeight, windowDC, 0, 0, 0xcc0020);

            GDI32.SelectObject(hDC, ptr4);

            GDI32.DeleteDC(hDC);

            User32.ReleaseDC(handle, windowDC);

            Image image = Image.FromHbitmap(hObject);

            GDI32.DeleteObject(hObject);

            return image;

        }



        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {

            this.CaptureWindow(handle).Save(filename, format);

        }



        private class GDI32
        {

            public const int SRCCOPY = 0xcc0020;



            [DllImport("gdi32.dll")]

            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

            [DllImport("gdi32.dll")]

            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]

            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("gdi32.dll")]

            public static extern bool DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll")]

            public static extern bool DeleteObject(IntPtr hObject);

            [DllImport("gdi32.dll")]

            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        }



        private class User32
        {

            [DllImport("user32.dll")]

            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]

            public static extern IntPtr GetWindowDC(IntPtr hWnd);

            [DllImport("user32.dll")]

            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

            [DllImport("user32.dll")]

            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);



            [StructLayout(LayoutKind.Sequential)]

            public struct RECT
            {

                public int left;

                public int top;

                public int right;

                public int bottom;

            }

        }

    }

}



