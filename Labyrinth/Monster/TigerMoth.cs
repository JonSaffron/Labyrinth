using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Monster
    {
    sealed class TigerMoth : Monster
        {
        public TigerMoth(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            var t = this.World.Content.Load<Texture2D>("sprites/Monsters/TigerMoth");
            this.NormalAnimation = Animation.LoopingAnimation(t, 4);
            
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
