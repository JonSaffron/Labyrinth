using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    sealed class Butterfly : Monster, ILayEggs
        {
        public Butterfly(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/Butterfly", 3));
            
            this.Flitters = true;
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.LaysEggs = true;
            this.MonsterShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
            }

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.FullPursuit();
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
