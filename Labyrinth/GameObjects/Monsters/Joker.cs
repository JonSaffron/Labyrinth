using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Joker : Monster, ILayEggs
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

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Cautious:
                    return GlobalServices.MonsterMovementFactory.Cautious();
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

        public Monster LayAnEgg()
            {
            var result = GlobalServices.GameState.CreateMonster(this.GetType(), this.Position, this.OriginalEnergy);
            return result;
            }
        }
    }
