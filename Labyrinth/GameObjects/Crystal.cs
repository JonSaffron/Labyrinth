using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Crystal : StaticItem, IValuable
        {
        public int CrystalId { get; }
        public decimal Score { get; }
        private bool _isTaken;

        public Crystal(AnimationPlayer animationPlayer, Vector2 position, int id, int score, int energy) : base(animationPlayer, position)
            {
            this.CrystalId = id;
            this.Score = score;
            this.Energy = energy;

            var a = Animation.LoopingAnimation("Sprites/Crystal/Crystal", 4);
            this.Ap.PlayAnimation(a);
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
        }
    }
