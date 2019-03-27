using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Crystal : StaticItem, IValuable
        {
        public int CrystalId { get; }
        public decimal Score { get; }
        private bool _isTaken;
        private readonly LoopedAnimation _animationPlayer;

        public Crystal(Vector2 position, int id, int score, int energy) : base(position)
            {
            this.CrystalId = id;
            this.Score = score;
            this.Energy = energy;
            this._animationPlayer = new LoopedAnimation(this, "Sprites/Crystal/Crystal", 8);
            this.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Intangible);
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.StaticItem);
            }

        public override bool IsExtant
            {
            get
                {
                var result = base.IsExtant && !this._isTaken;
                return result;
                }
            }

        public void SetTaken()
            {
            this._isTaken = true;
            }

        public override IRenderAnimation RenderAnimation => this._animationPlayer;

        public override bool Update(GameTime gameTime)
            {
            this._animationPlayer.Update(gameTime);
            return false;
            }
        }
    }
