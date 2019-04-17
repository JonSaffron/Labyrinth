using Labyrinth.Services.Display;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    public class Potion : StaticItem
        {
        private readonly StaticAnimation _animationPlayer;

        public Potion(Vector2 position) : base(position)
            {
            this._animationPlayer = new StaticAnimation(this, "Sprites/Flask");
            }

        public override bool IsExtant => true;

        public override IRenderAnimation RenderAnimation => this._animationPlayer;

        public override bool Update(GameTime gameTime)
            {
            //this._animationPlayer.Update(gameTime);
            return false;
            }
        }
    }
