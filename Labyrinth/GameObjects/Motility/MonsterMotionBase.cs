using System;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    abstract class MonsterMotionBase : IMonsterMotion
        {
        protected readonly Monster Monster;

        protected MonsterMotionBase([NotNull] Monster monster)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            }

        protected virtual Direction DetermineDirection()
            {
            return Direction.None;
            }

        public virtual bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();
            if (direction != Direction.None)
                {
                this.Monster.ConfirmDirectionToMoveIn(direction, out Direction feasibleDirection);
                direction = feasibleDirection;
                }

            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction);
            return true;
            }
        }
    }
