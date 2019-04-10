using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Explosion : MovingItem, IMunition
        {
        private bool _isExtant;
        public IGameObject Originator { get; }  // the mine that caused the explosion
        private readonly LinearAnimation _animationPlayer;

        public Explosion(Vector2 position, int energy, IGameObject originator) : base(position)
            {
            this.Energy = energy;
            this._animationPlayer = new LinearAnimation(this, "Sprites/Props/LongBang", 8);
            this._isExtant = true;
            this.Originator = originator;
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.Bang);
            }

        public override bool IsExtant => base.IsExtant && this._isExtant;

        public override void ReduceEnergy(int energyToRemove)
            {
            // nothing to do
            }

        public override IRenderAnimation RenderAnimation => this._animationPlayer;

        public override bool Update(GameTime gameTime)
            {
            if (!this._animationPlayer.HasReachedEndOfAnimation)
                {
                this._animationPlayer.Update(gameTime);
                }
            else
                {
                this._isExtant = false;
                }
            return this._isExtant;
            }
        }
    }
