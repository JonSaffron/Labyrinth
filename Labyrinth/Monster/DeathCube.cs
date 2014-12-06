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

        public override int InstantDeath()
            {
            if (!this.IsExtant)
                return 0;
            
            // no score for crushing (presumably because it doesn't fight back)
            this.Energy = 0;
            this.World.Game.SoundLibrary.Play(GameSound.MonsterDies);
            return 0;
            }
        }
    }
