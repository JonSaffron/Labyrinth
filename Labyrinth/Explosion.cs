using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class Explosion : Shot
        {
        private bool _isExtant;

        public Explosion(World world, Vector2 position, int energy) : base(world, position)
            {
            this.Energy = energy;
            var a = Animation.SingleRunAnimation(World, "Sprites/Props/LongBang", 2);
            Ap.PlayAnimation(a);
            Ap.AnimationFinished += AnimationFinished;

            this._isExtant = true;
            }

        private void AnimationFinished(object sender, EventArgs e)
            {
            this._isExtant = false;
            }

        public override bool IsExtant
            {
            get
                {
                return this._isExtant;
                }
            }

        public override void ReduceEnergy(int energyToRemove)
            {
            // nothing to do
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.Bang;
                }
            }

        public override bool Update(GameTime gameTime)
            {
            return false;
            }
        }
    }
