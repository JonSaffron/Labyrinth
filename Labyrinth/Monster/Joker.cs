using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class Joker : Monster
        {
        public Joker(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/Joker", 4);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.Flitters = true;
            this.Mobility = MonsterMobility.Cautious;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.LaysEggs = true;
            this.SplitsOnHit = true;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
            }

        protected override Func<Monster, World, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Cautious:
                    return MonsterMovement.DetermineDirectionCautious;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
