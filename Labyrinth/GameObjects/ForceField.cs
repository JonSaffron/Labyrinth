using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    class ForceField : StaticItem
        {
        private readonly int _crystalRequired;

        public ForceField(World world, Vector2 position, int crystalRequired) : base(world, position)
            {
            this._crystalRequired = crystalRequired;

            var a = Animation.LoopingAnimation(World, "Sprites/Props/ForceField", 3);
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

        public override int DrawOrder
            {
            get
                {
                return (int) SpriteDrawOrder.ForceField;
                }
            }
        }
    }
