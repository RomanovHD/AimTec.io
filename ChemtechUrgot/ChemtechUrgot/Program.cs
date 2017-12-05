using System;
using System.Collections.Generic;
using System.Drawing;
using Aimtec;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Events;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.TargetSelector;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Damage;
using Aimtec.SDK.Prediction.Skillshots;
using System.Linq;
using Aimtec.SDK.Util.Cache;

namespace ChemtechUrgot
{
    class Program
    {
        public static Obj_AI_Hero Player;
        static void Main(string[] args)
        {
            GameEvents.GameStart += GameEvents_GameStart;
        }

        private static void GameEvents_GameStart()
        {
            Player = ObjectManager.GetLocalPlayer();
            if (Player.ChampionName != "Urgot")
                return;

            Setup.Initialize();
            Spells.Initialize();

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate()
        {
            if (Player.GetSpell(SpellSlot.W).ToggleState == 2)
                Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);

            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            Killsteal();

            Flee();

            switch (Orbwalker.Implementation.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case OrbwalkingMode.Laneclear:
                    Laneclear();
                    Jungleclear();
                    break;
            }
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
            if (Spells.Q.Ready)
            {
                return Player.CalculateDamage(enemy, DamageType.Physical, (float)new double[] { 25, 70, 115, 160, 205 }[Player.SpellBook.GetSpell(SpellSlot.Q).Level] + Player.TotalAttackDamage * 0.7);
            }
            return 0;
        }

        private static double Wdmg(Obj_AI_Base enemy)
        {
            if (Spells.W.Ready)
            {
                return Player.CalculateDamage(enemy, DamageType.Physical, (float)new double[] { 0.2, 0.24, 0.28, 0.32, 0.36 }[Player.SpellBook.GetSpell(SpellSlot.W).Level] * Player.TotalAttackDamage + 12 * 15);
            }
            return 0;
        }

        private static double Edmg(Obj_AI_Base enemy)
        {
            if (Spells.E.Ready)
            {
                return Player.CalculateDamage(enemy, DamageType.Physical, (float)new double[] { 60, 100, 140, 180, 220 }[Player.SpellBook.GetSpell(SpellSlot.E).Level] + Player.TotalAttackDamage * 0.5);
            }
            return 0;
        }

        private static double Rdmg(Obj_AI_Base enemy)
        {
            if (Spells.R.Ready)
            {
                return Player.CalculateDamage(enemy, DamageType.Physical, (float)new double[] { 50, 175, 300 }[Player.SpellBook.GetSpell(SpellSlot.R).Level] + Player.TotalAttackDamage * 0.5 + enemy.Health/enemy.MaxHealth * 25);
            }
            return 0;
        }

