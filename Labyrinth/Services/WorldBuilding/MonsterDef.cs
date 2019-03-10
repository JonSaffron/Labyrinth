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
        public Vector2 Position { get; set; }
        public int Energy { get; set; }
        public bool IsEgg { get; set; }
        public int? TimeBeforeHatching { get; set; }
        public MonsterMobility? Mobility { get; set;}
        public Direction? InitialDirection { get; set; }
        public MonsterMobility? MobilityAfterInjury { get; set; }
        public ChangeRooms? ChangeRooms { get; set; }
        public ChangeRooms? ChangeRoomsAfterInjury { get; set; }
        public bool? LaysMushrooms { get; set; }
        public bool? LaysEggs { get; set; }
        public bool? SplitsOnHit { get; set; }
        public bool? ShootsAtPlayer { get; set; }
        public bool? ShootsOnceProvoked { get; set; }
        public bool? ShotsBounceOff { get; set; }
        public bool? IsActive { get; set; }

        public static MonsterDef FromExistingMonster([NotNull] Monster monster)
            {
            if (monster == null)
                throw new ArgumentNullException();

            var result = new MonsterDef
                {
                Breed = monster.Breed,
                Position = monster.TilePosition.ToPosition(),
                Energy = monster.OriginalEnergy,
                Mobility = monster.Mobility,
                InitialDirection = Direction.None,
                ChangeRooms = monster.ChangeRooms,
                IsEgg = false,
                LaysMushrooms = monster.Behaviours.Has<LaysMushroom>(),
                LaysEggs = monster.Behaviours.Has<LaysEgg>(),
                SplitsOnHit = monster.Behaviours.Has<SpawnsUponDeath>(),
                ShootsAtPlayer = monster.Behaviours.Has<ShootsAtPlayer>(),
                ShootsOnceProvoked = monster.Behaviours.Has<StartsShootingWhenHurt>(),
                ShotsBounceOff = monster.Properties.Get(GameObjectProperties.EffectOfShot) == EffectOfShot.Reflection,
                IsActive = monster.IsActive
                };

            if (result.Mobility == MonsterMobility.Patrolling)
                {
                throw new InvalidOperationException("Cannot clone a monster which is patrolling.");
                }

            return result;
            }

        internal static MonsterDef FromXml([NotNull] XmlElement mdef)
            {
            MonsterDef result = new MonsterDef
                {
                // todo change worlddef monster Type to Breed?
                Breed = mdef.GetAttribute(nameof(result.Breed)),
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

            string mobilityAfterInjury = mdef.GetAttribute(nameof(result.MobilityAfterInjury));
            if (!string.IsNullOrEmpty(mobilityAfterInjury))
                {
                result.MobilityAfterInjury = (MonsterMobility) Enum.Parse(typeof(MonsterMobility), mobilityAfterInjury);
                }

            string changeRooms = mdef.GetAttribute(nameof(result.ChangeRooms));
            if (!string.IsNullOrEmpty(changeRooms))
                {
                result.ChangeRooms = (ChangeRooms)Enum.Parse(typeof(ChangeRooms), changeRooms);
                }

            string changeRoomsAfterInjury = mdef.GetAttribute(nameof(ChangeRoomsAfterInjury));
            if (!string.IsNullOrEmpty(changeRoomsAfterInjury))
                {
                result.ChangeRoomsAfterInjury = (ChangeRooms) Enum.Parse(typeof(ChangeRooms), changeRoomsAfterInjury);
                }

            string isEggAttribute = mdef.GetAttribute(nameof(result.IsEgg));
            if (!string.IsNullOrEmpty(isEggAttribute))
                {
                result.IsEgg = XmlConvert.ToBoolean(isEggAttribute);
                }

            string timeBeforeHatchingAttribute = mdef.GetAttribute(nameof(result.TimeBeforeHatching));
            if (!string.IsNullOrEmpty(timeBeforeHatchingAttribute))
                {
                result.TimeBeforeHatching = int.Parse(timeBeforeHatchingAttribute) | 1;
                }

            string laysMushrooms = mdef.GetAttribute(nameof(result.LaysMushrooms));
            if (!string.IsNullOrEmpty(laysMushrooms))
                {
                result.LaysMushrooms = XmlConvert.ToBoolean(laysMushrooms);
                }

            string laysEggs = mdef.GetAttribute(nameof(result.LaysEggs));
            if (!string.IsNullOrEmpty(laysEggs))
                {
                result.LaysEggs = XmlConvert.ToBoolean(laysEggs);
                }

            string splitsOnHit = mdef.GetAttribute(nameof(result.SplitsOnHit));
            if (!string.IsNullOrEmpty(splitsOnHit))
                {
                result.SplitsOnHit = XmlConvert.ToBoolean(splitsOnHit);
                }

            string shootsAtPlayer = mdef.GetAttribute(nameof(result.ShootsAtPlayer));
            if (!string.IsNullOrEmpty(shootsAtPlayer))
                {
                result.ShootsAtPlayer = XmlConvert.ToBoolean(shootsAtPlayer);
                }

            string shootsOnceProvoked = mdef.GetAttribute(nameof(result.ShootsOnceProvoked));
            if (!string.IsNullOrEmpty(shootsOnceProvoked))
                {
                result.ShootsOnceProvoked = XmlConvert.ToBoolean(shootsOnceProvoked);
                }

            string shotsBounceOffAttribute = mdef.GetAttribute(nameof(result.ShotsBounceOff));
            if (!string.IsNullOrEmpty(shotsBounceOffAttribute))
                {
                result.ShotsBounceOff = XmlConvert.ToBoolean(shotsBounceOffAttribute);;
                }

            string isActiveAttribute = mdef.GetAttribute(nameof(result.IsActive));
            if (!string.IsNullOrEmpty(isActiveAttribute))
                {
                result.IsActive = XmlConvert.ToBoolean(isActiveAttribute);;
                }

            return result;
            }
        }
    }
