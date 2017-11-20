using System;
using Labyrinth.GameObjects.Movement;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class RotaFloater : Monster
        {
        // todo see if rotafloater can be better animated

        protected RotaFloater(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            }

        protected override IMonsterMovement GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Placid:
                    //return GlobalServices.MonsterMovementFactory.StandardRolling(this.InitialDirection);
                    return new PatrolPerimeter(MonsterMovement.RandomDirection());
                case MonsterMobility.Patrolling:
                    return GlobalServices.MonsterMovementFactory.StandardPatrolling(this.InitialDirection);
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.RotaFloaterCyanMovement();
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
