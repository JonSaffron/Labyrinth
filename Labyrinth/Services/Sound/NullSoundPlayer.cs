using System;

namespace Labyrinth.Services.Sound
    {
    class NullSoundPlayer : ISoundPlayer
        {
        public void Play(GameSound gameSound)
            {
            // nothing to do
            }

        public void PlayWithCallback(GameSound gameSound, EventHandler callback)
            {
            // nothing to do
            }

        public void PlayForObject(GameSound gameSound, IGameObject gameObject)
            {
            // nothing to do
            }

        public void PlayForObjectWithCallback(GameSound gameSound, IGameObject gameObject, EventHandler callback)
            {
            // nothing to do
            }

        public void Enable()
            {
            // nothing to do
            }

        public void Disable()
            {
            // nothing to do
            }

        public void TurnItUp()
            {
            // nothing to do
            }

        public void TurnItDown()
            {
            // nothing to do
            }

        public SoundLibrary SoundLibrary
            {
            get
                {
                return new SoundLibrary();
                }
            }
        }
    }
