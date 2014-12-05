using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class Boulder : MovingItem
        {
        // Animations
        private Animation _staticImage;
        
        // Constants for controlling movement
        private const float StandardSpeed = AnimationPlayer.BaseSpeed * 2.5f;
        
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
            _staticImage = Animation.StaticAnimation(World, "Sprites/Boulder/Boulder");
            }

        public void Reset(Vector2 position)
            {
            this.Position = position;
            this.Direction = Direction.None;
            }

        public override bool IsExtant
            {
            get
                {
                return true;
                }
            }

        //[Obsolete]
        //public PushStatus CanBePushed(Direction d)
        //    {
        //    TilePos tp = TilePos.TilePosFromPosition(Position);
        //    bool canMoveForwards = World.IsTileUnoccupied(TilePos.GetPositionAfterOneMove(tp, d), false);
        //    if (canMoveForwards)
        //        return PushStatus.Yes;

        //    Direction oppositeDirection = d.Reversed();
        //    TilePos playersPositionAfterBouncing = TilePos.GetPositionAfterOneMove(TilePos.GetPositionAfterOneMove(tp, oppositeDirection), oppositeDirection);
        //    bool canMoveBackwards = World.IsTileUnoccupied(playersPositionAfterBouncing, false);
        //    var result = canMoveBackwards ? PushStatus.Bounce : PushStatus.No;
        //    return result;
        //    }
        
        /// <summary>
        /// Used to begin a push action when the boulder is shot
        /// </summary>
        /// <param name="direction">Which direction the boulder should move</param>
        public void Push(Direction direction)
            {
            var ps = CanBePushed(direction, false);
            switch (ps)
                {
                case PushStatus.No:
                    return;

                case PushStatus.Yes:
                    {
                    this.Direction = direction;
                    this.MovingTowards = this.TilePosition.GetPositionAfterOneMove(direction).ToPosition();
                    this.CurrentVelocity = StandardSpeed;
                    return;
                    }

                default:
                    throw new InvalidOperationException();
                }
            }

        /// <summary>
        /// Used to begin a push or bounce action by the player
        /// </summary>
        /// <param name="byWhom">The game object that is acting on the boulder</param>
        /// <param name="direction">Which direction the specified game object is directing the boulder</param>
        public void PushOrBounce(MovingItem byWhom, Direction direction)
            {
            var ps = CanBePushedOrBounced(byWhom, direction);
            switch (ps)
                {
                case PushStatus.No:
                    return;

                case PushStatus.Yes:
                    {
                    this.Direction = direction;
                    this.MovingTowards = this.TilePosition.GetPositionAfterOneMove(direction).ToPosition();
                    this.CurrentVelocity = StandardSpeed;
                    return;
                    }

                case PushStatus.Bounce:
                    {
                    this.Direction = direction.Reversed();
                    this.MovingTowards = this.TilePosition.GetPositionAfterOneMove(this.Direction).ToPosition();
                    this.CurrentVelocity = AnimationPlayer.BounceBackSpeed;

                    // todo this._player.BounceBack(this._player.Direction.Reversed(), gameTime);
                    byWhom.BounceBack(this.Direction);
                    this.World.Game.SoundLibrary.Play(GameSound.BoulderBounces);
                    return;
                    }

                default:
                    throw new InvalidOperationException();
                }
            }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool isMoving = this.Position != this.MovingTowards;
            if (isMoving)
                {
                float remainingMovement = (CurrentVelocity * elapsed);
                bool hasArrivedAtDestination;
                ContinueMove(ref remainingMovement, out hasArrivedAtDestination);
                if (hasArrivedAtDestination)
                    this.Direction = Direction.None;
                }
            return isMoving;
            }

        public override ObjectSolidity Solidity
            {
            get
                {
                return ObjectSolidity.Moveable;
                }
            }
        }
    }
