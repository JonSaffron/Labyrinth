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
        private readonly Monster.Monster _monster1;
        private readonly Monster.Monster _monster2;

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
            this._monster1 = items.OfType<Monster.Monster>().FirstOrDefault();
            this._monster2 = items.OfType<Monster.Monster>().Skip(1).FirstOrDefault();
            }

        public void Collide()
            {
            if (
                (this._player != null && !this._player.IsExtant)
             || (this._monster1 != null && !this._monster1.IsExtant)
             || (this._monster2 != null && !this._monster2.IsExtant)
             || (this._boulder != null && !this._boulder.IsExtant))
                return;

            if (this._player != null && this._monster1 != null)
                {
                int monsterEnergy = this._monster1.InstantlyExpire();
                this._player.ReduceEnergy(monsterEnergy);
                this._world.AddLongBang(this._monster1.Position);
                this._world.Game.SoundLibrary.Play(GameSound.PlayerCollidesWithMonster);
                return;
                }
                
            if (this._boulder != null && this._player != null)
                {
                Vector2 difference = this._player.Position - this._boulder.Position;
                float distanceApart = Math.Abs(difference.Length());
                if (distanceApart >= 40)
                    return;

                if (this._boulder.Direction == Direction.None && this._player.Direction != Direction.None && !this._player.IsBouncingBack)
                    {
                    // cannot crush if not moving - can it be pushed?
                    this._boulder.PushOrBounce(this._player, this._player.Direction);
                    return;
                    }
                
                // is the player in danger of being crushed?
                if (this._boulder.Direction == this._player.Direction)
                    return;

                var playerPosition = this._player.Direction == Direction.None ? this._player.Position : this._player.MovingTowards;
                var playerTile = TilePos.TilePosFromPosition(playerPosition);
                var boulderTile = TilePos.TilePosFromPosition(this._boulder.MovingTowards);
                if (playerTile == boulderTile)
                    {
                    this._player.InstantlyExpire();
                    this._world.AddLongBang(this._player.Position);
                    }
                return;
                }

            if (this._boulder != null && this._monster1 != null)
                {
                var monsterTile = TilePos.TilePosFromPosition(this._monster1.Position);
                var boulderTile = TilePos.TilePosFromPosition(this._boulder.MovingTowards);
                if (monsterTile == boulderTile)
                    {
                    var energy = this._monster1.InstantlyExpire();
                    this._world.AddLongBang(this._monster1.Position);
                    this._world.Game.SoundLibrary.Play(GameSound.MonsterDies);
                    if (!(this._monster1 is DeathCube))
                        {
                        var score = ((energy >> 1) + 1) * 20;
                        this._world.IncreaseScore(score);
                        }
                    }
                
                //return;
                }
            }
        }
    }
