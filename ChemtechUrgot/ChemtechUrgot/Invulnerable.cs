namespace ChemtechUrgot
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Aimtec;
    using Aimtec.SDK.Extensions;

    public class Invulnerable
    {
        #region Static Fields

        private static readonly List<InvulnerableEntry> PEntries = new List<InvulnerableEntry>();

        #endregion

        #region Constructors and Destructors

        static Invulnerable()
        {
            PEntries.AddRange(
                new List<InvulnerableEntry>
                    {
                        new InvulnerableEntry("UndyingRage") { ChampionName = "Tryndamere", MinHealthPercent = 1, CheckFunction = (target, type) => ((Obj_AI_Hero)target).HealthPercent() <= 1 },
                        new InvulnerableEntry("Kayle") { ChampionName = "JudicatorIntervention" },
                        new InvulnerableEntry("fizztrickslamsounddummy") { ChampionName = "Fizz" },
                        new InvulnerableEntry("VladimirSanguinePool") { ChampionName = "Vladimir" },
                        new InvulnerableEntry("FioraW") { ChampionName = "Fiora" },
                        new InvulnerableEntry("JaxCounterStrike") { ChampionName = "Jax", DamageType = DamageType.Physical },
                        new InvulnerableEntry("BlackShield") { IsShield = true, DamageType = DamageType.Magical },
                        new InvulnerableEntry("BansheesVeil") { IsShield = true, DamageType = DamageType.Magical },
                        new InvulnerableEntry("SivirE") { ChampionName = "Sivir", IsShield = true },
                        new InvulnerableEntry("ShroudofDarkness") { ChampionName = "Nocturne", IsShield = true },
                        new InvulnerableEntry("KindredrNoDeathBuff") { MinHealthPercent = 10, CheckFunction = (target, type) => ((Obj_AI_Hero)target).HealthPercent() <= 10 }
                    });
        }

        #endregion

        #region Public Properties

        public static ReadOnlyCollection<InvulnerableEntry> Entries
        {
            get
            {
                return PEntries.AsReadOnly();
            }
        }

        #endregion

        #region Public Methods and Operators

        public static bool Check(
            Obj_AI_Hero hero,
            DamageType damageType = DamageType.True,
            bool ignoreShields = true,
            float damage = -1f)
        {
            if (hero.Buffs.Any(b => b.Type == BuffType.Invulnerability) || hero.IsInvulnerable)
            {
                return true;
            }
            foreach (var entry in Entries)
            {
                if (entry.ChampionName == null || entry.ChampionName == hero.ChampionName)
                {
                    if (entry.DamageType == null || entry.DamageType == damageType)
                    {
                        if (hero.HasBuff(entry.BuffName))
                        {
                            if (!ignoreShields || !entry.IsShield)
                            {
                                if (entry.CheckFunction == null || ExecuteCheckFunction(entry, hero, damageType))
                                {
                                    if (damage <= 0 || entry.MinHealthPercent <= 0
                                        || (hero.Health - damage) / hero.MaxHealth * 100 < entry.MinHealthPercent)
                                    {
                                        return true;
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static void Deregister(InvulnerableEntry entry)
        {
            if (PEntries.Any(i => i.BuffName.Equals(entry.BuffName)))
            {
                PEntries.Remove(entry);
            }
        }

        public static InvulnerableEntry GetItem(
            string buffName,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return PEntries.FirstOrDefault(w => w.BuffName.Equals(buffName, stringComparison));
        }

        public static void Register(InvulnerableEntry entry)
        {
            if (!string.IsNullOrEmpty(entry.BuffName) && !PEntries.Any(i => i.BuffName.Equals(entry.BuffName)))
            {
                PEntries.Add(entry);
            }
        }

        #endregion

        #region Methods

        private static bool ExecuteCheckFunction(InvulnerableEntry entry, Obj_AI_Hero hero, DamageType damageType)
        {
            return entry != null && entry.CheckFunction(hero, damageType);
        }

        #endregion
    }

    public class InvulnerableEntry
    {
        #region Constructors and Destructors

        public InvulnerableEntry(string buffName)
        {
            this.BuffName = buffName;
        }

        #endregion

        #region Public Properties

        public string BuffName { get; set; }

        public string ChampionName { get; set; }

        public Func<Obj_AI_Base, DamageType, bool> CheckFunction { get; set; }

        public DamageType? DamageType { get; set; }

        public bool IsShield { get; set; }

        public int MinHealthPercent { get; set; }

        #endregion
    }
}