using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class Thresher : Monster, ILayEggs
        {
        protected Thresher(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            }

        protected override Func<Monster, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Placid:
                    return MonsterMovement.RandomDirection;
                case MonsterMobility.Aggressive:
                    return MonsterMovement.DetermineDirectionSemiAggressive;
                case MonsterMobility.Cautious:
                    return MonsterMovement.DetermineDirectionCautious;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

        public Monster LayAnEgg()
            {
            var typeOfMonster = this.GetType().Name;
            var result = GlobalServices.GameState.Create(typeOfMonster, this.Position, this.OriginalEnergy);
            return result;
            }
        }
    }
