using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class KillerCubeGreen : KillerCube
        {
        public KillerCubeGreen(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "sprites/Monsters/KillerCubeGreen", 3);
            
            this.Mobility = MonsterMobility.Static;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
            }

        protected override Func<Monster, World, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            if (mobility != MonsterMobility.Static)
                throw new ArgumentOutOfRangeException();
            return null;
            }
        }
    }
