using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardHook_test
{
    internal class KeyboardHook
    {
        protected const int KEYBOARDHOOK_LL = 0x000D;   //13
        protected const int KEYBOARDHOOK_KEY_DOWN = 0x0100;   //256
        protected const int KEYBOARDHOOK_KEY_UP = 0x0101;   //257
        protected const int KEYBOARDHOOK_SYSKEY_DOWN = 0x0104;   //260
        protected const int KEYBOARDHOOK_SYSKEY_UP = 0x0105;   //260


        delegate int delegateHookCallback(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, delegateHookCallback lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        IntPtr hookPtr = IntPtr.Zero;

        public void Hook()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                // フックを行う
                // 第1引数   フックするイベントの種類
                //   13はキーボードフックを表す
                // 第2引数 フック時のメソッドのアドレス
                //   フックメソッドを登録する
                // 第3引数   インスタンスハンドル
                //   現在実行中のハンドルを渡す
                // 第4引数   スレッドID
                //   0を指定すると、すべてのスレッドでフックされる
                hookPtr = SetWindowsHookEx(
                    KEYBOARDHOOK_LL,
                    HookCallback,
                    GetModuleHandle(curModule.ModuleName),
                    0
                );
            }
        }

        int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0) return 1;

            var KeyCode = Marshal.ReadInt32(lParam);    // フックしたキー
            switch ((int)wParam)
            {
                case KEYBOARDHOOK_KEY_DOWN: 
                    KeyDownEvent(this, new KeyEventArg(KeyCode));
                    break;
                case KEYBOARDHOOK_KEY_UP:
                    KeyUpEvent(this, new KeyEventArg(KeyCode));
                    break;
                default:
                    break;
            }
            
            // 1を戻すとフックしたキーが捨てられます
            return 1;
        }

        public void HookEnd()
        {
            UnhookWindowsHookEx(hookPtr);
            hookPtr = IntPtr.Zero;
        }

        public delegate void KeyEventHandler(object sender, KeyEventArg e);
        public event KeyEventHandler KeyDownEvent;
        public event KeyEventHandler KeyUpEvent;
    }

    public class KeyEventArg : EventArgs
    {
        public int KeyCode { get; }
        public KeyEventArg(int KeyCode)
        {
            this.KeyCode = KeyCode;
        }
    }
}
