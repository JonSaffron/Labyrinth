using System;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class TigerMoth : Monster, ILayEggs
        {
        public TigerMoth(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("Sprites/Monsters/TigerMoth", 4));
            
            this.Mobility = MonsterMobility.Aggressive;
            this.ChangeRooms = ChangeRooms.MovesRoom;
            this.LaysMushrooms = true;
            this.ShootBehaviour = MonsterShootBehaviour.WhenProvoked;
            }

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
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
