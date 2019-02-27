using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Explosion : MovingItem, IMunition
        {
        private bool _isExtant;
        public IGameObject Originator { get; }  // the mine that caused the explosion

        public Explosion(AnimationPlayer animationPlayer, Vector2 position, int energy, IGameObject originator) : base(animationPlayer, position)
            {
            this.Energy = energy;
            var a = Animation.ManualAnimation("Sprites/Props/LongBang", 2);
            Ap.PlayAnimation(a);
            this._isExtant = true;
            this.Originator = originator;
            }

        public override bool IsExtant => base.IsExtant && this._isExtant;

        public override void ReduceEnergy(int energyToRemove)
            {
            // nothing to do
            }

        public override int DrawOrder => (int) SpriteDrawOrder.Bang;

        public override bool Update(GameTime gameTime)
            {
            if (!this.Ap.AdvanceManualAnimation(gameTime))
                this._isExtant = false;
            return this._isExtant;
            }
        }
    }
