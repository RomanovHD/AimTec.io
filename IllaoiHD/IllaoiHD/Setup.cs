using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;
using System.Linq;

namespace IllaoiHD
{
    public static class Setup
    {
        public static Menu Main;
        public static Menu Combo;
        public static Menu Harass;
        public static Menu Clear;
        public static Menu Killsteal;
        public static Menu Draw;

        public static void Initialize()
        {
            {
                Main = new Menu("Illaoi", "IllaoiHD", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("combo", "Combo")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuBool("W", "W - Harsh Lesson"),
                    new MenuBool("E", "E - Test of Spirit"),
                    new MenuBool("R", "R - Leap of Fight"),
                    new MenuSlider("Rx", "Enemies to R", 3, 1, 5),
                    new MenuSlider("M", "Mana % to Combo", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("harass", "Harass")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuBool("W", "W - Harsh Lesson"),
                    new MenuBool("E", "E - Test of Spirit"),
                    new MenuSlider("M", "Mana % to Harass", 60),
                    new MenuKeyBind("K", "Harass Toggle Key", KeyCode.S, KeybindType.Toggle),
                };
                Main.Add(Harass);
            }

            {
                Clear = new Menu("clear", "Laneclear")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuSlider("Qx", "Minions to Q", 5, 1, 7),
                    new MenuSlider("M", "Mana % to Clear", 40),
                    new MenuKeyBind("K", "Laneclear Toggle Key", KeyCode.A, KeybindType.Toggle),
                };
                Main.Add(Clear);
            }

            {
                Killsteal = new Menu("killsteal", "Killsteal")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuBool("R", "R - Leap of Fight"),
                    new MenuSlider("M", "Mana % to Killsteal", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Draw = new Menu("draw", "Draw")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuBool("W", "W - Harsh Lesson", false),
                    new MenuBool("E", "E - Test of Spirit"),
                    new MenuBool("R", "R - Leap of Fight", false),
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