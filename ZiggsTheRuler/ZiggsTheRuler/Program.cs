using System.Collections.Generic;
using System.Drawing;
using Aimtec;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Events;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.Prediction.Skillshots;
using Aimtec.SDK.TargetSelector;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Damage;
using System.Linq;

namespace ZiggsTheRuler
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
            if (Player.ChampionName != "Ziggs")
                return;

            Setup.Initialize();
            Spells.Initialize();

            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            if (Player.GetSpell(SpellSlot.W).ToggleState == 2)
            {
                Spells.W.Cast();
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
                    Turret();
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
            if (Player.ManaPercent() < Setup.Combo["Mana"].As<MenuSlider>().Value)
                return;

            if (Setup.Combo["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(target => target.IsValidTarget(Spells.R.Range)))
                {
                    Spells.R.CastIfWillHit(target, Setup.Combo["Rx"].As<MenuSlider>().Value);
                }
            }

            if (Setup.Combo["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                var pred = Spells.Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    Spells.Q.Cast(pred.CastPosition);
                }
            }

            if (Setup.Combo["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range);
                var pred = Spells.E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    Spells.E.Cast(pred.CastPosition);
                }
            }

            if (Setup.Combo["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 0)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                var dist = Player.ServerPosition.Distance(target.ServerPosition);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    var mode = Setup.Combo["Wmode"].As<MenuList>().Value;
                    if (mode == 0)
                    {
                        Spells.W.Cast(Player.ServerPosition.Extend(pred.CastPosition, dist + 150));
                    }
                    if (mode == 1)
                    {
                        Spells.W.Cast(Player.ServerPosition.Extend(pred.CastPosition, dist - 150));
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Player.ManaPercent() < Setup.Harass["Mana"].As<MenuSlider>().Value || !Setup.Harass["Key"].Enabled)
                return;

            if (Setup.Harass["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                var pred = Spells.Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    Spells.Q.Cast(pred.CastPosition);
                }
            }

            if (Setup.Harass["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range);
                var pred = Spells.E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    Spells.E.Cast(pred.CastPosition);
                }
            }

            if (Setup.Harass["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 0)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                var dist = Player.ServerPosition.Distance(target.ServerPosition);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    var mode = Setup.Harass["Wmode"].As<MenuList>().Value;
                    if (mode == 0)
                    {
                        Spells.W.Cast(Player.ServerPosition.Extend(pred.CastPosition, dist + 150));
                    }
                    if (mode == 1)
                    {
                        Spells.W.Cast(Player.ServerPosition.Extend(pred.CastPosition, dist - 150));
                    }
                }
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
            if (Player.ManaPercent() < Setup.Clear["Mana"].As<MenuSlider>().Value || !Setup.Clear["Key"].Enabled)
                return;

            if (Setup.Clear["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var circle = FarmPos.GetCircularClearLocation(Spells.E.Range, Spells.E.Width, Setup.Clear["Ex"].As<MenuSlider>().Value);
                if (circle != null)
                    Spells.E.Cast(circle.CastPosition);
            }

            if (Setup.Clear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var circle = FarmPos.GetCircularClearLocation(Spells.Q.Range, Spells.Q.Width, Setup.Clear["Qx"].As<MenuSlider>().Value);
                if (circle != null)
                    Spells.Q.Cast(circle.CastPosition);
            }
        }

        private static void Turret()
        {
            if (Player.ManaPercent() < Setup.Clear["Mana"].As<MenuSlider>().Value || !Setup.Clear["Key"].Enabled)
                return;

            var turret = GameObjects.EnemyTurrets.FirstOrDefault(t => t.IsValidTarget(Spells.W.Range) && t.HealthPercent() < 22.5 + 2.5 * Player.GetSpell(SpellSlot.W).Level);
            if (turret != null && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 0)
            {
                Spells.W.Cast(turret);
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
            if (Player.ManaPercent() < Setup.Clear["Mana"].As<MenuSlider>().Value || !Setup.Clear["Key"].Enabled)
                return;

            if (Setup.Clear["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var Eminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.E.Range)).ToList();
                foreach (var minion in Eminions)
                    if (Eminions.Count >= 1)
                        Spells.E.Cast(minion);
            }

            if (Setup.Clear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var Qminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.Q.Range)).ToList();
                foreach (var minion in Qminions)
                    if (Qminions.Count >= 1)
                        Spells.Q.Cast(minion);
            }
        }

        private static void Flee()
        {
            if (!Setup.Flee["Key"].Enabled)
            {
                return;
            }

            Player.IssueOrder(OrderType.MoveTo, Game.CursorPos);

            if (Player.ManaPercent() < Setup.Flee["Mana"].As<MenuSlider>().Value)
                return;

            if (Setup.Flee["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range);
                var pred = Spells.E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                    Spells.E.Cast(pred.CastPosition);
            }

            if (Setup.Flee["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 0)
            {
                var mode = Setup.Flee["Wmode"].As<MenuList>().Value;
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                var dist = Player.ServerPosition.Distance(target.ServerPosition);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    if (mode == 0)
                    {
                        Spells.W.Cast(Player.ServerPosition.Extend(pred.CastPosition, dist - 250));
                    }
                    if (mode == 2 && target.Distance(Player.ServerPosition) < 600)
                    {
                        Spells.W.Cast(Player.ServerPosition.Extend(pred.CastPosition, dist - dist/2));
                    }
                }
                if (mode == 1)
                {
                    Spells.W.Cast(Player.ServerPosition.Extend(Player.ServerPosition, Game.CursorPos.Distance(Player.ServerPosition) - 250));
                }
            }
        }

        private static void Killsteal()
        {
            if (Player.IsRecalling() && Setup.Killsteal["Recall"].As<MenuBool>().Enabled)
                return;

            if (Player.ManaPercent() < Setup.Killsteal["Mana"].As<MenuSlider>().Value)
                return;

            if (Setup.Killsteal["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.R.Range);
                var pred = Spells.R.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Rdmg(target) > target.Health)
                {
                    Spells.R.Cast(pred.CastPosition);
                }
            }

            if (Setup.Killsteal["W"].As<MenuBool>().Enabled && Spells.W.Ready && Player.GetSpell(SpellSlot.W).ToggleState == 0)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Wdmg(target) > target.Health)
                {
                    Spells.W.Cast(pred.CastPosition);
                }
            }

            if (Setup.Killsteal["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range);
                var pred = Spells.E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Edmg(target) > target.Health)
                {
                    Spells.E.Cast(pred.CastPosition);
                }
            }

            if (Setup.Killsteal["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                var pred = Spells.Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Qdmg(target) > target.Health)
                {
                    Spells.Q.Cast(pred.CastPosition);
                }
            }
        }

        private static void Render_OnPresent()
        {
            if (Setup.Draw["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
                Render.Circle(Player.Position, Spells.Q.Range, 30, Color.LimeGreen);
            if (Setup.Draw["W"].As<MenuBool>().Enabled && Spells.W.Ready)
                Render.Circle(Player.Position, Spells.W.Range, 30, Color.Cyan);
            if (Setup.Draw["E"].As<MenuBool>().Enabled && Spells.E.Ready)
                Render.Circle(Player.Position, Spells.E.Range, 30, Color.Red);
            if (Setup.Draw["R"].As<MenuBool>().Enabled && Spells.R.Ready)
                Render.Circle(Player.Position, Spells.R.Range, 30, Color.DarkBlue);

            if (Setup.Draw["Key"].As<MenuBool>().Enabled)
            {
                Vector2 TextPos;
                var playerpos = Render.WorldToScreen(Player.Position, out TextPos);
                var xOffset = (int)TextPos.X;
                var yOffset = (int)TextPos.Y;

                if (Setup.Clear["Key"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.LimeGreen, "Clear Key Toggle = On");
                }
                if (!Setup.Clear["Key"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.Red, "Clear Key Toggle = Off");
                }

                if (Setup.Harass["Key"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.LimeGreen, "Harass Key Toggle = On");
                }
                if (!Setup.Harass["Key"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.Red, "Harass Key Toggle = Off");
                }
            }

            if (Setup.Draw["Damage"].As<MenuBool>().Enabled)
            {
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(h => h is Obj_AI_Hero && h.IsValidTarget() && h.IsVisible && h.IsOnScreen)
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
