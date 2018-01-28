namespace Labyrinth
    {
    public readonly struct TileRect
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

        public bool Contains(TilePos tp)
            {
            var result = 
                    this.TopLeft.X <= tp.X 
                &&  tp.X < this.TopLeft.X + this.Width
                &&  this.TopLeft.Y <= tp.Y
                &&  tp.Y < this.TopLeft.Y + this.Height;
            return result;
            }
        }
    }
