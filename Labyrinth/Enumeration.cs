using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Labyrinth
    {
    public abstract class Enumeration : IComparable<Enumeration>
        {
        public int Value { get; }
        public string DisplayName { get; }

        protected Enumeration()
            {
            }

        protected Enumeration(int value, string displayName)
            {
            this.Value = value;
            this.DisplayName = displayName;
            }

        public override string ToString()
            {
            return this.DisplayName;
            }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
            {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
                {
                var instance = new T();

                if (info.GetValue(instance) is T locatedValue)
                    {
                    yield return locatedValue;
                    }
                }
            }

        public override bool Equals(object obj)
            {
            if (!(obj is Enumeration otherValue))
                {
                return false;
                }

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
            }

        public override int GetHashCode()
            {
            return Value.GetHashCode();
            }

        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
            {
            var absoluteDifference = Math.Abs(firstValue.Value - secondValue.Value);
            return absoluteDifference;
            }

        public static T FromValue<T>(int value) where T : Enumeration, new()
            {
            var matchingItem = Parse<T, int>(value, "value", item => item.Value == value);
            return matchingItem;
            }

        public static T FromDisplayName<T>(string displayName) where T : Enumeration, new()
            {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.DisplayName == displayName);
            return matchingItem;
            }

        private static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate) where T : Enumeration, new()
            {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                {
                var message = $"'{value}' is not a valid {description} in {typeof(T)}";
                throw new ApplicationException(message);
                }

            return matchingItem;
            }

        public int CompareTo(Enumeration other)
            {
        // ReSharper disable once RedundantNameQualifier
#pragma warning disable IDE0041 // Use 'is null' check
        if (object.ReferenceEquals(other, null))
                return 1;
#pragma warning restore IDE0041 // Use 'is null' check
        return Value.CompareTo(other.Value);
            }
        }
    }
