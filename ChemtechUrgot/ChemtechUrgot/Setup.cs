using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;
using System.Linq;

namespace ChemtechUrgot
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
                Main = new Menu("Urgot", "ChemtechUrgot v0.0.0.2", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("combo", "Combo")
                {
                    new MenuBool("Q", "Q - Corrosive Charge"),
                    new MenuBool("W", "W - Purge"),
                    new MenuBool("E", "E - Disdain"),
                    new MenuBool("R", "R - Fear Beyond Death"),
                    new MenuSlider("M", "Mana % to Combo", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("harass", "Harass")
                {
                    new MenuBool("Q", "Q - Corrosive Charge"),
                    new MenuBool("W", "W - Purge"),
                    new MenuBool("E", "E - Disdain"),
                    new MenuSlider("M", "Mana % to Harass", 60),
                };
                Main.Add(Harass);
            }

            {
                Laneclear = new Menu("laneclear", "Laneclear")
                {
                    new MenuBool("Q", "Q - Corrosive Charge"),
                    new MenuSlider("Qx", "Minions to Q", 3, 1, 3),
                    new MenuBool("W", "W - Purge"),
                    new MenuSlider("Wx", "Minions to W", 6, 1, 7),
                    new MenuBool("E", "E - Disdain"),
                    new MenuSlider("Ex", "Minions to E", 6, 1, 7),
                    new MenuSlider("M", "Mana % to Clear", 40),
                };
                Main.Add(Laneclear);
            }

            {
                Jungleclear = new Menu("jungleclear", "Jungleclear")
                {
                    new MenuBool("Q", "Q - Corrosive Charge"),
                    new MenuSlider("Qx", "Minions to Q", 1, 1, 4),
                    new MenuBool("W", "W - Purge"),
                    new MenuSlider("Wx", "Minions to W", 1, 1, 4),
                    new MenuBool("E", "E - Disdain"),
                    new MenuSlider("Ex", "Minions to E", 1, 1, 4),
                    new MenuSlider("M", "Mana % to Clear", 30),
                };
                Main.Add(Jungleclear);
            }

            {
                Flee = new Menu("flee", "Flee")
                {
                    new MenuBool("E", "E - Disdain"),
                    new MenuSlider("M", "Mana % to Flee", 0),
                };
                Main.Add(Flee);
            }

            {
                Killsteal = new Menu("killsteal", "Killsteal")
                {
                    new MenuBool("Q", "Q - Corrosive Charge"),
                    new MenuBool("E", "E - Disdain"),
                    new MenuBool("R", "R - Fear Beyond Death"),
                    new MenuSlider("M", "Mana % to Killsteal", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Misc = new Menu("misc", "Misc")
                {
                    new MenuSlider("E", "E - Custom Range", 425, 225, 475),
                    new MenuKeyBind("LK", "Clear spells toggle key", KeyCode.A, KeybindType.Toggle),
                    new MenuKeyBind("HK", "Harass spells toggle key", KeyCode.S, KeybindType.Toggle),
                    new MenuKeyBind("FK", "Flee hold key", KeyCode.Z, KeybindType.Press),
                };
                Main.Add(Misc);
            }

            {
                Draw = new Menu("draw", "Draw")
                {
                    new MenuBool("Q", "Q - Corrosive Charge"),
                    new MenuBool("W", "W - Purge", false),
                    new MenuBool("E", "E - Disdain"),
                    new MenuBool("R", "R - Fear Beyond Death", false),
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