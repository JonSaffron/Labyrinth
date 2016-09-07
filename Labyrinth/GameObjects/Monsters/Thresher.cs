using System;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class Thresher : Monster, ILayEggs
        {
        protected Thresher(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            }

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Placid:
                    return GlobalServices.MonsterMovementFactory.Placid();
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.SemiAggressive();
                case MonsterMobility.Cautious:
                    return GlobalServices.MonsterMovementFactory.Cautious();
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
