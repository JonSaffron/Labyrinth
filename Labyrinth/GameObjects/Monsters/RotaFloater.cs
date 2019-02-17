using System;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class RotaFloater : Monster
        {
        protected RotaFloater(AnimationPlayer animationPlayer, Vector2 position, int energy) : base("", animationPlayer, position, energy)
            {
            }

        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Placid:
                    return GlobalServices.MonsterMovementFactory.StandardRolling(this, this.InitialDirection);
                case MonsterMobility.Patrolling:
                    return GlobalServices.MonsterMovementFactory.StandardPatrolling(this, this.InitialDirection);
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.RotaFloaterCyanMovement(this);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
