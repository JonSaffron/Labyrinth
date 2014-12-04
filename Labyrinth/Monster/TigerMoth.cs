using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.Monster
    {
    sealed class TigerMoth : Monster
        {
        public TigerMoth(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.NormalAnimation = Animation.LoopingAnimation(World, "Sprites/Monsters/TigerMoth", 4);
            
            this.CurrentVelocity = AnimationPlayer.BaseSpeed;
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.MovesRoom;
            this.LaysMushrooms = true;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsHavingBeenShot;
            }

        protected override Func<Monster, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Aggressive:
                    return MonsterMovement.DetermineDirectionSemiAggressive;
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
