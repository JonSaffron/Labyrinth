using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth
    {
    class NullSoundPlayer : ISoundPlayer
        {
        public IGameSoundInstance GetSoundEffectInstance(GameSound gameSound)
            {
            var result = new NullGameSoundInstance();
            return result;
            }

        public void Play(SoundEffectInstance soundEffectInstance)
            {
            // nothing to do
            }

        public void Play(GameSound gameSound)
            {
            // nothing to do
            }

        public void Play(GameSound gameSound, SoundEffectFinished callback)
            {
            if (callback == null)
                throw new ArgumentNullException("callback");

            var args = new SoundEffectFinishedEventArgs(gameSound);
            callback(this, args);
            }
        }
    }
