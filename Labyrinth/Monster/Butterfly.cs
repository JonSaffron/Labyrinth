using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class Butterfly : Monster
        {
        public Butterfly(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/Butterfly", 3);
            
            this.Flitters = true;
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.LaysEggs = true;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
            }

        protected override Func<Monster, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Aggressive:
                    return MonsterMovement.DetermineDirectionFullPursuit;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
