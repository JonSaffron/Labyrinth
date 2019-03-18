using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Labyrinth.DataStructures;

namespace Labyrinth
    {
    /// <summary>
    /// A set of helpful methods for working with rectangles.
    /// </summary>
    public static class RectangleExtensions
        {
        public static IEnumerable<TilePos> PointsInside(this Rectangle rect)
            {
            for (int y = rect.Y; y < rect.Bottom; y++)
                {
                for (int x = rect.X; x < rect.Right; x++)
                    {
                    var item = new TilePos(x, y);
                    yield return item;
                    }
                }
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

        public static Rectangle GetRectangleFromDefinition(XmlElement area)
            {
            int x = int.Parse(area.GetAttribute("Left"));
            int y = int.Parse(area.GetAttribute("Top"));
            int width = int.Parse(area.GetAttribute("Width"));
            int height = int.Parse(area.GetAttribute("Height"));
            Rectangle result = new Rectangle(x, y, width, height);
            return result;
            }
        }
    }
