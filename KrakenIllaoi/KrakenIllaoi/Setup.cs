using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;
using System.Linq;

namespace KrakenIllaoi
{
    public static class Setup
    {
        public static Menu Main;
        public static Menu Combo;
        public static Menu Harass;
        public static Menu Laneclear;
        public static Menu Jungleclear;
        public static Menu Killsteal;
        public static Menu Misc;
        public static Menu Draw;

        public static void Initialize()
        {
            {
                Main = new Menu("Illaoi", "KrakenIllaoi v0.0.0.2", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("combo", "Combo")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuBool("W", "W - Harsh Lesson"),
                    new MenuBool("E", "E - Test of Spirit"),
                    new MenuBool("R", "R - Leap of Faith"),
                    new MenuSlider("Rx", "Min enemies to R", 2, 1, 5),
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
                };
                Main.Add(Harass);
            }

            {
                Laneclear = new Menu("laneclear", "Laneclear")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuSlider("Qx", "Minions to Q", 5, 1, 7),
                    new MenuSlider("M", "Mana % to Clear", 40),
                };
                Main.Add(Laneclear);
            }

            {
                Jungleclear = new Menu("jungleclear", "Jungleclear")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuSlider("Qx", "Minions to Q", 1, 1, 4),
                    new MenuBool("W", "W - Harsh Lesson"),
                    new MenuSlider("M", "Mana % to Clear", 30),
                };
                Main.Add(Jungleclear);
            }

            {
                Killsteal = new Menu("killsteal", "Killsteal")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuBool("W", "W - Harsh Lesson"),
                    new MenuBool("R", "R - Leap of Faith"),
                    new MenuSlider("M", "Mana % to Killsteal", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Misc = new Menu("misc", "Misc")
                {
                    new MenuBool("AA", "Reset AA with Tiamat items"),
                    new MenuKeyBind("LK", "Clear spells toggle key", KeyCode.A, KeybindType.Toggle),
                    new MenuKeyBind("HK", "Harass spells toggle key", KeyCode.S, KeybindType.Toggle),
                };
                Main.Add(Misc);
            }

            {
                Draw = new Menu("draw", "Draw")
                {
                    new MenuBool("Q", "Q - Tentacle Smash"),
                    new MenuBool("W", "W - Harsh Lesson", false),
                    new MenuBool("E", "E - Test of Spirit"),
                    new MenuBool("R", "R - Leap of Faith", false),
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
