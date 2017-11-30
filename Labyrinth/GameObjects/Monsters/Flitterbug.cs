using System;
using Labyrinth.GameObjects.Actions;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    abstract class Flitterbug : Monster
        {
        protected Flitterbug(AnimationPlayer animationPlayer, Vector2 position, int energy) : base(animationPlayer, position, energy)
            {
            this.MovementBehaviours.Add<Flitter>();
            }

        protected override IMonsterMotion GetMethodForDeterminingDirection(MonsterMobility mobility)
            {
            switch (mobility)
                {
                case MonsterMobility.Cautious:
                    return GlobalServices.MonsterMovementFactory.Cautious(this);
                case MonsterMobility.Aggressive:
                    return GlobalServices.MonsterMovementFactory.SemiAggressive(this);
                default:
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
