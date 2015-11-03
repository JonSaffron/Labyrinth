using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Labyrinth
    {
    public struct TileRect
        {
        public readonly TilePos TopLeft;
        public readonly int Width;
        public readonly int Height;

        public TileRect(TilePos topLeft, int width, int height)
            {
            this.TopLeft = topLeft;
            this.Width = width;
            this.Height = height;
            }
        }
    }
