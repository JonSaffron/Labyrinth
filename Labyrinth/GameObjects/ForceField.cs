using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class ForceField : StaticItem
        {
        private readonly int _crystalRequired;
        private readonly AnimationPlayer _animationPlayer;

        public ForceField(Vector2 position, int crystalRequired) : base(position)
            {
            this._crystalRequired = crystalRequired;
            this._animationPlayer = new AnimationPlayer(this);

            var a = Animation.LoopingAnimation("Sprites/Props/ForceField", 6);
            this._animationPlayer.PlayAnimation(a);
            this.Properties.Set(GameObjectProperties.EffectOfShot, EffectOfShot.Reflection);
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.ForceField);
            }

        public override bool IsExtant
            {
            get
                {
                bool result = !GlobalServices.GameState.Player.HasPlayerGotCrystal(this._crystalRequired);
                return result;
                }
            }

        public override IRenderAnimation RenderAnimation => this._animationPlayer;

        public override bool Update(GameTime gameTime)
            {
            this._animationPlayer.Update(gameTime);
            return false;
            }
        }
    }
