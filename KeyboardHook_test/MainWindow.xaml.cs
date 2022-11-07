using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Runtime.CompilerServices.RuntimeHelpers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace KeyboardHook_test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        KeyboardHook keyboardHook = null;
        KeyStatus keyStatus = null;

        public MainWindow()
        {
            this.InitializeComponent();

            keyboardHook = new KeyboardHook();
            keyStatus = new KeyStatus();
            
            keyboardHook.KeyDownEvent += KeyboardHook_KeyDownEvent;
            keyboardHook.KeyUpEvent += KeyboardHook_KeyUpEvent;
        }

        private void KeyboardHook_KeyUpEvent(object sender, KeyEventArg e)
        {
            if (keyStatus.getKey(e.KeyCode))
            {
                keyStatus.delKey(e.KeyCode);
                //Debug.WriteLine("UP : " + KeyCode);
            }
        }

        private void KeyboardHook_KeyDownEvent(object sender, KeyEventArg e)
        {
            if (!keyStatus.getKey(e.KeyCode))
            {
                keyStatus.setKey(e.KeyCode);
                //Debug.WriteLine("DOWN : " + KeyCode);
                if (keyStatus.compare()) Debug.WriteLine("match config");
            }
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            keyboardHook.Hook();
            Debug.WriteLine("Hook!");
        }
        private void myButton_Click_2(object sender, RoutedEventArgs e)
        {
            keyboardHook.HookEnd();
            Debug.WriteLine("UnHook!");
        }
    }
}
