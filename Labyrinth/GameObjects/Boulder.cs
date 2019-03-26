using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Boulder : MovingItem
        {
        // Animations
        private readonly Animation _staticImage = Animation.StaticAnimation("Sprites/Boulder/Boulder");
        private readonly StaticAnimation _animationPlayer;
        
        /// <summary>
        /// Constructs a new boulder.
        /// </summary>
        public Boulder(Vector2 position) : base(position)
            {
            this._animationPlayer = new StaticAnimation(this);
            this._animationPlayer.PlayAnimation(_staticImage);
            this.MovementBoundary = GlobalServices.BoundMovementFactory.GetWorldBoundary();
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.Boulder);
            this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Moveable);
            }
        
        public override bool IsExtant => true;

        public override IRenderAnimation RenderAnimation => this._animationPlayer;

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            var elapsed = gameTime.ElapsedGameTime.TotalSeconds;

            bool result = false;
            if (this.CurrentMovement.IsMoving)
                {
                this.TryToCompleteMoveToTarget(ref elapsed);
                result = true;
                }
            return result;
            }
        }
    }
