using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;
using System.Linq;

namespace FrostAshe
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
                Main = new Menu("Ashe", "[7.24] AsheTheRuler", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("combo", "Combo")
                {
                    new MenuBool("Q", "Q - Ranger's Focus"),
                    new MenuBool("W", "W - Voley"),
                    new MenuBool("R", "R - Enchanted Crystal Arrow"),
                    new MenuSlider("AA", "AA after R to kill", 6, 1, 15),
                    new MenuSlider("M", "Mana % to Combo", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("harass", "Harass")
                {
                    new MenuBool("Q", "Q - Ranger's Focus"),
                    new MenuBool("W", "W - Voley"),
                    new MenuSlider("M", "Mana % to Harass", 60),
                };
                Main.Add(Harass);
            }

            {
                Laneclear = new Menu("laneclear", "Laneclear")
                {
                    new MenuBool("Q", "Q - Ranger's Focus"),
                    new MenuSlider("Qx", "Minions to Q", 5, 1, 7),
                    new MenuBool("W", "W - Voley"),
                    new MenuSlider("Wx", "Minions to W", 5, 1, 7),
                    new MenuSlider("M", "Mana % to Clear", 40),
                };
                Main.Add(Laneclear);
            }

            {
                Jungleclear = new Menu("jungleclear", "Jungleclear")
                {
                    new MenuBool("Q", "Q - Ranger's Focus"),
                    new MenuSlider("Qx", "Minions to Q", 1, 1, 4),
                    new MenuBool("W", "W - Voley"),
                    new MenuSlider("Wx", "Minions to W", 1, 1, 4),
                    new MenuSlider("M", "Mana % to Clear", 30),
                };
                Main.Add(Jungleclear);
            }

            {
                Killsteal = new Menu("killsteal", "Killsteal")
                {
                    new MenuBool("W", "W - Voley"),
                    new MenuBool("R", "R - Enchanted Crystal Arrow"),
                    new MenuSlider("M", "Mana % to Killsteal", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Flee = new Menu("flee", "Flee")
                {
                    new MenuBool("W", "W - Voley"),
                    new MenuBool("R", "R - Enchanted Crystal Arrow", false),
                    new MenuSlider("M", "Mana % to Flee", 0),
                };
                Main.Add(Flee);
            }

            {
                Misc = new Menu("misc", "Misc")
                {
                    new MenuSlider("RR", "R cast range", 1500, 1200, 4500),
                    new MenuKeyBind("RK", "Semi-manual R cast key", KeyCode.T, KeybindType.Press),
                    new MenuKeyBind("LK", "Clear spells toggle key", KeyCode.A, KeybindType.Toggle),
                    new MenuKeyBind("HK", "Harass spells toggle key", KeyCode.S, KeybindType.Toggle),
                    new MenuKeyBind("FK", "Flee hold Key", KeyCode.Z, KeybindType.Press),
                };
                Main.Add(Misc);
            }

            {
                Draw = new Menu("draw", "Draw")
                {
                    new MenuBool("W", "W - Voley"),
                    new MenuBool("R", "R - Enchanted Crystal Arrow"),
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
