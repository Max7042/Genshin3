using System;
using System.Drawing;
using Tesseract;

namespace GenshinOverlay {
    public class IMG {
        public static decimal Capture(IntPtr handle, Point pos, Size size) {
            if(handle == IntPtr.Zero) { return 0; }

            Bitmap b = CaptureWindowArea(handle, pos, size);
            if(b == null) { return 0; }

            using(Pix pixc = PixConverter.ToPix(b)) {
                b.Dispose();
                using(Pix pixg = pixc.ConvertRGBToGray(0, 0, 0)) {
                    using(Pix pixs = pixg.ScaleGrayLI(Config.OCRScaleFactor, Config.OCRScaleFactor)) {
                        //pix = pix.UnsharpMaskingGray(5, 2.5f); //issues with light text on light bg
                        using(Pix pixb = pixs.BinarizeOtsuAdaptiveThreshold(2000, 2000, 0, 0, 0.0f)) {
                            float pixAvg = pixb.AverageOnLine(0, 0, pixb.Width - 1, 0, 1);
                            pixAvg += pixb.AverageOnLine(0, pixb.Height - 1, pixb.Width - 1, pixb.Height - 1, 1);
                            pixAvg += pixb.AverageOnLine(0, 0, 0, pixb.Height - 1, 1);
                            pixAvg += pixb.AverageOnLine(pixb.Width - 1, 0, pixb.Width - 1, pixb.Height - 1, 1);
                            pixAvg /= 4.0f;
                            using(Pix pixi = pixAvg > 0.5f ? pixb.Invert() : pixb) {
                                using(Pix pixn = pixi.SelectBySize(Config.OCRNoiseSize, Config.OCRNoiseSize, Config.OCRNoiseConnectivity, Config.OCRNoiseType, Config.OCRNoiseRelation)) {
                                    //pixn.ClipToForeground(IntPtr.Zero); --todo
                                    using(Pix pix = pixn.AddBorder(Config.OCRPadding, 0)) {
                                        pix.XRes = 300;
                                        pix.YRes = 300;

                                        string ocrText = "";
                                        using(TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default)) {
                                            using(Page page = engine.Process(pix, PageSegMode.SingleLine)) {
                                                ocrText = page.GetText();
                                                if(page.GetMeanConfidence() >= Config.OCRMinimumConfidence) {
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
