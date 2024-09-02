using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public static class Extensions
        {
        public static Direction Reversed(this Direction d)
            {
            if (!Enum.IsDefined(typeof(Direction), d))
                throw new InvalidEnumArgumentException(nameof(d), (int)d, typeof(Direction));
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
            if (!Enum.IsDefined(typeof(Direction), d))
                throw new InvalidEnumArgumentException(nameof(d), (int)d, typeof(Direction));
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
            if (!Enum.IsDefined(typeof(Direction), d))
                throw new InvalidEnumArgumentException(nameof(d), (int)d, typeof(Direction));
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
            if (!Enum.IsDefined(typeof(ObjectCapability), oc))
                throw new InvalidEnumArgumentException(nameof(oc), (int)oc, typeof(ObjectCapability));
            switch (oc)
                {
                case ObjectCapability.CanPushOthers:
                case ObjectCapability.CanPushOrCauseBounceBack:
                    return true;
                }
            return false;
            }

        public static bool WillReplenish(this FruitPopulationMethod populationMethod)
            {
            if (!Enum.IsDefined(typeof(FruitPopulationMethod), populationMethod))
                throw new InvalidEnumArgumentException(nameof(populationMethod), (int)populationMethod,
                    typeof(FruitPopulationMethod));
            switch (populationMethod)
                {
                case FruitPopulationMethod.GradualPopulation:
                case FruitPopulationMethod.InitialPopulationWithReplenishment:
                    return true;
                }
            return false;
            }

        public static bool CanChangeRooms(this ChangeRooms changeRooms)
            {
            if (!Enum.IsDefined(typeof(ChangeRooms), changeRooms))
                throw new InvalidEnumArgumentException(nameof(changeRooms), (int)changeRooms, typeof(ChangeRooms));
            if (changeRooms == ChangeRooms.FollowsPlayer || changeRooms == ChangeRooms.MovesRoom)
                return true;
            return false;
            }

        internal static bool IsAlive(this IMovingItem mi)
            {
            if (mi == null) throw new ArgumentNullException(nameof(mi));
            var result = mi.IsExtant;
            return result;
            }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T: class
            {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            // ReSharper disable once LoopCanBeConvertedToQuery (leads to slow delegate)
            foreach (T? item in source)
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
        public static T? GetAttributeOfType<T>(this Enum enumVal) where T: Attribute
            {
            if (enumVal == null) throw new ArgumentNullException(nameof(enumVal));
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            var result = (attributes.Length > 0) ? (T)attributes[0] : null;
            return result;
            }

        /// <summary>
        /// Plays a sound which is centred on this instance
        /// </summary>
        /// <param name="gameObject">The GameObject to attach the sound to</param>
        /// <param name="gameSound">Sets which sound to play</param>
        public static void PlaySound(this IGameObject gameObject, GameSound gameSound)
            {
            if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));
            GlobalServices.SoundPlayer.PlayForObject(gameSound, gameObject, GlobalServices.CentrePointProvider);
            }

        /// <summary>
        /// Plays a sound which is centred on this instance and triggers a specified callback when the sound completes
        /// </summary>
        /// <param name="gameObject">The GameObject to attach the sound to</param>
        /// <param name="gameSound">Sets which sound to play</param>
        /// <param name="callback">The routine to call when the sound finishes playing</param>
        public static void PlaySoundWithCallback(this IGameObject gameObject, GameSound gameSound, EventHandler callback)
            {
            if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));
            GlobalServices.SoundPlayer.PlayForObjectWithCallback(gameSound, gameObject, GlobalServices.CentrePointProvider, callback);
            }

        public static bool CanMoveInDirection(this IMovingItem gameObject, Direction direction)
            {
            if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));
            IMovementChecker mc = gameObject.Properties.Get(GameObjectProperties.MovementChecker);
            var result = mc.CanMoveForwards(gameObject, direction);
            return result;
            }

        public static XmlNodeList SelectNodesEx(this XmlElement xmlElement, string xpath, XmlNamespaceManager namespaceManager)
            {
            if (xmlElement == null) throw new ArgumentNullException(nameof(xmlElement));
            if (xpath == null) throw new ArgumentNullException(nameof(xpath));
            if (namespaceManager == null) throw new ArgumentNullException(nameof(namespaceManager));
            return xmlElement.SelectNodes(xpath, namespaceManager)!;
            }
        }
    }
