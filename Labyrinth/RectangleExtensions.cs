﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    /// <summary>
    /// A set of helpful methods for working with rectangles.
    /// </summary>
    public static class RectangleExtensions
        {
        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
            {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            var centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            var centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
            }

        public static Vector2 GetCentre(this Rectangle rect)
            {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);
            }
            
        public static bool IncludesPoint(this Rectangle rect, Vector2 point)
            {
            return point.X >= rect.X && point.X <= rect.Right && point.Y >= rect.Y && point.Y <= rect.Bottom;
            }
        
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
        
        public static bool Contains(this Rectangle rect, TilePos tp)
            {
            var p = new Point(tp.X, tp.Y);
            return rect.Contains(p);
            }
        }
    }
