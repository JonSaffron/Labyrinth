using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class Thresher : Monster, ILayEggs
        {
        protected Thresher(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            }

        protected override Func<Monster, World, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
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
            var result = Create(typeOfMonster, this.World, this.Position, this.OriginalEnergy);
            return result;
            }
        }
    }
