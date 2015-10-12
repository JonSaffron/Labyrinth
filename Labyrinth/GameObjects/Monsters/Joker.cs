using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class Joker : Monster, ILayEggs
        {
        public Joker(World world, Vector2 position, int energy) : base(world, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation(World, "Sprites/Monsters/Joker", 4));
            
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

        public Monster LayAnEgg()
            {
            var typeOfMonster = this.GetType().Name;
            var result = Create(typeOfMonster, this.World, this.Position, this.OriginalEnergy);
            return result;
            }
        }
    }
