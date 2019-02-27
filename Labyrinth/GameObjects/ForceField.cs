using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class ForceField : StaticItem
        {
        private readonly int _crystalRequired;

        public ForceField(AnimationPlayer animationPlayer, Vector2 position, int crystalRequired) : base(animationPlayer, position)
            {
            this._crystalRequired = crystalRequired;

            var a = Animation.LoopingAnimation("Sprites/Props/ForceField", 3);
            this.Ap.PlayAnimation(a);
            this.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Reflection);
            }

        public override bool IsExtant
            {
            get
                {
                bool result = !GlobalServices.GameState.Player.HasPlayerGotCrystal(this._crystalRequired);
                return result;
                }
            }

        public override int DrawOrder => (int) SpriteDrawOrder.ForceField;
        }
    }
