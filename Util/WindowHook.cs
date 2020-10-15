using System;
using System.ComponentModel;
using System.Diagnostics;

namespace GenshinOverlay {
    public delegate void WinEventProc(IntPtr hWinEventHook, User32.SWEH_Events eventType, IntPtr hwnd, User32.SWEH_ObjectId idObject, long idChild, uint dwEventThread, uint dwmsEventTime);

    public class WindowEventArgs : HandledEventArgs {
        public IntPtr Handle { get; private set; }
        public WindowEventArgs(IntPtr hwnd) {
            Handle = hwnd;
        }
    }

    public class WindowHook {
        private readonly WinEventProc WinEventListener;
        private readonly IntPtr ForegroundEventHook = IntPtr.Zero;
        private IntPtr MainHandle = IntPtr.Zero;
        public event EventHandler<WindowEventArgs> WindowHandleChanged;
        private readonly string TargetProcessName;

        public WindowHook(string _targetProcessName) {
            TargetProcessName = _targetProcessName;
            WinEventListener = new WinEventProc(WinEventProcCallback);
            ForegroundEventHook = User32.SetWinEventHook(User32.SWEH_Events.EVENT_SYSTEM_FOREGROUND, User32.SWEH_Events.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, WinEventListener, 0, 0, User32.SWEH_dwFlags.WINEVENT_OUTOFCONTEXT | User32.SWEH_dwFlags.WINEVENT_SKIPOWNPROCESS);
        }

        public void Unhook() {
            User32.WinEventUnhook(ForegroundEventHook);
        }

        private void WinEventProcCallback(IntPtr hWinEventHook, User32.SWEH_Events dwEvent, IntPtr hWnd, User32.SWEH_ObjectId idObject, long idChild, uint dwEventThread, uint dwmsEventTime) {
            if(dwEvent == User32.SWEH_Events.EVENT_SYSTEM_FOREGROUND) {
                Process targetProc = GetProcess((int)User32.GetWindowProcessThread(hWnd));
                if(targetProc != null) {
                    MainHandle = (targetProc.ProcessName == TargetProcessName) ? targetProc.MainWindowHandle : IntPtr.Zero;
                    WindowHandleChanged(this, new WindowEventArgs(MainHandle));
                }
            }
        }

        private Process GetProcess(int id) {
            Process proc;
            try {
                proc = Process.GetProcessById(id); 
            } catch { 
                return null;
            }
            return proc;
        }
    }
}
