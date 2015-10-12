using System;
using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth
    {
    public interface ISpriteLibrary : IDisposable
        {
        Texture2D GetSprite(string name);
        }
    }
