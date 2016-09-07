using System;
using Labyrinth.Annotations;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth.Services.WorldBuilding
    {
    public struct MonsterDef
        {
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
        public MonsterShootBehaviour? ShootBehaviour { get; set; }
        public bool? ShotsBounceOff { get; set; }
        public bool? IsActive { get; set; }

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

        public string MonsterType
            {
            get { return this._type != null ? this._type.Name : null; }
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
                Type = monster.GetType(),
                Position = monster.Position,
                Energy = monster.Energy,
                Mobility = monster.Mobility,
                InitialDirection = monster.InitialDirection,
                ChangeRooms = monster.ChangeRooms,
                IsEgg = false,
                LaysMushrooms = monster.LaysMushrooms,
                LaysEggs = monster.LaysEggs,
                SplitsOnHit = monster.SplitsOnHit,
                ShootBehaviour = monster.ShootBehaviour,
                ShotsBounceOff = monster.ShotsBounceOff,
                IsActive = monster.IsActive
                };
            return result;
            }
        }
    }
