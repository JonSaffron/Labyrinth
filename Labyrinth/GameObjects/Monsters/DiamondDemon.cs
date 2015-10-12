using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class DiamondDemon : Monster, ILayEggs
        {
        public DiamondDemon(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/DiamondDemon", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.Flitters = true;
            this.LaysEggs = true;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
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

        public Monster LayAnEgg()
            {
            var typeOfMonster = this.GetType().Name;
            var result = Create(typeOfMonster, this.World, this.Position, this.OriginalEnergy);
            return result;
            }
        }
    }
