using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Boulder : MovingItem
        {
        // Animations
        private Animation _staticImage;
        
        /// <summary>
        /// Constructs a new boulder.
        /// </summary>
        public Boulder(AnimationPlayer animationPlayer, Vector2 position) : base(animationPlayer, position)
            {
            LoadContent();
            Ap.PlayAnimation(_staticImage);
            this.MovementBoundary = GlobalServices.BoundMovementFactory.GetWorldBoundary();
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.Boulder);
            this.Properties.Set(GameObjectProperties.Solidity, ObjectSolidity.Moveable);
            }
        
        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        private void LoadContent()
            {
            // Load animated textures.
            _staticImage = Animation.StaticAnimation("Sprites/Boulder/Boulder");
            }

        public override bool IsExtant => true;

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
