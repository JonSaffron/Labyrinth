using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
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

        public static bool WillReplenish(this FruitPopulationMethod populationMethod)
            {
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
            if (changeRooms == ChangeRooms.FollowsPlayer || changeRooms == ChangeRooms.MovesRoom)
                return true;
            return false;
            }

        public static bool IsEgg(this MonsterState monsterState)
            {
            switch (monsterState)
                {
                case MonsterState.Egg:
                case MonsterState.Hatching:
                    return true;
                }
            return false;
            }

        internal static bool IsAlive(this IMovingItem mi)
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

        /// <summary>
        /// Plays a sound which is centred on this instance
        /// </summary>
        /// <param name="gameObject">The gameobject to attach the sound to</param>
        /// <param name="gameSound">Sets which sound to play</param>
        public static void PlaySound(this IGameObject gameObject, GameSound gameSound)
            {
            GlobalServices.SoundPlayer.PlayForObject(gameSound, gameObject, GlobalServices.CentrePointProvider);
            }

        /// <summary>
        /// Plays a sound which is centred on this instance and triggers a specified callback when the sound completes
        /// </summary>
        /// <param name="gameObject">The gameobject to attach the sound to</param>
        /// <param name="gameSound">Sets which sound to play</param>
        /// <param name="callback">The routine to call when the sound finishes playing</param>
        public static void PlaySoundWithCallback(this IGameObject gameObject, GameSound gameSound, EventHandler callback)
            {
            GlobalServices.SoundPlayer.PlayForObjectWithCallback(gameSound, gameObject, GlobalServices.CentrePointProvider, callback);
            }

        public static bool CanMoveInDirection(this IMovingItem gameObject, Direction direction)
            {
            var mc = new MovementChecker(gameObject);
            var result = mc.CanMove(direction);
            return result;
            }

        public static void PushOrBounce(this IMovingItem objectDoingThePushing, IMovingItem objectBeingPushed, Direction direction)
            {
            var moa = new MovementOfAnother(objectDoingThePushing);
            moa.PushOrBounce(objectBeingPushed, direction);
            }

        [NotNull]
        public static XmlNodeList SelectNodesEx([NotNull] this XmlElement xmlElement, [NotNull] string xpath, [NotNull] XmlNamespaceManager namespaceManager)
            {
            // ReSharper disable AssignNullToNotNullAttribute
            return xmlElement.SelectNodes(xpath, namespaceManager);
            // ReSharper restore AssignNullToNotNullAttribute
            }
        }
    }
