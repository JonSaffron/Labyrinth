using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth
    {
    class SoundPlayer : ISoundPlayer
        {
        private readonly SoundLibrary _soundLibrary;

        public SoundPlayer(SoundLibrary soundLibrary)
            {
            this._soundLibrary = soundLibrary;
            }

        /// <summary>
        /// Retrieves an instance of a sound effect.
        /// </summary>
        /// <param name="gameSound">Identifies the sound effect</param>
        /// <returns>A reference to a sound effect. This should be retained whilst the sound is playing.</returns>
        public IGameSoundInstance GetSoundEffectInstance(GameSound gameSound)
            {
            var soundEffectInstance = this._soundLibrary[gameSound].CreateInstance();
            var result = new GameSoundInstance(soundEffectInstance);
            return result;
            }

        public void Play(GameSound gameSound)
            {
            SoundEffect soundEffect = this._soundLibrary[gameSound];
            soundEffect.Play();
            }

        public void Play(GameSound gameSound, SoundEffectFinished callback)
            {
            if (callback == null)
                throw new ArgumentNullException("callback");

            var soundEffectInstance = this._soundLibrary.GetTrackedInstance(gameSound, callback);
            soundEffectInstance.Play();
            }

        public void Play(SoundEffectInstance soundEffectInstance)
            {
            soundEffectInstance.Play();
            }
        }
    }
