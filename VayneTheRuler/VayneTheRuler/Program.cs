using System.Collections.Generic;
using System.Drawing;
using Aimtec;
using Aimtec.SDK.Menu.Components;
using Aimtec.SDK.Events;
using Aimtec.SDK.Orbwalking;
using Aimtec.SDK.TargetSelector;
using Aimtec.SDK.Extensions;
using Aimtec.SDK.Damage;
using Aimtec.SDK.Util;
using System.Linq;

namespace VayneTheRuler
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
            Orbwalker.Implementation.PreAttack += ImplementationOnPreAttack;
            Orbwalker.Implementation.PostAttack += ImplementationOnPostAttack;
        }

        private static void Game_OnUpdate()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            Killsteal();

            AutoPeel();

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
                    Clear();
                    break;
            }
        }

        private static void ImplementationOnPreAttack(object enemy, PreAttackEventArgs events)
        {
            var turret = GameObjects.EnemyTurrets.FirstOrDefault(t => !t.IsDead);

            if (turret.ServerPosition.Distance(Player.ServerPosition) > turret.AttackRange && Player.HasBuff("vaynetumblefade"))
            {
                if (Player.CountEnemyHeroesInRange(1200) >= Setup.Combo["Stealth"].As<MenuSlider>().Value)
                {
                    events.Cancel = true;
                }

                if (Player.HasBuff("summonerexhaust"))
                {
                    events.Cancel = true;
                }
            }

            if (Setup.Misc["FocusW"].As<MenuBool>().Enabled)
            {
                var force = TargetSelector.GetOrderedTargets(850).FirstOrDefault(h => h.IsValidTarget() && h.GetRealBuffCount("vaynesilvereddebuff") == 2);
                if (force != null)
                {
                    events.Target = force;
                }
            }
        }

        private static void ImplementationOnPostAttack(object enemy, PostAttackEventArgs events)
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            var turret = GameObjects.EnemyTurrets.FirstOrDefault(t => !t.IsDead);

            if (Orbwalker.Implementation.Mode.Equals(OrbwalkingMode.Combo) && Setup.Combo["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                Obj_AI_Hero target = events.Target as Obj_AI_Hero;
                if (target != null && Setup.Combo["Mana"].As<MenuSlider>().Value < Player.ManaPercent())
                {
                    var endPos = Player.ServerPosition.Extend(Game.CursorPos, 300);
                    if (endPos.CountEnemyHeroesInRange(850) < Setup.Misc["Qx"].As<MenuSlider>().Value &&
                        endPos.Distance(target.ServerPosition) < Player.AttackRange)
                    {
                        if (target.AttackRange < Player.AttackRange && endPos.Distance(target.ServerPosition) < target.AttackRange)
                        {
                            return;
                        }
                        if (endPos.Distance(turret.ServerPosition) < turret.AttackRange && Setup.Misc["Turret"].As<MenuBool>().Enabled)
                        {
                            return;
                        }
                        Spells.Q.Cast(endPos);
                    }
                }
            }

            if (Orbwalker.Implementation.Mode.Equals(OrbwalkingMode.Mixed) && Setup.Harass["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                Obj_AI_Hero target = events.Target as Obj_AI_Hero;
                if (target != null && Setup.Harass["Mana"].As<MenuSlider>().Value < Player.ManaPercent() && Setup.Harass["Key"].Enabled)
                {
                    var endPos = Player.ServerPosition.Extend(Game.CursorPos, 300);
                    if (endPos.CountEnemyHeroesInRange(850) < Setup.Misc["Qx"].As<MenuSlider>().Value &&
                        endPos.Distance(target.ServerPosition) < Player.AttackRange)
                    {
                        if (target.AttackRange < Player.AttackRange && endPos.Distance(target.ServerPosition) < target.AttackRange)
                        {
                            return;
                        }
                        if (Setup.Harass["QW"].As<MenuBool>().Enabled && target.GetBuffCount("vaynesilvereddebuff") != 2)
                        {
                            return;
                        }
                        if (endPos.Distance(turret.ServerPosition) < turret.AttackRange && Setup.Misc["Turret"].As<MenuBool>().Enabled)
                        {
                            return;
                        }
                        Spells.Q.Cast(endPos);
                    }
                }
            }

            if (Orbwalker.Implementation.Mode.Equals(OrbwalkingMode.Laneclear) && Setup.Clear["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                Obj_AI_Minion target = events.Target as Obj_AI_Minion;
                if (target != null && Setup.Clear["Mana"].As<MenuSlider>().Value < Player.ManaPercent() && Setup.Clear["Key"].Enabled)
                {
                    var endPos = Player.ServerPosition.Extend(Game.CursorPos, 300);
                    if (endPos.CountEnemyHeroesInRange(850) < Setup.Misc["Qx"].As<MenuSlider>().Value &&
                        endPos.Distance(target.ServerPosition) < Player.AttackRange)
                    {
                        if (target.AttackRange < Player.AttackRange && endPos.Distance(target.ServerPosition) < target.AttackRange)
                        {
                            return;
                        }
                        if (endPos.Distance(turret.ServerPosition) < turret.AttackRange && Setup.Misc["Turret"].As<MenuBool>().Enabled)
                        {
                            return;
                        }
                        Spells.Q.Cast(endPos);
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
                var level = Player.SpellBook.GetSpell(SpellSlot.Q).Level;
                return Player.CalculateDamage(enemy, DamageType.Physical, (0.45 + 0.05 * level + Player.TotalAttackDamage));
            }
            return 0;
        }

        private static double Wdmg(Obj_AI_Base enemy)
        {
            if (enemy.GetBuffCount("vaynesilvereddebuff") == 2)
            {
                var level = Player.SpellBook.GetSpell(SpellSlot.W).Level;
                return enemy.MaxHealth * (0.02 + 0.02 * level);
            }
            return 0;
        }

        private static double Edmg(Obj_AI_Base enemy)
        {
            if (Spells.E.Ready)
            {
                var level = Player.SpellBook.GetSpell(SpellSlot.E).Level;
                for (int i = 1; i < 450; i += (int)enemy.BoundingRadius)
                {
                    var CC = enemy.ServerPosition.Extend(Player.ServerPosition, -i);
                    var Flag = NavMesh.WorldToCell(CC).Flags;
                    if (Flag.HasFlag(NavCellFlags.Wall) || Flag.HasFlag(NavCellFlags.Building))
                    {
                        return Player.CalculateDamage(enemy, DamageType.Physical, (30 + 70 * level + (Player.TotalAttackDamage - Player.BaseAttackDamage)));
                    }
                    return Player.CalculateDamage(enemy, DamageType.Physical, (float)new double[] { 50, 90, 120, 155, 190 }[level] + (Player.TotalAttackDamage - Player.BaseAttackDamage) * 0.5);
                }
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
                var level = Player.SpellBook.GetSpell(SpellSlot.R).Level;
                return Player.CalculateDamage(enemy, DamageType.Physical, (Player.TotalAttackDamage + (10 + 20 * level)));
            }
            return 0;
        }

        private static void Codemn(Obj_AI_Base enemy)
        {
            for (int i = 1; i < 450;)
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

        private static void FlashCodemn(Obj_AI_Base enemy)
        {
            for (int i = 1; i < 450;)
            {
                var CC = enemy.ServerPosition.Extend(Game.CursorPos, -i);
                var Flag = NavMesh.WorldToCell(CC).Flags;
                var CC2 = enemy.ServerPosition.Extend(Player.ServerPosition, -i);
                var Flag2 = NavMesh.WorldToCell(CC).Flags;
                if (Flag2.HasFlag(NavCellFlags.Wall) || Flag2.HasFlag(NavCellFlags.Building))
                {
                    if (enemy.IsValidTarget(550))
                    {
                        Spells.E.Cast(enemy);
                    }
                }
                if (Flag.HasFlag(NavCellFlags.Wall) || Flag.HasFlag(NavCellFlags.Building))
                {
                    if (enemy.IsValidTarget() && Spells.Flash.Ready && CC.Distance(Player.ServerPosition) < Spells.Flash.Range)
                    {
                        Spells.Flash.Cast(CC);
                        Spells.E.Cast(enemy);
                    }
                }
            }
        }

        private static void Combo()
        {
            if (Player.ManaPercent() < Setup.Combo["Mana"].As<MenuSlider>().Value)
                return;

            if (Setup.Combo["R"].As<MenuBool>().Enabled && Spells.R.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                if (target != null && Player.CountEnemyHeroesInRange(1400) >= Setup.Combo["Rx"].As<MenuSlider>().Value)
                {
                    Spells.R.Cast();
                }
                if (target != null && Setup.Combo["1v1"].As<MenuBool>().Enabled && Wdmg(target) * Player.AttackSpeedMod / 3 + RAD(target) * Player.AttackSpeedMod * 4 > target.Health && Wdmg(target) * Player.AttackSpeedMod / 3 + RAD(target) * Player.AttackSpeedMod * 2 < target.Health)
                {
                    Spells.R.Cast();
                }
            }

            if (Setup.Combo["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range);
                if (target != null)
                {
                    Codemn(target);
                }
            }

            if (Setup.Combo["Flash"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range + Spells.Flash.Range);
                if (target != null && Wdmg(target) * Player.AttackSpeedMod / 3 + RAD(target) * Player.AttackSpeedMod * 3.5 > target.Health && Wdmg(target) * Player.AttackSpeedMod / 3 + RAD(target) * Player.AttackSpeedMod * 1.75 < target.Health)
                {
                    FlashCodemn(target);
                }
            }

            var turret = GameObjects.EnemyTurrets.FirstOrDefault(t => !t.IsDead);

            if (Setup.Combo["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                if (target != null && Setup.Misc["Engage"].As<MenuBool>().Enabled && Player.ServerPosition.Distance(target.ServerPosition) > Player.AttackRange)
                {
                    var endPos = Player.ServerPosition.Extend(Game.CursorPos, 300);
                    if (endPos.CountEnemyHeroesInRange(850) < Setup.Misc["Qx"].As<MenuSlider>().Value &&
                        endPos.Distance(target.ServerPosition) < Player.AttackRange)
                    {
                        if (target.AttackRange < Player.AttackRange && endPos.Distance(target.ServerPosition) < target.AttackRange)
                        {
                            return;
                        }
                        if (endPos.Distance(turret.ServerPosition) < turret.AttackRange && Setup.Misc["Turret"].As<MenuBool>().Enabled)
                        {
                            return;
                        }
                        Spells.Q.Cast(endPos);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Player.ManaPercent() < Setup.Harass["Mana"].As<MenuSlider>().Value || !Setup.Harass["Key"].Enabled)
                return;

            if (Setup.Harass["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.E.Range);
                if (target != null)
                {
                    Codemn(target);
                }
                if (Setup.Harass["EW"].As<MenuBool>().Enabled && target.GetBuffCount("vaynesilvereddebuff") == 2)
                {
                    Spells.E.Cast(target);
                }
            }

            var turret = GameObjects.EnemyTurrets.FirstOrDefault(t => !t.IsDead);

            if (Setup.Harass["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                var target = TargetSelector.GetTarget(Spells.Q.Range);
                if (target != null && Setup.Misc["Engage"].As<MenuBool>().Enabled && Player.ServerPosition.Distance(target.ServerPosition) > Player.AttackRange)
                {
                    var endPos = Player.ServerPosition.Extend(Game.CursorPos, 300);
                    if (endPos.CountEnemyHeroesInRange(850) < Setup.Misc["Qx"].As<MenuSlider>().Value &&
                        endPos.Distance(target.ServerPosition) < Player.AttackRange)
                    {
                        if (target.AttackRange < Player.AttackRange && endPos.Distance(target.ServerPosition) < target.AttackRange)
                        {
                            return;
                        }
                        if (Setup.Harass["QW"].As<MenuBool>().Enabled && target.GetBuffCount("vaynesilvereddebuff") != 2)
                        {
                            return;
                        }
                        if (endPos.Distance(turret.ServerPosition) < turret.AttackRange && Setup.Misc["Turret"].As<MenuBool>().Enabled)
                        {
                            return;
                        }
                        Spells.Q.Cast(endPos);
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

        private static void Clear()
        {
            if (Player.ManaPercent() < Setup.Clear["Mana"].As<MenuSlider>().Value || !Setup.Clear["Key"].Enabled)
                return;

            if (Setup.Clear["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                foreach (var minion in GameObjects.Jungle.Where(m => m.IsValidTarget(Spells.E.Range)).ToList())
                    if (minion.IsValidTarget() && !Setup.Clear["Ekill"].As<MenuBool>().Enabled)
                    {
                        Codemn(minion);
                    }
                    else if (minion.IsValidTarget() && Edmg(minion) > minion.Health && Setup.Clear["Ekill"].As<MenuBool>().Enabled)
                    {
                        Spells.E.Cast(minion);
                    }
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
                if (target.IsValidTarget())
                    Spells.E.Cast(target);
            }
            if (Setup.Flee["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
            {
                Spells.Q.Cast(Game.CursorPos);
                if (Setup.Misc["Anim"].As<MenuBool>().Enabled)
                {
                    DelayAction.Queue(200, () => MenuGUI.DoEmote(EmoteType.Dance));
                }
            }
        }

        private static void AutoPeel()
        {
            if (Setup.Misc["Peel"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(450);
                if (target != null && target.AttackRange < 400 && Setup.Misc["HP"].As<MenuSlider>().Value > Player.HealthPercent())
                {
                    Spells.E.Cast(target);
                }
            }
        }

        private static void Killsteal()
        {
            if (Player.ManaPercent() < Setup.Killsteal["Mana"].As<MenuSlider>().Value)
                return;

            if (Setup.Killsteal["E"].As<MenuBool>().Enabled && Spells.E.Ready)
            {
                var target = TargetSelector.GetTarget(550);
                if (target.IsValidTarget() && Edmg(target) > target.Health)
                {
                    Spells.E.Cast(target);
                }
            }
        }

        private static void Render_OnPresent()
        {
            if (Player.IsDead || MenuGUI.IsChatOpen())
            {
                return;
            }

            if (Setup.Draw["Q"].As<MenuBool>().Enabled && Spells.Q.Ready)
                Render.Circle(Player.Position, Spells.Q.Range, 30, Color.LimeGreen);

            if (Setup.Orb["EAA"].As<MenuBool>().Enabled)
                foreach (var enemy in GameObjects.EnemyHeroes)
                    if (enemy.IsOnScreen && !enemy.IsDead)
                        Render.Circle(enemy.ServerPosition, enemy.AttackRange, 30, Color.Red);

            if (Setup.Orb["TAA"].As<MenuBool>().Enabled)
                foreach (var turret in GameObjects.EnemyTurrets)
                    if (turret.IsOnScreen && !turret.IsDead)
                        Render.Circle(turret.ServerPosition, 875, 30, Color.Red);
            foreach (var allyt in GameObjects.AllyTurrets)
                if (allyt.IsOnScreen && !allyt.IsDead)
                    Render.Circle(allyt.ServerPosition, 875, 30, Color.LimeGreen);

            if (Setup.Draw["S"].As<MenuBool>().Enabled)
            {
                Vector2 TextPos;
                var playerpos = Render.WorldToScreen(Player.Position, out TextPos);
                var xOffset = (int)TextPos.X;
                var yOffset = (int)TextPos.Y;

                if (Setup.Clear["Key"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.LimeGreen, "Clear with Spells: On");
                }
                if (!Setup.Clear["Key"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 20, Color.Red, "Clear with Spells: Off");
                }

                if (Setup.Harass["Key"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.LimeGreen, "Harass with Spells: On");
                }
                if (!Setup.Harass["Key"].Enabled)
                {
                    Render.Text(xOffset - 50, yOffset + 40, Color.Red, "Harass with Spells: Off");
                }
            }

            if (Setup.Draw["D"].As<MenuBool>().Enabled)
            {
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(h => h is Obj_AI_Hero && h.IsValidTarget() && h.IsOnScreen)
                    .ToList()
                    .ForEach(
                        unit =>
                        {

                            var heroUnit = unit as Obj_AI_Hero;
                            int width = 103;
                            int xOffset = SxOffset(heroUnit);
                            int yOffset = SyOffset(heroUnit);
                            var barPos = unit.FloatingHealthBarPosition;
                            barPos.X += xOffset + 22;
                            barPos.Y += yOffset - 16;
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