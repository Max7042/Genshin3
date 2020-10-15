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
using DxColor = SharpDX.Color;
using DxSize = SharpDX.Size2;

namespace GenshinOverlay {
    public partial class OverlayWindow : Form {
        public IntPtr CurrentHandle = IntPtr.Zero;
        public IntPtr GenshinHandle = IntPtr.Zero;
        private IntPtr OverlayHandle;
        private D2D1.Factory Factory;
        private WindowRenderTarget Render;
        private SolidColorBrush FG1ColorBrush;
        private SolidColorBrush FG2ColorBrush;
        private SolidColorBrush SELColorBrush;
        private SolidColorBrush BGColorBrush;
        private SolidColorBrush DebugColorBrush;

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
            HwndRenderTargetProperties renderProps = new HwndRenderTargetProperties {
                Hwnd = OverlayHandle,
                PixelSize = new DxSize(Width, Height),
                PresentOptions = PresentOptions.None
            };
            Render = new WindowRenderTarget(Factory, new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied)), renderProps);
            Render.AntialiasMode = AntialiasMode.PerPrimitive;
            DebugColorBrush = new SolidColorBrush(Render, DxColor.Red);
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
                    } else {
                        if(GenshinHandle != IntPtr.Zero) {
                            Party.SelectedCharacter = Party.GetSelectedCharacter(CurrentHandle);
                        }
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
                                DrawLine(25.0M, 100.0M, 2, 1);
                                DrawLine(60.0M, 100.0M, 3, 1);
                            } else {
                                DrawCircle(0.0M, 100.0M, 0, 1);
                                DrawCircle(75.0M, 100.0M, 1, 1);
                                DrawCircle(25.0M, 100.0M, 2, 1);
                                DrawCircle(60.0M, 100.0M, 3, 1);
                            }

                            Render.DrawRectangle(new SharpDX.RectangleF(Config.CooldownTextLocation.X, Config.CooldownTextLocation.Y, Config.CooldownTextSize.Width, Config.CooldownTextSize.Height), DebugColorBrush, 1f);

                            Render.DrawRectangle(new SharpDX.RectangleF(Config.PartyNumLocation.X, Config.PartyNumLocation.Y + (Config.PartyNumYOffset * 0), 1, 1), DebugColorBrush, 1f);
                            Render.DrawRectangle(new SharpDX.RectangleF(Config.PartyNumLocation.X, Config.PartyNumLocation.Y + (Config.PartyNumYOffset * 1), 1, 1), DebugColorBrush, 1f);
                            Render.DrawRectangle(new SharpDX.RectangleF(Config.PartyNumLocation.X, Config.PartyNumLocation.Y + (Config.PartyNumYOffset * 2), 1, 1), DebugColorBrush, 1f);
                            Render.DrawRectangle(new SharpDX.RectangleF(Config.PartyNumLocation.X, Config.PartyNumLocation.Y + (Config.PartyNumYOffset * 3), 1, 1), DebugColorBrush, 1f);
                        } else {
                            foreach(Character c in Party.Characters) {
                                if(Config.CooldownBarMode <= 3) {
                                    DrawLine(c.Cooldown, c.Max, Party.Characters.IndexOf(c), Party.SelectedCharacter);
                                } else {
                                    DrawCircle(c.Cooldown, c.Max, Party.Characters.IndexOf(c), Party.SelectedCharacter);
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

        private void DrawLine(decimal currentValue, decimal maxValue, int thisIndex, int selectedIndex) {
            PointF location = new PointF(Config.CooldownBarLocation.X + (Config.CooldownBarOffset.X * thisIndex), Config.CooldownBarLocation.Y + (Config.CooldownBarOffset.Y * thisIndex));
            if(thisIndex == selectedIndex) {
                if(Config.CooldownBarOffset.Y > 0) {
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
            PointF location = new PointF(Config.CooldownBarLocation.X + (Config.CooldownBarOffset.X * thisIndex), Config.CooldownBarLocation.Y + (Config.CooldownBarOffset.Y * thisIndex));
            if(thisIndex == selectedIndex) {
                if(Config.CooldownBarOffset.Y > 0) {
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
            System.Drawing.Color fg1 = (System.Drawing.Color)new ColorConverter().ConvertFromString(Config.CooldownBarFG1Color);
            System.Drawing.Color fg2 = (System.Drawing.Color)new ColorConverter().ConvertFromString(Config.CooldownBarFG2Color);
            System.Drawing.Color sel = (System.Drawing.Color)new ColorConverter().ConvertFromString(Config.CooldownBarSelectedFGColor);
            System.Drawing.Color bg = (System.Drawing.Color)new ColorConverter().ConvertFromString(Config.CooldownBarBGColor);

            FG1ColorBrush = new SolidColorBrush(Render, new DxColor(fg1.R, fg1.G, fg1.B, fg1.A));
            FG2ColorBrush = new SolidColorBrush(Render, new DxColor(fg2.R, fg2.G, fg2.B, fg2.A));
            SELColorBrush = new SolidColorBrush(Render, new DxColor(sel.R, sel.G, sel.B, sel.A));
            BGColorBrush = new SolidColorBrush(Render, new DxColor(bg.R, bg.G, bg.B, bg.A));
        }

        public void UpdateBrushes() {
            System.Drawing.Color fg1 = (System.Drawing.Color)new ColorConverter().ConvertFromString(Config.CooldownBarFG1Color);
            System.Drawing.Color fg2 = (System.Drawing.Color)new ColorConverter().ConvertFromString(Config.CooldownBarFG2Color);
            System.Drawing.Color sel = (System.Drawing.Color)new ColorConverter().ConvertFromString(Config.CooldownBarSelectedFGColor);
            System.Drawing.Color bg = (System.Drawing.Color)new ColorConverter().ConvertFromString(Config.CooldownBarBGColor);

            FG1ColorBrush.Color = DxColor.FromRgba(fg1.R | fg1.G << 8 | fg1.B << 16 | fg1.A << 24);
            FG2ColorBrush.Color = DxColor.FromRgba(fg2.R | fg2.G << 8 | fg2.B << 16 | fg2.A << 24);
            SELColorBrush.Color = DxColor.FromRgba(sel.R | sel.G << 8 | sel.B << 16 | sel.A << 24);
            BGColorBrush.Color = DxColor.FromRgba(bg.R | bg.G << 8 | bg.B << 16 | bg.A << 24);
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
