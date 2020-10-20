using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace GenshinOverlay {
    public static class Config {
        private static Options Options { get; set; } = new Options();
        public static List<Template> Templates { get; set; }

        public static Point CooldownTextLocation { get { return Options.CooldownTextLocation; } set { Options.CooldownTextLocation = value; } }
        public static Size CooldownTextSize { get { return Options.CooldownTextSize; } set { Options.CooldownTextSize = value; } }
        public static int CooldownText2LocationX { get { return Options.CooldownText2LocationX; } set { Options.CooldownText2LocationX = value; } }
        public static Dictionary<string, Point> PartyNumLocations { get { return Options.PartyNumLocations; } set { Options.PartyNumLocations = value; } }
        public static int[] PartyNumBarOffsets { get { return Options.PartyNumBarOffsets; } set { Options.PartyNumBarOffsets = value; } }
        public static int CooldownBarMode { get { return Options.CooldownBarMode; } set { Options.CooldownBarMode = value; } }
        public static Point CooldownBarLocation { get { return Options.CooldownBarLocation; } set { Options.CooldownBarLocation = value; } }
        public static float CooldownBarXOffset { get { return Options.CooldownBarXOffset; } set { Options.CooldownBarXOffset = value; } }
        public static float[] CooldownBarYOffsets { get { return Options.CooldownBarYOffsets; } set { Options.CooldownBarYOffsets = value; } }
        public static Size CooldownBarSize { get { return Options.CooldownBarSize; } set { Options.CooldownBarSize = value; } }
        public static string CooldownBarBGColor { get { return Options.CooldownBarBGColor; } set { Options.CooldownBarBGColor = value; } }
        public static string CooldownBarFG1Color { get { return Options.CooldownBarFG1Color; } set { Options.CooldownBarFG1Color = value; } }
        public static string CooldownBarFG2Color { get { return Options.CooldownBarFG2Color; } set { Options.CooldownBarFG2Color = value; } }
        public static string CooldownBarSelectedFGColor { get { return Options.CooldownBarSelectedFGColor; } set { Options.CooldownBarSelectedFGColor = value; } }
        public static int CooldownBarSelOffset { get { return Options.CooldownBarSelOffset; } set { Options.CooldownBarSelOffset = value; } }
        public static PointF CooldownBarTextOffset { get { return Options.CooldownBarTextOffset; } set { Options.CooldownBarTextOffset = value; } }
        public static string CooldownBarTextFont { get { return Options.CooldownBarTextFont; } set { Options.CooldownBarTextFont = value; } }
        public static float CooldownBarTextFontSize { get { return Options.CooldownBarTextFontSize; } set { Options.CooldownBarTextFontSize = value; } }
        public static string CooldownBarTextReady { get { return Options.CooldownBarTextReady; } set { Options.CooldownBarTextReady = value; } }
        public static bool CooldownBarTextZeroPrefix { get { return Options.CooldownBarTextZeroPrefix; } set { Options.CooldownBarTextZeroPrefix = value; } }
        public static int CooldownBarTextDecimal { get { return Options.CooldownBarTextDecimal; } set { Options.CooldownBarTextDecimal = value; } }
        public static string CooldownBarTextBGColor { get { return Options.CooldownBarTextBGColor; } set { Options.CooldownBarTextBGColor = value; } }
        public static string CooldownBarTextFG1Color { get { return Options.CooldownBarTextFG1Color; } set { Options.CooldownBarTextFG1Color = value; } }
        public static string CooldownBarTextFG2Color { get { return Options.CooldownBarTextFG2Color; } set { Options.CooldownBarTextFG2Color = value; } }
        public static string CooldownBarTextSelectedFGColor { get { return Options.CooldownBarTextSelectedFGColor; } set { Options.CooldownBarTextSelectedFGColor = value; } }
        public static int[] CooldownOverride { get { return Options.CooldownOverride; } set { Options.CooldownOverride = value; } }

        public static string ProcessName { get { return Options.ProcessName; } set { Options.ProcessName = value; } }
        public static int PauseDelay { get { return Options.PauseDelay; } set { Options.PauseDelay = value; } }
        public static int CooldownTickRateInMs { get { return Options.CooldownTickRateInMs; } set { Options.CooldownTickRateInMs = value; } }
        public static int CooldownOCRRateInMs { get { return Options.CooldownOCRRateInMs; } set { Options.CooldownOCRRateInMs = value; } }
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
                File.WriteAllText(Application.StartupPath + @"\GenshinOverlay.Config.ini", JsonConvert.SerializeObject(Options, Formatting.Indented));
            }
        }

        public static void LoadTemplates() {
            Templates = new List<Template>();
            SetupDefaultTemplates();
            if(!File.Exists(Application.StartupPath + @"\GenshinOverlay.Templates.ini")) {
                File.WriteAllText(Application.StartupPath + @"\GenshinOverlay.Templates.ini", JsonConvert.SerializeObject(Templates, Formatting.Indented));
            } else {
                List<Template> _templates = JsonConvert.DeserializeObject<List<Template>>(File.ReadAllText(Application.StartupPath + @"\GenshinOverlay.Templates.ini"));
                foreach(Template _t in _templates) {
                    Template t = Templates.Find(x => x.Resolution == _t.Resolution);
                    if(_t.Properties.CooldownTextLocation == null) { _t.Properties.CooldownTextLocation = t != null ? t.Properties.CooldownTextLocation : new Point(); }
                    if(_t.Properties.CooldownTextSize == null) { _t.Properties.CooldownTextSize = t != null ? t.Properties.CooldownTextSize : new Size(); }
                    if(_t.Properties.PartyNumLocations == null) {
                        _t.Properties.PartyNumLocations = t != null ? t.Properties.PartyNumLocations : new Dictionary<string, Point>() {
                            { "4 #1", Point.Empty }, { "4 #2", Point.Empty }, { "4 #3", Point.Empty }, { "4 #4", Point.Empty },
                            { "3 #1", Point.Empty }, { "3 #2", Point.Empty }, { "3 #3", Point.Empty },
                            { "2 #1", Point.Empty }, { "2 #2", Point.Empty }
                        };
                    }
                    if(_t.Properties.PartyNumBarOffsets == null) { _t.Properties.PartyNumBarOffsets = t != null ? t.Properties.PartyNumBarOffsets : new int[] { 0, 0, 0 }; }
                    if(_t.Properties.CooldownBarLocation == null) { _t.Properties.CooldownBarLocation = t != null ? t.Properties.CooldownBarLocation : new Point(); }
                    if(_t.Properties.CooldownBarSize == null) { _t.Properties.CooldownBarSize = t != null ? t.Properties.CooldownBarSize : new Size(); }
                    if(_t.Properties.CooldownBarYOffsets == null) { _t.Properties.CooldownBarYOffsets = t != null ? t.Properties.CooldownBarYOffsets : new float[] { 60f, 60f, 60f }; }
                }
                Templates = _templates;
                File.WriteAllText(Application.StartupPath + @"\GenshinOverlay.Templates.ini", JsonConvert.SerializeObject(Templates, Formatting.Indented));
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
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1244, 260) }, { "4 #2", new Point(1244, 320) }, { "4 #3", new Point(1244, 381) }, { "4 #4", new Point(1244, 442) },
                        { "3 #1", new Point(1244, 287) }, { "3 #2", new Point(1244, 350) }, { "3 #3", new Point(1244, 415) },
                        { "2 #1", new Point(1244, 324) }, { "2 #2", new Point(1244, 385) }
                    },
                    PartyNumBarOffsets =  new int[] { 0, 27, 59 },
                    CooldownBarLocation = new Point(1130, 280),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 60.8f, 64.3f, 64.3f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1280, 768),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1123, 726),
                    CooldownTextSize = new Size(35, 18),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1244, 308) }, { "4 #2", new Point(1244, 368) }, { "4 #3", new Point(1244, 429) }, { "4 #4", new Point(1244, 490) },
                        { "3 #1", new Point(1244, 335) }, { "3 #2", new Point(1244, 399) }, { "3 #3", new Point(1244, 463) },
                        { "2 #1", new Point(1244, 372) }, { "2 #2", new Point(1244, 433) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 27, 59 },
                    CooldownBarLocation = new Point(1130, 328),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 60.8f, 64.3f, 64.3f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1280, 800),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1123, 758),
                    CooldownTextSize = new Size(35, 18),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1244, 340) }, { "4 #2", new Point(1244, 400) }, { "4 #3", new Point(1244, 461) }, { "4 #4", new Point(1244, 522) },
                        { "3 #1", new Point(1244, 366) }, { "3 #2", new Point(1244, 431) }, { "3 #3", new Point(1244, 494) },
                        { "2 #1", new Point(1244, 405) }, { "2 #2", new Point(1244, 466) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 27, 59 },
                    CooldownBarLocation = new Point(1130, 360),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 60.8f, 64.3f, 64.3f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1280, 960),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1123, 918),
                    CooldownTextSize = new Size(35, 18),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1244, 500) }, { "4 #2", new Point(1244, 560) }, { "4 #3", new Point(1244, 622) }, { "4 #4", new Point(1244, 682) },
                        { "3 #1", new Point(1244, 527) }, { "3 #2", new Point(1244, 590) }, { "3 #3", new Point(1244, 654) },
                        { "2 #1", new Point(1244, 564) }, { "2 #2", new Point(1244, 627) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 27, 59 },
                    CooldownBarLocation = new Point(1130, 520),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 60.8f, 64.3f, 64.3f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1280, 1024),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1123, 982),
                    CooldownTextSize = new Size(35, 18),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1244, 564) }, { "4 #2", new Point(1244, 624) }, { "4 #3", new Point(1244, 685) }, { "4 #4", new Point(1244, 747) },
                        { "3 #1", new Point(1244, 591) }, { "3 #2", new Point(1244, 655) }, { "3 #3", new Point(1244, 719) },
                        { "2 #1", new Point(1244, 629) }, { "2 #2", new Point(1244, 691) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 27, 59 },
                    CooldownBarLocation = new Point(1130, 584),
                    CooldownBarSize = new Size(43, 4),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 60.8f, 64.3f, 64.3f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1360, 768),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1193, 722),
                    CooldownTextSize = new Size(36, 18),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1321, 276) }, { "4 #2", new Point(1321, 343) }, { "4 #3", new Point(1321, 406) }, { "4 #4", new Point(1321, 471) },
                        { "3 #1", new Point(1321, 306) }, { "3 #2", new Point(1321, 374) }, { "3 #3", new Point(1321, 442) },
                        { "2 #1", new Point(1321, 347) }, { "2 #2", new Point(1321, 412) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 29, 63 },
                    CooldownBarLocation = new Point(1200, 298),
                    CooldownBarSize = new Size(47, 4),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 64.8f, 67.7f, 67.7f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1366, 768),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1198, 723),
                    CooldownTextSize = new Size(37, 18),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1327, 276) }, { "4 #2", new Point(1327, 340) }, { "4 #3", new Point(1327, 405) }, { "4 #4", new Point(1327, 470) },
                        { "3 #1", new Point(1327, 304) }, { "3 #2", new Point(1327, 373) }, { "3 #3", new Point(1327, 441) },
                        { "2 #1", new Point(1327, 344) }, { "2 #2", new Point(1327, 411) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 29, 63 },
                    CooldownBarLocation = new Point(1205, 296),
                    CooldownBarSize = new Size(47, 4),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 64.8f, 68f, 68f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1600, 900),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1404, 841),
                    CooldownTextSize = new Size(42, 22),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1554, 318) }, { "4 #2", new Point(1554, 394) }, { "4 #3", new Point(1554, 470) }, { "4 #4", new Point(1554, 546) },
                        { "3 #1", new Point(1554, 352) }, { "3 #2", new Point(1554, 432) }, { "3 #3", new Point(1554, 512) },
                        { "2 #1", new Point(1554, 400) }, { "2 #2", new Point(1554, 477) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 33, 73 },
                    CooldownBarLocation = new Point(1412, 343),
                    CooldownBarSize = new Size(55, 5),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 75.7f, 80f, 80f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1600, 1024),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1404, 965),
                    CooldownTextSize = new Size(42, 22),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1554, 443) }, { "4 #2", new Point(1554, 519) }, { "4 #3", new Point(1554, 594) }, { "4 #4", new Point(1554, 671) },
                        { "3 #1", new Point(1554, 477) }, { "3 #2", new Point(1554, 557) }, { "3 #3", new Point(1554, 636) },
                        { "2 #1", new Point(1554, 525) }, { "2 #2", new Point(1554, 601) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 33, 73 },
                    CooldownBarLocation = new Point(1412, 467),
                    CooldownBarSize = new Size(54, 5),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 76f, 80f, 80f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1680, 1050),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1474, 988),
                    CooldownTextSize = new Size(44, 22),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1632, 438) }, { "4 #2", new Point(1632, 517) }, { "4 #3", new Point(1632, 597) }, { "4 #4", new Point(1632, 677) },
                        { "3 #1", new Point(1632, 474) }, { "3 #2", new Point(1632, 558) }, { "3 #3", new Point(1632, 642) },
                        { "2 #1", new Point(1632, 524) }, { "2 #2", new Point(1632, 603) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 35, 77 },
                    CooldownBarLocation = new Point(1482, 464),
                    CooldownBarSize = new Size(58, 5),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 79.7f, 84f, 84f }
                }
            });

            Templates.Add(new Template() {
                Resolution = new Size(1920, 1080),
                Properties = new Props() {
                    CooldownTextLocation = new Point(1685, 978),
                    CooldownTextSize = new Size(49, 27),
                    CooldownText2LocationX = 0,
                    PartyNumLocations = new Dictionary<string, Point>() {
                        { "4 #1", new Point(1863, 351) }, { "4 #2", new Point(1863, 440) }, { "4 #3", new Point(1863, 531) }, { "4 #4", new Point(1863, 624) },
                        { "3 #1", new Point(1863, 392) }, { "3 #2", new Point(1863, 488) }, { "3 #3", new Point(1863, 582) },
                        { "2 #1", new Point(1863, 446) }, { "2 #2", new Point(1863, 536) }
                    },
                    PartyNumBarOffsets = new int[] { 0, 41, 89 },
                    CooldownBarLocation = new Point(1692, 381),
                    CooldownBarSize = new Size(67, 5),
                    CooldownBarXOffset = 0,
                    CooldownBarYOffsets = new float[] { 91.2f, 96f, 96f }
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
        public int CooldownText2LocationX { get; set; }
        public Dictionary<string, Point> PartyNumLocations { get; set; }
        public int[] PartyNumBarOffsets { get; set; }
        public Point CooldownBarLocation { get; set; }
        public Size CooldownBarSize { get; set; }
        public float CooldownBarXOffset { get; set; }
        public float[] CooldownBarYOffsets { get; set; }
    }

    public class Options {
        public Point CooldownTextLocation { get; set; } = Point.Empty;
        public Size CooldownTextSize { get; set; } = new Size(35, 18);
        public int CooldownText2LocationX { get; set; } = 0;
        public Dictionary<string, Point> PartyNumLocations { get; set; } = new Dictionary<string, Point>() {
            { "4 #1", Point.Empty }, { "4 #2", Point.Empty }, { "4 #3", Point.Empty }, { "4 #4", Point.Empty },
            { "3 #1", Point.Empty }, { "3 #2", Point.Empty }, { "3 #3", Point.Empty },
            { "2 #1", Point.Empty }, { "2 #2", Point.Empty }
        };
        public int[] PartyNumBarOffsets = new int[] { 0, 0, 0 };
        public int CooldownBarMode { get; set; } = 3;
        public Point CooldownBarLocation { get; set; } = Point.Empty;
        public float CooldownBarXOffset { get; set; } = 0;
        public float[] CooldownBarYOffsets { get; set; } = new float[] { 60f, 60f, 60f };
        public Size CooldownBarSize { get; set; } = new Size(43, 4);
        public string CooldownBarBGColor { get; set; } = "#8D020202";
        public string CooldownBarFG1Color { get; set; } = "#FF6D45B4";
        public string CooldownBarFG2Color { get; set; } = "#FFDA3A4D";
        public string CooldownBarSelectedFGColor { get; set; } = "#FF6D45B4";
        public int CooldownBarSelOffset { get; set; } = 0;
        public PointF CooldownBarTextOffset { get; set; } = PointF.Empty;
        public string CooldownBarTextFont { get; set; } = "Segoe UI";
        public float CooldownBarTextFontSize { get; set; } = 0;
        public string CooldownBarTextReady { get; set; } = "";
        public bool CooldownBarTextZeroPrefix { get; set; } = true;
        public int CooldownBarTextDecimal { get; set; } = 0;
        public string CooldownBarTextBGColor { get; set; } = "#FF020202";
        public string CooldownBarTextFG1Color { get; set; } = "#FF999999";
        public string CooldownBarTextFG2Color { get; set; } = "#00999999";
        public string CooldownBarTextSelectedFGColor { get; set; } = "#FF999999";
        public int[] CooldownOverride { get; set; } = new int[] { 0, 0, 0, 0 };

        public string ProcessName { get; set; } = "GenshinImpact";
        public int PauseDelay { get; set; } = 500;
        public int CooldownTickRateInMs { get; set; } = 10;
        public int CooldownOCRRateInMs { get; set; } = 100;
        public int CooldownMaxPossible { get; set; } = 90;
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
