using System;
using System.Collections.Generic;
using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class StandardShot : Shot
        {
        // Movement
        public ShotType ShotType { get; private set; }
        public bool HasRebounded { get; private set; }
        public Direction DirectionOfTravel { get; private set; }

        private double _timeToTravel;
        private readonly Dictionary<Direction, Rectangle> _boundingRectangles;

        public StandardShot(AnimationPlayer animationPlayer, Vector2 position, Direction d, int energy, ShotType shotType) : base(animationPlayer, position)
            {
            this.Energy = energy;
            this.ShotType = shotType;
            this.DirectionOfTravel = d;
            SetDirectionAndDestination();

            string textureName;
            switch (this.ShotType)
                {
                case ShotType.Player:
                    textureName = "Sprites/Shot/RedShot";
                    break;
                case ShotType.Monster: 
                    textureName = "Sprites/Shot/GreenShot";
                    break;
                default:
                    throw new InvalidOperationException();
                }
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
            switch (this.DirectionOfTravel)
                {
                case Direction.Left:
                case Direction.Right:
                    distanceToTravel = (decimal) (Constants.RoomSizeInPixels.X * 1.25);
                    Ap.Rotation = 0.0f;
                    break;
                case Direction.Up:
                case Direction.Down:
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
                Rectangle result = this._boundingRectangles[this.DirectionOfTravel];
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
            bool result = false;
            var timeRemaining = gameTime.ElapsedGameTime.TotalSeconds;
            while (timeRemaining > 0 && this._timeToTravel > 0)
                {
                if (!base.IsMoving)
                    SetDirectionAndDestination();

                result = true;
                var timeBeforeMove = timeRemaining;
                bool moveCompleted = !this.TryToCompleteMoveToTarget(ref timeRemaining);
                this._timeToTravel -= (timeBeforeMove - timeRemaining);
                if (!moveCompleted)
                    break;
                }

            return result;
            }

        private void SetDirectionAndDestination()
            {
            this.Move(this.DirectionOfTravel, this.StandardSpeed);
            }

        public void Reverse()
            {
            if (this.HasRebounded)
                throw new InvalidOperationException();
            
            this.DirectionOfTravel = this.DirectionOfTravel.Reversed();
            this.PlaySound(GameSound.ShotBounces);
            this.HasRebounded = true;
            ResetTimeToTravel();
            }

        public override bool IsMoving => true;

        public override ObjectCapability Capability => ObjectCapability.CanPushOthers;

        protected override decimal StandardSpeed => Constants.BaseSpeed * 4;
        }
    }
