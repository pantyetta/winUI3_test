using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ABI.Windows.Foundation;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.Graphics.Capture;
using WinRT.Interop;

namespace Screenshot_test
{
    public class WindowCapture
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll")]
        private static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("dwmapi.dll")]
        private static extern IntPtr DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("User32.dll")]
        private static extern IntPtr WindowFromPoint(POINT pt);

        [DllImport("User32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        public WindowCapture()
        {
            mouseHook = new MouseHook();
            mouseHook.LButtonDownEvent += MouseHook_LButtonDownEvent;
            mouseHook.LButtonUpEvent += MouseHook_LButtonUpEvent;
        }

        private void MouseHook_LButtonUpEvent(object sender, MouseEventArg e)
        {
            mouseHook.HookEnd();
        }

        private void MouseHook_LButtonDownEvent(object sender, MouseEventArg e)
        {
            POINT pt = new POINT();
            pt.x = e.Point.X;
            pt.y = e.Point.Y;

            IntPtr hWnd = WindowFromPoint(pt);
            Bitmap bitmap = shot(GetAncestor(hWnd, GA_ROOT));

            Debug.WriteLine(bitmap != null ? true : false);

            string savePath = @"C:\Users\pancho\Desktop\";

            bitmap.Save(savePath + "window.png", System.Drawing.Imaging.ImageFormat.Png);
            bitmap.Dispose();
        }

        public void Hook()
        {
            mouseHook.Hook();
        }

        private const int SRCCOPY = 13369376;
        int DWMWA_EXTENDED_FRAME_BOUNDS = 9;
        const uint GA_ROOT = 2;
        MouseHook mouseHook = null;

        public Bitmap shot(IntPtr handle)
        {
            Graphics graphics = null;
            IntPtr winDC = IntPtr.Zero;
            IntPtr hDC = IntPtr.Zero;

            try
            {
                winDC = GetWindowDC(handle);

                DwmGetWindowAttribute(handle, DWMWA_EXTENDED_FRAME_BOUNDS, out RECT bounds, Marshal.SizeOf(typeof(RECT)));
                GetWindowRect(handle, out RECT rect);

                var offsetX = bounds.left - rect.left;
                var offsetY = bounds.top - rect.top;

                Bitmap bmp = new Bitmap(bounds.right - bounds.left, bounds.bottom - bounds.top);

                graphics = Graphics.FromImage(bmp);
                hDC = graphics.GetHdc();

                BitBlt(hDC, 0, 0, bmp.Width, bmp.Height, winDC, offsetX, offsetY, SRCCOPY);
                return bmp;
            }
            catch(Exception e) 
            {
                Debug.WriteLine(message: e.Message);
                return null;
            }
            finally
            {
                if (graphics != null && hDC != IntPtr.Zero)
                    graphics.ReleaseHdc(hDC);
                if (graphics != null)
                    graphics.Dispose();
                if (winDC != IntPtr.Zero)
                    ReleaseDC(handle, winDC);
            }

        }


        public async Task StartCaptureAsync(IntPtr hWnd)
        {
            // The GraphicsCapturePicker follows the same pattern the
            // file pickers do.
            var picker = new GraphicsCapturePicker();
            InitializeWithWindow.Initialize(picker, hWnd);
            var item = await picker.PickSingleItemAsync();

        }
    }
}
