namespace ChemtechUrgot
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using Aimtec;
    using Aimtec.SDK.Prediction.Health;
    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Components;
    using Aimtec.SDK.Orbwalking;
    using Aimtec.SDK.TargetSelector;
    using Aimtec.SDK.Util.Cache;
    using Aimtec.SDK.Prediction.Skillshots;
    using Aimtec.SDK.Util;

    using Spell = Aimtec.SDK.Spell;
    using Aimtec.SDK.Events;

    internal class ChemtechUrgot
    {
        public static Menu Menu = new Menu("Urgot", "Chemtech Urgot v0.2", true);
        public static Orbwalker Orbwalker = new Orbwalker();
        public static Obj_AI_Hero Player = ObjectManager.GetLocalPlayer();
        public static Spell Q, W, E, R;

        public void LoadSpells()
        {
            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 490);
            E = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 1600);

            Q.SetSkillshot(0.25f, 150, 1700, false, SkillshotType.Circle);
            E.SetSkillshot(0.25f, 160, 1000, false, SkillshotType.Line);
            R.SetSkillshot(0.25f, 90, 3200, false, SkillshotType.Line);
        }
        public ChemtechUrgot()
        {
            Orbwalker.Attach(Menu);
            var Combo = new Menu("combo", "Combo");
            {
                Combo.Add(new MenuBool("Q", "Q - Corrosive Charge"));
                Combo.Add(new MenuBool("W", "W - Purge"));
                Combo.Add(new MenuBool("E", "E - Disdain"));
                Combo.Add(new MenuBool("R", "R - Fear Beyond Death"));
                Combo.Add(new MenuSeperator("I", "Smart R when combo killable"));
                Combo.Add(new MenuSlider("M", "Min mana % to combo", 0));
            }
            Menu.Add(Combo);
            var Harass = new Menu("harass", "Harass");
            {
                Harass.Add(new MenuBool("Q", "Q - Corrosive Charge"));
                Harass.Add(new MenuBool("W", "W - Purge"));
                Harass.Add(new MenuBool("E", "E - Disdain", false));
                Harass.Add(new MenuSlider("M", "Min mana % to harass", 60));
            }
            Menu.Add(Harass);
            var LaneClear = new Menu("laneclear", "Laneclear");
            {
                LaneClear.Add(new MenuBool("Q", "Q - Corrosive Charge"));
                LaneClear.Add(new MenuSlider("Qx", "Min minions to Q", 3, 1, 7));
                LaneClear.Add(new MenuBool("W", "W - Purge"));
                LaneClear.Add(new MenuSlider("Wx", "Min minions to W", 7, 1, 7));
                LaneClear.Add(new MenuBool("E", "E - Purge"));
                LaneClear.Add(new MenuSlider("Ex", "Min minions to E", 6, 1, 7));
                LaneClear.Add(new MenuSlider("M", "Min mana % to laneclear", 40));
            }
            Menu.Add(LaneClear);
            var JungleClear = new Menu("jungleclear", "Jungleclear");
            {
                JungleClear.Add(new MenuBool("Q", "Q - Corrosive Charge"));
                JungleClear.Add(new MenuSlider("Qx", "Min minions to Q", 4, 1, 4));
                JungleClear.Add(new MenuBool("W", "W - Purge"));
                JungleClear.Add(new MenuSlider("Wx", "Min minions to W", 4, 1, 4));
                JungleClear.Add(new MenuBool("E", "E - Disdain"));
                JungleClear.Add(new MenuSlider("Ex", "Min minions to E", 1, 1, 4));
                JungleClear.Add(new MenuSlider("M", "Min mana % to jungleclear", 30));
            }
            Menu.Add(JungleClear);
            var Flee = new Menu("flee", "Flee");
            {
                Flee.Add(new MenuBool("E", "E - Disdain"));
                Flee.Add(new MenuSlider("M", "Min mana % to flee", 5));
            }
            Menu.Add(Flee);
            var Killsteal = new Menu("killsteal", "Killsteal");
            {
                Killsteal.Add(new MenuBool("Q", "Q - Corrosive Charge"));
                Killsteal.Add(new MenuBool("E", "E - Disdain"));
                Killsteal.Add(new MenuBool("R", "R - Fear Beyond Death"));
                Killsteal.Add(new MenuSlider("M", "Min mana % to killsteal", 5));
            }
            Menu.Add(Killsteal);
            var Misc = new Menu("misc", "Misc");
            {
                Misc.Add(new MenuSlider("E", "E - Custom Range", 425, 225, 475));
                Misc.Add(new MenuKeyBind("LK", "Clear spells toggle key", KeyCode.A, KeybindType.Toggle));
                Misc.Add(new MenuKeyBind("HK", "Harass spells toggle key", KeyCode.S, KeybindType.Toggle));
                Misc.Add(new MenuKeyBind("FK", "Flee hold key", KeyCode.Z, KeybindType.Press));
            }
            Menu.Add(Misc);
            var Drawings = new Menu("drawings", "Drawings");
            {
                Drawings.Add(new MenuBool("Q", "Q - Corrosive Charge"));
                Drawings.Add(new MenuBool("W", "W - Purge", false));
                Drawings.Add(new MenuBool("E", "E - Disdain"));
                Drawings.Add(new MenuBool("R", "R - Fear Beyond Death"));
                Drawings.Add(new MenuBool("S", "Spell Toggle Status"));
                Drawings.Add(new MenuBool("D", "Damage over Health Bar"));
            }
            Menu.Add(Drawings);
            Menu.Attach();

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;

            LoadSpells();
            Console.WriteLine("Chemtech Urgot v0.2 loaded. Have fun!");
        }

        public static readonly List<string> SpecialChampions = new List<string> { "Annie", "Jhin" };
        public static int SxOffset(Obj_AI_Hero target)
        {
            return SpecialChampions.Contains(target.ChampionName) ? 1 : 10;
        }
        public static int SyOffset(Obj_AI_Hero target)
        {
            return SpecialChampions.Contains(target.ChampionName) ? 3 : 20;
        }

        private static double Qdmg(Obj_AI_Base enemy)
        {
            if (Q.Ready)
            {
                return Player.GetSpellDamage(enemy, SpellSlot.Q);
            }
            return 0;
        }
        private static double Wdmg(Obj_AI_Base enemy)
        {
            if (W.Ready)
            {
                return Player.GetSpellDamage(enemy, SpellSlot.W) * 15;
            }
            return 0;
        }
        private static double Edmg(Obj_AI_Base enemy)
        {
            if (E.Ready)
            {
                return Player.GetSpellDamage(enemy, SpellSlot.E);
            }
            return 0;
        }
        private static double Rdmg(Obj_AI_Base enemy)
        {
            if (R.Ready)
            {
                return Player.GetSpellDamage(enemy, SpellSlot.R);
            }
            return 0;
        }

        private static void Render_OnPresent()
        {
            if (Menu["Drawings"]["Q"].As<MenuBool>().Enabled && Q.Ready)
                Render.Circle(Player.Position, Q.Range, 30, Color.LimeGreen);
            if (Menu["Drawings"]["W"].As<MenuBool>().Enabled && W.Ready)
                Render.Circle(Player.Position, W.Range, 30, Color.Cyan);
            if (Menu["Drawings"]["E"].As<MenuBool>().Enabled && E.Ready)
                Render.Circle(Player.Position, E.Range, 30, Color.Pink);
            if (Menu["Drawings"]["R"].As<MenuBool>().Enabled && R.Ready)
                Render.Circle(Player.Position, R.Range, 30, Color.DarkBlue);

            if (Menu["Drawings"]["S"].As<MenuBool>().Enabled)
            {
                Vector2 TextPos;
                var playerpos = Render.WorldToScreen(Player.Position, out TextPos);
                var xOffset = (int)TextPos.X;
                var yOffset = (int)TextPos.Y;

                if (Menu["Misc"]["LK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.LimeGreen, "Clear with Spells: On");
                }
                if (!Menu["Misc"]["LK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.Red, "Clear with Spells: Off");
                }

                if (Menu["Misc"]["HK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.LimeGreen, "Harass with Spells: On");
                }
                if (!Menu["Misc"]["HK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.Red, "Harass with Spells: Off");
                }
            }

            if (Menu["Drawings"]["D"].As<MenuBool>().Enabled)
            {
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(h => h is Obj_AI_Hero && h.IsValidTarget() && h.IsValidTarget(1800))
                    .ToList()
                    .ForEach(
                        unit =>
                        {

                            var heroUnit = unit as Obj_AI_Hero;
                            int width = 103;
                            int xOffset = SxOffset(heroUnit);
                            int yOffset = SyOffset(heroUnit);
                            var barPos = unit.FloatingHealthBarPosition;
                            barPos.X += xOffset;
                            barPos.Y += yOffset;
                            var drawEndXPos = barPos.X + width * (unit.HealthPercent() / 100);
                            var drawQStartXPos =
                                (float)(barPos.X + (unit.Health >
                                                     Qdmg(unit)
                                             ? width * ((unit.Health - (Qdmg(unit)
                                             )) /
                                                        unit.MaxHealth * 100 / 100)
                                             : 0));

                            Render.Line(drawQStartXPos, barPos.Y, drawEndXPos, barPos.Y, 12, true,
                                unit.Health < Qdmg(unit) + Wdmg(unit) * 15 + Edmg(unit) + Rdmg(unit)
                                    ? Color.Purple
                                    : Color.LimeGreen);

                            var drawWStartXPos =
                                (float)(barPos.X + (unit.Health >
                                                     Qdmg(unit) + Wdmg(unit)
                                             ? width * ((unit.Health - (Qdmg(unit) + Wdmg(unit) * 15
                                             )) /
                                                        unit.MaxHealth * 100 / 100)
                                             : 0));

                            Render.Line(drawWStartXPos, barPos.Y, drawQStartXPos, barPos.Y, 12, true,
                                unit.Health < Qdmg(unit) + Wdmg(unit) * 15 + Edmg(unit) + Rdmg(unit)
                                    ? Color.Purple
                                    : Color.Cyan);

                            var drawEStartXPos =
                                (float)(barPos.X + (unit.Health >
                                                     Qdmg(unit) + Wdmg(unit) * 15 + Edmg(unit)
                                             ? width * ((unit.Health - (Qdmg(unit) + Wdmg(unit) * 15 + Edmg(unit)
                                             )) /
                                                        unit.MaxHealth * 100 / 100)
                                             : 0));

                            Render.Line(drawEStartXPos, barPos.Y, drawWStartXPos, barPos.Y, 12, true,
                                unit.Health < Qdmg(unit) + Wdmg(unit) * 15 + Edmg(unit) + Rdmg(unit)
                                    ? Color.Purple
                                    : Color.Pink);

                            var drawRStartXPos =
                                (float)(barPos.X + (unit.Health >
                                                     Qdmg(unit) + Wdmg(unit) + Edmg(unit) + Rdmg(unit)
                                             ? width * ((unit.Health - (Qdmg(unit) + Wdmg(unit) * 15 + Edmg(unit) + Rdmg(unit)
                                             )) /
                                                        unit.MaxHealth * 100 / 100)
                                             : 0));

                            Render.Line(drawRStartXPos, barPos.Y, drawEStartXPos, barPos.Y, 12, true,
                                unit.Health < Qdmg(unit) + Wdmg(unit) * 15 + Edmg(unit) + Rdmg(unit)
                                    ? Color.Purple
                                    : Color.DarkBlue);

                        });
            }
        }

        private void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }
            if (Player.GetSpell(SpellSlot.W).ToggleState == 2)
            {
                Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);
            }
            switch (Orbwalker.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case OrbwalkingMode.Laneclear:
                    LaneClear();
                    JungleClear();
                    break;
            }
            if (Menu["Misc"]["FK"].Enabled)
            {
                Flee();
            }
            Killsteal();
        }

        public static Obj_AI_Hero GetBestEnemyHeroTarget()
        {
            return GetBestEnemyHeroTargetInRange(float.MaxValue);
        }
        public static Obj_AI_Hero GetBestEnemyHeroTargetInRange(float range)
        {
            var ts = TargetSelector.Implementation;
            var target = ts.GetTarget(range);
            if (target != null && target.IsValidTarget() && !Invulnerable.Check(target))
            {
                return target;
            }
            var firstTarget = ts.GetOrderedTargets(range)
                .FirstOrDefault(t => t.IsValidTarget() && !Invulnerable.Check(t));
            if (firstTarget != null)
            {
                return firstTarget;
            }
            return null;
        }

        private void Combo()
        {
            if (Player.ManaPercent() < Menu["Combo"]["M"].As<MenuSlider>().Value)
                return;

            if (Menu["Combo"]["R"].As<MenuBool>().Enabled && R.Ready)
            {
                var target = TargetSelector.GetTarget(E.Range);
                var pred = R.GetPrediction(target);
                if (target != null && pred.HitChance >= HitChance.VeryHigh && target.MaxHealth * 0.25 + Qdmg(target) + Wdmg(target) * 15 + Edmg(target) + Rdmg(target) > target.Health)
                {
                    R.Cast(target);
                }

            }
            if (Menu["Combo"]["Q"].As<MenuBool>().Enabled && Q.Ready)
            {
                var target = TargetSelector.GetTarget(Q.Range);
                var pred = Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Q.Cast(target);
            }
            if (Menu["Combo"]["E"].As<MenuBool>().Enabled && E.Ready)
            {
                var target = TargetSelector.GetTarget(Menu["Misc"]["E"].As<MenuSlider>().Value);
                var pred = E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    E.Cast(target);
            }
            if (Menu["Combo"]["W"].As<MenuBool>().Enabled && W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var target = TargetSelector.GetTarget(W.Range);
                if (target.IsValidTarget())
                    W.Cast();
            }
        }

        private void Harass()
        {
            if (Player.ManaPercent() < Menu["Harass"]["M"].As<MenuSlider>().Value || !Menu["Misc"]["HK"].Enabled)
                return;

            if (Menu["Harass"]["Q"].As<MenuBool>().Enabled && Q.Ready)
            {
                var target = TargetSelector.GetTarget(Q.Range);
                var pred = Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Q.Cast(target);
            }
            if (Menu["Harass"]["E"].As<MenuBool>().Enabled && E.Ready)
            {
                var target = TargetSelector.GetTarget(Menu["Misc"]["E"].As<MenuSlider>().Value);
                var pred = E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    E.Cast(target);
            }
            if (Menu["Harass"]["W"].As<MenuBool>().Enabled && W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var target = TargetSelector.GetTarget(W.Range);
                if (target.IsValidTarget())
                    W.Cast();
            }
        }

        public static List<Obj_AI_Minion> GetEnemyLaneMinionsTargets()
        {
            return GetEnemyLaneMinionsTargetsInRange(float.MaxValue);
        }

        public static List<Obj_AI_Minion> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(m => m.IsValidTarget(range)).ToList();
        }

        private static void LaneClear()
        {
            if (Player.ManaPercent() < Menu["LaneClear"]["M"].As<MenuSlider>().Value || !Menu["Misc"]["LK"].Enabled)
                return;

            if (Menu["LaneClear"]["Q"].As<MenuBool>().Enabled && Q.Ready)
            {
                var circle = FarmPos.GetCircularClearLocation(Q.Range, Q.Width, Menu["LaneClear"]["Qx"].As<MenuSlider>().Value);
                if (circle != null)
                    Q.Cast(circle.CastPosition);
            }
            if (Menu["LaneClear"]["E"].As<MenuBool>().Enabled && Q.Ready)
            {
                var line = FarmPos.GetLineClearLocation(Menu["Misc"]["E"].As<MenuSlider>().Value, E.Width);
                if (line != null && line.numberOfMinionsHit >= Menu["LaneClear"]["Ex"].As<MenuSlider>().Value)
                    E.Cast(line.CastPosition);
            }
            if (Menu["LaneClear"]["W"].As<MenuBool>().Enabled && W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var Wminions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(W.Range)).ToList();
                foreach (var minion in Wminions)
                    if (Wminions.Count >= Menu["LaneClear"]["Wx"].As<MenuSlider>().Value)
                        W.Cast();
            }
        }

        public static List<Obj_AI_Minion> GetGenericJungleMinionsTargets()
        {
            return GetGenericJungleMinionsTargetsInRange(float.MaxValue);
        }

        public static List<Obj_AI_Minion> GetGenericJungleMinionsTargetsInRange(float range)
        {
            return GameObjects.Jungle.Where(m => !GameObjects.JungleSmall.Contains(m) && m.IsValidTarget(range)).ToList();
        }

        private void JungleClear()
        {
            if (Player.ManaPercent() < Menu["JungleClear"]["M"].As<MenuSlider>().Value || !Menu["Misc"]["LK"].Enabled)
                return;

            if (Menu["JungleClear"]["Q"].As<MenuBool>().Enabled && Q.Ready)
            {
                var Qminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Q.Range)).ToList();
                foreach (var minion in Qminions)
                    if (Qminions.Count >= Menu["JungleClear"]["Qx"].As<MenuSlider>().Value)
                        Q.Cast(minion);
            }
            if (Menu["JungleClear"]["E"].As<MenuBool>().Enabled && E.Ready)
            {
                var Eminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Menu["Misc"]["E"].As<MenuSlider>().Value)).ToList();
                foreach (var minion in Eminions)
                    if (Eminions.Count >= Menu["JungleClear"]["Wx"].As<MenuSlider>().Value)
                        E.Cast(minion);
            }
            if (Menu["JungleClear"]["W"].As<MenuBool>().Enabled && W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var Wminions = GameObjects.Jungle.Where(m => m.IsValidTarget(W.Range)).ToList();
                foreach (var minion in Wminions)
                    if (Wminions.Count >= Menu["JungleClear"]["Wx"].As<MenuSlider>().Value)
                        W.Cast();
            }
        }

        private static void Flee()
        {
            Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);

            if (Player.ManaPercent() < Menu["Flee"]["M"].As<MenuSlider>().Value)
                return;

            if (Menu["Flee"]["Q"].As<MenuBool>().Enabled && Q.Ready)
            {
                var target = TargetSelector.GetTarget(Q.Range);
                var pred = Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Q.Cast(target);
            }
            if (Menu["Flee"]["E"].As<MenuBool>().Enabled && E.Ready)
            {
                E.Cast(Game.CursorPos);
            }
        }

        private static void Killsteal()
        {
            if (R.Ready && Player.GetSpell(SpellSlot.R).ToggleState == 2)
            {
                R.Cast();
            }

            if (Player.ManaPercent() < Menu["Killsteal"]["M"].As<MenuSlider>().Value)
                return;

            if (Menu["Killsteal"]["R"].As<MenuBool>().Enabled && R.Ready)
            {
                var target = TargetSelector.GetTarget(R.Range);
                var pred = R.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.VeryHigh && target.HealthPercent() < 25)
                    R.Cast(target);
            }
            if (Menu["Killsteal"]["E"].As<MenuBool>().Enabled && E.Ready)
            {
                var target = TargetSelector.GetOrderedTargets(Menu["Misc"]["E"].As<MenuSlider>().Value).FirstOrDefault(x => Player.GetSpellDamage(x, SpellSlot.E) >= x.Health);
                var pred = E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    E.Cast(target);
            }
            if (Menu["Killsteal"]["Q"].As<MenuBool>().Enabled && Q.Ready)
            {
                var target = TargetSelector.GetOrderedTargets(Q.Range).FirstOrDefault(x => Player.GetSpellDamage(x, SpellSlot.Q) >= x.Health);
                var pred = Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Q.Cast(target);
            }
        }
    }
}