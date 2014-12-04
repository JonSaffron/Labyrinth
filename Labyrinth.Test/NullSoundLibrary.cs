using Microsoft.Xna.Framework.Content;

namespace Labyrinth.Test
    {
    class NullSoundLibrary : ISoundLibrary
        {
        public void LoadContent(ContentManager cm)
            {
            // nothing to do
            }

        public IGameSoundInstance GetSoundEffectInstance(GameSound gameSound)
            {
            return new NullGameSoundInstance();
            }

        public void Play(GameSound gameSound)
            {
            // nothing to do
            }

        public void Play(GameSound gameSound, SoundLibrary.SoundEffectFinished callback)
            {
            // nothing to do
            }

        public void CheckForStoppedInstances()
            {
            // nothing to do
            }
        }
    }
