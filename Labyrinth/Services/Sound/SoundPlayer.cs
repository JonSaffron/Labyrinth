using System;
using Labyrinth.GameObjects;

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
            ISoundEffectInstance soundEffect = this._soundLibrary[gameSound];
            soundEffect.Play();
            }

        public void Play(GameSound gameSound, SoundEffectFinished callback)
            {
            if (callback == null)
                throw new ArgumentNullException("callback");

            throw new NotImplementedException();
            }

        public void Play(GameSound gameSound, StaticItem gameObject)
            {
            throw new NotImplementedException();
            }

        public void Play(GameSound gameSound, StaticItem gameObject, SoundEffectFinished callback)
            {
            throw new NotImplementedException();
            }
        }
    }
