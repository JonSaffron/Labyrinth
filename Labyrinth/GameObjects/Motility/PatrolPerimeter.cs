using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Labyrinth.GameObjects.Motility
    {
    class PatrolPerimeter : MonsterMotionBase
        {
        private Direction _lastDirection;
        private AttachmentToWall _attachment;
        private AttachmentMode _attachmentMode;

        public enum AttachmentToWall
            {
            FollowWallOnLeft = -1,
            FollowWallOnRight = 1
            }

        public enum AttachmentMode
            {
            Attached,
            TurningCorner,
            Detached
            }

        private static readonly Direction[] FollowWallOnLeft = { Direction.Left, Direction.Down, Direction.Right, Direction.Up };
        private static readonly Direction[] FollowWallOnRight = { Direction.Left, Direction.Up, Direction.Right, Direction.Down };
            
        public PatrolPerimeter([NotNull] Monster monster, Direction initialDirection) : base(monster)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(initialDirection), "May not be None");
            this._lastDirection = initialDirection;
            this._attachment = AttachmentToWall.FollowWallOnLeft;
            }

        protected override Direction DetermineDirection()
            {
            Direction result;

            switch (this._attachmentMode)
                {
                case AttachmentMode.Attached:
                    result = DetermineDirectionWhenAttached();
                    break;

                case AttachmentMode.TurningCorner:
                    result = DetermineDirectionWhenTurningCorner();
                    break;

                case AttachmentMode.Detached:
                    result = DetermineDirectionWhenDetached();
                    break;

                default:
                    throw new InvalidOperationException();
                }
            return result;
            }

        private Direction DetermineDirectionWhenAttached()
            { 
            using (var directions = GetPreferredDirections(this._lastDirection, this._attachment).GetEnumerator())
                {
                directions.MoveNext();
                var newDirection = directions.Current;

                var canGoInNewDirection = this.Monster.CanMoveInDirection(newDirection);
                if (!canGoInNewDirection)
                    {
                    // so there's definitely one or more walls. Just need to get the first direction we can go in.
                    this._attachmentMode = AttachmentMode.Attached;
                    while (directions.MoveNext())
                        {
                        newDirection = directions.Current;
                        if (this.Monster.CanMoveInDirection(newDirection))
                            {
                            return newDirection;
                            }
                        }
                    // can't go in any direction
                    return Direction.None;
                    }

                // monster could go in any direction as it's not next to any wall - continue current direction
                this._attachmentMode = AttachmentMode.TurningCorner;
                return newDirection;
                }
            }

        private Direction DetermineDirectionWhenTurningCorner()
            {
            using (var directions = GetPreferredDirections(this._lastDirection, this._attachment).GetEnumerator())
                {
                directions.MoveNext();
                var newDirection = directions.Current;

                var hasWallVanished = this.Monster.CanMoveInDirection(newDirection);
                this._attachmentMode = hasWallVanished ? AttachmentMode.Detached : AttachmentMode.Attached;
                while (directions.MoveNext())
                    {
                    newDirection = directions.Current;
                    if (this.Monster.CanMoveInDirection(newDirection))
                        {
                        return newDirection;
                        }
                    this._attachmentMode = AttachmentMode.Attached;
                    }
                // can't go in any direction
                return Direction.None;
                }
            }

        private Direction DetermineDirectionWhenDetached()
            {
            if (this.Monster.CanMoveInDirection(this._lastDirection))
                {
                return this._lastDirection;
                }

            // Aha. We have bumped into something so we need to re-attach.
            SwitchDirection();
            this._attachmentMode = AttachmentMode.Attached;

            using (var directions = GetPreferredDirections(this._lastDirection.Reversed(), this._attachment).GetEnumerator())
                {
                while (directions.MoveNext())
                    {
                    var newDirection = directions.Current;
                    if (this.Monster.CanMoveInDirection(newDirection))
                        {
                        // Upon attaching, we need to reverse the rotation that the monster uses to plan its next move
                        return newDirection;
                        }
                    }
                }

            // can't go in any direction
            this._attachmentMode = AttachmentMode.Detached;
            return Direction.None;
            }

        private void SwitchDirection()
            {
            if (this._attachment == AttachmentToWall.FollowWallOnLeft)
                this._attachment = AttachmentToWall.FollowWallOnRight;
            else if (this._attachment == AttachmentToWall.FollowWallOnRight)
                this._attachment = AttachmentToWall.FollowWallOnLeft;
            else
                throw new InvalidOperationException();
            }

         public static IEnumerable<Direction> GetPreferredDirections(Direction direction, AttachmentToWall attachment)
            {
            Direction[] dirs;
            if (attachment == AttachmentToWall.FollowWallOnLeft)
                dirs = FollowWallOnLeft;
            else if (attachment == AttachmentToWall.FollowWallOnRight)
                dirs = FollowWallOnRight;
            else
                throw new InvalidOperationException();
            var start = Array.IndexOf(dirs, direction);
            for (int i = 0; i < 4; i++)
                {
                var elementIndex = (start + 5 - i) % 4;
                yield return dirs[elementIndex];
                }
            }

       public override bool SetDirectionAndDestination()
            {
            Direction direction = DetermineDirection();

            if (direction == Direction.None)
                {
                this.Monster.StandStill();
                return false;
                }

            this.Monster.Move(direction);
            this._lastDirection = direction;
            return true;
            }

        public AttachmentToWall CurrentAttachmentToWall => this._attachment;

        public AttachmentMode CurrentAttachmentMode => this._attachmentMode;
        }
    }
