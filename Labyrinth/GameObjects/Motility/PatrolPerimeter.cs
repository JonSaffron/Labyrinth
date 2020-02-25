using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    class PatrolPerimeter : MonsterMotionBase
        {
        private Direction _lastDirection;

        public enum AttachmentToWall
            {
            FollowWallOnLeft = -1,
            FollowWallOnRight = 1
            }

        private static readonly Direction[] FollowWallOnLeft = { Direction.Left, Direction.Down, Direction.Right, Direction.Up };
        private static readonly Direction[] FollowWallOnRight = { Direction.Left, Direction.Up, Direction.Right, Direction.Down };

        [NotNull] private IPatrolState _state;

        public PatrolPerimeter([NotNull] Monster monster, Direction initialDirection) : base(monster)
            {
            if (initialDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(initialDirection), "May not be None");
            this._lastDirection = initialDirection;
            this.CurrentAttachmentToWall = AttachmentToWall.FollowWallOnLeft;
            this._state = new AttachedState(this);
            }

        public override ConfirmedDirection GetDirection()
            {
            ConfirmedDirection result = this._state.GetDirection();
            this._lastDirection = result;
            return result;
            }

        private interface IPatrolState
            {
            ConfirmedDirection GetDirection();
            }

        private class AttachedState : IPatrolState
            {
            private readonly PatrolPerimeter _parent;

            public AttachedState(PatrolPerimeter patrolPerimeter)
                {
                this._parent = patrolPerimeter;
                }

            public ConfirmedDirection GetDirection()
                { 
                using (var directions = GetPreferredDirections(this._parent._lastDirection, this._parent.CurrentAttachmentToWall).GetEnumerator())
                    {
                    directions.MoveNext();
                    var newDirection = directions.Current;

                    var canGoInNewDirection = this._parent.Monster.CanMoveInDirection(newDirection);
                    if (!canGoInNewDirection)
                        {
                        // so there's definitely one or more walls. Just need to get the first direction we can go in.
                        while (directions.MoveNext())
                            {
                            newDirection = directions.Current;
                            if (this._parent.Monster.CanMoveInDirection(newDirection))
                                {
                                return new ConfirmedDirection(newDirection);
                                }
                            }
                        // can't go in any direction
                        return ConfirmedDirection.None;
                        }

                    // monster could go in any direction as it's not next to any wall
                    // - turn the corner to follow the wall and hopefully we'll re-attach next move 
                    this._parent._state = new TurningCornerState(this._parent);
                    return new ConfirmedDirection(newDirection);
                    } 
                }
            }

        private class TurningCornerState : IPatrolState
            {
            private readonly PatrolPerimeter _parent;

            public TurningCornerState(PatrolPerimeter patrolPerimeter)
                {
                this._parent = patrolPerimeter;
                }

            public ConfirmedDirection GetDirection()
                {
                using (var directions = GetPreferredDirections(this._parent._lastDirection, this._parent.CurrentAttachmentToWall).GetEnumerator())
                    {
                    directions.MoveNext();
                    var newDirection = directions.Current;

                    var hasWallVanished = this._parent.Monster.CanMoveInDirection(newDirection);
                    this._parent._state = hasWallVanished ? (IPatrolState) new DetachedState(this._parent) : new AttachedState(this._parent);
                    while (directions.MoveNext())
                        {
                        newDirection = directions.Current;
                        if (this._parent.Monster.CanMoveInDirection(newDirection))
                            {
                            return new ConfirmedDirection(newDirection);
                            }
                        this._parent._state = new AttachedState(this._parent);
                        }

                    // can't go in any direction
                    return ConfirmedDirection.None;
                    }
                }
            }

        private class DetachedState : IPatrolState
            {
            private readonly PatrolPerimeter _parent;

            public DetachedState(PatrolPerimeter patrolPerimeter)
                {
                this._parent = patrolPerimeter;
                }

            public ConfirmedDirection GetDirection()
                {
                if (this._parent.Monster.CanMoveInDirection(this._parent._lastDirection))
                    {
                    return new ConfirmedDirection(this._parent._lastDirection);
                    }

                // Aha. We have bumped into something so we need to re-attach.
                // Upon attaching, we need to reverse the rotation that the monster uses to plan its next move
                this._parent.SwitchDirection();
                this._parent._state = new AttachedState(this._parent);

                using (var directions = GetPreferredDirections(this._parent._lastDirection.Reversed(), this._parent.CurrentAttachmentToWall).GetEnumerator())
                    {
                    while (directions.MoveNext())
                        {
                        var newDirection = directions.Current;
                        if (this._parent.Monster.CanMoveInDirection(newDirection))
                            {
                            return new ConfirmedDirection(newDirection);
                            }
                        }
                    }

                // can't go in any direction
                return ConfirmedDirection.None;
                }
            }

        private void SwitchDirection()
            {
            if (this.CurrentAttachmentToWall == AttachmentToWall.FollowWallOnLeft)
                this.CurrentAttachmentToWall = AttachmentToWall.FollowWallOnRight;
            else if (this.CurrentAttachmentToWall == AttachmentToWall.FollowWallOnRight)
                this.CurrentAttachmentToWall = AttachmentToWall.FollowWallOnLeft;
            else
                throw new InvalidOperationException();
            }

        /// <summary>
        /// Get a list of directions that could be turned towards in order of preference
        /// </summary>
        /// <param name="currentDirection">The current direction of travel</param>
        /// <param name="attachment">The side that the monster is attached to</param>
        /// <returns>A list of up to 4 directions</returns>
        /// <remarks>The first item will be 90 degrees to the <paramref name="currentDirection"/> as specified by <paramref name="attachment"/>.
        /// The second item will be the <paramref name="currentDirection"/>.</remarks>
        public static IEnumerable<Direction> GetPreferredDirections(Direction currentDirection, AttachmentToWall attachment)
            {
            Direction[] directions;
            if (attachment == AttachmentToWall.FollowWallOnLeft)
                directions = FollowWallOnLeft;
            else if (attachment == AttachmentToWall.FollowWallOnRight)
                directions = FollowWallOnRight;
            else
                throw new InvalidOperationException();

            var start = Array.IndexOf(directions, currentDirection);
            for (int i = 0; i < 4; i++)
                {
                var elementIndex = (start + 5 - i) % 4;
                yield return directions[elementIndex];
                }
            }

        // ReSharper disable once MemberCanBePrivate.Global - potentially useful 
        public AttachmentToWall CurrentAttachmentToWall { get; private set; }

        // ReSharper disable once UnusedMember.Global - potentially useful
        public bool IsAttached => this._state is AttachedState;

        public bool IsTurningCorner => this._state is TurningCornerState;

        public bool IsDetached => this._state is DetachedState;
        }
    }
