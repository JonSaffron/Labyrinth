using System;

namespace Labyrinth.Services.Sound
    {
    class SoundInfoAttribute : Attribute
        {
        public readonly int CacheSize;
        public readonly bool RequiresGameObject;
        public readonly string ResourceName;

        public SoundInfoAttribute(int cacheSize = 1, bool requiresGameObject = false, string resourceName = null) 
            { 
            this.CacheSize = cacheSize;
            this.RequiresGameObject = requiresGameObject;
            this.ResourceName = resourceName;
            }

        public override string ToString()
            {
            var result = string.Format("cacheSize {0}, requiresGameObject {1}, resourceName {2}", this.CacheSize, this.RequiresGameObject, this.ResourceName);
            return result;
            }
        }
    }
