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

namespace ShadowVayne
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
            if (Player.ChampionName != "Vayne")
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

        private static void ImplementationOnPostAttack(object enemy, PostAttackEventArgs events)
        {
            if (Orbwalker.Implementation.Mode.Equals(OrbwalkingMode.Combo) && Setup.Combo["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                Obj_AI_Hero target = events.Target as Obj_AI_Hero;
                if (target != null)
                {
                    var mode = Setup.Misc["Qm"].As<MenuList>().Value;

                    if (mode == 0)
                    {
                        Spells.Q.Cast(TumbleLogic.SideQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 1)
                    {
                        Spells.Q.Cast(TumbleLogic.SafeQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 2)
                    {
                        Spells.Q.Cast(TumbleLogic.AggresiveQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 3)
                    {
                        Spells.Q.Cast(Game.CursorPos);
                    }
                }
            }
            if (Orbwalker.Implementation.Mode.Equals(OrbwalkingMode.Mixed) && Setup.Harass["Q"].As<MenuBool>().Enabled && Spells.Q.Ready && Player.ManaPercent() < Setup.Harass["M"].As<MenuSlider>().Value)
            {
                Obj_AI_Hero target = events.Target as Obj_AI_Hero;
                if (target != null)
                {
                    var mode = Setup.Misc["Qm"].As<MenuList>().Value;

                    if (mode == 0)
                    {
                        Spells.Q.Cast(TumbleLogic.SideQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 1)
                    {
                        Spells.Q.Cast(TumbleLogic.SafeQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 2)
                    {
                        Spells.Q.Cast(TumbleLogic.AggresiveQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 3)
                    {
                        Spells.Q.Cast(Game.CursorPos);
                    }
                }
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
                return Player.CalculateDamage(enemy, DamageType.Physical, (float)new double[] { 0.5, 0.55, 0.6, 0.65, 0.7 }[Player.SpellBook.GetSpell(SpellSlot.Q).Level] + Player.TotalAttackDamage);
            }
            return 0;
        }

        private static double Wdmg(Obj_AI_Base enemy)
        {
            return 0;
        }

        private static double Edmg(Obj_AI_Base enemy)
        {
            if (Spells.E.Ready)
            {
                return Player.CalculateDamage(enemy, DamageType.Physical, (float)new double[] { 50, 90, 120, 155, 190 }[Player.SpellBook.GetSpell(SpellSlot.E).Level] + Player.TotalAttackDamage * 0.5);
            }
            return 0;
        }

        private static double Estundmg(Obj_AI_Base enemy)
        {
            if (Spells.E.Ready)
            {
                return Player.CalculateDamage(enemy, DamageType.Physical, (float)new double[] { 100, 170, 240, 310, 380 }[Player.SpellBook.GetSpell(SpellSlot.E).Level] + Player.TotalAttackDamage);
            }
            return 0;
        }

        private static double Rdmg(Obj_AI_Base enemy)
        {
            return 0;
        }

        private static double RAD(Obj_AI_Base enemy)
        {
            if (Spells.R.Ready)
            {
                return (float)new double[] { 30, 50, 70 }[Player.SpellBook.GetSpell(SpellSlot.R).Level];
            }
            return 0;
        }
        
        private static void Codemn(Obj_AI_Base enemy)
        {
            for (int i = 1; i < 475; i += (int)enemy.BoundingRadius)
            {
                var CC = enemy.ServerPosition.Extend(Player.ServerPosition, -i);
                var Flag = NavMesh.WorldToCell(CC).Flags;
                if (Flag.HasFlag(NavCellFlags.Wall) || Flag.HasFlag(NavCellFlags.Building))
                {
                    if (enemy.IsValidTarget(550))
                    {
                        Spells.E.Cast(enemy);
                    }
                }
            }
        }
        
        private static void Combo()
        {
            if (Player.ManaPercent() < Setup.Combo["M"].As<MenuSlider>().Value)
                return;

            if (Setup.Combo["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                var mode = Setup.Combo["Rm"].As<MenuList>().Value;

                if (mode == 0 || mode == 2)
                {
                    var target = TargetSelector.GetTarget(Spells.R.Range);
                    if (target != null && Player.GetAutoAttackDamage(target) + RAD(target) * Setup.Combo["AA"].As<MenuSlider>().Value > target.Health)
                    {
                        Spells.R.Cast();
                    }
                }
                if (mode == 1 || mode == 2)
                {
                    var target = TargetSelector.GetTarget(Spells.R.Range);
                    if (target != null && Player.CountEnemyHeroesInRange(Spells.R.Range + 350) >= Setup.Combo["TF"].As<MenuSlider>().Value)
                    {
                        Spells.R.Cast();
                    }
                }
            }
            if (Setup.Combo["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range);
                if (target != null)
                    Codemn(target);
            }
            if (Setup.Combo["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                if (target != null && target.Distance(Player.ServerPosition) > 750)
                {
                    Spells.Q.Cast(target);
                }
                if (target != null && target.Distance(Player.ServerPosition) > 550 && (target != null && target.Distance(Player.ServerPosition) < 750))
                {
                    var mode = Setup.Misc["Qm"].As<MenuList>().Value;

                    if (mode == 0)
                    {
                        Spells.Q.Cast(TumbleLogic.SideQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 1)
                    {
                        Spells.Q.Cast(TumbleLogic.SafeQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 2)
                    {
                        Spells.Q.Cast(TumbleLogic.AggresiveQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 3)
                    {
                        Spells.Q.Cast(Game.CursorPos);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Player.ManaPercent() < Setup.Harass["M"].As<MenuSlider>().Value || !Setup.Harass["K"].Enabled)
                return;


            if (Setup.Harass["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range);
                if (target != null)
                    Codemn(target);
            }
            if (Setup.Harass["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                if (target != null && target.Distance(Player.ServerPosition) > 750)
                {
                    Spells.Q.Cast(target);
                }
                if (target != null && target.Distance(Player.ServerPosition) > 550 && (target != null && target.Distance(Player.ServerPosition) < 750))
                {
                    var mode = Setup.Misc["Qm"].As<MenuList>().Value;

                    if (mode == 0)
                    {
                        Spells.Q.Cast(TumbleLogic.SideQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 1)
                    {
                        Spells.Q.Cast(TumbleLogic.SafeQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 2)
                    {
                        Spells.Q.Cast(TumbleLogic.AggresiveQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    }
                    if (mode == 3)
                    {
                        Spells.Q.Cast(Game.CursorPos);
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
            if (Player.ManaPercent() < Setup.Laneclear["M"].As<MenuSlider>().Value || !Setup.Misc["LK"].Enabled)
                return;

            if (Setup.Laneclear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var Qminions = GameObjects.EnemyMinions.Where(m => m.IsValidTarget(550)).ToList();
                foreach (var minion in Qminions)
                    if (Qminions.Count >= 3)
                        Spells.Q.Cast(Game.CursorPos);
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
                    if (Qminions.Count >= 1)
                        Spells.Q.Cast(Game.CursorPos);
            }
            if (Setup.Jungleclear["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                foreach (var minion in GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.E.Range)).ToList())
                    if (minion.IsValidTarget())
                        Codemn(minion);
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
                var target = TargetSelector.GetTarget(Spells.E.Range);
                if (target.IsValidTarget())
                    Spells.E.Cast(target);
            }
            if (Setup.Flee["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                Spells.Q.Cast(Game.CursorPos);
            }
        }

        private static void Killsteal()
        {
            if (Player.ManaPercent() < Setup.Killsteal["M"].As<MenuSlider>().Value)
                return;
            
            if (Setup.Killsteal["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(550);

                if (target.IsValidTarget() && Estundmg(target) > target.Health)
                {
                    Codemn(target);
                }
                if (target.IsValidTarget() && Edmg(target) > target.Health)
                {
                    Spells.E.Cast(target);
                } 
            }
        }

        private static void Render_OnPresent()
        {
            if (Setup.Draw["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
                Render.Circle(Player.Position, Spells.Q.Range, 30, Color.LimeGreen);

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