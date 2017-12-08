using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;

namespace TristanaTheRuler
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
                Main = new Menu("Tristana", "[7.24] Tristana the Ruler", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("Combo", "Combo")
                {
                    new MenuBool("Q", "Q - Rapid Fire"),
                    new MenuBool("W", "W - Rocket Jump"),
                    new MenuBool("E", "E - Explosive Charge"),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("Harass", "Harass")
                {
                    new MenuBool("Q", "Q - Rapid Fire"),
                    new MenuBool("W", "W - Rocket Jump", false),
                    new MenuBool("E", "E - Explosive Charge"),
                    new MenuSlider("Mana", "Min mana %", 60),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.S, KeybindType.Toggle),
                };
                Main.Add(Harass);
            }

            {
                Clear = new Menu("Clear", "Clear")
                {
                    new MenuBool("Q", "Q - Rapid Fire"),
                    new MenuSlider("Qx", "Min minions [Lane]", 5, 1, 7),
                    new MenuBool("E", "E - Explosive Charge"),
                    new MenuSeperator("Info", "(E for Turret and Jungle only)"),
                    new MenuSlider("Mana", "Min mana %", 30),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.A, KeybindType.Toggle),
                };
                Main.Add(Clear);
            }

            {
                Killsteal = new Menu("Killsteal", "Killsteal")
                {
                    new MenuBool("R", "R - Buster Shot"),
                    new MenuBool("Recall", "Disable Killsteal while recalling"),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Flee = new Menu("Flee", "Flee")
                {
                    new MenuBool("W", "W - Rocket Jump"),
                    new MenuBool("R", "R - Buster Shot"),
                    new MenuSlider("Mana", "Min mana %", 0),
                    new MenuKeyBind("Key", "Flee Key", KeyCode.Z, KeybindType.Press),
                };
                Main.Add(Flee);
            }

            {
                Draw = new Menu("Draw", "Draw")
                {
                    new MenuBool("W", "W - Rocket Jump"),
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