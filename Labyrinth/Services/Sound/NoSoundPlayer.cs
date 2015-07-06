using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth
    {
    class NoSoundPlayer : ISoundPlayer
        {
        private readonly SoundLibrary _soundLibrary;

        public NoSoundPlayer(SoundLibrary soundLibrary)
            {
            this._soundLibrary = soundLibrary;
            }

        public IGameSoundInstance GetSoundEffectInstance(GameSound gameSound)
            {
            var soundEffectInstance = this._soundLibrary[gameSound].CreateInstance();
            soundEffectInstance.Volume = 0;
            var result = new GameSoundInstance(soundEffectInstance);
            return result;
            }

        public void Play(GameSound gameSound)
            {
            // do nothing
            }

        public void Play(GameSound gameSound, SoundEffectFinished callback)
            {
            if (callback == null)
                throw new ArgumentNullException("callback");

            var soundEffectInstance = this._soundLibrary.GetTrackedInstance(gameSound, callback);
            soundEffectInstance.Volume = 0;
            soundEffectInstance.Play();
            }

        public void Play(SoundEffectInstance soundEffectInstance)
            {
            // nothing to do
            }
        }
    }
