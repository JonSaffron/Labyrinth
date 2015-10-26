using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class Joker : Monster, ILayEggs
        {
        public Joker(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/Joker", 4));
            
            this.Flitters = true;
            this.Mobility = MonsterMobility.Cautious;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.LaysEggs = true;
            this.SplitsOnHit = true;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
            }

        protected override Func<Monster, Direction> GetMethodForDeterminingDirection(MonsterMobility mobility)
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
            var result = GlobalServices.GameState.Create(typeOfMonster, this.Position, this.OriginalEnergy);
            return result;
            }
        }
    }
