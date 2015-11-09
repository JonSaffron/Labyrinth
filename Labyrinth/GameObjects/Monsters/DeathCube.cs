using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class DeathCube : Monster
        {
        public DeathCube(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("sprites/Monsters/DeathCube", 3));
            
            this.Mobility = MonsterMobility.Static;
            this.ShotsBounceOff = true;
            }

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            if (mobility != MonsterMobility.Static)
                throw new ArgumentOutOfRangeException();
            return null;
            }
        }
    }
