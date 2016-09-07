using System;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class DiamondDemon : Monster, ILayEggs
        {
        public DiamondDemon(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/DiamondDemon", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.Flitters = true;
            this.LaysEggs = true;
            this.ChangeRooms = ChangeRooms.FollowsPlayer;
            this.ShootBehaviour = MonsterShootBehaviour.ShootsImmediately;
            }

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Placid:
                    return GlobalServices.MonsterMovementFactory.Placid();
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.SemiAggressive();
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
