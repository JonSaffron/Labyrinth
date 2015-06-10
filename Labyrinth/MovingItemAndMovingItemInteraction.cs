using System;
using System.Linq;
using Labyrinth.Monster;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class MovingItemAndMovingItemInteraction : IInteraction
        {
        private readonly World _world;
        private readonly Player _player;
        private readonly Boulder _boulder;
        private readonly Mine _mine;
        private readonly Monster.Monster _monster1;
        private readonly Monster.Monster _monster2;

        private readonly MovingItem _moveableObject;
        private readonly MovingItem _insubstantialObject;

        public MovingItemAndMovingItemInteraction(World world, MovingItem movingItem1, MovingItem movingItem2)
            {
            if (world == null)
                throw new ArgumentNullException("world");
            if (movingItem1 == null)
                throw new ArgumentNullException("movingItem1");
            if (movingItem2 == null)
                throw new ArgumentNullException("movingItem2");
            if (movingItem1 is Shot)
                throw new ArgumentOutOfRangeException("movingItem1");
            if (movingItem2 is Shot)
                throw new ArgumentOutOfRangeException("movingItem2");

            this._world = world;
            var items = new[] {movingItem1, movingItem2};
            this._player = items.OfType<Player>().SingleOrDefault();
            this._boulder = items.OfType<Boulder>().SingleOrDefault();
            this._mine = items.OfType<Mine>().SingleOrDefault();
            this._monster1 = items.OfType<Monster.Monster>().FirstOrDefault();
            this._monster2 = items.OfType<Monster.Monster>().Skip(1).FirstOrDefault();

            this._moveableObject = items.FirstOrDefault(item => item.Solidity == ObjectSolidity.Moveable);
            this._insubstantialObject = items.FirstOrDefault(item => item.Solidity == ObjectSolidity.Insubstantial);
            }

        public void Collide()
            {
            if (
                (this._player != null && !this._player.IsExtant)
             || (this._monster1 != null && !this._monster1.IsExtant)
             || (this._monster2 != null && !this._monster2.IsExtant)
             || (this._boulder != null && !this._boulder.IsExtant)
             || (this._mine != null && !this._mine.IsExtant))
                return;

            if (this._mine != null)
                {
                var steppedOnBy = this._player ?? this._monster1 ?? (MovingItem) this._boulder;
                this._mine.SteppedOnBy(steppedOnBy);
                return;
                }

            if (this._player != null && this._monster1 != null)
                {
                int monsterEnergy = this._monster1.InstantlyExpire();
                this._player.ReduceEnergy(monsterEnergy);
                this._world.AddBang(this._monster1.Position, BangType.Long);
                this._world.Game.SoundPlayer.Play(GameSound.PlayerCollidesWithMonster);
                return;
                }
            
            if (this._moveableObject != null && this._insubstantialObject != null)
                {
                // test whether moveable object can be pushed or bounced
                if ((this._insubstantialObject.Capability == ObjectCapability.CanPushOthers || this._insubstantialObject.Capability == ObjectCapability.CanPushOrCauseBounceBack)
                    && this._moveableObject.Direction == Direction.None && this._insubstantialObject.Direction != Direction.None && !this._insubstantialObject.IsBouncingBack)
                    {
                    Vector2 difference = this._moveableObject.Position - this._insubstantialObject.Position;
                    float distanceApart = Math.Abs(difference.Length());
                    if (distanceApart < 40)
                        {
                        this._moveableObject.PushOrBounce(this._player, this._insubstantialObject.Direction);
                        return;
                        }
                    }

                // test whether the moveable object crushing the insubstantial object
                if (this._moveableObject.Direction != Direction.None && this._moveableObject.Direction != this._insubstantialObject.Direction)
                    {
                    var insubstantialObjectPosition = this._insubstantialObject.Direction == Direction.None ? this._insubstantialObject.Position : this._insubstantialObject.MovingTowards;
                    var insubtantialObjectTile = TilePos.TilePosFromPosition(insubstantialObjectPosition);
                    var moveableObjectTile = TilePos.TilePosFromPosition(this._moveableObject.MovingTowards);
                    if (insubtantialObjectTile == moveableObjectTile)
                        {
                        var energy = this._insubstantialObject.InstantlyExpire();
                        this._world.AddBang(this._insubstantialObject.Position, BangType.Long);
                        if (this._insubstantialObject is Monster.Monster)
                            {
                            this._world.Game.SoundPlayer.Play(GameSound.MonsterDies);
                            if (!(this._insubstantialObject is DeathCube))
                                {
                                var score = ((energy >> 1) + 1) * 20;
                                this._world.IncreaseScore(score);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
