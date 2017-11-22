using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class KillerCubeGreen : KillerCube
        {
        public KillerCubeGreen(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("sprites/Monsters/KillerCubeGreen", 3));
            
            this.Mobility = MonsterMobility.Stationary;
            }

        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            if (mobility != MonsterMobility.Stationary)
                throw new ArgumentOutOfRangeException();
            return null;
            }
        }
    }
