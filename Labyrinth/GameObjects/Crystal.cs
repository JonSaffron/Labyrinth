using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Crystal : StaticItem, IValuable
        {
        public int CrystalId { get; private set; }
        public decimal Score { get; private set; }
        private bool _isTaken;

        public Crystal(World world, Vector2 position, int id, decimal score, int energy) : base(world, position)
            {
            this.CrystalId = id;
            this.Score = score;
            this.Energy = energy;

            var a = Animation.LoopingAnimation("Sprites/Crystal/Crystal", 4);
            this.Ap.PlayAnimation(a);
            }

        public override bool IsExtant
            {
            get
                {
                var result = base.IsExtant && !this._isTaken;
                return result;
                }
            }

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.StaticItem;
                }
            }

        public void SetTaken()
            {
            this._isTaken = true;
            }
        }
    }
