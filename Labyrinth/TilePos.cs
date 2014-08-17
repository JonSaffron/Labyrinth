using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public struct TilePos
        {
        public readonly int X;
        public readonly int Y;
        
        public TilePos(int x, int y)
            {
            this.X = x;
            this.Y = y;
            }
        
        public static TilePos TilePosFromPosition(Vector2 position)
            {
            var x = (int)Math.Floor(position.X / Tile.Width);
            var y = (int)Math.Floor(position.Y / Tile.Height);
            return new TilePos(x, y);
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
            var r = new Rectangle(this.X * Tile.Width, this.Y * Tile.Height, Tile.Width, Tile.Height);
            var result = r.GetCentre();
            return result;
            }
            
        public override string ToString()
            {
            return string.Format("({0}, {1})", this.X, this.Y);
            }
        }
    }
