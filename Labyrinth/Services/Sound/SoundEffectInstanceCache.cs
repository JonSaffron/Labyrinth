using System;

namespace Labyrinth.Services.Sound
    {
    class SoundEffectInstanceCache : InstanceCache<ISoundEffectInstance>
        {
        public TimeSpan SoundDuration { get; private set; }
        private readonly GameSound _gameSound;

        public SoundEffectInstanceCache(int countOfEntries, TimeSpan soundDuration, GameSound gameSound, Func<ISoundEffectInstance> createNewInstance): base(countOfEntries, createNewInstance)
            {
            this.SoundDuration = soundDuration;
            this._gameSound = gameSound;
            }

        protected override ISoundEffectInstance CreateNewInstance()
            {
            var result = base.CreateNewInstance();
            result.InstanceName = string.Format("{0}{1}", this._gameSound, this.Position);
            return result;
            }

        public override string ToString()
            {
            string result = string.Format("Caching {0} of {1} sounds of {2}", this.Count, this.Size, this._gameSound);
            return result;
            }
        }
    }
