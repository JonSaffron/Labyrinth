using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    class Shot : MovingItem
        {
        // Movement
        public ShotType ShotType { get; private set; }
        public bool HasRebounded {get; private set; }

        private readonly Animation _staticImage;
        private float _distanceToTravel;

        // Constants for controlling movement
        private const float StandardSpeed = AnimationPlayer.BaseSpeed * 4;

        public Shot(World world, Vector2 position, Direction d, int energy, ShotType shotType) : base(world, position)
            {
            this.Direction = d;
            this.Energy = energy;
            this.ShotType = shotType;
            this.CurrentVelocity = StandardSpeed;

            string file;
            switch (this.ShotType)
                {
                case ShotType.Player:
                    file = "Sprites/Shot/RedShot";
                    break;
                case ShotType.Monster: 
                    file = "Sprites/Shot/GreenShot";
                    break;
                default:
                    throw new InvalidOperationException();
                }
            
            var texture = this.World.Content.Load<Texture2D>(file);
            this._staticImage = Animation.StaticAnimation(texture);
            Ap.PlayAnimation(_staticImage);
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
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public override Rectangle BoundingRectangle
            {
            get
                {
                int w, x, h, y;
                
                switch (this.Direction)
                    {
                    case Direction.Up:
                        w = this._staticImage.FrameWidth / 8;
                        x = (this._staticImage.FrameWidth / 2) - (w / 2);
                        h = this._staticImage.FrameHeight / 2;
                        y = 0;
                        break;
                    
                    case Direction.Down:
                        w = this._staticImage.FrameWidth / 8;
                        x = (this._staticImage.FrameWidth / 2) - (w / 2);
                        h = this._staticImage.FrameHeight / 2;
                        y = this._staticImage.FrameHeight / 2;
                        break;
                    
                    case Direction.Left:
                        w = this._staticImage.FrameWidth / 2;
                        x = 0;
                        h = this._staticImage.FrameHeight / 8;
                        y = (this._staticImage.FrameHeight / 2) - (h / 2);
                        break;
                    
                    case Direction.Right:
                        w = this._staticImage.FrameWidth / 2;
                        x = this._staticImage.FrameWidth / 2;
                        h = this._staticImage.FrameHeight / 8;
                        y = (this._staticImage.FrameHeight / 2) - (h / 2);
                        break;
                    
                    default:
                        throw new InvalidOperationException();
                    }
                        
                int left = (int)Math.Round(Position.X - Ap.Origin.X) + x;
                int top = (int)Math.Round(Position.Y - Ap.Origin.Y) + y;
                return new Rectangle(left, top, w, h);
                }
            }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var distance = this.CurrentVelocity * elapsed;
            Position += Animation.GetBaseVectorForDirection(this.Direction) * distance;
            this._distanceToTravel -= distance;
            if (this._distanceToTravel <= 0)
                this.Energy = 0;
            return true;
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

        public virtual void InstantDeath()
            {
            if (!this.IsExtant)
                return;
            
            this.Energy = 0;
            }
        }
    }
