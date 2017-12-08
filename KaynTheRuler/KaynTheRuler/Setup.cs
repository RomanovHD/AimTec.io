using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;

namespace KaynTheRuler
{
    public static class Setup
    {
        public static Menu Main;
        public static Menu Combo;
        public static Menu Harass;
        public static Menu Clear;
        public static Menu Killsteal;
        public static Menu Flee;
        public static Menu Draw;

        public static void Initialize()
        {
            {
                Main = new Menu("Kayn", "[7.24] Kayn the Ruler", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("Combo", "Combo")
                {
                    new MenuBool("Q", "Q - Reaping Slash"),
                    new MenuBool("W", "W - Blade's Reach"),
                    new MenuBool("R", "R - Umbral Trespass"),
                    new MenuList("Rmode", "Use ult to", new[]{"start","finish"}, 0),
                    new MenuBool("DPS", "Maximize DPS"),
                    new MenuSeperator("Info", "(Priorize AA before Q and W)"),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("Harass", "Harass")
                {
                    new MenuBool("Q", "Q - Reaping Slash"),
                    new MenuBool("W", "W - Blade's Reach"),
                    new MenuBool("DPS", "Maximize DPS"),
                    new MenuSeperator("Info", "(Priorize AA before Q and W)"),
                    new MenuSlider("Mana", "Min mana %", 60),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.S, KeybindType.Toggle),
                };
                Main.Add(Harass);
            }

            {
                Clear = new Menu("Clear", "Clear")
                {
                    new MenuBool("Q", "Q - Reaping Slash"),
                    new MenuSlider("Qx", "Min minions [Lane]", 5, 1, 7),
                    new MenuBool("W", "W - Blade's Reach"),
                    new MenuSlider("Wx", "Min minions [Lane]", 5, 1, 7),
                    new MenuSlider("Mana", "Min mana %", 30),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.A, KeybindType.Toggle),
                };
                Main.Add(Clear);
            }

            {
                Killsteal = new Menu("Killsteal", "Killsteal")
                {
                    new MenuBool("Q", "Q - Reaping Slash"),
                    new MenuBool("W", "W - Blade's Reach"),
                    new MenuBool("R", "R - Umbral Trespass"),
                    new MenuBool("Recall", "Disable Killsteal while recalling"),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Flee = new Menu("Flee", "Flee")
                {
                    new MenuBool("Q", "Q - Reaping Slash"),
                    new MenuList("Qmode", "Use Q to", new[]{"walljump","cursor"}, 0),
                    new MenuBool("W", "W - Blade's Reach"),
                    new MenuBool("E", "E - Shadow Step"),
                    new MenuSlider("Erange", "Active when distance to wall <", 175, 175, 1600),
                    new MenuSlider("Mana", "Min mana %", 0),
                    new MenuKeyBind("Key", "Flee Key", KeyCode.Z, KeybindType.Press),
                };
                Main.Add(Flee);
            }

            {
                Draw = new Menu("Draw", "Draw")
                {
                    new MenuBool("Q", "Q - Reaping Slash"),
                    new MenuBool("W", "W - Blade's Reach"),
                    new MenuBool("E", "E - Shadow Step"),
                    new MenuBool("R", "R - Umbral Trespass"),
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