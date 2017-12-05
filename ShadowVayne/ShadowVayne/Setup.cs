using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;
using System.Linq;

namespace ShadowVayne
{
    public static class Setup
    {
        public static Menu Main;
        public static Menu Combo;
        public static Menu Harass;
        public static Menu Laneclear;
        public static Menu Jungleclear;
        public static Menu Killsteal;
        public static Menu Flee;
        public static Menu Misc;
        public static Menu Draw;

        public static void Initialize()
        {
            {
                Main = new Menu("Vayne", "ShadowVayne v0.0.0.1", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("combo", "Combo")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuBool("E", "E - Codemn"),
                    new MenuBool("R", "R - Final Hour"),
                    new MenuList("Rm", "R logic", new[]{"1v1 only","Teamfight only","Both"}, 3),
                    new MenuSlider("AA", "1v1: AA after R to kill", 12, 1, 15),
                    new MenuSlider("TF", "TF: Min enemies to R", 2, 1, 5),
                    new MenuSlider("M", "Mana % to Combo", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("harass", "Harass")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuBool("E", "E - Codemn"),
                    new MenuSlider("M", "Mana % to Harass", 60),
                };
                Main.Add(Harass);
            }

            {
                Laneclear = new Menu("laneclear", "Laneclear")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuSlider("M", "Mana % to Clear", 40),
                };
                Main.Add(Laneclear);
            }

            {
                Jungleclear = new Menu("jungleclear", "Jungleclear")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuBool("E", "E - Codemn"),
                    new MenuSlider("M", "Mana % to Clear", 30),
                };
                Main.Add(Jungleclear);
            }

            {
                Flee = new Menu("flee", "Flee")
                {
                    new MenuBool("Q", "Q - Tumble"),
                    new MenuBool("E", "E - Codemn"),
                    new MenuSlider("M", "Mana % to Flee", 0),
                };
                Main.Add(Flee);
            }

            {
                Killsteal = new Menu("killsteal", "Killsteal")
                {
                    new MenuBool("E", "E - Codemn"),
                    new MenuSlider("M", "Mana % to Killsteal", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Misc = new Menu("misc", "Misc")
                {
                    new MenuList("Qm", "Q mode", new[]{"Side","Safe","Aggressive","Cursor"}, 0),
                    new MenuKeyBind("LK", "Clear spells toggle key", KeyCode.A, KeybindType.Toggle),
                    new MenuKeyBind("HK", "Harass spells toggle key", KeyCode.S, KeybindType.Toggle),
                    new MenuKeyBind("FK", "Flee hold key", KeyCode.Z, KeybindType.Press),
                };
                Main.Add(Misc);
            }

            {
                Draw = new Menu("draw", "Draw")
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