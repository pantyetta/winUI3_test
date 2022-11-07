using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;

namespace Screenshot_test
{
    internal class MouseHook
    {
        protected const int WH_MOUSE_LL = 14;

        [DllImport("User32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, MouseHookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("User32.dll")]
        private static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);
            
        [DllImport("User32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam);
        MouseHookProc mouseHookProc = null;

        private IntPtr hookId = IntPtr.Zero;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public void Hook()
        {
            if(hookId == IntPtr.Zero)
            {
                using(Process curProcess = Process.GetCurrentProcess())
                using(ProcessModule curModule = curProcess.MainModule)
                {
                    mouseHookProc = HookCallback;
                    hookId = SetWindowsHookEx(WH_MOUSE_LL, mouseHookProc, GetModuleHandle(curModule.ModuleName), 0);
                    Debug.WriteLine(message: "Mouse Hook");
                }
            }
        }

        public IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int WM_MOUSEMOVE = 0x0200;
            int WM_LBUTTONDOWN = 0x0201;
            int WM_LBUTTONUP = 0x0202;

            //int WM_RBUTTONDOWN = 0x0204;
            //int WM_RBUTTONUP = 0x0205;


            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                MSLLHOOKSTRUCT ms = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                LButtonDownEvent?.Invoke(this, new MouseEventArg(ms.pt.x, ms.pt.y));
            }

            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONUP)
            {
                MSLLHOOKSTRUCT ms = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                LButtonUpEvent?.Invoke(this, new MouseEventArg(ms.pt.x, ms.pt.y));
            }

            //  mouseMove以外は他アプリ(OSも)に渡さない
            if(nCode >= 0 && wParam != (IntPtr)WM_MOUSEMOVE)
            {
                return (IntPtr)1;
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public void HookEnd()
        {
            UnhookWindowsHookEx(hookId);
            hookId = IntPtr.Zero;
            Debug.WriteLine(message: "Hook End");
        }

        public delegate void MouseEventHandler(object sender, MouseEventArg e);
        public event MouseEventHandler LButtonDownEvent;
        public event MouseEventHandler LButtonUpEvent;
    }

    public class MouseEventArg : EventArgs
    {
        public MouseEventArg(int x, int y)
        {
            Point = new Point(x, y);
        }
        public Point Point
        {
            get;
            private set;
        }
    }
}
