using System;
using System.Collections.Generic;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public static class Extensions
        {
        public static Direction Reversed(this Direction d)
            {
            switch (d)
                {
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                default: throw new InvalidOperationException();
                }
            }
        
        internal static Vector2 ToVector(this Direction d)
            {
            switch (d)
                {
                case Direction.Left:
                    return -(Vector2.UnitX);
                case Direction.Right:
                    return Vector2.UnitX;
                case Direction.Up:
                    return -(Vector2.UnitY);
                case Direction.Down:
                    return Vector2.UnitY;
                default:
                    throw new InvalidOperationException();
                }
            }

        internal static bool IsAlive(this MovingItem mi)
            {
            var result = mi.IsExtant;
            return result;
            }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source) where T: class
            {
            if (source == null)
                throw new ArgumentNullException("source");
            // ReSharper disable once LoopCanBeConvertedToQuery (leads to slow delegate)
            foreach (T item in source)
                {
                if (item != null)
                    yield return item;
                }
            }
        }
    }
