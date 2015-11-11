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
            Direction = Direction.None;
            Ap.PlayAnimation(_staticImage);
            }
        
        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        private void LoadContent()
            {
            // Load animated textures.
            _staticImage = Animation.StaticAnimation("Sprites/Boulder/Boulder");
            }

        public void Reset(Vector2 position)
            {
            this.Position = position;
            this.MovingTowards = position;
            this.Direction = Direction.None;
            }

        public override bool IsExtant
            {
            get
                {
                return true;
                }
            }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public override bool Update(GameTime gameTime)
            {
            var elapsed = gameTime.ElapsedGameTime.TotalSeconds;

            bool result = false;
            if (this.IsMoving)
                {
                this.TryToCompleteMoveToTarget(ref elapsed);
                result = true;
                }
            return result;
            }

        public override ObjectSolidity Solidity
            {
            get
                {
                return ObjectSolidity.Moveable;
                }
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.Boulder;
                }
            }

        protected override decimal StandardSpeed
            {
            get
                {
                return Constants.BaseSpeed * 2.5m;
                }
            }
        }
    }
