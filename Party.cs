using System;
using System.Collections.Generic;
using System.Drawing;

namespace GenshinOverlay {
    public class Party {
        public static List<Character> Characters { get; set; } = new List<Character>() {
            new Character() { Cooldown = 0, Max = 0 },
            new Character() { Cooldown = 0, Max = 0 },
            new Character() { Cooldown = 0, Max = 0 },
            new Character() { Cooldown = 0, Max = 0 }
        };
        public static int PartySize { get; set; } = 4;
        public static int SelectedCharacter { get; set; } = -1;
        private static int _p2PauseStep = 0;

        public static int GetSelectedCharacter(IntPtr handle) {
            if(handle == IntPtr.Zero) { return -1; }
            int c0, c1, c2, c3;

            c0 = IMG.GetColorAt(handle, Config.PartyNumLocations["4 #1"].X, Config.PartyNumLocations["4 #1"].Y) == Color.FromArgb(255, 255, 255, 255) ? 1 : 0;
            c1 = IMG.GetColorAt(handle, Config.PartyNumLocations["4 #2"].X, Config.PartyNumLocations["4 #2"].Y) == Color.FromArgb(255, 255, 255, 255) ? 1 : 0;
            c2 = IMG.GetColorAt(handle, Config.PartyNumLocations["4 #3"].X, Config.PartyNumLocations["4 #3"].Y) == Color.FromArgb(255, 255, 255, 255) ? 1 : 0;
            c3 = IMG.GetColorAt(handle, Config.PartyNumLocations["4 #4"].X, Config.PartyNumLocations["4 #4"].Y) == Color.FromArgb(255, 255, 255, 255) ? 1 : 0;
            
            if(c0 + c1 + c2 + c3 == 4) {
                return -1;
            } else if(c0 + c1 + c2 + c3 == 3) {
                PartySize = 4;
                return c0 == 0 ? 0 : c1 == 0 ? 1 : c2 == 0 ? 2 : 3;
            } else if(c0 + c1 + c2 + c3 == 2 && PartySize == 4) {
                return SelectedCharacter;
            } else {
                c0 = IMG.GetColorAt(handle, Config.PartyNumLocations["3 #1"].X, Config.PartyNumLocations["3 #1"].Y) == Color.FromArgb(255, 255, 255, 255) ? 1 : 0;
                c1 = IMG.GetColorAt(handle, Config.PartyNumLocations["3 #2"].X, Config.PartyNumLocations["3 #2"].Y) == Color.FromArgb(255, 255, 255, 255) ? 1 : 0;
                c2 = IMG.GetColorAt(handle, Config.PartyNumLocations["3 #3"].X, Config.PartyNumLocations["3 #3"].Y) == Color.FromArgb(255, 255, 255, 255) ? 1 : 0;
                if(c0 + c1 + c2 == 2) {
                    PartySize = 3;
                    return c0 == 0 ? 0 : c1 == 0 ? 1 : 2;
                } else if(c0 + c1 + c2 == 1 && PartySize == 3) {
                    return SelectedCharacter;
                } else {
                    c0 = IMG.GetColorAt(handle, Config.PartyNumLocations["2 #1"].X, Config.PartyNumLocations["2 #1"].Y) == Color.FromArgb(255, 255, 255, 255) ? 1 : 0;
                    c1 = IMG.GetColorAt(handle, Config.PartyNumLocations["2 #2"].X, Config.PartyNumLocations["2 #2"].Y) == Color.FromArgb(255, 255, 255, 255) ? 1 : 0;
                    if(c0 + c1 == 1) {
                        PartySize = 2;
                        _p2PauseStep = 0;
                        return c0 == 0 ? 0 : 1;
                    } else if(c0 + c1 == 0 && _p2PauseStep < 2) {
                        _p2PauseStep++;
                        return SelectedCharacter;
                    }
                }
            }
            return -1;
        }
    }

    public class Character {
        public decimal Cooldown { get; set; }
        public decimal Max { get; set; }
        public bool Processing { get; set; }
    }
}
