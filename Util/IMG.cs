using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Tesseract;

namespace GenshinOverlay {
    public class IMG {
        public static ColorConverter ColorConverter = new ColorConverter();
        public static float Confidence = 0;
        public static string Text = "";

        public static decimal Capture(IntPtr handle, Point pos, Size size, bool debug = false) {
            if(handle == IntPtr.Zero) { return 0; }

            Bitmap b = CaptureWindowArea(handle, pos, size);
            if(b == null) { return 0; }
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

                                        string ocrText = "";
                                        using(TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                                            using(Page page = engine.Process(pix, PageSegMode.SingleLine)) {
                                                ocrText = page.GetText().Trim();
                                                Confidence = page.GetMeanConfidence();
                                                if(Confidence >= Config.OCRMinimumConfidence) {
                                                    Text = ocrText;
                                                    if(decimal.TryParse(ocrText, out decimal cooldown)) {
                                                        if(cooldown < Config.CooldownMaxPossible) {
                                                            return cooldown;
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

            return 0;
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
    }
}
