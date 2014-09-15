using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    class ForceField : StaticItem
        {
        private readonly int _crystalRequired;

        public ForceField(World world, Vector2 position, int crystalRequired) : base(world, position)
            {
            this._crystalRequired = crystalRequired;

            var texture = base.World.Content.Load<Texture2D>("Sprites/Props/ForceField");
            var a = Animation.LoopingAnimation(texture, 3);
            this.Ap.PlayAnimation(a);
            }

        public override bool IsExtant
            {
            get
                {
                bool result = !this.World.Player.HasPlayerGotCrystal(this._crystalRequired);
                return result;
                }
            }
        }
    }
