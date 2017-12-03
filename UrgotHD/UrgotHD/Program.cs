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

namespace UrgotHD
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
            {
                Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);
            }

            Killsteal();

            if (Setup.Flee["K"].Enabled)
            {
                Flee();
            }

            switch (Orbwalker.Implementation.Mode)
            {
                case OrbwalkingMode.Combo:
                    Combo();
                    break;
                case OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case OrbwalkingMode.Laneclear:
                    Clear();
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
                return Player.GetSpellDamage(enemy, SpellSlot.Q);
            }
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
            if (Spells.E.Ready)
            {
                return Player.GetSpellDamage(enemy, SpellSlot.E);
            }
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
                var target = TargetSelector.GetTarget(Spells.R.Range);
                var pred = Spells.R.GetPrediction(target);
                if (target != null && pred.HitChance >= HitChance.High && target.HealthPercent() < 25)
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
                var target = TargetSelector.GetTarget(Spells.E.Range);
                if (target.IsValidTarget())
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
                var target = TargetSelector.GetTarget(Spells.E.Range);
                if (target.IsValidTarget())
                    Spells.E.Cast(target);
            }
            if (Setup.Harass["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                if (target.IsValidTarget())
                    Spells.W.Cast();
            }
        }

        private static void Clear()
        {
            if (Player.ManaPercent() < Setup.Clear["M"].As<MenuSlider>().Value || !Setup.Clear["K"].Enabled)
                return;

            if (Setup.Clear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var circle = FarmPos.GetCircularClearLocation(Spells.Q.Range, Spells.Q.Width, Setup.Clear["Qx"].As<MenuSlider>().Value);
                if (circle != null)
                    Spells.Q.Cast(circle.CastPosition);
            }
            if (Setup.Combo["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 1)
            {
                var Wminions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(Spells.W.Range)).ToList();
                foreach (var minion in Wminions)
                    if (Wminions.Count >= Setup.Clear["Wx"].As<MenuSlider>().Value)
                        Spells.W.Cast();
            }
        }

        private static void Killsteal()
        {
            if (Spells.R.Ready && Player.GetSpell(SpellSlot.R).ToggleState == 2)
                Spells.R.Cast();

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
                var target = TargetSelector.GetOrderedTargets(Spells.E.Range).FirstOrDefault(x => Player.GetSpellDamage(x, SpellSlot.E) >= x.Health);
                if (target.IsValidTarget())
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

        private static void Flee()
        {
            Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);

            if (Player.ManaPercent() < Setup.Flee["M"].As<MenuSlider>().Value)
                return;

            if (Setup.Flee["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                var pred = Spells.Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.Q.Cast(target);
            }
            if (Setup.Flee["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                Spells.E.Cast(Game.CursorPos);
            }
        }

        private static void Render_OnPresent()
        {
            if (Setup.Draw["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
                Render.Circle(Player.Position, Spells.Q.Range, 30, Color.LimeGreen);
            if (Setup.Draw["W"].As<MenuBool>().Enabled && Spells.W.Ready)
                Render.Circle(Player.Position, Spells.W.Range, 30, Color.Cyan);
            if (Setup.Draw["E"].As<MenuBool>().Enabled && Spells.E.Ready)
                Render.Circle(Player.Position, Spells.E.Range, 30, Color.Pink);
            if (Setup.Draw["R"].As<MenuBool>().Enabled && Spells.R.Ready)
                Render.Circle(Player.Position, Spells.R.Range, 30, Color.DarkBlue);

            if (Setup.Draw["S"].As<MenuBool>().Enabled)
            {
                Vector2 TextPos;
                var playerpos = Render.WorldToScreen(Player.Position, out TextPos);
                var xOffset = (int)TextPos.X;
                var yOffset = (int)TextPos.Y;

                if (Setup.Clear["K"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.LimeGreen, "Clear with Spells: On");
                }
                if (!Setup.Clear["K"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.Red, "Clear with Spells: Off");
                }

                if (Setup.Harass["K"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.LimeGreen, "Harass with Spells: On");
                }
                if (!Setup.Harass["K"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.Red, "Harass with Spells: Off");
                }
            }

            if (Setup.Draw["D"].As<MenuBool>().Enabled)
            {
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(h => h is Obj_AI_Hero && h.IsValidTarget() && h.IsValidTarget(1800))
                    .ToList()
                    .ForEach(
                        unit =>
                        {

                            var heroUnit = unit as Obj_AI_Hero;
                            int width = 103;
                            int height = 8;
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