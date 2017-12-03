using Aimtec;
using Aimtec.SDK.Menu;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Util;
using System.Linq;

namespace AhriHD
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
                Main = new Menu("Ahri", "AhriHD", true);
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
                    new MenuKeyBind("K", "Harass Toggle Key", KeyCode.S, KeybindType.Toggle),
                };
                Main.Add(Harass);
            }
            
            {
                Clear = new Menu("clear", "Laneclear")
                {
                    new MenuBool("Q", "Q - Orb of Deception"),
                    new MenuSlider("Qx", "Minions to Q", 5, 1, 7),
                    new MenuBool("W", "W - Fox-Fire"),
                    new MenuSlider("Wx", "Minions to W", 3, 1, 3),
                    new MenuSlider("M", "Mana % to Clear", 40),
                    new MenuKeyBind("K", "Laneclear Toggle Key", KeyCode.A, KeybindType.Toggle),
                };
                Main.Add(Clear);
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
                    new MenuKeyBind("K", "Flee Key", KeyCode.Z, KeybindType.Press),
                };
                Main.Add(Flee);
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