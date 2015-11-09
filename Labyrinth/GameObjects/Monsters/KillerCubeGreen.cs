using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class KillerCubeGreen : KillerCube
        {
        public KillerCubeGreen(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("sprites/Monsters/KillerCubeGreen", 3));
            
            this.Mobility = MonsterMobility.Static;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
            }

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            if (mobility != MonsterMobility.Static)
                throw new ArgumentOutOfRangeException();
            return null;
            }
        }
    }
