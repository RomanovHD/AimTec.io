using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;

namespace VayneTheRuler
{
    public static class Setup
    {
        public static Menu Main;
        public static Menu Orb;
        public static Menu Combo;
        public static Menu Harass;
        public static Menu Clear;
        public static Menu Flee;
        public static Menu Killsteal;
        public static Menu Misc;
        public static Menu Draw;

        public static void Initialize()
        {
            {
                {
                    Main = new Menu("Vayne", "[7.24] Vayne the Ruler", true);
                    Orbwalker.Implementation.Attach(Main);
                }

                {
                    Orb = new Menu("Orb", "Orb Extra")
                {
                    new MenuBool("EAA", "Draw enemy AA range"),
                    new MenuBool("TAA", "Draw turret range"),
                };
                    Main.Add(Orb);
                }

                {
                    Combo = new Menu("Combo", "Combo")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuBool("W", "Focus W marked enemies"),
                    new MenuBool("E", "E - Codemn"),
                    new MenuBool("Flash", "Flash Codemn kill combo"),
                    new MenuBool("R", "R - Final Hour"),
                    new MenuSlider("Rx", "Min enemies", 2, 1, 5),
                    new MenuBool("1v1", "Enable 1v1 logic"),
                    new MenuSlider("Stealth", "Keep stealth for 1 sec if X enemies", 3, 1, 5),
                    new MenuSlider("Mana", "Min mana", 0),
                };
                    Main.Add(Combo);
                }

                {
                    Harass = new Menu("Harass", "Harass")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuBool("QW", "Only Q if enemy is W marked"),
                    new MenuBool("E", "E - Codemn"),
                    new MenuBool("EW", "Only E if enemy is W marked"),
                    new MenuSlider("Mana", "Min mana", 60),
                    new MenuKeyBind("Key", "Harass Toggle Key", KeyCode.S, KeybindType.Toggle),
                };
                    Main.Add(Harass);
                }

                {
                    Clear = new Menu("Clear", "Clear")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuBool("E", "E - Codemn [Jungle]"),
                    new MenuBool("Ekill", "Only E if minion will be killed"),
                    new MenuSlider("Mana", "Min mana", 30),
                    new MenuKeyBind("Key", "Clear Toggle Key", KeyCode.A, KeybindType.Toggle),
                };
                    Main.Add(Clear);
                }

                {
                    Flee = new Menu("Flee", "Flee")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuBool("E", "E - Codemn"),
                    new MenuSlider("Mana", "Min mana", 0),
                    new MenuKeyBind("Key", "Flee Key", KeyCode.Z, KeybindType.Press),
                };
                    Main.Add(Flee);
                }

                {
                    Killsteal = new Menu("Killsteal", "Killsteal")
                {
                    new MenuBool("E", "E - Codemn"),
                    new MenuSlider("Mana", "Min mana", 0),
                };
                    Main.Add(Killsteal);
                }

                {
                    Misc = new Menu("Misc", "Misc")
                {
                    new MenuBool("FocusW", "Focus W marked enemies"),
                    new MenuBool("Turret", "Block Q under enemy turrets"),
                    new MenuSlider("Qx", "Block Q into X enemies", 3, 2, 5),
                    new MenuBool("Engage", "Use Q as engage"),
                    new MenuBool("Peel", "Push melee champions"),
                    new MenuSlider("HP", "Push when under % HP", 30),
                };
                    Main.Add(Misc);
                }

                {
                    Draw = new Menu("Draw", "Draw")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuBool("S", "Spell Toggle Status"),
                    new MenuBool("D", "Damage over Health Bar"),
                };
                    Main.Add(Draw);
                }

                {
                    Main.Attach();
                }
            }
        }
    }
}