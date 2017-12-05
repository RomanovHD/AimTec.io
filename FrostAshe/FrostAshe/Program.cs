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

namespace FrostAshe
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
            if (Player.ChampionName != "Ashe")
                return;

            Setup.Initialize();
            Spells.Initialize();

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.Implementation.PostAttack += ImplementationOnPostAttack;
        }

        private static void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            if (Setup.Misc["RK"].Enabled)
            {
                SemiR();
            }

            if (Setup.Misc["FK"].Enabled)
            {
                Flee();
            }

            Killsteal();

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

        private static void ImplementationOnPostAttack(object enemy, PostAttackEventArgs events)
        {
            if (Orbwalker.Implementation.Mode.Equals(OrbwalkingMode.Combo) && Setup.Combo["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                Obj_AI_Hero target = events.Target as Obj_AI_Hero;
                if (target != null)
                    Spells.W.Cast(target);
            }
            if (Orbwalker.Implementation.Mode.Equals(OrbwalkingMode.Combo) && Setup.Combo["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                Spells.Q.Cast();
            }
            if (Orbwalker.Implementation.Mode.Equals(OrbwalkingMode.Mixed) && Setup.Harass["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                Obj_AI_Hero target = events.Target as Obj_AI_Hero;
                if (target != null)
                    Spells.W.Cast(target);
            }
            if (Orbwalker.Implementation.Mode.Equals(OrbwalkingMode.Mixed) && Setup.Harass["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                Spells.Q.Cast();
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
            return 0;
        }

        private static double Wdmg(Obj_AI_Base enemy)
        {
            if (Spells.W.Ready)
            {
                return Player.GetSpellDamage(enemy, SpellSlot.W);
            }
            return 0;
        }

        private static double Edmg(Obj_AI_Base enemy)
        {
            return 0;
        }

        private static double Rdmg(Obj_AI_Base enemy)
        {
            if (Spells.R.Ready)
            {
                return Player.GetSpellDamage(enemy, SpellSlot.R);
            }
            return 0;
        }

        private static void Combo()
        {
            if (Player.ManaPercent() < Setup.Combo["M"].As<MenuSlider>().Value)
                return;

            if (Setup.Combo["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                var target = TargetSelector.GetTarget(Setup.Misc["RR"].As<MenuSlider>().Value);
                var pred = Spells.R.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Rdmg(target) + Wdmg(target) + Player.GetAutoAttackDamage(target) * Setup.Combo["AA"].As<MenuSlider>().Value > target.Health)
                    Spells.R.Cast(target);
            }
            if (Setup.Combo["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && target.Distance(Player.ServerPosition) > Spells.Q.Range)
                    Spells.W.Cast(target);
            }
        }

        private static void Harass()
        {
            if (Player.ManaPercent() < Setup.Harass["M"].As<MenuSlider>().Value || !Setup.Misc["HK"].Enabled)
                return;

            if (Setup.Harass["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.W.Cast(target);
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

            if (Setup.Laneclear["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var Wminions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Spells.W.Range)).ToList();
                foreach (var minion in Wminions)
                    if (Wminions.Count >= Setup.Laneclear["Wx"].As<MenuSlider>().Value)
                        Spells.W.Cast(minion);
            }
            if (Setup.Laneclear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var Qminions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Spells.Q.Range)).ToList();
                foreach (var minion in Qminions)
                    if (Qminions.Count >= Setup.Laneclear["Qx"].As<MenuSlider>().Value)
                        Spells.Q.Cast();
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

            if (Setup.Jungleclear["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var Wminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.W.Range)).ToList();
                foreach (var minion in Wminions)
                    if (Wminions.Count >= Setup.Jungleclear["Wx"].As<MenuSlider>().Value)
                        Spells.W.Cast(minion);
            }
            if (Setup.Jungleclear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var Qminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.Q.Range)).ToList();
                foreach (var minion in Qminions)
                    if (Qminions.Count >= Setup.Jungleclear["Qx"].As<MenuSlider>().Value)
                        Spells.Q.Cast();
            }
        }

        private static void Killsteal()
        {
            if (Player.ManaPercent() < Setup.Killsteal["M"].As<MenuSlider>().Value)
                return;

            if (Setup.Killsteal["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                var target = TargetSelector.GetOrderedTargets(Setup.Misc["RR"].As<MenuSlider>().Value).FirstOrDefault(x => Player.GetSpellDamage(x, SpellSlot.R) >= x.Health);
                var pred = Spells.R.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.R.Cast(target);
            }
            if (Setup.Killsteal["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var target = TargetSelector.GetOrderedTargets(Spells.W.Range).FirstOrDefault(x => Player.GetSpellDamage(x, SpellSlot.W) >= x.Health);
                var pred = Spells.W.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.W.Cast(target);
            }
        }

        private static void Flee()
        {
            Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);

            if (Player.ManaPercent() < Setup.Flee["M"].As<MenuSlider>().Value)
                return;

            if (Setup.Flee["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                var target = TargetSelector.GetTarget(Setup.Misc["RR"].As<MenuSlider>().Value);
                var pred = Spells.R.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.R.Cast(target);
            }
            if (Setup.Flee["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.W.Cast(target);
            }
        }

        private static void SemiR()
        {
            if (Setup.Flee["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                var target = TargetSelector.GetTarget(Setup.Misc["RR"].As<MenuSlider>().Value);
                var pred = Spells.R.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.R.Cast(target);
            }
        }

        private static void Render_OnPresent()
        {
            if (Setup.Draw["W"].As<MenuBool>().Enabled && Spells.W.Ready)
                Render.Circle(Player.Position, Spells.W.Range, 30, Color.Cyan);
            if (Setup.Draw["R"].As<MenuBool>().Enabled && Spells.R.Ready)
                Render.Circle(Player.Position, Setup.Misc["RR"].As<MenuSlider>().Value, 30, Color.DarkBlue);

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
                if (Setup.Misc["RK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 60, Color.LimeGreen, "Semi-manual R: On");
                }
                if (!Setup.Misc["RK"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 60, Color.Red, "Semi-manual R: Off");
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