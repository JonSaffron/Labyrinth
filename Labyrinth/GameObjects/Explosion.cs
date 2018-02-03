using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class Explosion : MovingItem, IShot
        {
        private bool _isExtant;
        private readonly IGameObject _originator;

        public Explosion(AnimationPlayer animationPlayer, Vector2 position, int energy, IGameObject originator) : base(animationPlayer, position)
            {
            this.Energy = energy;
            var a = Animation.ManualAnimation("Sprites/Props/LongBang", 2);
            Ap.PlayAnimation(a);
            this._isExtant = true;
            this._originator = originator;
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

        public IGameObject Originator => this._originator;

        public bool HasRebounded => false;
        }
    }
