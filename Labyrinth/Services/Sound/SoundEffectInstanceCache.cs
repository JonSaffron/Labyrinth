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
            result.InstanceName = $"{this._gameSound}{this.Position}";
            return result;
            }

        public override string ToString()
            {
            string result = string.Format($"Cache for {this._gameSound}: {this.Count} of {this.Size} used");
            return result;
            }
        }
    }
