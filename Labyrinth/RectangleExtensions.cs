using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    /// <summary>
    /// A set of helpful methods for working with rectangles.
    /// </summary>
    public static class RectangleExtensions
        {
        public static Rectangle NewRectangle(int x, int y, int right, int bottom)
            {
            return new Rectangle(x, y, right - x + 1, bottom - y + 1);
            }
        
        public static IEnumerable<TilePos> PointsInside(this Rectangle rect)
            {
            var result = new List<TilePos>();
            for (int y = rect.Y; y < rect.Bottom; y++)
                for (int x = rect.X; x < rect.Right; x++)
                    result.Add(new TilePos(x, y));
            return result;
            }
        
        public static bool ContainsTile(this Rectangle rect, TilePos tp)
            {
            var p = new Point(tp.X, tp.Y);
            var result = rect.Contains(p);
            return result;
            }

        public static bool ContainsPosition(this Rectangle rect, Vector2 point)
            {
            var p = new Point((int) point.X, (int) point.Y);
            var result = rect.Contains(p);
            return result;
            }
        }
    }
