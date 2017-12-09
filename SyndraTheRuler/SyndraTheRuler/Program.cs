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
using Aimtec.SDK.Util;
using System.Linq;

namespace SyndraTheRuler
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
            if (Player.ChampionName != "Syndra")
                return;

            Setup.Initialize();
            Spells.Initialize();

            Obj_AI_Base.OnProcessSpellCast += ProcessCast;
            Render.OnPresent += Render_OnPresent;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static int lastw;

        private static int delay;

        private static void ProcessCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs events)
        {
            if (events.Sender.IsMe)
            {
                if (events.SpellData.Name == "SyndraW")
                {
                    Spells.W.LastCastAttemptT = Game.TickCount + 100;
                    lastw = Game.TickCount + Game.Ping + 20;
                    delay = 1000 + Game.TickCount;
                }
            }
        }

        private static void Game_OnUpdate()
        {
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
                var DamagePerSphere = Player.CalculateDamage(enemy, DamageType.Magical, 45 + 45 * Player.SpellBook.GetSpell(SpellSlot.R).Level);
                var Spheres = GameObjects.AllGameObjects.Where(x => x.Name == "Seed" && x.IsValid && !x.IsDead && x.Distance(Player) <= Spells.R.Range).ToList();
                foreach (var number in Spheres);
                return DamagePerSphere * 3 + DamagePerSphere * Spheres.Count;
            }
            return 0;
        }

        private static GameObject Objects()
        {
            var orb = GameObjects.AllGameObjects.FirstOrDefault(
                x => x.Name == "Seed" && x.IsValid && !x.IsDead && x.Distance(Player) <= Spells.W.Range);
            var minion =
                GameObjects.EnemyMinions.FirstOrDefault(
                    m => m.Distance(Player) < Spells.W.Range && m.IsValidTarget());
            var jungle =
                GameObjects.Jungle.FirstOrDefault(
                    m => m.Distance(Player) < Spells.W.Range && m.IsValidTarget());
            if (orb != null)
            {
                return orb;
            }
            if (minion != null)
            {
                return minion;
            }
            if (jungle != null)
            {
                return jungle;
            }
            return null;
        }


        private static void Combo()
        {
            if (Player.ManaPercent() < Setup.Combo["Mana"].As<MenuSlider>().Value)
                return;

            if (Setup.Combo["E"].As<MenuBool>().Enabled && Spells.E.Ready && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.EQ.Range);
                var pred = Spells.EQ.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    if (Player.ServerPosition.Distance(target.ServerPosition) > Spells.Q.Range)
                    {
                        var EDelay = Spells.Q.Delay - (Spells.E.Delay + ((Player.Distance(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range))) / Spells.E.Speed));
                        Spells.Q.Cast(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range));
                        DelayAction.Queue((int)EDelay * 1000, () => Spells.E.Cast(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range)));
                    }
                    else
                    {
                        var EDelay = Spells.Q.Delay - (Spells.E.Delay + ((Player.Distance(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range))) / Spells.E.Speed));
                        Spells.Q.Cast(pred.CastPosition.Extend(Player.ServerPosition, -150));
                        DelayAction.Queue((int)EDelay * 2000, () => Spells.E.Cast(pred.CastPosition.Extend(Player.ServerPosition, -150)));
                    }
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

            if (Setup.Combo["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                if (target.IsValidTarget() && !Player.HasBuff("syndrawtooltip") && delay < Game.TickCount)
                {
                    if (Objects() != null && Objects().Distance(Player.ServerPosition) < Spells.W.Range - 25)
                    {
                        Spells.W.Cast(Objects().ServerPosition);
                        delay = Game.TickCount + 1000;
                        Spells.W.LastCastAttemptT = Game.TickCount;
                    }
                }
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Player.HasBuff("syndrawtooltip") && Game.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 100)
                {
                    Spells.W.Cast(pred.CastPosition);
                }
            }
        }

        private static void Harass()
        {
            if (Player.ManaPercent() < Setup.Harass["Mana"].As<MenuSlider>().Value || !Setup.Harass["Key"].Enabled)
                return;

            if (Setup.Harass["E"].As<MenuBool>().Enabled && Spells.E.Ready && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.EQ.Range);
                var pred = Spells.EQ.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    if (Player.ServerPosition.Distance(target.ServerPosition) > Spells.Q.Range)
                    {
                        var EDelay = Spells.Q.Delay - (Spells.E.Delay + ((Player.Distance(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range))) / Spells.E.Speed));
                        Spells.Q.Cast(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range));
                        DelayAction.Queue((int)EDelay * 1000, () => Spells.E.Cast(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range)));
                    }
                    else
                    {
                        var EDelay = Spells.Q.Delay - (Spells.E.Delay + ((Player.Distance(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range))) / Spells.E.Speed));
                        Spells.Q.Cast(pred.CastPosition.Extend(Player.ServerPosition, -150));
                        DelayAction.Queue((int)EDelay * 2000, () => Spells.E.Cast(pred.CastPosition.Extend(Player.ServerPosition, -150)));
                    }
                }
            }

            if (Setup.Harass["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                var pred = Spells.Q.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    Spells.Q.Cast(pred.CastPosition);
                }
            }
            
            if (Setup.Harass["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                if (target.IsValidTarget() && !Player.HasBuff("syndrawtooltip") && delay < Game.TickCount)
                {
                    if (Objects() != null && Objects().Distance(Player.ServerPosition) < Spells.W.Range - 25)
                    {
                        Spells.W.Cast(Objects().ServerPosition);
                        delay = Game.TickCount + 1000;
                        Spells.W.LastCastAttemptT = Game.TickCount;
                    }
                }
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Player.HasBuff("syndrawtooltip") && Game.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 100)
                {
                    Spells.W.Cast(pred.CastPosition);
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

            if (Setup.Clear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var circle = FarmPos.GetCircularClearLocation(Spells.Q.Range, Spells.Q.Width, Setup.Clear["Qx"].As<MenuSlider>().Value);
                if (circle != null)
                    Spells.Q.Cast(circle.CastPosition);
            }

            if (Setup.Clear["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                if (Player.HasBuff("syndrawtooltip") && Game.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 100)
                {
                    var circle = FarmPos.GetCircularClearLocation(Spells.W.Range, Spells.W.Width, Setup.Clear["Wx"].As<MenuSlider>().Value);
                    if (circle != null)
                        Spells.W.Cast(circle.CastPosition);
                }
                if (!Player.HasBuff("syndrawtooltip") && delay < Game.TickCount)
                {
                    if (Objects() != null && Objects().Distance(Player.ServerPosition) < Spells.W.Range - 25)
                    {
                        Spells.W.Cast(Objects().ServerPosition);
                        delay = Game.TickCount + 1000;
                        Spells.W.LastCastAttemptT = Game.TickCount;
                    }
                }
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

            if (Setup.Clear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var Qminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.Q.Range)).ToList();
                foreach (var minion in Qminions)
                    if (Qminions.Count >= 1 && minion.HasBuff("dummypercenthealthresistance"))
                    {
                        if (Qminions.Count == 1 && minion.HasBuff("syndrawbuff"))
                        {
                            return;
                        }
                        else
                        {
                            Spells.Q.Cast(minion);
                        }
                    }
            }

            if (Setup.Clear["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                if (Player.HasBuff("syndrawtooltip") && Game.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 100)
                {
                    var W2minions = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.W.Range)).ToList();
                    foreach (var minion in W2minions)
                        if (W2minions.Count == 1 && minion.HasBuff("syndrawbuff"))
                            Spells.W.Cast(Game.CursorPos);
                }
                var Wminions = GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.W.Range -25)).ToList();
                foreach (var minion in Wminions)
                    if (Wminions.Count >= 1 && minion.HasBuff("dummypercenthealthresistance") && Game.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 100)
                        Spells.W.Cast(minion);
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

            if (Setup.Flee["E"].As<MenuBool>().Enabled && Spells.E.Ready && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.EQ.Range);
                var pred = Spells.EQ.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    if (Player.ServerPosition.Distance(target.ServerPosition) > Spells.Q.Range)
                    {
                        var EDelay = Spells.Q.Delay - (Spells.E.Delay + ((Player.Distance(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range))) / Spells.E.Speed));
                        Spells.Q.Cast(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range));
                        DelayAction.Queue((int)EDelay * 1000, () => Spells.E.Cast(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range)));
                    }
                    else
                    {
                        var EDelay = Spells.Q.Delay - (Spells.E.Delay + ((Player.Distance(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range))) / Spells.E.Speed));
                        Spells.Q.Cast(pred.CastPosition.Extend(Player.ServerPosition, -150));
                        DelayAction.Queue((int)EDelay * 2000, () => Spells.E.Cast(pred.CastPosition.Extend(Player.ServerPosition, -150)));
                    }
                }
            }

            if (Setup.Flee["E"].As<MenuBool>().Enabled && Spells.E.Ready && !Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range);
                var pred = Spells.E.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High)
                {
                    Spells.E.Cast(pred.CastPosition);
                }
            }

            if (Setup.Flee["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                if (target.IsValidTarget() && !Player.HasBuff("syndrawtooltip") && delay < Game.TickCount)
                {
                    if (Objects() != null && Objects().Distance(Player.ServerPosition) < Spells.W.Range - 25)
                    {
                        Spells.W.Cast(Objects().ServerPosition);
                        delay = Game.TickCount + 1000;
                        Spells.W.LastCastAttemptT = Game.TickCount;
                    }
                }
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Player.HasBuff("syndrawtooltip") && Game.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 100)
                {
                    Spells.W.Cast(pred.CastPosition);
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
                if (target.IsValidTarget() && Rdmg(target) > target.Health)
                {
                    Spells.R.Cast(target);
                }
            }

            if (Setup.Killsteal["W"].As<MenuBool>().Enabled && Spells.W.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.W.Range);
                var pred = Spells.W.GetPrediction(target);
                if (target.IsValidTarget() && !Player.HasBuff("syndrawtooltip") && delay < Game.TickCount && Wdmg(target) > target.Health)
                {
                    if (Objects() != null && Objects().Distance(Player.ServerPosition) < Spells.W.Range - 25)
                    {
                        Spells.W.Cast(Objects().ServerPosition);
                        delay = Game.TickCount + 1000;
                        Spells.W.LastCastAttemptT = Game.TickCount;
                    }
                }
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Player.HasBuff("syndrawtooltip") && Game.TickCount - Spells.W.LastCastAttemptT > Game.Ping + 100 && Wdmg(target) > target.Health)
                {
                    Spells.W.Cast(pred.CastPosition);
                }
            }

            if (Setup.Killsteal["E"].As<MenuBool>().Enabled && Spells.E.Ready && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.EQ.Range);
                var pred = Spells.EQ.GetPrediction(target);
                if (target.IsValidTarget() && pred.HitChance >= HitChance.High && Edmg(target) > target.Health)
                {
                    if (Player.ServerPosition.Distance(target.ServerPosition) > Spells.Q.Range)
                    {
                        var EDelay = Spells.Q.Delay - (Spells.E.Delay + ((Player.Distance(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range))) / Spells.E.Speed));
                        Spells.Q.Cast(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range));
                        DelayAction.Queue((int)EDelay * 1000, () => Spells.E.Cast(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range)));
                    }
                    else
                    {
                        var EDelay = Spells.Q.Delay - (Spells.E.Delay + ((Player.Distance(Player.ServerPosition.Extend(pred.CastPosition, Spells.Q.Range))) / Spells.E.Speed));
                        Spells.Q.Cast(pred.CastPosition.Extend(Player.ServerPosition, -150));
                        DelayAction.Queue((int)EDelay * 2000, () => Spells.E.Cast(pred.CastPosition.Extend(Player.ServerPosition, -150)));
                    }
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
                Render.Circle(Player.Position, Spells.EQ.Range, 30, Color.Pink);
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