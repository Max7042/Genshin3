using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace GenshinOverlay {
    public static class Config {
        private static Options Options { get; set; } = new Options();
        public static List<Template> Templates { get; set; } = new List<Template>();

        public static Point CooldownTextLocation { get { return Options.CooldownTextLocation; } set { Options.CooldownTextLocation = value; } }
        public static Size CooldownTextSize { get { return Options.CooldownTextSize; } set { Options.CooldownTextSize = value; } }
        public static Point PartyNumLocation { get { return Options.PartyNumLocation; } set { Options.PartyNumLocation = value; } }
        public static int PartyNumYOffset { get { return Options.PartyNumYOffset; } set { Options.PartyNumYOffset = value; } }

        public static int CooldownBarMode { get { return Options.CooldownBarMode; } set { Options.CooldownBarMode = value; } }
        public static Point CooldownBarLocation { get { return Options.CooldownBarLocation; } set { Options.CooldownBarLocation = value; } }
        public static PointF CooldownBarOffset { get { return Options.CooldownBarOffset; } set { Options.CooldownBarOffset = value; } }
        public static Size CooldownBarSize { get { return Options.CooldownBarSize; } set { Options.CooldownBarSize = value; } }
        public static string CooldownBarBGColor { get { return Options.CooldownBarBGColor; } set { Options.CooldownBarBGColor = value; } }
        public static string CooldownBarFG1Color { get { return Options.CooldownBarFG1Color; } set { Options.CooldownBarFG1Color = value; } }
        public static string CooldownBarFG2Color { get { return Options.CooldownBarFG2Color; } set { Options.CooldownBarFG2Color = value; } }
        public static string CooldownBarSelectedFGColor { get { return Options.CooldownBarSelectedFGColor; } set { Options.CooldownBarSelectedFGColor = value; } }
        public static int CooldownBarSelOffset { get { return Options.CooldownBarSelOffset; } set { Options.CooldownBarSelOffset = value; } }
        public static int[] CooldownOverride { get { return Options.CooldownOverride; } set { Options.CooldownOverride = value; } }

        public static string ProcessName { get { return Options.ProcessName; } set { Options.ProcessName = value; } }
        public static int PauseDelay { get { return Options.PauseDelay; } set { Options.PauseDelay = value; } }
        public static int CooldownTickRateInMs { get { return Options.CooldownTickRateInMs; } set { Options.CooldownTickRateInMs = value; } }
        public static int CooldownMaxPossible { get { return Options.CooldownMaxPossible; } set { Options.CooldownMaxPossible = value; } }
        public static decimal CooldownOffset { get { return Options.CooldownOffset; } set { Options.CooldownOffset = value; } }
        public static decimal CooldownPauseSubtraction { get { return Options.CooldownPauseSubtraction; } set { Options.CooldownPauseSubtraction = value; } }
        public static decimal CooldownMinimumReapply { get { return Options.CooldownMinimumReapply; } set { Options.CooldownMinimumReapply = value; } }
        public static decimal CooldownMinimumOverride { get { return Options.CooldownMinimumOverride; } set { Options.CooldownMinimumOverride = value; } }
        public static float OCRMinimumConfidence { get { return Options.OCRMinimumConfidence; } set { Options.OCRMinimumConfidence = value; } }
        public static float OCRScaleFactor { get { return Options.OCRScaleFactor; } set { Options.OCRScaleFactor = value; } }
        public static int OCRPadding { get { return Options.OCRPadding; } set { Options.OCRPadding = value; } }
        public static int OCRNoiseSize { get { return Options.OCRNoiseSize; } set { Options.OCRNoiseSize = value; } }
        public static int OCRNoiseConnectivity { get { return Options.OCRNoiseConnectivity; } set { Options.OCRNoiseConnectivity = value; } }
        public static int OCRNoiseType { get { return Options.OCRNoiseType; } set { Options.OCRNoiseType = value; } }
        public static int OCRNoiseRelation { get { return Options.OCRNoiseRelation; } set { Options.OCRNoiseRelation = value; } }
        public static int ConfigTheme { get { return Options.ConfigTheme; } set { Options.ConfigTheme = value; } }

        public static void Load() {
            if(!File.Exists(Application.StartupPath + @"\GenshinOverlay.Config.ini")) {
                File.WriteAllText(Application.StartupPath + @"\GenshinOverlay.Config.ini", JsonConvert.SerializeObject(Options, Formatting.Indented));
            } else {
                Options = JsonConvert.DeserializeObject<Options>(File.ReadAllText(Application.StartupPath + @"\GenshinOverlay.Config.ini"));
            }

            if(!File.Exists(Application.StartupPath + @"\GenshinOverlay.Templates.ini")) {
                SetupDefaultTemplates();
                File.WriteAllText(Application.StartupPath + @"\GenshinOverlay.Templates.ini", JsonConvert.SerializeObject(Templates, Formatting.Indented));
            } else {
                Templates = JsonConvert.DeserializeObject<List<Template>>(File.ReadAllText(Application.StartupPath + @"\GenshinOverlay.Templates.ini"));
            }
        }

        public static void Save() {
            File.WriteAllText(Application.StartupPath + @"\GenshinOverlay.Config.ini", JsonConvert.SerializeObject(Options, Formatting.Indented));
        }

        private static void SetupDefaultTemplates() {
            Templates.Add(new Template() {
                Resolution = new Size(1280, 720),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1123, 678),
                    CooldownTextSize = new Size(35, 18),
                    PartyNumLocation = new Point(1244, 260),
                    PartyNumYOffset = 61,
                    CooldownBarLocation = new Point(1130, 280),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarOffset = new PointF(0, 60.8f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1280, 768),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1123, 726),
                    CooldownTextSize = new Size(35, 18),
                    PartyNumLocation = new Point(1244, 308),
                    PartyNumYOffset = 61,
                    CooldownBarLocation = new Point(1130, 328),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarOffset = new PointF(0, 60.8f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1280, 800),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1123, 758),
                    CooldownTextSize = new Size(35, 18),
                    PartyNumLocation = new Point(1244, 340),
                    PartyNumYOffset = 61,
                    CooldownBarLocation = new Point(1130, 360),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarOffset = new PointF(0, 60.8f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1280, 960),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1123, 918),
                    CooldownTextSize = new Size(35, 18),
                    PartyNumLocation = new Point(1244, 500),
                    PartyNumYOffset = 61,
                    CooldownBarLocation = new Point(1130, 520),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarOffset = new PointF(0, 60.8f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1280, 1024),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1123, 982),
                    CooldownTextSize = new Size(35, 18),
                    PartyNumLocation = new Point(1244, 564),
                    PartyNumYOffset = 61,
                    CooldownBarLocation = new Point(1130, 584),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarOffset = new PointF(0, 60.8f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1360, 768),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1193, 722),
                    CooldownTextSize = new Size(36, 18),
                    PartyNumLocation = new Point(1321, 278),
                    PartyNumYOffset = 65,
                    CooldownBarLocation = new Point(1200, 298),
                    CooldownBarSize = new Size(47, 4),
                    CooldownBarOffset = new PointF(0, 64.6f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1366, 768),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1198, 723),
                    CooldownTextSize = new Size(37, 18),
                    PartyNumLocation = new Point(1327, 276),
                    PartyNumYOffset = 65,
                    CooldownBarLocation = new Point(1205, 296),
                    CooldownBarSize = new Size(47, 4),
                    CooldownBarOffset = new PointF(0, 64.9f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1600, 900),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1404, 841),
                    CooldownTextSize = new Size(42, 22),
                    PartyNumLocation = new Point(1554, 318),
                    PartyNumYOffset = 76,
                    CooldownBarLocation = new Point(1412, 343),
                    CooldownBarSize = new Size(55, 5),
                    CooldownBarOffset = new PointF(0, 75.7f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1600, 1024),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1404, 965),
                    CooldownTextSize = new Size(42, 22),
                    PartyNumLocation = new Point(1554, 443),
                    PartyNumYOffset = 76,
                    CooldownBarLocation = new Point(1412, 467),
                    CooldownBarSize = new Size(54, 5),
                    CooldownBarOffset = new PointF(0, 75.9f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1680, 1050),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1474, 988),
                    CooldownTextSize = new Size(44, 22),
                    PartyNumLocation = new Point(1632, 438),
                    PartyNumYOffset = 80,
                    CooldownBarLocation = new Point(1482, 464),
                    CooldownBarSize = new Size(58, 5),
                    CooldownBarOffset = new PointF(0, 79.6f)
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1920, 1080),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1685, 976),
                    CooldownTextSize = new Size(49, 27),
                    PartyNumLocation = new Point(1863, 351),
                    PartyNumYOffset = 91,
                    CooldownBarLocation = new Point(1692, 381),
                    CooldownBarSize = new Size(67, 5),
                    CooldownBarOffset = new PointF(0, 90.7f)
                }
            });
        }
    }

    public class Template {
        public Size Resolution { get; set; }
        public Props Properties { get; set; }
    }

    public class Props {
        public Point CooldownTextLocation { get; set; }
        public Size CooldownTextSize { get; set; }
        public Point PartyNumLocation { get; set; }
        public int PartyNumYOffset { get; set; }
        public Point CooldownBarLocation { get; set; }
        public Size CooldownBarSize { get; set; }
        public PointF CooldownBarOffset { get; set; }
    }

    public class Options {
        public Point CooldownTextLocation { get; set; } = new Point(0, 0);
        public Size CooldownTextSize { get; set; } = new Size(35, 18);
        public Point PartyNumLocation { get; set; } = new Point(0, 0);
        public int PartyNumYOffset { get; set; } = 61;

        public int CooldownBarMode { get; set; } = 3;
        public Point CooldownBarLocation { get; set; } = new Point(0, 0);
        public PointF CooldownBarOffset { get; set; } = new PointF(0, 60.8f);
        public Size CooldownBarSize { get; set; } = new Size(43, 4);
        public string CooldownBarBGColor { get; set; } = "#8D020202";
        public string CooldownBarFG1Color { get; set; } = "#FF6D45B4";
        public string CooldownBarFG2Color { get; set; } = "#FFDA3A4D";
        public string CooldownBarSelectedFGColor { get; set; } = "#FF6D45B4";
        public int CooldownBarSelOffset { get; set; } = 0;
        public int[] CooldownOverride { get; set; } = new int[] { 0, 0, 0, 0 };

        public string ProcessName { get; set; } = "GenshinImpact";
        public int PauseDelay { get; set; } = 500;
        public int CooldownTickRateInMs { get; set; } = 10;
        public int CooldownMaxPossible { get; set; } = 60;
        public decimal CooldownOffset { get; set; } = 0.0M;
        public decimal CooldownPauseSubtraction { get; set; } = 1.0M;
        public decimal CooldownMinimumReapply { get; set; } = 1.0M;
        public decimal CooldownMinimumOverride { get; set; } = 2.0M;
        public float OCRMinimumConfidence { get; set; } = 0.8f;
        public float OCRScaleFactor { get; set; } = 3.5f;
        public int OCRPadding { get; set; } = 10;
        public int OCRNoiseSize { get; set; } = 5; // 1.2~1.9 * ScaleFactor
        public int OCRNoiseConnectivity { get; set; } = 8;
        public int OCRNoiseType { get; set; } = 5;
        public int OCRNoiseRelation { get; set; } = 2;
        public int ConfigTheme { get; set; } = 2;
    }
}
