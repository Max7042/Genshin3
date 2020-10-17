using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;

namespace GenshinOverlay {
    public partial class MainWindow : MetroForm {
        private OverlayWindow OverlayWindow;
        private KeyboardHook KeyHook;

        private WindowHook WinHook;

        public MainWindow() {
            InitializeComponent();
        }
        
        private void MainWindow_Load(object sender, EventArgs e) {
            ConfigPanel.Visible = false;

            if(!IsAdmin()) {
                DialogResult res = MetroMessageBox.Show(this, $"\nGenshinOverlay must be started as Administrator.", "Genshin Overlay - Error", MessageBoxButtons.OK, Theme, MessageBoxDefaultButton.Button1, 135);
                if(res == DialogResult.OK) {
                    Environment.Exit(0);
                }
            }

            Config.Load();

            Theme = (MetroThemeStyle)Config.ConfigTheme;
            MStyleManager.Theme = (MetroThemeStyle)Config.ConfigTheme;
            OverlayWindow = new OverlayWindow();

            WinHook = new WindowHook(Config.ProcessName);
            WinHook.WindowHandleChanged += WinHook_WindowHandleChanged;
            
            KeyHook = new KeyboardHook(new List<Keys>() {  Keys.E });
            KeyHook.KeyUp += KeyHook_KeyUp;

            if(Config.CooldownTextLocation == Point.Empty || Config.PartyNumLocations["4 #1"] == Point.Empty) {
                ConfigureOverlayMessage.Visible = true;
                ConfigureOverlayButton.Location = new Point(ConfigureOverlayButton.Location.X, ConfigureOverlayButton.Location.Y + 20);
            } else {
                ConfigureOverlayMessage.Visible = false;
            }

            Activate();
            FocusMe();
        }

