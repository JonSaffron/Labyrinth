using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class DiamondDemon : Monster
        {
        public DiamondDemon(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/DiamondDemon", 4);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.Mobility = MonsterMobility.Aggressive;
            this.Flitters = true;
            this.LaysEggs = true;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
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

        protected override Monster Clone()
            {
            var result = (DiamondDemon)this.MemberwiseClone();
            return result;
            }
        }
    }
