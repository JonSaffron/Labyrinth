using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            var texture = base.World.Content.Load<Texture2D>("Sprites/Crystal/Crystal");
            var a = Animation.LoopingAnimation(texture, 4);
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

        public override TouchResult OnTouched(Player p)
            {
            if (this.World.HowManyCrystalsRemain() > 1)
                this.World.Game.SoundLibrary.Play(GameSound.PlayerCollectsCrystal);
            p.AddEnergy(Energy);
            base.World.IncreaseScore(Score);
            p.CrystalCollected(this);
            return TouchResult.RemoveObject;
            }

        public override ShotStatus OnShot(Shot s)
            {
            return ShotStatus.CarryOn;
            }
        }
    }
