using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    abstract class Flitterbug : Monster
        {
        protected Flitterbug(World world, Vector2 position, int energy) : base(world, position, energy)
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

        protected override Monster Clone()
            {
            var result = (Flitterbug)this.MemberwiseClone();
            return result;
            }
        }
    }
