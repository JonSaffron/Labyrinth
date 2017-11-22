using System;
using System.Collections.Generic;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class StandardShot : Shot
        {
        public StaticItem Originator { get; }
        public bool HasRebounded { get; private set; }
        public Orientation Orientation { get; }
        
        private Direction _directionOfTravel;

        private double _timeToTravel;
        private readonly Dictionary<Direction, Rectangle> _boundingRectangles;

        private IEnumerator<bool> _movementIterator;
        private double _remainingTime;

        public StandardShot(AnimationPlayer animationPlayer, Vector2 position, Direction d, int energy, StaticItem originator) : base(animationPlayer, position)
            {
            if (d == Direction.None)
                throw new ArgumentOutOfRangeException("d");

            this.Energy = energy;
            this.Originator = originator;
            this._directionOfTravel = d;
            this.Orientation = d.Orientation();

            string textureName = originator is Player ? "Sprites/Shot/RedShot" : "Sprites/Shot/GreenShot";
            var staticImage = Animation.StaticAnimation(textureName);
            Ap.PlayAnimation(staticImage);
            ResetTimeToTravel();

            this._boundingRectangles = GetBoundingRectangles();
            }

        private Dictionary<Direction, Rectangle> GetBoundingRectangles()
            {
            var result = new Dictionary<Direction, Rectangle>();

            const int shortSideLength = Constants.TileLength / 8; // 4 pixels
            const int longSideLength = Constants.TileLength / 2; // 16 pixels
            var boundsForHorizontalMovement = new Rectangle(-(longSideLength / 2), -(shortSideLength / 2), longSideLength, shortSideLength);
            var boundsForVerticalMovement = new Rectangle(-(shortSideLength / 2), -(longSideLength / 2), shortSideLength, longSideLength);
            result.Add(Direction.Up, boundsForVerticalMovement);
            result.Add(Direction.Down, boundsForVerticalMovement);
            result.Add(Direction.Left, boundsForHorizontalMovement);
            result.Add(Direction.Right, boundsForHorizontalMovement);
            return result;
            }

        private void ResetTimeToTravel()
            {
            decimal distanceToTravel;
            switch (this._directionOfTravel.Orientation())
                {
                case Orientation.Horizontal:
                    distanceToTravel = (decimal) (Constants.RoomSizeInPixels.X * 1.25);
                    Ap.Rotation = 0.0f;
                    break;
                case Orientation.Vertical:
                    distanceToTravel = (decimal) (Constants.RoomSizeInPixels.Y * 1.25);
                    Ap.Rotation = (float)(Math.PI * 90.0f / 180f);
                    break;
                default:
                    throw new InvalidOperationException();
                }
            this._timeToTravel = (double) (distanceToTravel / this.StandardSpeed);
            }
        
        /// <summary>
        /// Gets a rectangle which bounds this object in world space.
        /// </summary>
        public override Rectangle BoundingRectangle
            {
            get
                {
                Rectangle result = this._boundingRectangles[this._directionOfTravel];
                result.Offset((int) this.Position.X, (int) this.Position.Y);
                return result;
                }
            }

        public override int DrawOrder => (int) SpriteDrawOrder.Shot;

        public override bool IsExtant
            {
            get
                {
                var result = base.IsExtant && this._timeToTravel > 0;
                return result;
                }
            }

        /// <summary>
        /// Handles moving the shot along
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            this._remainingTime = gameTime.ElapsedGameTime.TotalSeconds;
            if (this._movementIterator == null)
                this._movementIterator = this._movementIterator = Move().GetEnumerator();
            this._movementIterator.MoveNext();
            var result = this._movementIterator.Current;
            return result;
            }

        public override void ResetPosition(Vector2 position)
            {
            base.ResetPosition(position);
            // it's essential to reset the iterator 
            this._movementIterator = null;
            }
        
        private IEnumerable<bool> Move()
            {
            bool hasMovedSinceLastCall = false;
            while (true)
                {
                if (this.SetDirectionAndDestination())
                    {
                    hasMovedSinceLastCall = true;
                    while (true)
                        {
                        if (this.TryToCompleteMoveToTarget(ref this._remainingTime))
                            break;

                        yield return true;  // we have moved
                        }

                    }
                else
                    {
                    yield return hasMovedSinceLastCall;
                    hasMovedSinceLastCall = false;
                    }
                }
            // ReSharper disable once IteratorNeverReturns - this is deliberate
            }

        private bool SetDirectionAndDestination()
            {
            if (this._timeToTravel > 0)
                {
                this.Move(this._directionOfTravel, this.StandardSpeed);
                this._timeToTravel -= Constants.TileLength / (double) this.StandardSpeed;
                return true;
                }
            return false;
            }

        public void Reverse()
            {
            if (this.HasRebounded)
                throw new InvalidOperationException();
            
            this._directionOfTravel = this._directionOfTravel.Reversed();
            this.PlaySound(GameSound.ShotBounces);
            this.HasRebounded = true;
            ResetTimeToTravel();
            }

        public override bool IsMoving => true;

        public override ObjectCapability Capability => ObjectCapability.CanPushOthers;

        public override decimal StandardSpeed => Constants.BaseSpeed * 4;
        }
    }
