using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace GenshinOverlay {
    public class User32 {
        [DllImport("user32.dll", SetLastError = true)] public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll")] public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")] public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")] public static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")] public static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")] public static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookData lParam);
        [DllImport("kernel32.dll")] public static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("user32.dll", SetLastError = false)] public static extern IntPtr SetWinEventHook(SWEH_Events eventMin, SWEH_Events eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, uint idProcess, uint idThread, SWEH_dwFlags dwFlags);
        [DllImport("user32.dll", SetLastError = false)] public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        [DllImport("user32.dll", SetLastError = true)] public static extern void GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")] public static extern int GetWindowLong(IntPtr hWnd, WindowLong nIndex);
        [DllImport("user32.dll")] public static extern int SetWindowLong(IntPtr hWnd, WindowLong nIndex, int dwNewLong);
        [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
        [DllImport("dwmapi.dll")] public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);
        [DllImport("dwmapi.dll")] public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] margins);

        public enum WindowLong {
            GWL_EXSTYLE = -20
        }
        public enum ExtendedWindowStyles {
            WS_EX_TRANSPARENT = 0x20,
            WS_EX_LAYERED = 0x80000
        }

        [Flags]
        public enum DwmWindowAttribute : uint {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_LAST
        }

        public enum SWEH_dwFlags : uint {
            WINEVENT_OUTOFCONTEXT = 0x0000,
            WINEVENT_SKIPOWNTHREAD = 0x0001,
            WINEVENT_SKIPOWNPROCESS = 0x0002,
            WINEVENT_INCONTEXT = 0x0004
        }

        public enum SWEH_Events : uint {
            EVENT_MIN = 0x00000001,
            EVENT_MAX = 0x7FFFFFFF,
            EVENT_SYSTEM_SOUND = 0x0001,
            EVENT_SYSTEM_ALERT = 0x0002,
            EVENT_SYSTEM_FOREGROUND = 0x0003,
            EVENT_SYSTEM_MENUSTART = 0x0004,
            EVENT_SYSTEM_MENUEND = 0x0005,
            EVENT_SYSTEM_MENUPOPUPSTART = 0x0006,
            EVENT_SYSTEM_MENUPOPUPEND = 0x0007,
            EVENT_SYSTEM_CAPTURESTART = 0x0008,
            EVENT_SYSTEM_CAPTUREEND = 0x0009,
            EVENT_SYSTEM_MOVESIZESTART = 0x000A,
            EVENT_SYSTEM_MOVESIZEEND = 0x000B,
            EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C,
            EVENT_SYSTEM_CONTEXTHELPEND = 0x000D,
            EVENT_SYSTEM_DRAGDROPSTART = 0x000E,
            EVENT_SYSTEM_DRAGDROPEND = 0x000F,
            EVENT_SYSTEM_DIALOGSTART = 0x0010,
            EVENT_SYSTEM_DIALOGEND = 0x0011,
            EVENT_SYSTEM_SCROLLINGSTART = 0x0012,
            EVENT_SYSTEM_SCROLLINGEND = 0x0013,
            EVENT_SYSTEM_SWITCHSTART = 0x0014,
            EVENT_SYSTEM_SWITCHEND = 0x0015,
            EVENT_SYSTEM_MINIMIZESTART = 0x0016,
            EVENT_SYSTEM_MINIMIZEEND = 0x0017,
            EVENT_SYSTEM_DESKTOPSWITCH = 0x0020,
            EVENT_SYSTEM_END = 0x00FF,
            EVENT_OEM_DEFINED_START = 0x0101,
            EVENT_OEM_DEFINED_END = 0x01FF,
            EVENT_UIA_EVENTID_START = 0x4E00,
            EVENT_UIA_EVENTID_END = 0x4EFF,
            EVENT_UIA_PROPID_START = 0x7500,
            EVENT_UIA_PROPID_END = 0x75FF,
            EVENT_CONSOLE_CARET = 0x4001,
            EVENT_CONSOLE_UPDATE_REGION = 0x4002,
            EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003,
            EVENT_CONSOLE_UPDATE_SCROLL = 0x4004,
            EVENT_CONSOLE_LAYOUT = 0x4005,
            EVENT_CONSOLE_START_APPLICATION = 0x4006,
            EVENT_CONSOLE_END_APPLICATION = 0x4007,
            EVENT_CONSOLE_END = 0x40FF,
            EVENT_OBJECT_CREATE = 0x8000,
            EVENT_OBJECT_DESTROY = 0x8001,
            EVENT_OBJECT_SHOW = 0x8002,
            EVENT_OBJECT_HIDE = 0x8003,
            EVENT_OBJECT_REORDER = 0x8004,
            EVENT_OBJECT_FOCUS = 0x8005,
            EVENT_OBJECT_SELECTION = 0x8006,
            EVENT_OBJECT_SELECTIONADD = 0x8007,
            EVENT_OBJECT_SELECTIONREMOVE = 0x8008,
            EVENT_OBJECT_SELECTIONWITHIN = 0x8009,
            EVENT_OBJECT_STATECHANGE = 0x800A,
            EVENT_OBJECT_LOCATIONCHANGE = 0x800B,
            EVENT_OBJECT_NAMECHANGE = 0x800C,
            EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D,
            EVENT_OBJECT_VALUECHANGE = 0x800E,
            EVENT_OBJECT_PARENTCHANGE = 0x800F,
            EVENT_OBJECT_HELPCHANGE = 0x8010,
            EVENT_OBJECT_DEFACTIONCHANGE = 0x8011,
            EVENT_OBJECT_ACCELERATORCHANGE = 0x8012,
            EVENT_OBJECT_INVOKED = 0x8013,
            EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014,
            EVENT_OBJECT_CONTENTSCROLLED = 0x8015,
            EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016,
            EVENT_OBJECT_END = 0x80FF,
            EVENT_AIA_START = 0xA000,
            EVENT_AIA_END = 0xAFFF
        }

        public enum SWEH_ObjectId : long {
            OBJID_WINDOW = 0x00000000,
            OBJID_SYSMENU = 0xFFFFFFFF,
            OBJID_TITLEBAR = 0xFFFFFFFE,
            OBJID_MENU = 0xFFFFFFFD,
            OBJID_CLIENT = 0xFFFFFFFC,
            OBJID_VSCROLL = 0xFFFFFFFB,
            OBJID_HSCROLL = 0xFFFFFFFA,
            OBJID_SIZEGRIP = 0xFFFFFFF9,
            OBJID_CARET = 0xFFFFFFF8,
            OBJID_CURSOR = 0xFFFFFFF7,
            OBJID_ALERT = 0xFFFFFFF6,
            OBJID_SOUND = 0xFFFFFFF5,
            OBJID_QUERYCLASSNAMEIDX = 0xFFFFFFF4,
            OBJID_NATIVEOM = 0xFFFFFFF0
        }

        public static bool WinEventUnhook(IntPtr hWinEventHook) {
            return UnhookWinEvent(hWinEventHook);
        }

        public static uint GetWindowProcessThread(IntPtr hWnd) {
            new UIPermission(UIPermissionWindow.AllWindows).Demand();
            GetWindowThreadProcessId(hWnd, out uint procId);
            return procId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom) {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X {
                get { return Left; }
                set { Right -= (Left - value); Left = value; }
            }

            public int Y {
                get { return Top; }
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public System.Drawing.Point Location {
                get { return new System.Drawing.Point(Left, Top); }
                set { X = value.X; Y = value.Y; }
            }

            public System.Drawing.Size Size {
                get { return new System.Drawing.Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }

            public static implicit operator System.Drawing.Rectangle(RECT r) {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(System.Drawing.Rectangle r) {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2) {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2) {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r) {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj) {
                if(obj is RECT)
                    return Equals((RECT)obj);
                else if(obj is System.Drawing.Rectangle)
                    return Equals(new RECT((System.Drawing.Rectangle)obj));
                return false;
            }

            public bool Empty() {
                return Left == 0 && Right == 0 && Top == 0 && Bottom == 0;
            }

            public override int GetHashCode() {
                return ((System.Drawing.Rectangle)this).GetHashCode();
            }

            public override string ToString() {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }
    }
}
