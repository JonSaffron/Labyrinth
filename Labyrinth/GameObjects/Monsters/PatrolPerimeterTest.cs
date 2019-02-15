using System;
using Labyrinth.GameObjects.Movement;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class PatrolPerimiterTest : Monster
        {
        public PatrolPerimiterTest(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.SetNormalAnimation(Animation.LoopingAnimation("sprites/Monsters/DeathCube", 3));
            this.Mobility = MonsterMobility.Patrolling;
            }

        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Patrolling:
                    {
                    var result = GlobalServices.MonsterMovementFactory.PatrolPerimeter(this, this.InitialDirection);
                    GlobalServices.Game.Services.RemoveService(typeof(PatrolPerimeter));
                    GlobalServices.Game.Services.AddService(typeof(PatrolPerimeter), result);
                    return result;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }

        public override decimal StandardSpeed => base.StandardSpeed / 2;
        }
    }
