using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public interface IRenderAnimation
        {
        void Draw(ISpriteBatch spriteBatch, ISpriteLibrary spriteLibrary);
        }

    public interface IDynamicAnimation : IRenderAnimation
        {
        void Update(GameTime gameTime);
        }
    }
