using System;
using System.Collections.Generic;

namespace Labyrinth.Services.Sound
    {
    class ActiveSoundService : IActiveSoundService
        {
        private readonly List<IActiveSound> _activeSounds = new List<IActiveSound>();

        public void Add(IActiveSound activeSound)
            {
            if (activeSound == null)
                throw new ArgumentNullException("activeSound");

            this._activeSounds.RemoveAll(item => item.SoundEffectInstance == activeSound.SoundEffectInstance);
            this._activeSounds.Add(activeSound);
            activeSound.Play();
            }

        public void Clear()
            {
            foreach (var activeSound in this._activeSounds)
                {
                activeSound.Stop();
                }
            }

        public void Update()
            {
            List<IActiveSound> instancesToRemove = null;

            foreach (var item in this._activeSounds)
                {
                if (item.CanBeRemoved)
                    {
                    if (instancesToRemove == null)
                        instancesToRemove = new List<IActiveSound>();
                    instancesToRemove.Add(item);
                    }
                else
                    item.Update();
                }

            if (instancesToRemove == null)
                return;

            foreach (var item in instancesToRemove)
                {
                this._activeSounds.Remove(item);
                }
            }

        public int Count
            {
            get
                {
                var result = this._activeSounds.Count;
                return result;
                }
            }

        public IActiveSound this[int index]
            {
            get
                {
                var result = this._activeSounds[index];
                return result;
                }
            }
        }
    }
