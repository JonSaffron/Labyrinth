﻿using System;
using Labyrinth.GameObjects;

namespace Labyrinth.Services.Sound
    {
    class NoSoundPlayer : ISoundPlayer
        {
        private readonly SoundLibrary _soundLibrary;

        public NoSoundPlayer(SoundLibrary soundLibrary)
            {
            this._soundLibrary = soundLibrary;
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
