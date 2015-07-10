using System;
using Labyrinth.Services.Display;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class StandardShot : Shot
        {
        // Movement
        public ShotType ShotType { get; private set; }
        public bool HasRebounded { get; private set; }

        private double _timeToTravel;
        private Direction _directionOfTravel;

        public StandardShot(World world, Vector2 position, Direction d, int energy, ShotType shotType) : base(world, position)
            {
            this.Energy = energy;
            this.ShotType = shotType;
            this._directionOfTravel = d;
            TrySetDirectionAndDestination();

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
            var staticImage = Animation.StaticAnimation(World, textureName);
            Ap.PlayAnimation(staticImage);
            ResetTimeToTravel();
            }

        private void ResetTimeToTravel()
            {
            decimal distanceToTravel;
            switch (this.Direction)
                {
                case Direction.Left:
                case Direction.Right:
                    distanceToTravel = World.WindowSizeX * 1.25m * Tile.Width;
                    Ap.Rotation = 0.0f;
                    break;
                case Direction.Up:
                case Direction.Down:
                    distanceToTravel = World.WindowSizeY * 1.25m * Tile.Height;
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
                int w, h;
                switch (this._directionOfTravel)
                    {
                    case Direction.Up:
                    case Direction.Down:
                        w = Tile.Width / 8;
                        h = Tile.Height / 2;
                        break;
                    
                    case Direction.Left:
                    case Direction.Right:
                        w = Tile.Width / 2;
                        h = Tile.Height / 8;
                        break;
                    
                    default:
                       throw new InvalidOperationException();
                    }
                var x = (int) Math.Round(this.Position.X - (w / 2.0));
                var y = (int) Math.Round(this.Position.Y - (h / 2.0));
                return new Rectangle(x, y, w, h);
                }
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.Shot;
                }
            }

        /// <summary>
        /// Handles moving the shot along
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            this.OriginalPosition = this.Position;
            if (this._timeToTravel <= 0)
                {
                this.InstantlyExpire();
                return false;
                }

            bool result = false;
            var timeRemaining = gameTime.ElapsedGameTime.TotalSeconds;
            while (timeRemaining > 0 && this._timeToTravel > 0)
                {
                if (!this.IsMoving && !TrySetDirectionAndDestination())
                    break;

                result = true;
                var timeBeforeMove = timeRemaining;
                bool moveCompleted = !this.TryToCompleteMoveToTarget(ref timeRemaining);
                this._timeToTravel -= (timeBeforeMove - timeRemaining);
                if (!moveCompleted)
                    break;
                }

            return result;
            }

        private bool TrySetDirectionAndDestination()
            {
            this.Move(this._directionOfTravel, this.StandardSpeed);
            return true;
            }

        public void Reverse()
            {
            if (this.HasRebounded)
                throw new InvalidOperationException();
            
            this._directionOfTravel = this._directionOfTravel.Reversed();
            this.World.Game.SoundPlayer.Play(GameSound.ShotBounces);
            this.HasRebounded = true;
            ResetTimeToTravel();
            }

        public void SetPosition(Vector2 newPosition)
            {
            this.Position = newPosition;
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
                return AnimationPlayer.BaseSpeed * 4;
                }
            }
        }
    }
