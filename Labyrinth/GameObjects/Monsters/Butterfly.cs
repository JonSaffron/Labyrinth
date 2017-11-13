using System;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Butterfly : Monster, ILayEggs
        {
        public Butterfly(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/Butterfly", 3));
            
            this.Flitters = true;
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.LaysEggs = true;
            this.ShootBehaviour = MonsterShootBehaviour.Yes;
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

        public MonsterDef LayAnEgg()
            {
            var result = MonsterDef.FromExistingMonster(this);
            return result;
            }
        }
    }
