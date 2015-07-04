using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class CrazyCrawler : Monster
        {
        public CrazyCrawler(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation(World, "sprites/Monsters/CrazyCrawler", 3));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ShotsBounceOff = true;
            }

        protected override Func<Monster, World, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
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
