using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;

namespace TwitchTheRuler
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
                Main = new Menu("Twitch", "[7.24] Twitch the Ruler", true);
                Orbwalker.Implementation.Attach(Main);
            }

            {
                Combo = new Menu("Combo", "Combo")
                {
                    new MenuBool("Q", "Q - Ambush"),
                    new MenuBool("W", "W - Venom Cask"),
                    new MenuBool("R", "R - Spray and Pray"),
                    new MenuSlider("Rx", "Min enemies", 3, 1, 5),
                    new MenuBool("Rcombo", "R combo with X AA"),
                    new MenuSlider("Raa", "Max AA after ult", 4, 1, 10),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Combo);
            }

            {
                Harass = new Menu("Harass", "Harass")
                {
                    new MenuBool("Q", "Q - Ambush"),
                    new MenuBool("W", "W - Venom Cask"),
                    new MenuBool("E", "E - Contaminate"),
                    new MenuSlider("Ex", "Min E stacks", 6, 1, 6),
                    new MenuSlider("Mana", "Min mana %", 60),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.S, KeybindType.Toggle),
                };
                Main.Add(Harass);
            }

            {
                Clear = new Menu("Clear", "Clear")
                {
                    new MenuBool("Q", "Q - Ambush [jungle]"),
                    new MenuBool("W", "W - Venom Cask"),
                    new MenuSlider("Wx", "Min minions [Lane]", 5, 1, 7),
                    new MenuBool("E", "E - Contaminate"),
                    new MenuSlider("Ex", "Min minions [Lane]", 5, 1, 7),
                    new MenuBool("Es", "E smite (Red/Blue/Dragon/Herald/Baron)"),
                    new MenuSlider("Mana", "Min mana %", 30),
                    new MenuKeyBind("Key", "Key Toggle", KeyCode.A, KeybindType.Toggle),
                };
                Main.Add(Clear);
            }

            {
                Killsteal = new Menu("Killsteal", "Killsteal")
                {
                    new MenuBool("E", "E - Contaminate"),
                    new MenuBool("R", "R - Spray and Pray"),
                    new MenuBool("Recall", "Disable Killsteal while recalling", false),
                    new MenuSlider("Mana", "Min mana %", 0),
                };
                Main.Add(Killsteal);
            }

            {
                Flee = new Menu("Flee", "Flee")
                {
                    new MenuBool("Q", "Q - Ambush"),
                    new MenuBool("W", "W - Venom Cask"),
                    new MenuSlider("Mana", "Min mana %", 0),
                    new MenuKeyBind("Key", "Flee Key", KeyCode.Z, KeybindType.Press),
                };
                Main.Add(Flee);
            }

            {
                Draw = new Menu("Draw", "Draw")
                {
                    new MenuBool("W", "W - Venom Cask"),
                    new MenuBool("E", "E - Contaminate"),
                    new MenuBool("R", "R - Spray and Pray"),
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