        private static void Combo()
        {
            if (Player.ManaPercent() < Setup.Combo["M"].As<MenuSlider>().Value)
                return;

            if (Setup.Combo["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                var target = TargetSelector.GetTarget(Setup.Misc["E"].As<MenuSlider>().Value);
                var pred = Spells.R.GetPrediction(target);
                if (target != null && pred.HitChance >= HitChance.High && Qdmg(target) + Wdmg(target) + Edmg(target) + Rdmg(target) > target.Health + target.MaxHealth * 0.25)
                {
                    Spells.R.Cast(target);
                }

            }
            if (Setup.Combo["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                var pred = Spells.Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.Q.Cast(target);
            }
            if (Setup.Combo["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Setup.Misc["E"].As<MenuSlider>().Value);
                var pred = Spells.E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.E.Cast(target);
            }
            if (Setup.Combo["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                if (target.IsValidTarget())
                    Spells.W.Cast();
            }
        }

        private static void Harass()
        {
            if (Player.ManaPercent() < Setup.Harass["M"].As<MenuSlider>().Value || !Setup.Harass["K"].Enabled)
                return;

            if (Setup.Harass["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                var pred = Spells.Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.Q.Cast(target);
            }
            if (Setup.Harass["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Setup.Misc["E"].As<MenuSlider>().Value);
                var pred = Spells.E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.E.Cast(target);
            }
            if (Setup.Harass["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                if (target.IsValidTarget())
                    Spells.W.Cast();
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

        private static void Laneclear()
        {
            if (Player.ManaPercent() < Setup.Laneclear["M"].As<MenuSlider>().Value || !Setup.Misc["LK"].Enabled)
                return;

            if (Setup.Laneclear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var circle = FarmPos.GetCircularClearLocation(Spells.Q.Range, Spells.Q.Width, Setup.Laneclear["Qx"].As<MenuSlider>().Value);
                if (circle != null)
                    Spells.Q.Cast(circle.CastPosition);
            }
            if (Setup.Laneclear["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var line = FarmPos.GetLineClearLocation(Setup.Misc["E"].As<MenuSlider>().Value, Spells.E.Width);
                if (line != null && line.numberOfMinionsHit >= Setup.Laneclear["Ex"].As<MenuSlider>().Value)
                    Spells.E.Cast(line.CastPosition);
            }
            if (Setup.Laneclear["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var Wminions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Spells.W.Range)).ToList();
                foreach (var minion in Wminions)
                    if (Wminions.Count >= Setup.Laneclear["Wx"].As<MenuSlider>().Value)
                        Spells.W.Cast();
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

        private static void Jungleclear()
        {
            if (Player.ManaPercent() < Setup.Jungleclear["M"].As<MenuSlider>().Value || !Setup.Misc["LK"].Enabled)
                return;

            if (Setup.Jungleclear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var Qminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.Q.Range)).ToList();
                foreach (var minion in Qminions)
                    if (Qminions.Count >= Setup.Jungleclear["Qx"].As<MenuSlider>().Value)
                        Spells.Q.Cast(minion);
            }
            if (Setup.Jungleclear["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var Eminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Setup.Misc["E"].As<MenuSlider>().Value)).ToList();
                foreach (var minion in Eminions)
                    if (Eminions.Count >= Setup.Jungleclear["Wx"].As<MenuSlider>().Value)
                        Spells.E.Cast(minion);
            }
            if (Setup.Jungleclear["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var Wminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.W.Range)).ToList();
                foreach (var minion in Wminions)
                    if (Wminions.Count >= Setup.Jungleclear["Wx"].As<MenuSlider>().Value)
                        Spells.W.Cast();
            }
        }

        private static void Flee()
        {
            if (!Setup.Misc["FK"].Enabled)
            {
                return;
            }

                Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);

            if (Player.ManaPercent() < Setup.Flee["M"].As<MenuSlider>().Value)
                return;

            if (Setup.Flee["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                Spells.E.Cast(Game.CursorPos);
            }
            if (Setup.Flee["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                var pred = Spells.Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.Q.Cast(target);
            }
        }

        private static void Killsteal()
        {
            if (Spells.R.Ready && Player.GetSpell(SpellSlot.R).ToggleState == 2)
            {
                Spells.R.Cast();
            }

            if (Player.ManaPercent() < Setup.Killsteal["M"].As<MenuSlider>().Value)
                return;

            if (Setup.Killsteal["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.R.Range);
                var pred = Spells.R.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && target.HealthPercent() < 25)
                    Spells.R.Cast(target);
            }
            if (Setup.Killsteal["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetOrderedTargets(Setup.Misc["E"].As<MenuSlider>().Value).FirstOrDefault(x => Player.GetSpellDamage(x, SpellSlot.E) >= x.Health);
                var pred = Spells.E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.E.Cast(target);
            }
            if (Setup.Killsteal["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetOrderedTargets(Spells.Q.Range).FirstOrDefault(x => Player.GetSpellDamage(x, SpellSlot.Q) >= x.Health);
                var pred = Spells.Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.Q.Cast(target);
            }
        }

        private static void Render_OnPresent()
        {
            if (Setup.Draw["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
                Render.Circle(Player.Position, Spells.Q.Range, 30, Color.LimeGreen);
            if (Setup.Draw["W"].As<MenuBool>().Enabled && Spells.W.Ready)
                Render.Circle(Player.Position, Spells.W.Range, 30, Color.Cyan);
            if (Setup.Draw["E"].As<MenuBool>().Enabled && Spells.E.Ready)
                Render.Circle(Player.Position, Setup.Misc["E"].As<MenuSlider>().Value, 30, Color.Pink);
            if (Setup.Draw["R"].As<MenuBool>().Enabled && Spells.R.Ready)
                Render.Circle(Player.Position, Spells.R.Range, 30, Color.DarkBlue);

            if (Setup.Draw["S"].As<MenuBool>().Enabled)
            {
                Vector2 TextPos;
                var playerpos = Render.WorldToScreen(Player.Position, out TextPos);
                var xOffset = (int)TextPos.X;
                var yOffset = (int)TextPos.Y;

                if (Setup.Misc["LK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.LimeGreen, "Clear with Spells: On");
                }
                if (!Setup.Misc["LK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.Red, "Clear with Spells: Off");
                }

                if (Setup.Misc["HK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.LimeGreen, "Harass with Spells: On");
                }
                if (!Setup.Misc["HK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.Red, "Harass with Spells: Off");
                }
            }

            if (Setup.Draw["D"].As<MenuBool>().Enabled)
            {
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(h => h is Obj_AI_Hero && h.IsValidTarget() && h.IsVisible)
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
                                unit.Health < Qdmg(unit) + Wdmg(unit) + Edmg(unit) + Rdmg(unit)
                                    ? Color.Purple
                                    : Color.LimeGreen);

                            var drawWStartXPos =
                                (float)(barPos.X + (unit.Health >
                                                     Qdmg(unit) + Wdmg(unit)
                                             ? width * ((unit.Health - (Qdmg(unit) + Wdmg(unit)
                                             )) /
                                                        unit.MaxHealth * 100 / 100)
                                             : 0));

                            Render.Line(drawWStartXPos, barPos.Y, drawQStartXPos, barPos.Y, 12, true,
                                unit.Health < Qdmg(unit) + Wdmg(unit) + Edmg(unit) + Rdmg(unit)
                                    ? Color.Purple
                                    : Color.Cyan);

                            var drawEStartXPos =
                                (float)(barPos.X + (unit.Health >
                                                     Qdmg(unit) + Wdmg(unit) + Edmg(unit)
                                             ? width * ((unit.Health - (Qdmg(unit) + Wdmg(unit) + Edmg(unit)
                                             )) /
                                                        unit.MaxHealth * 100 / 100)
                                             : 0));

                            Render.Line(drawEStartXPos, barPos.Y, drawWStartXPos, barPos.Y, 12, true,
                                unit.Health < Qdmg(unit) + Wdmg(unit) + Edmg(unit) + Rdmg(unit)
                                    ? Color.Purple
                                    : Color.Pink);

                            var drawRStartXPos =
                                (float)(barPos.X + (unit.Health >
                                                     Qdmg(unit) + Wdmg(unit) + Edmg(unit) + Rdmg(unit)
                                             ? width * ((unit.Health - (Qdmg(unit) + Wdmg(unit) + Edmg(unit) + Rdmg(unit)
                                             )) /
                                                        unit.MaxHealth * 100 / 100)
                                             : 0));

                            Render.Line(drawRStartXPos, barPos.Y, drawEStartXPos, barPos.Y, 12, true,
                                unit.Health < Qdmg(unit) + Wdmg(unit) + Edmg(unit) + Rdmg(unit)
                                    ? Color.Purple
                                    : Color.DarkBlue);

                        });
            }
        }
    }
}