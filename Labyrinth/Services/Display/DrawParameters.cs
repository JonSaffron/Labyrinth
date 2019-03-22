using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.Display
    {
    public struct DrawParameters
        {
        Texture2D Texture;
        Vector2 Position;
        Rectangle AreaWithinTexture;
        float Rotation;
        Vector2 Centre;
        SpriteEffects Effects;
        int DrawOrder;  // spritebatch depth has 0 at the front to 1 at the back
        }
    }
