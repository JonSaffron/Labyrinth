using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class Crystal : StaticItem
        {
        public int CrystalId { get; private set; }
        public int Score { get; private set; }
        private bool _isTaken;

        public Crystal(World world, Vector2 position, int id, int score, int energy) : base(world, position)
            {
            this.CrystalId = id;
            this.Score = score;
            this.Energy = energy;

            var a = Animation.LoopingAnimation(World, "Sprites/Crystal/Crystal", 4);
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

        public void SetTaken()
            {
            this._isTaken = true;
            }
        }
    }
