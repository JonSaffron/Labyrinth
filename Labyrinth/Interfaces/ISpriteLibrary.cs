using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public interface ISpriteLibrary
        {
        Texture2D GetSprite(string name);
        }
    }
