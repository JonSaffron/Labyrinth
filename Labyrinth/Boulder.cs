using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    class Boulder : MovingItem
        {
        // Movement
        //public Vector2 MovingTowards { get; private set; }
        
        // Animations
        private Animation _staticImage;
        
        // Constants for controlling movement
        private const float StandardSpeed = AnimationPlayer.BaseSpeed * 2.5f;
        private const float BounceBackSpeed = AnimationPlayer.BaseSpeed * 3;
        
        /// <summary>
        /// Constructs a new block.
        /// </summary>
        public Boulder(World world, Vector2 position) : base(world, position)
            {
            LoadContent();
            Direction = Direction.None;
            Ap.PlayAnimation(_staticImage);
            }
        
        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        private void LoadContent()
            {
            // Load animated textures.
            _staticImage = Animation.StaticAnimation(World.Content.Load<Texture2D>("Sprites/Boulder/Boulder"));
            }

        public override bool IsExtant
            {
            get
                {
                return true;
                }
            }

        public override TouchResult OnTouched(Player p)
            {
            //PushStatus ps = this.CanBePushed(p.Direction);
            //if (ps == PushStatus.No)
            //    return TouchResult.NoEffect;
            
            //Vector2 difference = p.Position - this.Position;
            //float distanceApart = Math.Abs(difference.Length());
            //if (distanceApart >= 40)
            //    return TouchResult.NoEffect;

            //switch (ps)
            //    {
            //    case PushStatus.Yes:
            //        this.Push(p.Direction, false);
            //        return TouchResult.NoEffect;

            //    case PushStatus.Bounce:
            //        this.BounceBack(p.Direction);
            //        return TouchResult.BounceBack;

            //    default:
            //        throw new InvalidOperationException();
            //    }
            return TouchResult.NoEffect;
            }

        public PushStatus CanBePushed(Direction d)
            {
            TilePos tp = TilePos.TilePosFromPosition(Position);
            bool canMoveForwards = World.IsTileUnoccupied(TilePos.GetPositionAfterOneMove(tp, d), false);
            if (canMoveForwards)
                return PushStatus.Yes;

            Direction oppositeDirection = d.Reversed();
            TilePos playersPositionAfterBouncing = TilePos.GetPositionAfterOneMove(TilePos.GetPositionAfterOneMove(tp, oppositeDirection), oppositeDirection);
            bool canMoveBackwards = World.IsTileUnoccupied(playersPositionAfterBouncing, false);
            var result = canMoveBackwards ? PushStatus.Bounce : PushStatus.No;
            return result;
            }
        
        public void Push(Direction d, bool isBounceBack)
            {
            TilePos tp = TilePos.TilePosFromPosition(this.Position);
            TilePos potentiallyMovingTowardsTile = TilePos.GetPositionAfterOneMove(tp, d);
            if (World.IsTileUnoccupied(potentiallyMovingTowardsTile, false))
                {
                this.Direction = d;
                this.MovingTowards = potentiallyMovingTowardsTile.ToPosition();
                this.CurrentVelocity = isBounceBack ? BounceBackSpeed : StandardSpeed;
                }
            }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public override void Update(GameTime gameTime)
            {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.Direction != Direction.None)
                {   // currently in motion
                switch (this.Direction)
                    {
                    case Direction.Left:
                        Position += new Vector2(-CurrentVelocity, 0) * elapsed;
                        if (Position.X < MovingTowards.X)
                            {
                            Position = new Vector2(MovingTowards.X, Position.Y);
                            this.Direction = Direction.None;
                            }
                        break;
                    case Direction.Right:
                        Position += new Vector2(CurrentVelocity, 0) * elapsed;
                        if (Position.X > MovingTowards.X)
                            {
                            Position = new Vector2(MovingTowards.X, Position.Y);
                            this.Direction = Direction.None;
                            }
                        break;
                    case Direction.Up:
                        Position += new Vector2(0, -CurrentVelocity) * elapsed;
                        if (Position.Y < MovingTowards.Y)
                            {
                            Position = new Vector2(Position.X, MovingTowards.Y);
                            this.Direction = Direction.None;
                            }
                        break;
                    case Direction.Down:
                        Position += new Vector2(0, CurrentVelocity) * elapsed;
                        if (Position.Y > MovingTowards.Y)
                            {
                            Position = new Vector2(Position.X, MovingTowards.Y);
                            this.Direction = Direction.None;
                            }
                        break;
                    }
                }
            }
        }
    }
