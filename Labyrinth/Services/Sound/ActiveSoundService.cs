using System;
using System.Collections.Generic;
using System.Linq;

namespace Labyrinth.Services.Sound
    {
    public class ActiveSoundService : IActiveSoundService
        {
        private readonly HashSet<IActiveSound> _activeSounds = new HashSet<IActiveSound>(new ActiveSoundComparer());

        public void Add(IActiveSound activeSound)
            {
            if (activeSound == null)
                throw new ArgumentNullException("activeSound");

            // slightly odd looking, but this means that if a SoundEffectInstance is added both as an ActiveSound and as an ActiveSoundForObject then the code copes
            this._activeSounds.Remove(activeSound);
            this._activeSounds.Add(activeSound);
            activeSound.Play();
            }

        public void Clear()
            {
            foreach (var activeSound in this._activeSounds)
                {
                activeSound.Stop();
                }
            Update();
            }

        public void Update()
            {
            this._activeSounds.RemoveWhere(item => item.HasFinishedPlaying);

            foreach (var item in this._activeSounds)
                {
                item.Update();
                }
            }

        /// <summary>
        /// Returns the number of active sounds being managed. Useful for testing.
        /// </summary>
        public int Count
            {
            get
                {
                var result = this._activeSounds.Count;
                return result;
                }
            }

        /// <summary>
        /// Returns the specified active sound. Useful for testing.
        /// </summary>
        /// <param name="index">The index of the active sound to return.</param>
        /// <returns>An instance of an active sound object.</returns>
        public IActiveSound this[int index]
            {
            get
                {
                var result = this._activeSounds.ElementAt(index);
                return result;
                }
            }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private class ActiveSoundComparer : IEqualityComparer<IActiveSound>
            {
            public bool Equals(IActiveSound x, IActiveSound y)
                {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                var result = x.SoundEffectInstance.InstanceName.Equals(y.SoundEffectInstance.InstanceName, StringComparison.Ordinal);
                return result;
                }

            public int GetHashCode(IActiveSound obj)
                {
                var result = obj.SoundEffectInstance.InstanceName.GetHashCode();
                return result;
                }
            }
        }
    }
