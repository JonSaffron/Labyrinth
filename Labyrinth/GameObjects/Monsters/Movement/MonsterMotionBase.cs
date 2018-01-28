﻿using System;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Movement
    {
    abstract class MonsterMotionBase : IMonsterMotion
        {
        protected readonly Monster Monster;

        protected MonsterMotionBase([NotNull] Monster monster)
            {
            this.Monster = monster ?? throw new ArgumentNullException(nameof(monster));
            }

        public virtual Direction DetermineDirection()
            {
            return Direction.None;
            }

        public virtual bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();
            if (direction != Direction.None)
                {
                direction = MonsterMovement.UpdateDirectionWhereMovementBlocked(this.Monster, direction);
                }

            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction, this.Monster.StandardSpeed);
            return true;
            }
        }
    }