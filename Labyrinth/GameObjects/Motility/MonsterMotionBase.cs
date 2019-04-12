using System;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    // todo prevent monster from entering level that is higher than player's highest level (21AF in disassembly)
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

            this.Monster.Move(direction, this.Monster.CurrentSpeed);
            return true;
            }
        }
    }
