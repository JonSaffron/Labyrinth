using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth.Services.Sound
    {
    class SoundPlayer : ISoundPlayer
        {
        private readonly SoundLibrary _soundLibrary;

        public SoundPlayer(SoundLibrary soundLibrary)
            {
            this._soundLibrary = soundLibrary;
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
