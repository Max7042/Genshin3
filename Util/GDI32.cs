using System;
using System.Runtime.InteropServices;

namespace GenshinOverlay {
    public class GDI32 {
        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] public extern static bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")] public extern static IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)] public extern static IntPtr CreateCompatibleDC([In] IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")] public extern static bool DeleteDC([In] IntPtr hdc);
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")] [return: MarshalAs(UnmanagedType.Bool)] public extern static bool DeleteObject([In] IntPtr hObject);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")] public extern static IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
        [DllImport("user32.dll", SetLastError = true)] public static extern IntPtr GetWindowDC(IntPtr window);
        [DllImport("gdi32.dll", SetLastError = true)] public static extern uint GetPixel(IntPtr dc, int x, int y);
        [DllImport("user32.dll", SetLastError = true)] public static extern int ReleaseDC(IntPtr window, IntPtr dc);
        [DllImport("gdi32.dll")] public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSY = 90,
        }

        public enum TernaryRasterOperations : uint {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062,
            CAPTUREBLT = 0x40000000
        }
    }
}
