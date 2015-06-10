using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public struct TilePos
        {
        public readonly int X;
        public readonly int Y;
        private const int HalfTileWidth = Tile.Width / 2;
        private const int HalfTileHeight = Tile.Height / 2;
        
        public TilePos(int x, int y)
            {
            this.X = x;
            this.Y = y;
            }
        
        /// <summary>
        /// Returns a TilePos object corresponding to the specified co-ordinate
        /// </summary>
        /// <param name="position">The position within the world. For a GameObject, pass in the co-ordinates of the object's origin.</param>
        /// <returns>A TilePos that contains the specified co-ordinate</returns>
        public static TilePos TilePosFromPosition(Vector2 position)
            {
            var intx = (int)position.X / Tile.Width;
            var inty = (int)position.Y / Tile.Height;

            var result = new TilePos(intx, inty);
            return result;
            }
        
        public static bool operator ==(TilePos first, TilePos second)
            {
            return (first.X == second.X) && (first.Y == second.Y);
            }
            
        public static bool operator !=(TilePos first, TilePos second)
            {
            return (first.X != second.X) || (first.Y != second.Y);
            }
        
        public override bool Equals(object obj)
            {
            if (!(obj is TilePos))
                {
                return false;
                }
            var tp = (TilePos)obj;
            return tp == this;
            }

        public override int GetHashCode()
            {
            return this.X % 1000 + (this.Y % 1000) * 1000;
            }
        
        internal TilePos GetPositionAfterOneMove(Direction d)
            {
            switch (d)
                {
                case Direction.Left:
                    return new TilePos(X - 1, Y);
                case Direction.Right:
                    return new TilePos(X + 1, Y);
                case Direction.Up:
                    return new TilePos(X, Y - 1);
                case Direction.Down:
                    return new TilePos(X, Y + 1);
                default:
                    throw new InvalidOperationException();
                }
            }

#warning does this need to be refactored? not every object will have its origin in the middle of tile.
        public Vector2 ToPosition()
            {
            int x = this.X * Tile.Width + HalfTileWidth;
            int y = this.Y * Tile.Height + HalfTileHeight;
            var result = new Vector2(x, y);
            return result;
            }
            
        public Rectangle ToRectangle()
            {
            var r = new Rectangle(this.X * Tile.Width, this.Y * Tile.Height, Tile.Width, Tile.Height);
            return r;
            }

        public int MortonCode
            {
            get
                {
                var result = (Part1By1(this.Y) << 1) + Part1By1(this.X);
                return result;
                }
            }

        private static int Part1By1(int x)
            {
            x &= 0x0000ffff;                  // x = ---- ---- ---- ---- fedc ba98 7654 3210
            x = (x ^ (x <<  8)) & 0x00ff00ff; // x = ---- ---- fedc ba98 ---- ---- 7654 3210
            x = (x ^ (x <<  4)) & 0x0f0f0f0f; // x = ---- fedc ---- ba98 ---- 7654 ---- 3210
            x = (x ^ (x <<  2)) & 0x33333333; // x = --fe --dc --ba --98 --76 --54 --32 --10
            x = (x ^ (x <<  1)) & 0x55555555; // x = -f-e -d-c -b-a -9-8 -7-6 -5-4 -3-2 -1-0
            return x;
            }

        public static TilePos FromMortonCode(int mortonCode)
            {
            if (mortonCode < 0)
                throw new ArgumentOutOfRangeException("mortonCode");

            int x = Compact1By1(mortonCode >> 0);
            int y = Compact1By1(mortonCode >> 1);
            var result = new TilePos(x, y);
            return result;
            }

        private static int Compact1By1(int x)
            {
            x &= 0x55555555;                  // x = -f-e -d-c -b-a -9-8 -7-6 -5-4 -3-2 -1-0
            x = (x ^ (x >>  1)) & 0x33333333; // x = --fe --dc --ba --98 --76 --54 --32 --10
            x = (x ^ (x >>  2)) & 0x0f0f0f0f; // x = ---- fedc ---- ba98 ---- 7654 ---- 3210
            x = (x ^ (x >>  4)) & 0x00ff00ff; // x = ---- ---- fedc ba98 ---- ---- 7654 3210
            x = (x ^ (x >>  8)) & 0x0000ffff; // x = ---- ---- ---- ---- fedc ba98 7654 3210
            return x;
            }

        public override string ToString()
            {
            return string.Format("({0}, {1})", this.X, this.Y);
            }
        }
    }
