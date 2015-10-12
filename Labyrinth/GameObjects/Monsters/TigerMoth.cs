using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class TigerMoth : Monster, ILayEggs
        {
        public TigerMoth(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/TigerMoth", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.MovesRoom;
            this.LaysMushrooms = true;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsHavingBeenShot;
            }

        protected override Func<Monster, World, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Aggressive:
                    return MonsterMovement.DetermineDirectionSemiAggressive;
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
