using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;

namespace BrandTheRuler
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
                Main = new Menu("Brand", "[7.24] Brand the Ruler", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("Combo", "Combo")
                {
                    new MenuBool("Q", "Q - Sear"),
                    new MenuBool("W", "W - Pillar of Flame"),
                    new MenuBool("E", "E - Conflagration"),
                    new MenuBool("R", "R - Pyroclasm"),
                    new MenuSlider("Rx", "Min enemies", 3, 1, 5),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("Harass", "Harass")
                {
                    new MenuBool("Q", "Q - Sear"),
                    new MenuBool("W", "W - Pillar of Flame"),
                    new MenuBool("E", "E - Conflagration"),
                    new MenuSlider("Mana", "Min mana %", 60),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.S, KeybindType.Toggle),
                };
                Main.Add(Harass);
            }

            {
                Clear = new Menu("Clear", "Clear")
                {
                    new MenuBool("Q", "Q - Sear [jungle]"),
                    new MenuBool("W", "W - Pillar of Flame"),
                    new MenuSlider("Wx", "Min minions [Lane]", 5, 1, 7),
                    new MenuBool("E", "E - Conflagration"),
                    new MenuSlider("Ex", "Min minions [Lane]", 5, 1, 7),
                    new MenuSlider("Mana", "Min mana %", 30),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.A, KeybindType.Toggle),
                };
                Main.Add(Clear);
            }

            {
                Killsteal = new Menu("Killsteal", "Killsteal")
                {
                    new MenuBool("Q", "Q - Sear"),
                    new MenuBool("W", "W - Pillar of Flame"),
                    new MenuBool("E", "E - Conflagration"),
                    new MenuBool("R", "R - Pyroclasm"),
                    new MenuBool("Recall", "Disable Killsteal while recalling", false),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Draw = new Menu("Draw", "Draw")
                {
                    new MenuBool("Q", "Q - Sear"),
                    new MenuBool("W", "W - Pillar of Flame"),
                    new MenuBool("E", "E - Conflagration"),
                    new MenuBool("R", "R - Pyroclasm"),
                    new MenuBool("Key", "Key Toggle Status"),
                    new MenuBool("Damage", "Damage"),
                };
                Main.Add(Draw);
            }

            {
                Main.Attach();
            }
        }
    }
}