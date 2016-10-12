using System;
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
        private readonly Rectangle _boundsForHorizontalMovement;
        private readonly Rectangle _boundsForVerticalMovement;

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

            const int shortSideLength = Constants.TileLength / 8; // 4 pixels
            const int longSideLength = Constants.TileLength / 2; // 16 pixels
            this._boundsForHorizontalMovement = new Rectangle(-(longSideLength / 2), -(shortSideLength / 2), longSideLength, shortSideLength);
            this._boundsForVerticalMovement = new Rectangle(-(shortSideLength / 2), -(longSideLength / 2), shortSideLength, longSideLength);
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
                Rectangle result;
                if (this.DirectionOfTravel.IsHorizontal())
                    result = this._boundsForHorizontalMovement;
                else if (this.DirectionOfTravel.IsVertical())
                    result = this._boundsForVerticalMovement;
                else
                    throw new InvalidOperationException();

                result.Offset((int) this.Position.X, (int) this.Position.Y);
                return result;
                }
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.Shot;
                }
            }

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

        public override bool IsMoving
            {
            get
                {
                return true;
                }
            }

        public override ObjectCapability Capability
            {
            get
                {
                return ObjectCapability.CanPushOthers;
                }
            }

        protected override decimal StandardSpeed
            {
            get
                {
                return Constants.BaseSpeed * 4;
                }
            }

        }
    }
