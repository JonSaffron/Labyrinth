using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class Flitterbug : Monster, ILayEggs
        {
        protected Flitterbug(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            }

        protected override Func<Monster, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Cautious:
                    return MonsterMovement.DetermineDirectionCautious;
                case MonsterMobility.Aggressive:
                    return MonsterMovement.DetermineDirectionSemiAggressive;
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
