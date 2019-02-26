using System;
using System.Xml;
using JetBrains.Annotations;
using Labyrinth.GameObjects;
using Labyrinth.GameObjects.Behaviour;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    public struct MonsterDef
        {
        public string Breed { get; set; }
        private Type _type;
        public Vector2 Position { get; set; }
        public int Energy { get; set; }
        public MonsterMobility? Mobility { get; set;}
        public Direction? InitialDirection { get; set; }
        public ChangeRooms? ChangeRooms { get; set; }
        public bool? IsEgg { get; set; }
        public int? TimeBeforeHatching { get; set; }
        public bool? LaysMushrooms { get; set; }
        public bool? LaysEggs { get; set; }
        public bool? SplitsOnHit { get; set; }
        public bool? ShootsAtPlayer { get; set; }
        public bool? ShootsOnceProvoked { get; set; }
        public bool? ShotsBounceOff { get; set; }
        public bool? IsActive { get; set; }

        [Obsolete("use Breed property instead")]
        public Type Type
            {
            get { return this._type; }
            set
                {
                if (!value.IsSubclassOf(typeof(Monster)))
                    throw new ArgumentOutOfRangeException();
                this._type = value;
                }
            }

        [Obsolete("use Breed property instead")]
        public string MonsterType
            {
            get { return this._type?.Name; }
            set
                {
                string typeName = "Labyrinth.GameObjects." + value;
                Type monsterType = Type.GetType(typeName);
                this._type = monsterType;
                }
            }

        public static MonsterDef FromExistingMonster([NotNull] Monster monster)
            {
            if (monster == null)
                throw new ArgumentNullException();
            if (monster.IsEgg)
                throw new ArgumentException("Cannot get properties of an egg.");

            var result = new MonsterDef
                {
                Breed = monster.Breed,
                Position = monster.TilePosition.ToPosition(),
                Energy = monster.OriginalEnergy,
                Mobility = monster.Mobility,
                InitialDirection = monster.InitialDirection,
                ChangeRooms = monster.ChangeRooms,
                IsEgg = false,
                LaysMushrooms = monster.Behaviours.Has<LaysMushroom>(),
                LaysEggs = monster.Behaviours.Has<LaysEgg>(),
                SplitsOnHit = monster.Behaviours.Has<SpawnsUponDeath>(),
                ShootsAtPlayer = monster.Behaviours.Has<ShootsAtPlayer>(),
                ShootsOnceProvoked = monster.Behaviours.Has<StartsShootingWhenHurt>(),
                ShotsBounceOff = monster.ShotsBounceOff,
                IsActive = monster.IsActive
                };
            return result;
            }

        internal static MonsterDef FromXml(XmlElement mdef)
            {
            MonsterDef result = new MonsterDef
                {
                // todo change worlddef monster Type to Breed?
                Breed = mdef.GetAttribute("Type"),
                Energy = int.Parse(mdef.GetAttribute(nameof(result.Energy)))
                };

            string initialDirection = mdef.GetAttribute(nameof(result.InitialDirection));
            if (!string.IsNullOrEmpty(initialDirection))
                {
                result.InitialDirection = (Direction)Enum.Parse(typeof(Direction), initialDirection);
                }

            string mobility = mdef.GetAttribute(nameof(result.Mobility));
            if (!string.IsNullOrEmpty(mobility))
                {
                result.Mobility = (MonsterMobility)Enum.Parse(typeof(MonsterMobility), mobility);
                }

            string changeRooms = mdef.GetAttribute(nameof(result.ChangeRooms));
            if (!string.IsNullOrEmpty(changeRooms))
                {
                result.ChangeRooms = (ChangeRooms)Enum.Parse(typeof(ChangeRooms), changeRooms);
                }

            string isEggAttribute = mdef.GetAttribute(nameof(result.IsEgg));
            if (!string.IsNullOrEmpty(isEggAttribute))
                {
                result.IsEgg = bool.Parse(isEggAttribute);
                }

            string timeBeforeHatchingAttribute = mdef.GetAttribute(nameof(result.TimeBeforeHatching));
            if (!string.IsNullOrEmpty(timeBeforeHatchingAttribute))
                {
                result.TimeBeforeHatching = int.Parse(timeBeforeHatchingAttribute) | 1;
                }

            string laysMushrooms = mdef.GetAttribute(nameof(result.LaysMushrooms));
            if (!string.IsNullOrEmpty(laysMushrooms))
                {
                result.LaysMushrooms = bool.Parse(laysMushrooms);
                }

            string laysEggs = mdef.GetAttribute(nameof(result.LaysEggs));
            if (!string.IsNullOrEmpty(laysEggs))
                {
                result.LaysEggs = bool.Parse(laysEggs);
                }

            string splitsOnHit = mdef.GetAttribute(nameof(result.SplitsOnHit));
            if (!string.IsNullOrEmpty(splitsOnHit))
                {
                result.SplitsOnHit = bool.Parse(splitsOnHit);
                }

            string shootsAtPlayer = mdef.GetAttribute(nameof(result.ShootsAtPlayer));
            if (!string.IsNullOrEmpty(shootsAtPlayer))
                {
                result.ShootsAtPlayer = bool.Parse(shootsAtPlayer);
                }

            string shootsOnceProvoked = mdef.GetAttribute(nameof(result.ShootsOnceProvoked));
            if (!string.IsNullOrEmpty(shootsOnceProvoked))
                {
                result.ShootsOnceProvoked = bool.Parse(shootsOnceProvoked);
                }

            string shotsBounceOffAttribute = mdef.GetAttribute(nameof(result.ShotsBounceOff));
            if (!string.IsNullOrEmpty(shotsBounceOffAttribute))
                {
                bool shotsBounceOff = bool.Parse(shotsBounceOffAttribute);
                result.ShotsBounceOff = shotsBounceOff;
                }

            string isActiveAttribute = mdef.GetAttribute(nameof(result.IsActive));
            if (!string.IsNullOrEmpty(isActiveAttribute))
                {
                bool isActive = bool.Parse(isActiveAttribute);
                result.IsActive = isActive;
                }

            return result;
            }
        }
    }
