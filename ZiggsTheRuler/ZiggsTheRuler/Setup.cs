using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;

namespace ZiggsTheRuler
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
                Main = new Menu("Ziggs", "[7.24] Ziggs the Ruler", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("Combo", "Combo")
                {
                    new MenuBool("Q", "Q - Bouncing Bomb"),
                    new MenuBool("W", "W - Satchel Charge"),
                    new MenuList("Wmode", "Use W to", new[]{"pull","push"}, 0),
                    new MenuBool("E", "E - Hexplosive Minefield"),
                    new MenuBool("R", "R - Mega Inferno Bomb"),
                    new MenuSlider("Rx", "Min enemies to ult", 3, 1, 5),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("Harass", "Harass")
                {
                    new MenuBool("Q", "Q - Bouncing Bomb"),
                    new MenuBool("W", "W - Satchel Charge"),
                    new MenuList("Wmode", "Use W to", new[]{"pull","push"}, 1),
                    new MenuBool("E", "E - Hexplosive Minefield"),
                    new MenuSlider("Mana", "Min mana %", 60),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.S, KeybindType.Toggle),
                };
                Main.Add(Harass);
            }

            {
                Clear = new Menu("Clear", "Clear")
                {
                    new MenuBool("Q", "Q - Bouncing Bomb"),
                    new MenuSlider("Qx", "Min minions [Lane]", 3, 1, 7),
                    new MenuBool("W", "W - Satchel Charge"),
                    new MenuSeperator("Info", "(W is for turret only)"),
                    new MenuBool("E", "E - Hexplosive Minefield"),
                    new MenuSlider("Ex", "Min minions [Lane]", 5, 1, 7),
                    new MenuSlider("Mana", "Min mana %", 30),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.A, KeybindType.Toggle),
                };
                Main.Add(Clear);
            }

            {
                Killsteal = new Menu("Killsteal", "Killsteal")
                {
                    new MenuBool("Q", "Q - Bouncing Bomb"),
                    new MenuBool("W", "W - Satchel Charge"),
                    new MenuBool("E", "E - Hexplosive Minefield"),
                    new MenuBool("R", "R - Mega Inferno Bomb"),
                    new MenuBool("Recall", "Disable Killsteal while recalling"),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Flee = new Menu("Flee", "Flee")
                {
                    new MenuBool("W", "W - Satchel Charge"),
                    new MenuList("Wmode", "Use W to", new[]{"push enemy","jump","both"}, 0),
                    new MenuBool("E", "E - Hexplosive Minefield"),
                    new MenuSlider("Mana", "Min mana %", 0),
                    new MenuKeyBind("Key", "Flee Key", KeyCode.Z, KeybindType.Press),
                };
                Main.Add(Flee);
            }

            {
                Draw = new Menu("Draw", "Draw")
                {
                    new MenuBool("Q", "Q - Bouncing Bomb"),
                    new MenuBool("W", "W - Satchel Charge"),
                    new MenuBool("E", "E - Hexplosive Minefield"),
                    new MenuBool("R", "R - Mega Inferno Bomb"),
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