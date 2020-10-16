using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;

using D2D1 = SharpDX.Direct2D1;
using DW = SharpDX.DirectWrite;
using DxColor = SharpDX.Color;
using DxSize = SharpDX.Size2;

namespace GenshinOverlay {
    public partial class OverlayWindow : Form {
        public IntPtr CurrentHandle = IntPtr.Zero;
        public IntPtr GenshinHandle = IntPtr.Zero;
        private IntPtr OverlayHandle;
        private D2D1.Factory Factory;
        private DW.Factory DWFactory;
        private DW.TextFormat TextFormat;
        private WindowRenderTarget Render;

        private SolidColorBrush FG1ColorBrush;
        private SolidColorBrush FG2ColorBrush;
        private SolidColorBrush SELColorBrush;
        private SolidColorBrush BGColorBrush;
        private SolidColorBrush ConfigColorBrush1;
        private SolidColorBrush ConfigColorBrush2;
        private SolidColorBrush ConfigColorBrush3;

        public bool IsConfiguring = false;
        private bool IsCleared = true;
        private bool IsPaused = false;
        private int pauseDelay = 0;

        public OverlayWindow() {
            InitializeComponent();
            OverlayHandle = Handle;
            BackColor = System.Drawing.Color.Black;
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            Visible = true;
            int initialStyle = User32.GetWindowLong(OverlayHandle, User32.WindowLong.GWL_EXSTYLE);
            User32.SetWindowLong(Handle, User32.WindowLong.GWL_EXSTYLE, initialStyle | (int)User32.ExtendedWindowStyles.WS_EX_LAYERED | (int)User32.ExtendedWindowStyles.WS_EX_TRANSPARENT);

            Factory = new D2D1.Factory(FactoryType.SingleThreaded);
            DWFactory = new DW.Factory(DW.FactoryType.Shared);
            TextFormat = new DW.TextFormat(DWFactory, "Segoe UI", 11f);
            HwndRenderTargetProperties renderProps = new HwndRenderTargetProperties {
                Hwnd = OverlayHandle,
                PixelSize = new DxSize(Width, Height),
                PresentOptions = PresentOptions.None
            };
            Render = new WindowRenderTarget(Factory, new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied)), renderProps);
            Render.AntialiasMode = AntialiasMode.PerPrimitive;
            ConfigColorBrush1 = new SolidColorBrush(Render, ARGBPackedColor(255, 255, 0, 0));
            ConfigColorBrush2 = new SolidColorBrush(Render, ARGBPackedColor(255, 225, 165, 0));
            ConfigColorBrush3 = new SolidColorBrush(Render, ARGBPackedColor(255, 155, 225, 0));
            InitializeBrushes();

            new Thread(() => {
                while(!IsDisposed) {
                    if(CurrentHandle != IntPtr.Zero && !IsConfiguring) {
                        Party.SelectedCharacter = Party.GetSelectedCharacter(CurrentHandle);
                        if(Party.SelectedCharacter == -1) {
                            if(!IsPaused && pauseDelay == 0) {
                                IsPaused = true;
                                pauseDelay = Config.PauseDelay;
                                foreach(Character c in Party.Characters) {
                                    if(c.Cooldown >= Config.CooldownPauseSubtraction) { c.Cooldown -= Config.CooldownPauseSubtraction; }
                                }
                            }
                        } else {
                            if(pauseDelay > 0) { pauseDelay -= 100; }
                            IsPaused = false;
                        }
                    } else if(GenshinHandle != IntPtr.Zero && !IsConfiguring) {
                        Party.SelectedCharacter = Party.GetSelectedCharacter(CurrentHandle);
                    }

                    Thread.Sleep(100);
                }
            }).Start();

