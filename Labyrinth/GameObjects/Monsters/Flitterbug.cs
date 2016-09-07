using System;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class Flitterbug : Monster, ILayEggs
        {
        protected Flitterbug(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            }

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Cautious:
                    return GlobalServices.MonsterMovementFactory.Cautious();
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
