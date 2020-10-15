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
        public static int SelectedCharacter { get; set; } = -1;

        public static int GetSelectedCharacter(IntPtr handle) {
            if(handle == IntPtr.Zero) { return -1; }
            bool c0 = IMG.GetColorAt(handle, Config.PartyNumLocation.X, Config.PartyNumLocation.Y) != Color.FromArgb(255, 255, 255, 255);
            bool c1 = IMG.GetColorAt(handle, Config.PartyNumLocation.X, Config.PartyNumLocation.Y + (Config.PartyNumYOffset)) != Color.FromArgb(255, 255, 255, 255);
            bool c2 = IMG.GetColorAt(handle, Config.PartyNumLocation.X, Config.PartyNumLocation.Y + (Config.PartyNumYOffset * 2)) != Color.FromArgb(255, 255, 255, 255);
            bool c3 = IMG.GetColorAt(handle, Config.PartyNumLocation.X, Config.PartyNumLocation.Y + (Config.PartyNumYOffset * 3)) != Color.FromArgb(255, 255, 255, 255);

            if((c0 && c1 && c2 && c3) || (!c0 && !c1 && !c2 && !c3)) {
                return -1;
            } else if(c0) {
                return 0;
            } else if(c1) {
                return 1;
            } else if(c2) {
                return 2;
            } else {
                return 3;
            }
        }
    }

    public class Character {
        public decimal Cooldown { get; set; }
        public decimal Max { get; set; }
        public bool Processing { get; set; }
    }
}
