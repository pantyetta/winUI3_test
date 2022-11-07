using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Capture;
using Windows.UI.Core;
using WinRT;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Screenshot_test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        string savePath = @"C:\Users\pancho\Desktop\";


        [DllImport("user32.dll")]
        extern static IntPtr GetForegroundWindow();

        WindowCapture windowCapture = null;
        AreaCapture areaCapture = null;
        MouseHook mouseHook = null;
        public MainWindow()
        {
            this.InitializeComponent();
            windowCapture = new WindowCapture();
            areaCapture = new AreaCapture();
            mouseHook = new MouseHook();

            mouseHook.LButtonDownEvent += MouseHook_LButtonDownEvent;
            mouseHook.LButtonUpEvent += MouseHook_LButtonUpEvent;


        }

        private void MouseHook_LButtonUpEvent(object sender, MouseEventArg e)
        {
            Debug.WriteLine($"Left: {e.Point.X} , {e.Point.Y}");
            mouseHook.HookEnd();
        }

        private void MouseHook_LButtonDownEvent(object sender, MouseEventArg e)
        {
            Debug.WriteLine($"Click: {e.Point.X} , {e.Point.Y}");
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";

            await Task.Delay(2000);
            Debug.WriteLine("shot!");

            IntPtr hWnd = GetForegroundWindow();
            //var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Bitmap bitmap = windowCapture.shot(hWnd);
            Debug.WriteLine(bitmap != null ? true : false);


            bitmap.Save(savePath + "shot.png", System.Drawing.Imaging.ImageFormat.Png);
            bitmap.Dispose();

        }

        private void myButton_Click_2(object sender, RoutedEventArgs e)
        {
            mouseHook.Hook();
        }

        private void myButton_Click_3(object sender, RoutedEventArgs e)
        {
            mouseHook.HookEnd();
        }

        private void myButton_Click_4(object sender, RoutedEventArgs e)
        {
            areaCapture.Hook();
        }

        private void myButton_Click_5(object sender, RoutedEventArgs e)
        {
            Bitmap bitmap = areaCapture.ScreenCapture();
            bitmap.Save(savePath + "full.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private async void myButton_Click_6(object sender, RoutedEventArgs e)
        {
            windowCapture.Hook();

            //var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            //await windowCapture.StartCaptureAsync(hWnd);
        }

    }
}
