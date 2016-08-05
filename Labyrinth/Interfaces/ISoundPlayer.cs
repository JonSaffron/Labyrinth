using System;
using Labyrinth.Services.Sound;

namespace Labyrinth
    {
    public interface ISoundPlayer
        {
        void Play(GameSound gameSound);
        void PlayWithCallback(GameSound gameSound, EventHandler callback);
        void PlayForObject(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider);
        void PlayForObjectWithCallback(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider, EventHandler callback);

        void Enable();
        void Disable();
        void TurnItUp();
        void TurnItDown();

        SoundLibrary SoundLibrary { get; }
        IActiveSoundService ActiveSoundService { get; }
        }
    }
