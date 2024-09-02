using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Grave : StaticItem
        {
        private readonly StaticAnimation _animationPlayer;

        public Grave(Vector2 position) : base(position)
            {
            this.Energy = 255;
            this._animationPlayer = new StaticAnimation(this, "Sprites/Props/Grave");
            this.Properties.Set(GameObjectProperties.DrawOrder, SpriteDrawOrder.StaticItem);
            }

        public override IRenderAnimation RenderAnimation => this._animationPlayer;
        }
    }
