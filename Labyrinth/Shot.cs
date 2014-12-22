using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class Shot : MovingItem
        {
        // Movement
        public ShotType ShotType { get; private set; }
        public bool HasRebounded { get; private set; }

        private float _distanceToTravel;

        // Constants for controlling movement
        private const float StandardSpeed = AnimationPlayer.BaseSpeed * 4;

        public Shot(World world, Vector2 position, Direction d, int energy, ShotType shotType) : base(world, position)
            {
            this.Direction = d;
            this.Energy = energy;
            this.ShotType = shotType;
            this.CurrentVelocity = StandardSpeed;
            SetDestination();

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
            ResetDistanceToTravel();
            }

        private void ResetDistanceToTravel()
            {
            switch (this.Direction)
                {
                case Direction.Left:
                case Direction.Right:
                    this._distanceToTravel = World.WindowSizeX * 1.25f * Tile.Width;
                    Ap.Rotation = 0.0f;
                    break;
                case Direction.Up:
                case Direction.Down:
                    this._distanceToTravel = World.WindowSizeY * 1.25f * Tile.Height;
                    Ap.Rotation = (float)(Math.PI * 90.0f / 180f);
                    break;
                default:
                    throw new InvalidOperationException();
                }
            }
        
        /// <summary>
        /// Gets a rectangle which bounds this object in world space.
        /// </summary>
        public override Rectangle BoundingRectangle
            {
            get
                {
                int w, x, h, y;
                
                switch (this.Direction)
                    {
                    case Direction.Up:
                        w = Tile.Width / 8;
                        x = (Tile.Width / 2) - (w / 2);
                        h = Tile.Height / 2;
                        y = 0;
                        break;
                    
                    case Direction.Down:
                        w = Tile.Width / 8;
                        x = (Tile.Width / 2) - (w / 2);
                        h = Tile.Height / 2;
                        y = Tile.Height / 2;
                        break;
                    
                    case Direction.Left:
                        w = Tile.Width / 2;
                        x = 0;
                        h = Tile.Height / 8;
                        y = (Tile.Height / 2) - (h / 2);
                        break;
                    
                    case Direction.Right:
                        w = Tile.Width / 2;
                        x = Tile.Width / 2;
                        h = Tile.Height / 8;
                        y = (Tile.Height / 2) - (h / 2);
                        break;
                    
                    default:
                        throw new InvalidOperationException();
                    }
                        
                int left = (int)Math.Round(Position.X - AnimationPlayer.Origin.X) + x;
                int top = (int)Math.Round(Position.Y - AnimationPlayer.Origin.Y) + y;
                return new Rectangle(left, top, w, h);
                }
            }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            if (this._distanceToTravel <= 0 || this.Direction == Direction.None)
                {
                this.InstantlyExpire();
                return false;
                }

            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float remainingMovement = CurrentVelocity * elapsed;
            this._distanceToTravel -= remainingMovement;
            while (remainingMovement > 0)
                {
                bool needToChooseDirection;
                this.ContinueMove(ref remainingMovement, out needToChooseDirection);
                if (needToChooseDirection)
                    SetDestination();
                }

            return true;
            }

        private void SetDestination()
            {
            TilePos potentiallyMovingTowardsTile = TilePos.GetPositionAfterOneMove(this.TilePosition, this.Direction);
            this.MovingTowards = potentiallyMovingTowardsTile.ToPosition();
            }

        public void Reverse()
            {
            if (this.HasRebounded)
                throw new InvalidOperationException();
            
            this.Direction = this.Direction.Reversed();
            this.World.Game.SoundLibrary.Play(GameSound.ShotBounces);
            this.HasRebounded = true;
            ResetDistanceToTravel();
            }
        }
    }
