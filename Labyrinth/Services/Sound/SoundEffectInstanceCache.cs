using System;

namespace Labyrinth.Services.Sound
    {
    internal class SoundEffectInstanceCache : InstanceCache<ISoundEffectInstance>
        {
        private readonly GameSound _gameSound;
        private readonly Func<Microsoft.Xna.Framework.Audio.SoundEffectInstance> _createSoundEffectInstance;

        public SoundEffectInstanceCache(int countOfEntries, GameSound gameSound, Func<Microsoft.Xna.Framework.Audio.SoundEffectInstance> createSoundEffectInstance): base(countOfEntries)
            {
            this._gameSound = gameSound;
            this._createSoundEffectInstance = createSoundEffectInstance;
            }

        protected override ISoundEffectInstance CreateNewInstance()
            {
            var instanceName = $"{this._gameSound}{this.Position}";
            var soundEffectInstance = this._createSoundEffectInstance();
            var result = new SoundEffectInstance(soundEffectInstance, instanceName);
            return result;
            }

        public override string ToString()
            {
            string result = $"Cache for {this._gameSound}: {this.Count} of {this.Size} used";
            return result;
            }
        }
    }
