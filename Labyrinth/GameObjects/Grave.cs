using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Grave : StaticItem
        {
        private readonly AnimationPlayer _animationPlayer;

        public Grave(Vector2 position) : base(position)
            {
            this.Energy = 255;
            this._animationPlayer = new AnimationPlayer(this);

            var a = Animation.StaticAnimation("Sprites/Props/Grave");
            this._animationPlayer.PlayAnimation(a);
            this.Properties.Set(GameObjectProperties.DrawOrder, (int) SpriteDrawOrder.StaticItem);
            }

        public override IRenderAnimation RenderAnimation => this._animationPlayer;
        }
    }
