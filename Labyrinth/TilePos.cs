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
        
        internal static TilePos GetPositionAfterOneMove(TilePos tp, Direction d)
            {
            switch (d)
                {
                case Direction.Left:
                    return new TilePos(tp.X - 1, tp.Y);
                case Direction.Right:
                    return new TilePos(tp.X + 1, tp.Y);
                case Direction.Up:
                    return new TilePos(tp.X, tp.Y - 1);
                case Direction.Down:
                    return new TilePos(tp.X, tp.Y + 1);
                default:
                    throw new InvalidOperationException();
                }
            }

        public Vector2 ToPosition()
            {
            int x = this.X * Tile.Width + HalfTileWidth;
            int y = this.Y * Tile.Height + HalfTileHeight;
            var result = new Vector2(x, y);
            return result;
            }
            
        public override string ToString()
            {
            return string.Format("({0}, {1})", this.X, this.Y);
            }
        }
    }
