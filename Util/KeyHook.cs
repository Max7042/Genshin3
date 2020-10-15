using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace GenshinOverlay {
    public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookData lParam);
    public delegate void KeyboardHookEvent(int wParam, KeyboardHookData lParam);

    [Serializable]
    public struct KeyboardHookData {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    public class KeyHookEventArgs : HandledEventArgs {
        public Keys Key { get; private set; }

        public KeyHookEventArgs(Keys key) {
            Key = key;
        }
    }

    public class KeyboardHook {
        protected IntPtr hhook = IntPtr.Zero;
        protected KeyboardHookProc hookDelegate;
        public event EventHandler<KeyHookEventArgs> KeyUp;
        private List<Keys> HookedKeys;

        private enum KeyState : int {
            KeyDown = 0x100,
            KeyUp = 0x101,
            SysKeyDown = 0x104,
            SysKeyUp = 0x105
        }

        public KeyboardHook(List<Keys> hookedKeys) {
            HookedKeys = hookedKeys;
            Hook();
        }

        ~KeyboardHook() { Unhook(); }

        public virtual void Hook() {
            hookDelegate = new KeyboardHookProc(HookProc);
            IntPtr hInstance = User32.LoadLibrary("User32");
            hhook = User32.SetWindowsHookEx(13, hookDelegate, hInstance, 0);
        }

        public virtual void Unhook() {
            User32.UnhookWindowsHookEx(hhook);
        }

        private int HookProc(int code, int wParam, ref KeyboardHookData lParam) {
            if(code >= 0) {
                Keys key = (Keys)lParam.vkCode;
                if(HookedKeys.Contains(key)) {
                    if((wParam == (int)KeyState.KeyUp) && (KeyUp != null)) {
                        KeyUp(this, new KeyHookEventArgs(key));
                    }
                }
            }

            return User32.CallNextHookEx(hhook, code, wParam, ref lParam);
        }
    }
}
