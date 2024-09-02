using System;

namespace Labyrinth.Services.Sound
    {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Field)]
    internal class SoundInfoAttribute : Attribute
        {
        public readonly int CacheSize;
        public readonly bool RequiresGameObject;
        public readonly string? ResourceName;

        public SoundInfoAttribute(int cacheSize = 1, bool requiresGameObject = false, string? resourceName = null)
            { 
            this.CacheSize = cacheSize;
            this.RequiresGameObject = requiresGameObject;
            this.ResourceName = resourceName;
            }

        public override string ToString()
            {
            var result = $"cacheSize {this.CacheSize}, requiresGameObject {this.RequiresGameObject}, resourceName {this.ResourceName}";
            return result;
            }
        }
    }
