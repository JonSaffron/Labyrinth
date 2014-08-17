using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    // sealed so that we can safely make virtual method calls from the constructor
    abstract class Thresher : Monster
        {
        protected Thresher(World world, Vector2 position, int energy) : base(world, position, energy)
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

        protected override Monster Clone()
            {
            var result = (Thresher)this.MemberwiseClone();
            return result;
            }
        }
    }
