﻿using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class ThresherCyan : Thresher
        {
        public ThresherCyan(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/ThresherCyan", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.LaysMushrooms = true;
            }
        }
    }
