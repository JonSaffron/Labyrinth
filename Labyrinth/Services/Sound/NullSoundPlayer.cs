using System;

namespace Labyrinth.Services.Sound
    {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

        public void PlayForObject(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider)
            {
            // nothing to do
            }

        public void PlayForObjectWithCallback(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider, EventHandler callback)
            {
            // nothing to do
            }

        public void Unmute()
            {
            // nothing to do
            }

        public void Mute()
            {
            // nothing to do
            }

        public bool IsMuted => true;

        public void TurnUpTheVolume()
            {
            // nothing to do
            }

        public void TurnDownTheVolume()
            {
            // nothing to do
            }

        public SoundLibrary SoundLibrary => new SoundLibrary();

        public IActiveSoundService ActiveSoundService => new ActiveSoundService();
        }
    }