        public static bool IsAdmin() {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void WinHook_WindowHandleChanged(object sender, WindowEventArgs e) {
            OverlayWindow.CurrentHandle = e.Handle;
            if(e.Handle != IntPtr.Zero) {
                OverlayWindow.GenshinHandle = OverlayWindow.CurrentHandle;
            }
        }

        private void KeyHook_KeyUp(object sender, KeyHookEventArgs e) {
            if(OverlayWindow.GenshinHandle == IntPtr.Zero) { return; }
            if(Config.CooldownTextLocation == Point.Empty || Config.PartyNumLocations["4 #1"] == Point.Empty) { return; }
            if(e.Key == Keys.E) {
                if(Party.SelectedCharacter == -1 || Party.Characters[Party.SelectedCharacter].Cooldown > Config.CooldownMinimumReapply || Party.Characters[Party.SelectedCharacter].Processing) { return; }
                int c = Party.SelectedCharacter;
                Party.Characters[c].Processing = true;

                new Thread(() => {
                    Thread.Sleep(100);
                    Point captureLocation = new Point(Config.CooldownTextLocation.X, Config.CooldownTextLocation.Y);
                    Size captureSize = new Size(Config.CooldownTextSize.Width, Config.CooldownTextSize.Height);
                    
                    decimal currentCooldown = IMG.Capture(OverlayWindow.CurrentHandle, captureLocation, captureSize);
                    while(c == Party.SelectedCharacter && currentCooldown == 0) {
                        Thread.Sleep(100);
                        currentCooldown = IMG.Capture(OverlayWindow.CurrentHandle, captureLocation, captureSize);
                    }
                    if(c != Party.SelectedCharacter) {
                        Party.Characters[c].Cooldown = 0;
                        Party.Characters[c].Max = 0;
                    } else {
                        if(Config.CooldownOverride[c] > 0) { 
                            if(currentCooldown < Config.CooldownMinimumOverride) {
                                Party.Characters[c].Cooldown = Config.CooldownOverride[c];
                                Party.Characters[c].Max = Config.CooldownOverride[c];
                            } else {
                                Party.Characters[c].Cooldown = currentCooldown;
                                Party.Characters[c].Max = currentCooldown;
                            }
                        } else {
                            Party.Characters[c].Cooldown = currentCooldown + Config.CooldownOffset;
                            Party.Characters[c].Max = Party.Characters[c].Cooldown;
                        }
                    }
                    Party.Characters[c].Processing = false;
                }).Start();
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e) {
            KeyHook.Unhook();
            WinHook.Unhook();
            Environment.Exit(0);
        }

        #region "Config":
        private void ConfigureOverlayButton_Click(object sender, EventArgs e) {
            Process proc = Process.GetProcesses().Where(x => x.ProcessName == Config.ProcessName).FirstOrDefault();
            if(proc == null) {
                MetroMessageBox.Show(this, $"\nGenshin Impact must be running first.", "Process Error", MessageBoxButtons.OK, Theme, MessageBoxDefaultButton.Button1, 135);
                return;
            } else {
                OverlayWindow.GenshinHandle = proc.MainWindowHandle;
            }

            User32.GetClientRect(OverlayWindow.GenshinHandle, out User32.RECT rect);
            if(rect.Empty()) {
                MetroMessageBox.Show(this, $"\nGenshin Impact client area could not be detected.\n> Please ensure Genshin Impact is not minimized.\n> Fullscreen is not supported but fullscreen borderless is.", "Client Error", MessageBoxButtons.OK, Theme, MessageBoxDefaultButton.Button1, 160);
                return;
            }

            OverlayWindow.IsConfiguring = true;
            if(ConfigureOverlayMessage.Visible) {
                ConfigureOverlayMessage.Visible = false;
                ConfigureOverlayButton.Location = new Point(ConfigureOverlayButton.Location.X, ConfigureOverlayButton.Location.Y - 20);
            }
            ConfigureOverlayButton.Visible = false;
            ConfigPanel.Visible = true;

            CooldownBarsYOffsetText.ForeColor = Color.FromArgb(255, 255, 0, 0);

            CooldownTextXPosTrack.Maximum = rect.Width;
            CooldownTextYPosTrack.Maximum = rect.Height;
            PartyNumXPosTrack.Maximum = rect.Width;
            PartyNumYPosTrack.Maximum = rect.Height;
            CooldownBarsXPosTrack.Maximum = rect.Width;
            CooldownBarsYPosTrack.Maximum = rect.Height;
            CooldownTextXPosTrack.MouseWheelBarPartitions = CooldownTextXPosTrack.Maximum - CooldownTextXPosTrack.Minimum;
            CooldownTextYPosTrack.MouseWheelBarPartitions = CooldownTextYPosTrack.Maximum - CooldownTextYPosTrack.Minimum;
            PartyNumXPosTrack.MouseWheelBarPartitions = PartyNumXPosTrack.Maximum - PartyNumXPosTrack.Minimum;
            PartyNumYPosTrack.MouseWheelBarPartitions = PartyNumYPosTrack.Maximum - PartyNumYPosTrack.Minimum;
            CooldownBarsXPosTrack.MouseWheelBarPartitions = CooldownBarsXPosTrack.Maximum - CooldownBarsXPosTrack.Minimum;
            CooldownBarsYPosTrack.MouseWheelBarPartitions = CooldownBarsYPosTrack.Maximum - CooldownBarsYPosTrack.Minimum;

            if(Config.CooldownTextLocation == Point.Empty || Config.PartyNumLocations["4 #1"] == Point.Empty) {
                AssumeDefaultValues(rect);
            }
            UpdateControlValues();
        }

        private void AssumeDefaultValues(User32.RECT rect) {
            Template template = Config.Templates.Find(x => x.Resolution == rect.Size);
            if(template != null) {
                Config.CooldownTextLocation = template.Properties.CooldownTextLocation;
                Config.CooldownTextSize = template.Properties.CooldownTextSize;
                Config.PartyNumLocations = template.Properties.PartyNumLocations;
                Config.PartyNumBarOffsets = template.Properties.PartyNumBarOffsets;
                Config.CooldownBarLocation = template.Properties.CooldownBarLocation;
                Config.CooldownBarSize = template.Properties.CooldownBarSize;
                Config.CooldownBarXOffset = template.Properties.CooldownBarXOffset;
                Config.CooldownBarYOffsets = template.Properties.CooldownBarYOffsets;
            }
        }

        private void UpdateControlValues() {
            CooldownTextXPosTrack.Value = Config.CooldownTextLocation.X > CooldownTextXPosTrack.Maximum ? CooldownTextXPosTrack.Maximum : Config.CooldownTextLocation.X;
            CooldownTextYPosTrack.Value = Config.CooldownTextLocation.Y > CooldownTextYPosTrack.Maximum ? CooldownTextYPosTrack.Maximum : Config.CooldownTextLocation.Y;
            CooldownTextWidthTrack.Value = Config.CooldownTextSize.Width > CooldownTextWidthTrack.Maximum ? CooldownTextWidthTrack.Maximum : Config.CooldownTextSize.Width;
            CooldownTextHeightTrack.Value = Config.CooldownTextSize.Height > CooldownTextHeightTrack.Maximum ? CooldownTextHeightTrack.Maximum : Config.CooldownTextSize.Height;

            if(PartyNumComboBox.SelectedItem == null) {
                PartyNumComboBox.SelectedItem = "4 #1";
            }
            UpdatePartyNumTrackValues();

            ConfigTabControl.SelectedIndex = 1;

            CooldownBarsXPosTrack.Value = Config.CooldownBarLocation.X > CooldownBarsXPosTrack.Maximum ? CooldownBarsXPosTrack.Maximum : Config.CooldownBarLocation.X;
            CooldownBarsYPosTrack.Value = Config.CooldownBarLocation.Y > CooldownBarsYPosTrack.Maximum ? CooldownBarsYPosTrack.Maximum : Config.CooldownBarLocation.Y;
            CooldownBarsWidthTrack.Value = Config.CooldownBarSize.Width > CooldownBarsWidthTrack.Maximum ? CooldownBarsWidthTrack.Maximum : Config.CooldownBarSize.Width;
            CooldownBarsHeightTrack.Value = Config.CooldownBarSize.Height > CooldownBarsHeightTrack.Maximum ? CooldownBarsHeightTrack.Maximum : Config.CooldownBarSize.Height;
            CooldownBarsXOffsetTrack.Value = (int)(Config.CooldownBarXOffset * 10) > CooldownBarsXOffsetTrack.Maximum ? CooldownBarsXOffsetTrack.Maximum : (int)(Config.CooldownBarXOffset * 10);
            CooldownBarsModeTrack.Value = Config.CooldownBarMode > CooldownBarsModeTrack.Maximum ? CooldownBarsModeTrack.Maximum : Config.CooldownBarMode;
            CooldownBarsSelOffsetTrack.Value = Config.CooldownBarSelOffset > CooldownBarsSelOffsetTrack.Maximum ? CooldownBarsSelOffsetTrack.Maximum : Config.CooldownBarSelOffset;

            CooldownBarTextXOffsetTrack.Value = (int)(Config.CooldownBarTextOffset.X * 10) > CooldownBarTextXOffsetTrack.Maximum ? CooldownBarTextXOffsetTrack.Maximum : (int)(Config.CooldownBarTextOffset.X * 10);
            CooldownBarTextYOffsetTrack.Value = (int)(Config.CooldownBarTextOffset.Y * 10) > CooldownBarTextYOffsetTrack.Maximum ? CooldownBarTextYOffsetTrack.Maximum : (int)(Config.CooldownBarTextOffset.Y * 10);
            CooldownBarTextSizeTrack.Value = (int)Config.CooldownBarTextFontSize > CooldownBarTextSizeTrack.Maximum ? CooldownBarTextSizeTrack.Maximum : (int)Config.CooldownBarTextFontSize;
            CooldownBarTextReadyText.Text = Config.CooldownBarTextReady;
            CooldownBarTextZeroPrefix.Checked = Config.CooldownBarTextZeroPrefix;
            CooldownBarTextDecimalTrack.Value = Config.CooldownBarTextDecimal > CooldownBarTextDecimalTrack.Maximum ? CooldownBarTextDecimalTrack.Maximum : Config.CooldownBarTextDecimal;

            if(CooldownBarTextFontComboBox.Items.Count == 0) {
                using(InstalledFontCollection fontsCollection = new InstalledFontCollection()) {
                    FontFamily[] fontFamilies = fontsCollection.Families;
                    foreach(FontFamily font in fontFamilies) {
                        CooldownBarTextFontComboBox.Items.Add(font.Name);
                    }
                }
            }
            CooldownBarTextFontComboBox.SelectedItem = Config.CooldownBarTextFont;

            CooldownPropMaxTrack.Value = Config.CooldownMaxPossible;
            CooldownPropOffsetTrack.Value = (int)(Config.CooldownOffset * 10);
            CooldownPropReapplyTrack.Value = (int)(Config.CooldownMinimumReapply * 10);
            CooldownPropOverrideTrack.Value = (int)(Config.CooldownMinimumOverride * 10);
            CooldownPropPauseTrack.Value = (int)(Config.CooldownPauseSubtraction * 10);
            CooldownPropTickTrack.Value = Config.CooldownTickRateInMs;
            CooldownPropConfTrack.Value = (int)(Config.OCRMinimumConfidence * 100);

            FG1ColourText.Text = Config.CooldownBarFG1Color;
            FG2ColourText.Text = Config.CooldownBarFG2Color;
            BGColourText.Text = Config.CooldownBarBGColor;
            SelColourText.Text = Config.CooldownBarSelectedFGColor;
            FG1TColourText.Text = Config.CooldownBarTextFG1Color;
            FG2TColourText.Text = Config.CooldownBarTextFG2Color;
            BGTColourText.Text = Config.CooldownBarTextBGColor;
            SelTColourText.Text = Config.CooldownBarTextSelectedFGColor;
            CooldownOverride1Text.Text = Config.CooldownOverride[0].ToString();
            CooldownOverride2Text.Text = Config.CooldownOverride[1].ToString();
            CooldownOverride3Text.Text = Config.CooldownOverride[2].ToString();
            CooldownOverride4Text.Text = Config.CooldownOverride[3].ToString();
            ToggleTheme.Checked = Config.ConfigTheme == 2;
        }

        private void UpdatePartyNumTrackValues() {
            string item = PartyNumComboBox.SelectedItem.ToString();
            PartyNumBarOffsetsText.Visible = true;
            PartyNumBarOffsetsTrack.Visible = true;
            if(item.Contains("4 ")) {
                Party.PartySize = 4;
                PartyNumBarOffsetsText.Visible = false;
                PartyNumBarOffsetsTrack.Visible = false;
                CooldownBarsYOffsetTrack.Value = (int)(Config.CooldownBarYOffsets[0] * 10) > CooldownBarsYOffsetTrack.Maximum ? CooldownBarsYOffsetTrack.Maximum : (int)(Config.CooldownBarYOffsets[0] * 10);
                PartyNumXPosText.ForeColor = Color.FromArgb(255, 255, 0, 0);
                PartyNumYPosText.ForeColor = Color.FromArgb(255, 255, 0, 0);
                PartyNumBarOffsetsText.ForeColor = Color.FromArgb(255, 255, 0, 0);
                CooldownBarsYOffsetText.ForeColor = Color.FromArgb(255, 255, 0, 0);
            } else if(item.Contains("3 ")) {
                Party.PartySize = 3;
                PartyNumBarOffsetsTrack.Value = Config.PartyNumBarOffsets[1];
                CooldownBarsYOffsetTrack.Value = (int)(Config.CooldownBarYOffsets[1] * 10) > CooldownBarsYOffsetTrack.Maximum ? CooldownBarsYOffsetTrack.Maximum : (int)(Config.CooldownBarYOffsets[1] * 10);
                PartyNumXPosText.ForeColor = Color.FromArgb(255, 225, 165, 0);
                PartyNumYPosText.ForeColor = Color.FromArgb(255, 225, 165, 0);
                PartyNumBarOffsetsText.ForeColor = Color.FromArgb(255, 225, 165, 0);
                CooldownBarsYOffsetText.ForeColor = Color.FromArgb(255, 225, 165, 0);
            } else {
                Party.PartySize = 2;
                PartyNumBarOffsetsTrack.Value = Config.PartyNumBarOffsets[2];
                CooldownBarsYOffsetTrack.Value = (int)(Config.CooldownBarYOffsets[2] * 10) > CooldownBarsYOffsetTrack.Maximum ? CooldownBarsYOffsetTrack.Maximum : (int)(Config.CooldownBarYOffsets[2] * 10);
                PartyNumXPosText.ForeColor = Color.FromArgb(255, 155, 225, 0);
                PartyNumYPosText.ForeColor = Color.FromArgb(255, 155, 225, 0);
                PartyNumBarOffsetsText.ForeColor = Color.FromArgb(255, 155, 225, 0);
                CooldownBarsYOffsetText.ForeColor = Color.FromArgb(255, 155, 225, 0);
            }

            PartyNumXPosTrack.Value = Config.PartyNumLocations[item].X > PartyNumXPosTrack.Maximum ? PartyNumXPosTrack.Maximum : Config.PartyNumLocations[item].X;
            PartyNumYPosTrack.Value = Config.PartyNumLocations[item].Y > PartyNumYPosTrack.Maximum ? PartyNumYPosTrack.Maximum : Config.PartyNumLocations[item].Y;
        }

        #region "Config Click Events":
        private bool colorDialogOpen = false;
        private void FG1ColourText_Click(object sender, EventArgs e) {
            if(colorDialogOpen) { return; }
            colorDialogOpen = true;
            MetroColorDialog.Result res = new MetroColorDialog().Show(this, MetroColorDialog.HexStringToColor(FG1ColourText.Text));
            if(res.Status == MetroColorDialog.Status.Selected) {
                FG1ColourText.Text = MetroColorDialog.ColorToHexString(res.Color);
            }
            colorDialogOpen = false;
            FocusMe();
        }
        private void FG2ColourText_Click(object sender, EventArgs e) {
            if(colorDialogOpen) { return; }
            colorDialogOpen = true;
            MetroColorDialog.Result res = new MetroColorDialog().Show(this, MetroColorDialog.HexStringToColor(FG2ColourText.Text));
            if(res.Status == MetroColorDialog.Status.Selected) {
                FG2ColourText.Text = MetroColorDialog.ColorToHexString(res.Color);
            }
            colorDialogOpen = false;
            FocusMe();
        }
        private void BGColourText_Click(object sender, EventArgs e) {
            if(colorDialogOpen) { return; }
            colorDialogOpen = true;
            MetroColorDialog.Result res = new MetroColorDialog().Show(this, MetroColorDialog.HexStringToColor(BGColourText.Text));
            if(res.Status == MetroColorDialog.Status.Selected) {
                BGColourText.Text = MetroColorDialog.ColorToHexString(res.Color);
            }
            colorDialogOpen = false;
            FocusMe();
        }
        private void SelColourText_Click(object sender, EventArgs e) {
            if(colorDialogOpen) { return; }
            colorDialogOpen = true;
            MetroColorDialog.Result res = new MetroColorDialog().Show(this, MetroColorDialog.HexStringToColor(SelColourText.Text));
            if(res.Status == MetroColorDialog.Status.Selected) {
                SelColourText.Text = MetroColorDialog.ColorToHexString(res.Color);
            }
            colorDialogOpen = false;
            FocusMe();
        }
        private void FG1TColourText_Click(object sender, EventArgs e) {
            if(colorDialogOpen) { return; }
            colorDialogOpen = true;
            MetroColorDialog.Result res = new MetroColorDialog().Show(this, MetroColorDialog.HexStringToColor(FG1TColourText.Text));
            if(res.Status == MetroColorDialog.Status.Selected) {
                FG1TColourText.Text = MetroColorDialog.ColorToHexString(res.Color);
            }
            colorDialogOpen = false;
            FocusMe();
        }
        private void FG2TColourText_Click(object sender, EventArgs e) {
            if(colorDialogOpen) { return; }
            colorDialogOpen = true;
            MetroColorDialog.Result res = new MetroColorDialog().Show(this, MetroColorDialog.HexStringToColor(FG2TColourText.Text));
            if(res.Status == MetroColorDialog.Status.Selected) {
                FG2TColourText.Text = MetroColorDialog.ColorToHexString(res.Color);
            }
            colorDialogOpen = false;
            FocusMe();
        }
        private void BGTColourText_Click(object sender, EventArgs e) {
            if(colorDialogOpen) { return; }
            colorDialogOpen = true;
            MetroColorDialog.Result res = new MetroColorDialog().Show(this, MetroColorDialog.HexStringToColor(BGTColourText.Text));
            if(res.Status == MetroColorDialog.Status.Selected) {
                BGTColourText.Text = MetroColorDialog.ColorToHexString(res.Color);
            }
            colorDialogOpen = false;
            FocusMe();
        }
        private void SelTColourText_Click(object sender, EventArgs e) {
            if(colorDialogOpen) { return; }
            colorDialogOpen = true;
            MetroColorDialog.Result res = new MetroColorDialog().Show(this, MetroColorDialog.HexStringToColor(SelTColourText.Text));
            if(res.Status == MetroColorDialog.Status.Selected) {
                SelTColourText.Text = MetroColorDialog.ColorToHexString(res.Color);
            }
            colorDialogOpen = false;
            FocusMe();
        }
        private void AutoButton_Click(object sender, EventArgs e) {
            User32.GetClientRect(OverlayWindow.GenshinHandle, out User32.RECT rect);
            AssumeDefaultValues(rect);
            UpdateControlValues();
        }
        private void SaveButton_Click(object sender, EventArgs e) {
            ConfigPanel.Visible = false;
            ConfigureOverlayButton.Visible = true;
            Config.Save();
            OverlayWindow.IsConfiguring = false;
        }

        private void ToggleTheme_CheckedChanged(object sender, EventArgs e) {
            if(!ToggleTheme.Checked) {
                Config.ConfigTheme = 1;
                Theme = MetroThemeStyle.Light;
                MStyleManager.Theme = MetroThemeStyle.Light;
            } else {
                Config.ConfigTheme = 2;
                Theme = MetroThemeStyle.Dark;
                MStyleManager.Theme = MetroThemeStyle.Dark;
            }
        }

        private void DevLink_Click(object sender, EventArgs e) {
            Process.Start("https://streamlabs.com/primpri/tip");
        }
        #endregion //Click

        #region "Config ValueChanged/TextChanged Events":
        private void CooldownTextXPosTrack_ValueChanged(object sender, EventArgs e) {
            CooldownTextXPosText.Text = "X Pos: " + CooldownTextXPosTrack.Value.ToString();
            Config.CooldownTextLocation = new Point(CooldownTextXPosTrack.Value, Config.CooldownTextLocation.Y);
        }
        private void CooldownTextYPosTrack_ValueChanged(object sender, EventArgs e) {
            CooldownTextYPosText.Text = "Y Pos: " + CooldownTextYPosTrack.Value.ToString();
            Config.CooldownTextLocation = new Point(Config.CooldownTextLocation.X, CooldownTextYPosTrack.Value);
        }
        private void CooldownTextWidthTrack_ValueChanged(object sender, EventArgs e) {
            CooldownTextWidthText.Text = "Width: " + CooldownTextWidthTrack.Value.ToString();
            Config.CooldownTextSize = new Size(CooldownTextWidthTrack.Value, Config.CooldownTextSize.Height);
        }
        private void CooldownTextHeightTrack_ValueChanged(object sender, EventArgs e) {
            CooldownTextHeightText.Text = "Height: " + CooldownTextHeightTrack.Value.ToString();
            Config.CooldownTextSize = new Size(Config.CooldownTextSize.Width, CooldownTextHeightTrack.Value);
        }
        private void PartyNumXPosTrack_ValueChanged(object sender, EventArgs e) {
            PartyNumXPosText.Text = "X Pos: " + PartyNumXPosTrack.Value.ToString();
            Config.PartyNumLocations[PartyNumComboBox.SelectedItem.ToString()] = new Point(PartyNumXPosTrack.Value, Config.PartyNumLocations[PartyNumComboBox.SelectedItem.ToString()].Y);
        }
        private void PartyNumYPosTrack_ValueChanged(object sender, EventArgs e) {
            PartyNumYPosText.Text = "Y Pos: " + PartyNumYPosTrack.Value.ToString();
            Config.PartyNumLocations[PartyNumComboBox.SelectedItem.ToString()] = new Point(Config.PartyNumLocations[PartyNumComboBox.SelectedItem.ToString()].X, PartyNumYPosTrack.Value);
        }

        private void PartyNumBarOffsetsTrack_ValueChanged(object sender, EventArgs e) {
            PartyNumBarOffsetsText.Text = "Bar Offset: " + PartyNumBarOffsetsTrack.Value.ToString();
            if(PartyNumComboBox.SelectedItem.ToString().Contains("3 ")) {
                Config.PartyNumBarOffsets[1] = PartyNumBarOffsetsTrack.Value;
            } else if(PartyNumComboBox.SelectedItem.ToString().Contains("2 ")) {
                Config.PartyNumBarOffsets[2] = PartyNumBarOffsetsTrack.Value;
            }
        }
        private void CooldownBarsXPosTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarsXPosText.Text = "X Pos: " + CooldownBarsXPosTrack.Value.ToString();
            Config.CooldownBarLocation = new Point(CooldownBarsXPosTrack.Value, Config.CooldownBarLocation.Y);
        }
        private void CooldownBarsYPosTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarsYPosText.Text = "Y Pos: " + CooldownBarsYPosTrack.Value.ToString();
            Config.CooldownBarLocation = new Point(Config.CooldownBarLocation.X, CooldownBarsYPosTrack.Value);
        }
        private void CooldownBarsWidthTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarsWidthText.Text = "Width: " + CooldownBarsWidthTrack.Value.ToString();
            Config.CooldownBarSize = new Size(CooldownBarsWidthTrack.Value, Config.CooldownBarSize.Height);
        }
        private void CooldownBarsHeightTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarsHeightText.Text = "Height: " + CooldownBarsHeightTrack.Value.ToString();
            Config.CooldownBarSize = new Size(Config.CooldownBarSize.Width, CooldownBarsHeightTrack.Value);
        }
        private void CooldownBarsXOffsetTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarsXOffsetText.Text = "X Offset: " + ((float)CooldownBarsXOffsetTrack.Value / 10).ToString();
            Config.CooldownBarXOffset = (float)CooldownBarsXOffsetTrack.Value / 10;
        }
        private void CooldownBarsYOffsetTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarsYOffsetText.Text = "Y Offset: " + ((float)CooldownBarsYOffsetTrack.Value / 10).ToString();
            if(PartyNumComboBox.SelectedItem.ToString().Contains("4 ")) {
                Config.CooldownBarYOffsets[0] = (float)CooldownBarsYOffsetTrack.Value / 10;
            } else if(PartyNumComboBox.SelectedItem.ToString().Contains("3 ")) {
                Config.CooldownBarYOffsets[1] = (float)CooldownBarsYOffsetTrack.Value / 10;
            } else {
                Config.CooldownBarYOffsets[2] = (float)CooldownBarsYOffsetTrack.Value / 10;
            }
        }
        private void CooldownBarsRadiusTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarsModeText.Text = "Style: " + CooldownBarsModeTrack.Value.ToString();
            Config.CooldownBarMode = CooldownBarsModeTrack.Value;
        }
        private void CooldownBarsSelOffsetTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarsSelOffsetText.Text = "Sel. Offset: " + CooldownBarsSelOffsetTrack.Value.ToString();
            Config.CooldownBarSelOffset = CooldownBarsSelOffsetTrack.Value;
        }

        private void CooldownBarTextXOffsetTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarTextXOffsetText.Text = "X Offset: " + ((float)CooldownBarTextXOffsetTrack.Value / 10).ToString();
            Config.CooldownBarTextOffset = new PointF((float)CooldownBarTextXOffsetTrack.Value / 10, Config.CooldownBarTextOffset.Y);
        }
        private void CooldownBarTextYOffsetTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarTextYOffsetText.Text = "Y Offset: " + ((float)CooldownBarTextYOffsetTrack.Value / 10).ToString();
            Config.CooldownBarTextOffset = new PointF(Config.CooldownBarTextOffset.X, (float)CooldownBarTextYOffsetTrack.Value / 10);
        }
        private void CooldownBarTextFontComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            Config.CooldownBarTextFont = CooldownBarTextFontComboBox.SelectedItem.ToString();
        }
        private void CooldownBarTextSizeTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarTextSizeText.Text = "Font Size: " + CooldownBarTextSizeTrack.Value.ToString();
            Config.CooldownBarTextFontSize = CooldownBarTextSizeTrack.Value;
        }
        private void CooldownBarTextReadyText_TextChanged(object sender, EventArgs e) {
            Config.CooldownBarTextReady = CooldownBarTextReadyText.Text;
        }
        private void CooldownBarTextZeroPrefix_CheckedChanged(object sender, EventArgs e) {
            Config.CooldownBarTextZeroPrefix = CooldownBarTextZeroPrefix.Checked;
        }
        private void CooldownBarTextDecimalTrack_ValueChanged(object sender, EventArgs e) {
            CooldownBarTextDecimalText.Text = "Decimal: " + CooldownBarTextDecimalTrack.Value.ToString();
            Config.CooldownBarTextDecimal = CooldownBarTextDecimalTrack.Value;
        }

        private void CooldownPropMaxTrack_ValueChanged(object sender, EventArgs e) {
            CooldownPropMaxText.Text = "Max: " + CooldownPropMaxTrack.Value.ToString();
            Config.CooldownMaxPossible = CooldownPropMaxTrack.Value;
        }
        private void CooldownPropOffsetTrack_ValueChanged(object sender, EventArgs e) {
            CooldownPropOffsetText.Text = "Offset: " + ((decimal)CooldownPropOffsetTrack.Value / 10).ToString();
            Config.CooldownOffset = (decimal)CooldownPropOffsetTrack.Value / 10;
        }
        private void CooldownPropReapplyTrack_ValueChanged(object sender, EventArgs e) {
            CooldownPropReapplyText.Text = "Reapply: " + ((decimal)CooldownPropReapplyTrack.Value / 10).ToString();
            Config.CooldownMinimumReapply = (decimal)CooldownPropReapplyTrack.Value / 10;
        }
        private void CooldownPropOverrideTrack_ValueChanged(object sender, EventArgs e) {
            CooldownPropOverrideText.Text = "Override: " + ((decimal)CooldownPropOverrideTrack.Value / 10).ToString();
            Config.CooldownMinimumOverride = (decimal)CooldownPropOverrideTrack.Value / 10;
        }
        private void CooldownPropPauseTrack_ValueChanged(object sender, EventArgs e) {
            CooldownPropPauseText.Text = "Pause Sub: " + ((decimal)CooldownPropPauseTrack.Value / 10).ToString();
            Config.CooldownPauseSubtraction = (decimal)CooldownPropPauseTrack.Value / 10;
        }
        private void CooldownPropTickTrack_ValueChanged(object sender, EventArgs e) {
            CooldownPropTickText.Text = "Tick Rate: " + CooldownPropTickTrack.Value.ToString();
            Config.CooldownTickRateInMs = CooldownPropTickTrack.Value;
        }
        private void CooldownPropConfTrack_ValueChanged(object sender, EventArgs e) {
            CooldownPropConfText.Text = $"Confidence: {CooldownPropConfTrack.Value}%";
            Config.OCRMinimumConfidence = (float)CooldownPropConfTrack.Value / 100;
        }

        private void PartyNumComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdatePartyNumTrackValues();
        }

        private void FG1ColourText_TextChanged(object sender, EventArgs e) {
            Config.CooldownBarFG1Color = FG1ColourText.Text;
            FG1ColourText.ForeColor = (Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarFG1Color);
            OverlayWindow.UpdateBrushes();
        }
        private void FG2ColourText_TextChanged(object sender, EventArgs e) {
            Config.CooldownBarFG2Color = FG2ColourText.Text;
            FG2ColourText.ForeColor = (Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarFG2Color);
            OverlayWindow.UpdateBrushes();
        }
        private void BGColourText_TextChanged(object sender, EventArgs e) {
            Config.CooldownBarBGColor = BGColourText.Text;
            BGColourText.ForeColor = (Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarBGColor);
            OverlayWindow.UpdateBrushes();
        }
        private void SelColourText_TextChanged(object sender, EventArgs e) {
            Config.CooldownBarSelectedFGColor = SelColourText.Text;
            SelColourText.ForeColor = (Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarSelectedFGColor);
            OverlayWindow.UpdateBrushes();
        }
        private void FG1TColourText_TextChanged(object sender, EventArgs e) {
            Config.CooldownBarTextFG1Color = FG1TColourText.Text;
            FG1TColourText.ForeColor = (Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarTextFG1Color);
            OverlayWindow.UpdateBrushes();
        }
        private void FG2TColourText_TextChanged(object sender, EventArgs e) {
            Config.CooldownBarTextFG2Color = FG2TColourText.Text;
            FG2TColourText.ForeColor = (Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarTextFG2Color);
            OverlayWindow.UpdateBrushes();
        }
        private void BGTColourText_TextChanged(object sender, EventArgs e) {
            Config.CooldownBarTextBGColor = BGTColourText.Text;
            BGTColourText.ForeColor = (Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarTextBGColor);
            OverlayWindow.UpdateBrushes();
        }
        private void SelTColourText_TextChanged(object sender, EventArgs e) {
            Config.CooldownBarTextSelectedFGColor = SelTColourText.Text;
            SelTColourText.ForeColor = (Color)IMG.ColorConverter.ConvertFromString(Config.CooldownBarTextSelectedFGColor);
            OverlayWindow.UpdateBrushes();
        }
        private void CooldownOverride1Text_TextChanged(object sender, EventArgs e) {
            if(int.TryParse(CooldownOverride1Text.Text, out int cd)) {
                Config.CooldownOverride[0] = cd;
            } else {
                Config.CooldownOverride[0] = 0;
            }
        }
        private void CooldownOverride2Text_TextChanged(object sender, EventArgs e) {
            if(int.TryParse(CooldownOverride2Text.Text, out int cd)) {
                Config.CooldownOverride[1] = cd;
            } else {
                Config.CooldownOverride[1] = 0;
            }
        }
        private void CooldownOverride3Text_TextChanged(object sender, EventArgs e) {
            if(int.TryParse(CooldownOverride3Text.Text, out int cd)) {
                Config.CooldownOverride[2] = cd;
            } else {
                Config.CooldownOverride[2] = 0;
            }
        }
        private void CooldownOverride4Text_TextChanged(object sender, EventArgs e) {
            if(int.TryParse(CooldownOverride4Text.Text, out int cd)) {
                Config.CooldownOverride[3] = cd;
            } else {
                Config.CooldownOverride[3] = 0;
            }
        }
        #endregion //ValueChanged/TextChanged

        #endregion //Config

        private void ConfigTabControl_Selecting(object sender, TabControlCancelEventArgs e) {
            if(e.TabPageIndex == 0) {
                e.Cancel = true;
            }
        }
    }
}
