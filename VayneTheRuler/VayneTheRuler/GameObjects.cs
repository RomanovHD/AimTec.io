namespace VayneTheRuler
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Aimtec;

    public static class GameObjects
    {
        #region Static Fields

        private static readonly List<Obj_AI_Hero> AllyHeroesList = new List<Obj_AI_Hero>();

        private static readonly List<Obj_AI_Base> AllyList = new List<Obj_AI_Base>();

        private static readonly List<Obj_AI_Minion> AllyMinionsList = new List<Obj_AI_Minion>();

        private static readonly List<Obj_AI_Turret> AllyTurretsList = new List<Obj_AI_Turret>();

        private static readonly List<Obj_AI_Minion> AllyWardsList = new List<Obj_AI_Minion>();

        private static readonly List<AttackableUnit> AttackableUnitsList = new List<AttackableUnit>();

        private static readonly List<Obj_AI_Hero> EnemyHeroesList = new List<Obj_AI_Hero>();

        private static readonly List<Obj_AI_Base> EnemyList = new List<Obj_AI_Base>();

        private static readonly List<Obj_AI_Minion> EnemyMinionsList = new List<Obj_AI_Minion>();

        private static readonly List<Obj_AI_Turret> EnemyTurretsList = new List<Obj_AI_Turret>();

        private static readonly List<Obj_AI_Minion> EnemyWardsList = new List<Obj_AI_Minion>();

        private static readonly List<GameObject> GameObjectsList = new List<GameObject>();

        private static readonly List<Obj_AI_Hero> HeroesList = new List<Obj_AI_Hero>();

        private static readonly List<Obj_AI_Minion> JungleLargeList = new List<Obj_AI_Minion>();

        private static readonly List<Obj_AI_Minion> JungleLegendaryList = new List<Obj_AI_Minion>();

        private static readonly List<Obj_AI_Minion> JungleList = new List<Obj_AI_Minion>();

        private static readonly List<Obj_AI_Minion> JungleSmallList = new List<Obj_AI_Minion>();

        private static readonly string[] LargeNameRegex =
            {
                "SRU_Murkwolf[0-9.]{1,}", "SRU_Gromp", "SRU_Blue[0-9.]{1,}",
                "SRU_Razorbeak[0-9.]{1,}", "SRU_Red[0-9.]{1,}",
                "SRU_Krug[0-9]{1,}"
            };

        private static readonly string[] LegendaryNameRegex = { "SRU_Dragon", "SRU_Baron", "SRU_RiftHerald" };

        private static readonly List<Obj_AI_Minion> MinionsList = new List<Obj_AI_Minion>();

        private static readonly string[] SmallNameRegex = { "SRU_[a-zA-Z](.*?)Mini", "Sru_Crab" };

        private static readonly List<Obj_AI_Turret> TurretsList = new List<Obj_AI_Turret>();

        private static readonly List<Obj_AI_Minion> WardsList = new List<Obj_AI_Minion>();

        private static bool initialized;

        #endregion

        #region Constructors and Destructors

        static GameObjects()
        {
            Initialize();
        }

        #endregion

        #region Enums

        public enum JungleType
        {
            Unknown,

            Small,

            Large,

            Legendary
        }

        #endregion

        #region Public Properties

        public static IEnumerable<GameObject> AllGameObjects
        {
            get
            {
                return GameObjectsList;
            }
        }

        public static IEnumerable<Obj_AI_Base> Ally
        {
            get
            {
                return AllyList;
            }
        }

        public static IEnumerable<Obj_AI_Hero> AllyHeroes
        {
            get
            {
                return AllyHeroesList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> AllyMinions
        {
            get
            {
                return AllyMinionsList;
            }
        }

        public static IEnumerable<Obj_AI_Turret> AllyTurrets
        {
            get
            {
                return AllyTurretsList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> AllyWards
        {
            get
            {
                return AllyWardsList;
            }
        }

        public static IEnumerable<AttackableUnit> AttackableUnits
        {
            get
            {
                return AttackableUnitsList;
            }
        }

        public static IEnumerable<Obj_AI_Base> Enemy
        {
            get
            {
                return EnemyList;
            }
        }

        public static IEnumerable<Obj_AI_Hero> EnemyHeroes
        {
            get
            {
                return EnemyHeroesList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> EnemyMinions
        {
            get
            {
                return EnemyMinionsList;
            }
        }

        public static IEnumerable<Obj_AI_Turret> EnemyTurrets
        {
            get
            {
                return EnemyTurretsList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> EnemyWards
        {
            get
            {
                return EnemyWardsList;
            }
        }

        public static IEnumerable<Obj_AI_Hero> Heroes
        {
            get
            {
                return HeroesList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> Jungle
        {
            get
            {
                return JungleList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> JungleLarge
        {
            get
            {
                return JungleLargeList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> JungleLegendary
        {
            get
            {
                return JungleLegendaryList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> JungleSmall
        {
            get
            {
                return JungleSmallList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> Minions
        {
            get
            {
                return MinionsList;
            }
        }

        public static Obj_AI_Hero Player { get; set; }

        public static IEnumerable<Obj_AI_Turret> Turrets
        {
            get
            {
                return TurretsList;
            }
        }

        public static IEnumerable<Obj_AI_Minion> Wards
        {
            get
            {
                return WardsList;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static bool Compare(this GameObject gameObject, GameObject @object)
        {
            return gameObject != null && gameObject.IsValid && @object != null && @object.IsValid
                   && gameObject.NetworkId == @object.NetworkId;
        }

        public static IEnumerable<T> Get<T>()
            where T : GameObject, new()
        {
            return AllGameObjects.OfType<T>();
        }

        public static JungleType GetJungleType(this Obj_AI_Minion minion)
        {
            if (SmallNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Small;
            }

            if (LargeNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Large;
            }

            if (LegendaryNameRegex.Any(regex => Regex.IsMatch(minion.Name, regex)))
            {
                return JungleType.Legendary;
            }

            return JungleType.Unknown;
        }

        public static IEnumerable<T> GetNative<T>()
            where T : GameObject, new()
        {
            return ObjectManager.Get<T>();
        }

        #endregion

        #region Methods

        internal static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            Player = Player;

            HeroesList.AddRange(ObjectManager.Get<Obj_AI_Hero>());
            MinionsList.AddRange(ObjectManager.Get<Obj_AI_Minion>().Where(o => o.Team != GameObjectTeam.Neutral && !o.Name.Contains("ward")));
            TurretsList.AddRange(ObjectManager.Get<Obj_AI_Turret>());
            JungleList.AddRange(ObjectManager.Get<Obj_AI_Minion>().Where(o => o.Team == GameObjectTeam.Neutral && o.Name != "WardCorpse" && o.Name != "Barrel"));
            WardsList.AddRange(ObjectManager.Get<Obj_AI_Minion>().Where(o => o.Name.Contains("ward")));

            GameObjectsList.AddRange(ObjectManager.Get<GameObject>());
            AttackableUnitsList.AddRange(ObjectManager.Get<AttackableUnit>());

            EnemyHeroesList.AddRange(HeroesList.Where(o => o.IsEnemy));
            EnemyMinionsList.AddRange(MinionsList.Where(o => o.IsEnemy));
            EnemyTurretsList.AddRange(TurretsList.Where(o => o.IsEnemy));
            EnemyList.AddRange(EnemyHeroesList.Cast<Obj_AI_Base>().Concat(EnemyMinionsList).Concat(EnemyTurretsList));

            AllyHeroesList.AddRange(HeroesList.Where(o => o.IsAlly));
            AllyMinionsList.AddRange(MinionsList.Where(o => o.IsAlly));
            AllyTurretsList.AddRange(TurretsList.Where(o => o.IsAlly));
            AllyList.AddRange(
                AllyHeroesList.Cast<Obj_AI_Base>().Concat(AllyMinionsList).Concat(AllyTurretsList));

            JungleSmallList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Small));
            JungleLargeList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Large));
            JungleLegendaryList.AddRange(JungleList.Where(o => o.GetJungleType() == JungleType.Legendary));

            AllyWardsList.AddRange(WardsList.Where(o => o.IsAlly));
            EnemyWardsList.AddRange(WardsList.Where(o => o.IsEnemy));

            GameObject.OnCreate += OnCreate;
            GameObject.OnDestroy += OnDelete;
        }

        private static void OnCreate(GameObject sender)
        {
            GameObjectsList.Add(sender);

            var attackableUnit = sender as AttackableUnit;
            if (attackableUnit != null)
            {
                AttackableUnitsList.Add(attackableUnit);
            }

            var hero = sender as Obj_AI_Hero;
            if (hero != null)
            {
                HeroesList.Add(hero);
                if (hero.IsEnemy)
                {
                    EnemyHeroesList.Add(hero);
                    EnemyList.Add(hero);
                }
                else
                {
                    AllyHeroesList.Add(hero);
                    AllyList.Add(hero);
                }

                return;
            }

            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                if (minion.Team != GameObjectTeam.Neutral)
                {
                    if (minion.Name.Contains("ward"))
                    {
                        WardsList.Add(minion);
                        if (minion.IsEnemy)
                        {
                            EnemyWardsList.Add(minion);
                        }
                        else
                        {
                            AllyWardsList.Add(minion);
                        }
                    }
                    else
                    {
                        MinionsList.Add(minion);
                        if (minion.IsEnemy)
                        {
                            EnemyMinionsList.Add(minion);
                            EnemyList.Add(minion);
                        }
                        else
                        {
                            AllyMinionsList.Add(minion);
                            AllyList.Add(minion);
                        }
                    }
                }
                else if (minion.Name != "WardCorpse" && minion.Name != "Barrel")
                {
                    JungleList.Add(minion);
                    switch (minion.GetJungleType())
                    {
                        case JungleType.Small:
                            JungleSmallList.Add(minion);
                            break;
                        case JungleType.Large:
                            JungleLargeList.Add(minion);
                            break;
                        case JungleType.Legendary:
                            JungleLegendaryList.Add(minion);
                            break;
                    }
                }

                return;
            }

            var turret = sender as Obj_AI_Turret;
            if (turret != null)
            {
                TurretsList.Add(turret);
                if (turret.IsEnemy)
                {
                    EnemyTurretsList.Add(turret);
                    EnemyList.Add(turret);
                }
                else
                {
                    AllyTurretsList.Add(turret);
                    AllyList.Add(turret);
                }
            }
        }

        private static void OnDelete(GameObject sender)
        {
            foreach (var gameObject in GameObjectsList.Where(o => o.Compare(sender)).ToList())
            {
                GameObjectsList.Remove(gameObject);
            }

            var attackableUnit = sender as AttackableUnit;
            if (attackableUnit != null)
            {
                foreach (var attackableUnitObject in AttackableUnitsList.Where(a => a.Compare(attackableUnit)).ToList())
                {
                    AttackableUnitsList.Remove(attackableUnitObject);
                }
            }

            var hero = sender as Obj_AI_Hero;
            if (hero != null)
            {
                foreach (var heroObject in HeroesList.Where(h => h.Compare(hero)).ToList())
                {
                    HeroesList.Remove(heroObject);
                    if (hero.IsEnemy)
                    {
                        EnemyHeroesList.Remove(heroObject);
                        EnemyList.Remove(heroObject);
                    }
                    else
                    {
                        AllyHeroesList.Remove(heroObject);
                        AllyList.Remove(heroObject);
                    }
                }

                return;
            }

            var minion = sender as Obj_AI_Minion;
            if (minion != null)
            {
                if (minion.Team != GameObjectTeam.Neutral)
                {
                    if (minion.Name.Contains("ward"))
                    {
                        foreach (var wardObject in WardsList.Where(w => w.Compare(minion)).ToList())
                        {
                            WardsList.Remove(wardObject);
                            if (minion.IsEnemy)
                            {
                                EnemyWardsList.Remove(wardObject);
                            }
                            else
                            {
                                AllyWardsList.Remove(wardObject);
                            }
                        }
                    }
                    else
                    {
                        foreach (var minionObject in MinionsList.Where(m => m.Compare(minion)).ToList())
                        {
                            MinionsList.Remove(minionObject);
                            if (minion.IsEnemy)
                            {
                                EnemyMinionsList.Remove(minionObject);
                                EnemyList.Remove(minionObject);
                            }
                            else
                            {
                                AllyMinionsList.Remove(minionObject);
                                AllyList.Remove(minionObject);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var jungleObject in JungleList.Where(j => j.Compare(minion)).ToList())
                    {
                        JungleList.Remove(jungleObject);
                        switch (jungleObject.GetJungleType())
                        {
                            case JungleType.Small:
                                JungleSmallList.Remove(jungleObject);
                                break;
                            case JungleType.Large:
                                JungleLargeList.Remove(jungleObject);
                                break;
                            case JungleType.Legendary:
                                JungleLegendaryList.Remove(jungleObject);
                                break;
                        }
                    }
                }

                return;
            }

            var turret = sender as Obj_AI_Turret;
            if (turret != null)
            {
                foreach (var turretObject in TurretsList.Where(t => t.Compare(turret)).ToList())
                {
                    TurretsList.Remove(turretObject);
                    if (turret.IsEnemy)
                    {
                        EnemyTurretsList.Remove(turretObject);
                        EnemyList.Remove(turretObject);
                    }
                    else
                    {
                        AllyTurretsList.Remove(turretObject);
                        AllyList.Remove(turretObject);
                    }
                }
            }
        }

        #endregion
    }
}