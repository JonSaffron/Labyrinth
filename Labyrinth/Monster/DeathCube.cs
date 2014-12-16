using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class DeathCube : Monster
        {
        public DeathCube(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "sprites/Monsters/DeathCube", 3);
            
            this.Mobility = MonsterMobility.Static;
            this.ShotsBounceOff = true;
            }

        protected override Func<Monster, World, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            if (mobility != MonsterMobility.Static)
                throw new ArgumentOutOfRangeException();
            return null;
            }
        }
    }
