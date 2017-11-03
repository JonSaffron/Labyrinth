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

        public static Orientation Orientation(this Direction d)
            {
            switch (d)
                {
                case Direction.Left: 
                case Direction.Right:
                    return Labyrinth.Orientation.Horizontal;
                case Direction.Up:
                case Direction.Down:
                    return Labyrinth.Orientation.Vertical;
                default:
                    return Labyrinth.Orientation.None;
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

        public static bool CanMoveAnother(this ObjectCapability oc)
            {
            switch (oc)
                {
                case ObjectCapability.CanPushOthers:
                case ObjectCapability.CanPushOrCauseBounceBack:
                    return true;
                }
            return false;
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

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>string desc = myEnumVariable.GetAttributeOfType&lt;DescriptionAttribute&gt;().Description;</example>
        public static T GetAttributeOfType<T>(this Enum enumVal) where T:Attribute
            {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            var result = (attributes.Length > 0) ? (T)attributes[0] : null;
            return result;
            }
        }
    }