            new Thread(() => {
                Stopwatch sw = Stopwatch.StartNew();
                decimal cdTick;
                while(!IsDisposed) {
                    cdTick = (decimal)sw.ElapsedMilliseconds / 1000;
                    sw.Restart();
                    if(Party.SelectedCharacter != -1 || (GenshinHandle != IntPtr.Zero && !IsPaused)) {
                        foreach(Character c in Party.Characters) {
                            if(c.Cooldown > 0) {
                                if(c.Cooldown - cdTick < 0) {
                                    c.Cooldown = 0;
                                } else {
                                    c.Cooldown -= cdTick;
                                }
                            }
                        }
                    }

                    if((IsConfiguring && GenshinHandle != IntPtr.Zero) || (!IsConfiguring && CurrentHandle != IntPtr.Zero && Party.SelectedCharacter != -1)) {
                        IsCleared = false;
                        UpdateOverlayBounds(OverlayHandle, IsConfiguring ? GenshinHandle : CurrentHandle);

                        Render.BeginDraw();
                        Render.Clear(DxColor.Transparent);

                        if(IsConfiguring) {
                            if(Config.CooldownBarMode <= 3) {
                                DrawLine(0.0M, 100.0M, 0, 1);
                                DrawLine(75.0M, 100.0M, 1, 1);
                                if(Party.PartySize > 2) { DrawLine(25.0M, 100.0M, 2, 1); }
                                if(Party.PartySize > 3) { DrawLine(60.0M, 100.0M, 3, 1); }
                            } else {
                                DrawCircle(0.0M, 100.0M, 0, 1);
                                DrawCircle(75.0M, 100.0M, 1, 1);
                                if(Party.PartySize > 2) { DrawCircle(25.0M, 100.0M, 2, 1); }
                                if(Party.PartySize > 3) { DrawCircle(60.0M, 100.0M, 3, 1); }
                            }

                            Render.DrawRectangle(new SharpDX.RectangleF(Config.CooldownTextLocation.X, Config.CooldownTextLocation.Y, Config.CooldownTextSize.Width, Config.CooldownTextSize.Height), ConfigColorBrush1, 1f);

                            DrawDot("4#1", Config.PartyNumLocations["4 #1"].X, Config.PartyNumLocations["4 #1"].Y, ConfigColorBrush1);
                            DrawDot("4#2", Config.PartyNumLocations["4 #2"].X, Config.PartyNumLocations["4 #2"].Y, ConfigColorBrush1);
                            DrawDot("4#3", Config.PartyNumLocations["4 #3"].X, Config.PartyNumLocations["4 #3"].Y, ConfigColorBrush1);
                            DrawDot("4#4", Config.PartyNumLocations["4 #4"].X, Config.PartyNumLocations["4 #4"].Y, ConfigColorBrush1);
                            DrawDot("3#1", Config.PartyNumLocations["3 #1"].X, Config.PartyNumLocations["3 #1"].Y, ConfigColorBrush2);
                            DrawDot("3#2", Config.PartyNumLocations["3 #2"].X, Config.PartyNumLocations["3 #2"].Y, ConfigColorBrush2);
                            DrawDot("3#3", Config.PartyNumLocations["3 #3"].X, Config.PartyNumLocations["3 #3"].Y, ConfigColorBrush2);
                            DrawDot("2#1", Config.PartyNumLocations["2 #1"].X, Config.PartyNumLocations["2 #1"].Y, ConfigColorBrush3);
                            DrawDot("2#2", Config.PartyNumLocations["2 #2"].X, Config.PartyNumLocations["2 #2"].Y, ConfigColorBrush3);
                        } else {
                            for(int i = 0; i < Party.PartySize; i++) {
                                if(Config.CooldownBarMode <= 3) {
                                    DrawLine(Party.Characters[i].Cooldown, Party.Characters[i].Max, i, Party.SelectedCharacter);
                                } else {
                                    DrawCircle(Party.Characters[i].Cooldown, Party.Characters[i].Max, i, Party.SelectedCharacter);
                                }
                            }
                        }

                        Render.Flush();
                        Render.EndDraw();
                    } else {
                        if(Render != null && !IsCleared) {
                            IsCleared = true;
                            Render.BeginDraw();
                            Render.Clear(DxColor.Transparent);
                            Render.Flush();
                            Render.EndDraw();
                        }
                    }
                    Thread.Sleep(Config.CooldownTickRateInMs);
                }
            }).Start();
        }

        private void DrawDot(string text, int x, int y, SolidColorBrush b) {
            Render.DrawRectangle(new SharpDX.RectangleF(x, y, 1, 1), b, 1f);
            Render.DrawText(text, TextFormat, new SharpDX.RectangleF(x - 24, y - 8, 22, 6), b);
        }

        private void DrawLine(decimal currentValue, decimal maxValue, int thisIndex, int selectedIndex) {
            int partyIndex = (Party.PartySize - 4) * -1;
            PointF location = new PointF(Config.CooldownBarLocation.X + (Config.CooldownBarXOffset * thisIndex), Config.CooldownBarLocation.Y + (Config.CooldownBarYOffsets[partyIndex] * thisIndex) + Config.PartyNumBarOffsets[partyIndex]);
            if(thisIndex == selectedIndex) {
                if(Config.CooldownBarYOffsets[partyIndex] > 0) {
                    location.X += Config.CooldownBarSelOffset;
                } else {
                    location.Y += Config.CooldownBarSelOffset;
                }
            }
            Size scale = new Size(Config.CooldownBarSize.Width, Config.CooldownBarSize.Height);
            int capStyle = Config.CooldownBarMode;
            int val = (int)(scale.Width - (scale.Width * (currentValue / (maxValue + 0.01M))));

            using(StrokeStyle strokeStyleFG = new StrokeStyle(Factory, 
                new StrokeStyleProperties() { StartCap = capStyle > 3 ? CapStyle.Flat : (CapStyle)capStyle, EndCap = capStyle > 3 || val < scale.Width ? CapStyle.Flat : (CapStyle)capStyle })) {
                
                if((val < scale.Width) && ((thisIndex != selectedIndex && FG1ColorBrush.Color.A > 0) || (thisIndex == selectedIndex && SELColorBrush.Color.A > 0))) {
                    if(BGColorBrush.Color.A > 0) {
                        using(StrokeStyle strokeStyleBG = new StrokeStyle(Factory, 
                            new StrokeStyleProperties() { StartCap = val == 0 ? (CapStyle)capStyle : CapStyle.Flat, EndCap = capStyle > 3 ? CapStyle.Flat : (CapStyle)capStyle })) {

                            Render.DrawLine(new Vector2(location.X + val, location.Y), new Vector2(location.X + scale.Width, location.Y), BGColorBrush, scale.Height, strokeStyleBG);
                        }
                    }
                    Render.DrawLine(new Vector2(location.X, location.Y), new Vector2(location.X + val, location.Y), thisIndex == selectedIndex ? SELColorBrush : FG1ColorBrush, scale.Height, strokeStyleFG);
                } else if(val == scale.Width && FG2ColorBrush.Color.A > 0) {
                    Render.DrawLine(new Vector2(location.X, location.Y), new Vector2(location.X + val, location.Y), FG2ColorBrush, scale.Height, strokeStyleFG);
                }
            }
        }

        private void DrawCircle(decimal currentValue, decimal maxValue, int thisIndex, int selectedIndex) {
            int partyIndex = (Party.PartySize - 4) * -1;
            PointF location = new PointF(Config.CooldownBarLocation.X + (Config.CooldownBarXOffset * thisIndex), Config.CooldownBarLocation.Y + (Config.CooldownBarYOffsets[partyIndex] * thisIndex) + Config.PartyNumBarOffsets[partyIndex]);
            if(thisIndex == selectedIndex) {
                if(Config.CooldownBarYOffsets[partyIndex] > 0) {
                    location.X += Config.CooldownBarSelOffset;
                } else {
                    location.Y += Config.CooldownBarSelOffset;
                }
            }
            int scale = Config.CooldownBarSize.Height;
            int lineWidth = Config.CooldownBarSize.Width;
            bool drawPie = Config.CooldownBarSize.Width > Config.CooldownBarSize.Height * 2;

            int val = (int)(360 - (360 * (currentValue / (maxValue + 0.01M))));
            Vector2 circleCenter = new Vector2(location.X, location.Y);
            Vector2 circleStart = new Vector2(circleCenter.X, circleCenter.Y - scale);
            Vector2 circleStartBG = new Vector2(circleCenter.X, circleCenter.Y + scale);
            float thetaDegrees = (float)(val * 0.0174533);
            float thetaDegreesBG = (float)((360 - (180 - val)) * 0.0174533);
            Vector2 circleEnd = Matrix3x2.TransformPoint(Matrix3x2.Rotation(thetaDegrees, circleCenter), circleStart);
            Vector2 circleEndBG = Matrix3x2.TransformPoint(Matrix3x2.Rotation(thetaDegreesBG, circleCenter), circleStartBG);
            float dx = circleStart.X - circleCenter.X;
            float dy = circleStart.Y - circleCenter.Y;
            float radius = (float)Math.Sqrt(dx * dx + dy * dy);

            if(val < 360 && BGColorBrush.Color.A > 0 && ((thisIndex != selectedIndex && FG1ColorBrush.Color.A > 0) || (thisIndex == selectedIndex && SELColorBrush.Color.A > 0))) {
                using(PathGeometry pathGeometry = new PathGeometry(Factory)) {
                    using(GeometrySink geometrySink = pathGeometry.Open()) {
                        if(val <= 180) {
                            geometrySink.BeginFigure(drawPie ? circleCenter : circleStartBG, FigureBegin.Filled);
                            if(drawPie) { geometrySink.AddLine(circleStartBG); }
                            geometrySink.AddArc(new ArcSegment() {
                                Point = (val == 0) ? circleStart : circleEndBG,
                                Size = new Size2F(radius, radius),
                                RotationAngle = 0.0f,
                                SweepDirection = SweepDirection.CounterClockwise,
                                ArcSize = ArcSize.Small
                            });
                            geometrySink.EndFigure(drawPie ? FigureEnd.Closed : FigureEnd.Open);
                        }
                        geometrySink.BeginFigure(drawPie ? circleCenter : circleStart, FigureBegin.Filled);
                        if(drawPie) { geometrySink.AddLine(circleStart); }
                        geometrySink.AddArc(new ArcSegment() {
                            Point = (val <= 180) ? circleStartBG : circleEndBG,
                            Size = new Size2F(radius, radius),
                            RotationAngle = 0.0f,
                            SweepDirection = SweepDirection.CounterClockwise,
                            ArcSize = ArcSize.Small
                        });
                        geometrySink.EndFigure(drawPie ? FigureEnd.Closed : FigureEnd.Open);
                        geometrySink.Close();
                        if(drawPie) {
                            Render.FillGeometry(pathGeometry, BGColorBrush);
                        } else {
                            //using(StrokeStyle strokeStyle = new StrokeStyle(Factory, 
                            //    new StrokeStyleProperties() { DashCap = CapStyle.Flat, DashStyle = DashStyle.Solid, LineJoin = LineJoin.Miter, StartCap = CapStyle.Flat, EndCap = CapStyle.Flat, MiterLimit = 0, DashOffset = 0 })) {
                            //}
                            Render.DrawGeometry(pathGeometry, BGColorBrush, lineWidth); //, strokeStyle);
                        }
                    }
                }
            }

            if((thisIndex != selectedIndex && FG1ColorBrush.Color.A > 0) || (thisIndex == selectedIndex && SELColorBrush.Color.A > 0) || (FG2ColorBrush.Color.A > 0 && val == 360)) {
                using(PathGeometry pathGeometry = new PathGeometry(Factory)) {
                    using(GeometrySink geometrySink = pathGeometry.Open()) {
                        geometrySink.BeginFigure(drawPie ? circleCenter : circleStart, FigureBegin.Filled);
                        if(drawPie) { geometrySink.AddLine(circleStart); }
                        geometrySink.AddArc(new ArcSegment() {
                            Point = (val > 180) ? new Vector2(circleStart.X, circleStart.Y + (radius * 2)) : circleEnd,
                            Size = new Size2F(radius, radius),
                            RotationAngle = 0.0f,
                            SweepDirection = SweepDirection.Clockwise,
                            ArcSize = ArcSize.Small
                        });
                        geometrySink.EndFigure(drawPie ? FigureEnd.Closed : FigureEnd.Open);

                        if(val > 180) {
                            geometrySink.BeginFigure(drawPie ? circleCenter : new Vector2(circleStart.X, circleStart.Y + (radius * 2)), FigureBegin.Filled);
                            if(drawPie) { geometrySink.AddLine(new Vector2(circleStart.X, circleStart.Y + (radius * 2))); }
                            geometrySink.AddArc(new ArcSegment() {
                                Point = circleEnd,
                                Size = new Size2F(radius, radius),
                                RotationAngle = 0.0f,
                                SweepDirection = SweepDirection.Clockwise,
                                ArcSize = ArcSize.Small
                            });
                            geometrySink.EndFigure(drawPie ? FigureEnd.Closed : FigureEnd.Open);
                        }

                        geometrySink.Close();

                        if(drawPie) {
                            Render.FillGeometry(pathGeometry, val == 360 ? FG2ColorBrush : thisIndex != selectedIndex ? FG1ColorBrush : SELColorBrush);
                        } else {
                            //using(StrokeStyle strokeStyle = new StrokeStyle(Factory, 
                            //    new StrokeStyleProperties() { DashCap = CapStyle.Flat, DashStyle = DashStyle.Solid, LineJoin = LineJoin.Miter, StartCap = CapStyle.Flat, EndCap = CapStyle.Flat, MiterLimit = 0, DashOffset = 0 })) {
                            //}
                            Render.DrawGeometry(pathGeometry, val == 360 ? FG2ColorBrush : thisIndex != selectedIndex ? FG1ColorBrush : SELColorBrush, lineWidth); //, strokeStyle);
                        }
                    }
                }
            }
        }

        private void UpdateRenderer() {
            if(Render == null) { return; }
            Render.Resize(new DxSize(Width, Height));
        }

        private void InitializeBrushes() {
            System.Drawing.Color fg1 = (System.Drawing.Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarFG1Color);
            System.Drawing.Color fg2 = (System.Drawing.Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarFG2Color);
            System.Drawing.Color sel = (System.Drawing.Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarSelectedFGColor);
            System.Drawing.Color bg = (System.Drawing.Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarBGColor);

            FG1ColorBrush = new SolidColorBrush(Render, ARGBPackedColor(fg1));
            FG2ColorBrush = new SolidColorBrush(Render, ARGBPackedColor(fg2));
            SELColorBrush = new SolidColorBrush(Render, ARGBPackedColor(sel));
            BGColorBrush = new SolidColorBrush(Render, ARGBPackedColor(bg));
        }

        public void UpdateBrushes() {
            System.Drawing.Color fg1 = (System.Drawing.Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarFG1Color);
            System.Drawing.Color fg2 = (System.Drawing.Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarFG2Color);
            System.Drawing.Color sel = (System.Drawing.Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarSelectedFGColor);
            System.Drawing.Color bg = (System.Drawing.Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarBGColor);

            FG1ColorBrush.Color = ARGBPackedColor(fg1);
            FG2ColorBrush.Color = ARGBPackedColor(fg2);
            SELColorBrush.Color = ARGBPackedColor(sel);
            BGColorBrush.Color = ARGBPackedColor(bg);
        }

        private DxColor ARGBPackedColor(int a, int r, int g, int b) {
            return DxColor.FromRgba(r | g << 8 | b << 16 | a << 24);
        }
        private DxColor ARGBPackedColor(System.Drawing.Color c) {
            return DxColor.FromRgba(c.R | c.G << 8 | c.B << 16 | c.A << 24);
        }

        protected override void OnResize(EventArgs e) {
            int[] margins = new int[] { 0, 0, Width, Height };
            User32.DwmExtendFrameIntoClientArea(OverlayHandle, ref margins); //Handle
            UpdateRenderer();
        }

        private void UpdateOverlayBounds(IntPtr mainHandle, IntPtr clientHandle) {
            User32.DwmGetWindowAttribute(clientHandle, (int)User32.DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, out User32.RECT dstRect, Marshal.SizeOf<User32.RECT>());
            User32.SetWindowPos(mainHandle, clientHandle, dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }
        protected override void OnPaint(PaintEventArgs e) { }
    }
}
