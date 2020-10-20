using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tesseract;

namespace GenshinOverlay {
    public static class IMG {
        private static TesseractEngine Engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
        public static ColorConverter ColorConverter = new ColorConverter();
        public static float DesktopScale = 1;

        public struct OCRCapture {
            public decimal Cooldown { get; set; }
            public float Confidence { get; set; }
            public int Iterations { get; set; }
            public string Text { get; set; }

            public OCRCapture(decimal _cooldown = 0, float _confidence = 0, int _iterations = 0, string _text = "") {
                Cooldown = _cooldown;
                Confidence = _confidence;
                Iterations = _iterations;
                Text = _text;
            }
        }

        public static void Capture(IntPtr handle, Point pos, Size size, ref OCRCapture ocrCapture, bool debug = false) {
            if(handle == IntPtr.Zero) { return; }
            Bitmap b = CaptureWindowArea(handle, pos, size);
            if(b == null) { return; }
            if(debug) { Directory.CreateDirectory(Application.StartupPath + @"\debug"); }
            if(debug) { b.Save(Application.StartupPath + @"\debug\00_input.png"); }
            using(Pix pixc = PixConverter.ToPix(b)) {
                b.Dispose();
                using(Pix pixg = pixc.ConvertRGBToGray(0, 0, 0)) {
                    if(debug) { pixg.Save(Application.StartupPath + @"\debug\01_grayscale.png"); }
                    using(Pix pixs = pixg.ScaleGrayLI(Config.OCRScaleFactor, Config.OCRScaleFactor)) {
                        if(debug) { pixs.Save(Application.StartupPath + @"\debug\02_scale.png"); }
                        //pix = pix.UnsharpMaskingGray(5, 2.5f); //issues with light text on light bg
                        using(Pix pixb = pixs.BinarizeOtsuAdaptiveThreshold(2000, 2000, 0, 0, 0.0f)) {
                            if(debug) { pixb.Save(Application.StartupPath + @"\debug\03_binarize.png"); }
                            float pixAvg = pixb.AverageOnLine(0, 0, pixb.Width - 1, 0, 1);
                            pixAvg += pixb.AverageOnLine(0, pixb.Height - 1, pixb.Width - 1, pixb.Height - 1, 1);
                            pixAvg += pixb.AverageOnLine(0, 0, 0, pixb.Height - 1, 1);
                            pixAvg += pixb.AverageOnLine(pixb.Width - 1, 0, pixb.Width - 1, pixb.Height - 1, 1);
                            pixAvg /= 4.0f;
                            using(Pix pixi = pixAvg > 0.5f ? pixb.Invert() : pixb) {
                                if(debug) { pixi.Save(Application.StartupPath + $@"\debug\04_invert_{pixAvg > 0.5f}.png"); }
                                using(Pix pixn = pixi.SelectBySize(Config.OCRNoiseSize, Config.OCRNoiseSize, Config.OCRNoiseConnectivity, Config.OCRNoiseType, Config.OCRNoiseRelation)) {
                                    if(debug) { pixn.Save(Application.StartupPath + @"\debug\05_removenoise.png"); }
                                    //pixn.ClipToForeground(IntPtr.Zero);
                                    using(Pix pix = pixn.AddBorder(Config.OCRPadding, 0)) {
                                        if(debug) { pix.Save(Application.StartupPath + @"\debug\06_border.png"); }
                                        pix.XRes = 300;
                                        pix.YRes = 300;

                                        using(Page page = Engine.Process(pix, PageSegMode.SingleLine)) {
                                            ocrCapture.Text = page.GetText().Trim();
                                            ocrCapture.Confidence = page.GetMeanConfidence();
                                            ocrCapture.Iterations++;
                                            if(ocrCapture.Confidence >= Config.OCRMinimumConfidence) {
                                                if(decimal.TryParse(ocrCapture.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal cooldown)) {
                                                    if(cooldown < Config.CooldownMaxPossible) {
                                                        ocrCapture.Cooldown = cooldown;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void GetDesktopScale() {
            IntPtr desktop = User32.GetWindowDC(IntPtr.Zero);
            int LogicalScreenHeight = GDI32.GetDeviceCaps(desktop, (int)GDI32.DeviceCap.VERTRES);
            int PhysicalScreenHeight = GDI32.GetDeviceCaps(desktop, (int)GDI32.DeviceCap.DESKTOPVERTRES);
            int logpixelsy = GDI32.GetDeviceCaps(desktop, (int)GDI32.DeviceCap.LOGPIXELSY);
            User32.ReleaseDC(IntPtr.Zero, desktop);
            float screenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            float dpiScalingFactor = (float)logpixelsy / 96.0f;
            DesktopScale = screenScalingFactor > 1 ? screenScalingFactor : dpiScalingFactor;
        }

        private static Bitmap CaptureWindowArea(IntPtr handle, Point cropPos, Size cropSize) {
            IntPtr dc = User32.GetWindowDC(handle);
            if(dc != null) {
                IntPtr hdcDest = GDI32.CreateCompatibleDC(dc);
                IntPtr hBitmap = GDI32.CreateCompatibleBitmap(dc, cropSize.Width, cropSize.Height);
                IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
                if(GDI32.BitBlt(hdcDest, 0, 0, cropSize.Width, cropSize.Height, dc, cropPos.X, cropPos.Y, GDI32.TernaryRasterOperations.SRCCOPY)) {
                    GDI32.SelectObject(hdcDest, hOld);
                    GDI32.DeleteDC(hdcDest);
                    User32.ReleaseDC(handle, dc);
                    Bitmap b = Image.FromHbitmap(hBitmap);
                    GDI32.DeleteObject(hBitmap);
                    for(int x = 0; x < b.Width; x++) {
                        for(int y = 0; y < b.Height; y++) {
                            if(b.GetPixel(x, y) != Color.FromArgb(255, 0, 0, 0)) {
                                return b;
                            }
                        }
                    }
                    b.Dispose();
                } else {
                    GDI32.DeleteDC(hdcDest);
                    User32.ReleaseDC(handle, dc);
                    GDI32.DeleteObject(hBitmap);
                }
            }
            return null;
        }

        public static Color GetColorAt(IntPtr handle, int x, int y) {
            IntPtr dc = GDI32.GetWindowDC(handle);
            if(dc != null) {
                int a = (int)GDI32.GetPixel(dc, x, y);
                GDI32.ReleaseDC(handle, dc);
                return Color.FromArgb(255, (a >> 0) & 0xff, (a >> 8) & 0xff, (a >> 16) & 0xff);
            }
            return Color.FromArgb(255, 255, 255, 255);
        }

        public static Size Scaled(this Size size, int factor) {
            return new Size((int)(size.Width * ((float)factor / 100)), (int)(size.Height * ((float)factor / 100)));
        }

        public static Point Scaled(this Point pos, int factor) {
            return new Point((int)(pos.X * ((float)factor / 100)), (int)(pos.Y * ((float)factor / 100)));
        }

        public static int Scaled(this int val, int factor) {
            return (int)(val * ((float)factor / 100));
        }

        public static int[] Scaled(this int[] values, int factor) {
            return values.Select(x => (int)(x * ((float)factor / 100))).ToArray();
        }

        public static float Scaled(this float val, int factor) {
            return val * ((float)factor / 100);
        }

        public static float[] Scaled(this float[] values, int factor) {
            return values.Select(x => x * ((float)factor / 100)).ToArray();
        }

        public static Dictionary<string, Point> Scaled(this Dictionary<string, Point> dict, int factor) {
            Dictionary<string, Point> d = dict.ToDictionary(x => x.Key, x => x.Value);
            foreach(string key in d.Keys.ToList()) {
                d[key] = new Point((int)(d[key].X * ((float)factor / 100)), (int)(d[key].Y * ((float)factor / 100)));
            }
            return d;
        }
    }
}
