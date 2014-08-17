using System;
using System.Collections.Generic;

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
        
        internal static bool IsAlive(this MovingItem mi)
            {
            var result = mi.IsExtant;
            return result;
            }

        public static void AddRange<T>(this LinkedList<T> list, IEnumerable<T> items)
            {
            foreach (var item in items)
                list.AddLast(item);
            } 
        }
    }
