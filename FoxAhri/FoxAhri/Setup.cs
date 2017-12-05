using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;
using System.Linq;

namespace FoxAhri
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
                Main = new Menu("Ahri", "FoxAhri v0.0.0.2", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("combo", "Combo")
                {
                    new MenuBool("Q", "Q - Orb of Deception"),
                    new MenuBool("W", "W - Fox-Fire"),
                    new MenuBool("E", "E - Charm"),
                    new MenuBool("R", "R - Spirit Rush"),
                    new MenuSlider("M", "Mana % to Combo", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("harass", "Harass")
                {
                    new MenuBool("Q", "Q - Orb of Deception"),
                    new MenuBool("W", "W - Fox-Fire"),
                    new MenuBool("E", "E - Charm"),
                    new MenuSlider("M", "Mana % to Harass", 60),
                };
                Main.Add(Harass);
            }

            {
                Laneclear = new Menu("laneclear", "Laneclear")
                {
                    new MenuBool("Q", "Q - Orb of Deception"),
                    new MenuSlider("Qx", "Minions to Q", 5, 1, 7),
                    new MenuBool("W", "W - Fox-Fire"),
                    new MenuSlider("Wx", "Minions to W", 3, 1, 3),
                    new MenuSlider("M", "Mana % to Laneclear", 40),
                };
                Main.Add(Laneclear);
            }

            {
                Jungleclear = new Menu("jungleclear", "Jungleclear")
                {
                    new MenuBool("Q", "Q - Orb of Deception"),
                    new MenuSlider("Qx", "Minions to Q", 1, 1, 4),
                    new MenuBool("W", "W - Fox-Fire"),
                    new MenuSlider("Wx", "Minions to W", 1, 1, 4),
                    new MenuBool("E", "E - Charm"),
                    new MenuSlider("M", "Mana % to Jungleclear", 40),
                };
                Main.Add(Jungleclear);
            }

            {
                Killsteal = new Menu("killsteal", "Killsteal")
                {
                    new MenuBool("Q", "Q - Orb of Deception"),
                    new MenuBool("E", "E - Charm"),
                    new MenuSlider("M", "Mana % to Killsteal", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Flee = new Menu("flee", "Flee")
                {
                    new MenuBool("Q", "Q - Orb of Deception"),
                    new MenuBool("E", "E - Charm"),
                    new MenuSlider("M", "Mana % to Flee", 0),
                };
                Main.Add(Flee);
            }

            {
                Misc = new Menu("misc", "Misc")
                {
                    new MenuKeyBind("LK", "Clear spells toggle key", KeyCode.A, KeybindType.Toggle),
                    new MenuKeyBind("HK", "Harass spells toggle key", KeyCode.S, KeybindType.Toggle),
                    new MenuKeyBind("FK", "Flee hold key", KeyCode.Z, KeybindType.Press),
                };
                Main.Add(Misc);
            }

            {
                Draw = new Menu("draw", "Draw")
                {
                    new MenuBool("Q", "Q - Orb of Deception"),
                    new MenuBool("W", "W - Fox-Fire", false),
                    new MenuBool("E", "E - Charm"),
                    new MenuBool("R", "R - Spirit Rush", false),
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