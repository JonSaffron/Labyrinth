using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class CrazyCrawler : Monster
        {
        public CrazyCrawler(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("sprites/Monsters/CrazyCrawler", 3));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ShotsBounceOff = true;
            }

        protected override Func<Monster, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Placid:
                    return MonsterMovement.RandomDirection;
                case MonsterMobility.Aggressive:
                    return MonsterMovement.DetermineDirectionSemiAggressive;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
