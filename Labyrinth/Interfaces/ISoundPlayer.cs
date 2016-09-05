using System;
using Labyrinth.Services.Sound;

namespace Labyrinth
    {
    public interface ISoundPlayer
        {
        /// <summary>
        /// Play a sound
        /// </summary>
        /// <param name="gameSound">Indicates which sound to play</param>
        void Play(GameSound gameSound);
        /// <summary>
        /// Play a sound and trigger a callback upon its completion
        /// </summary>
        /// <param name="gameSound">Indicates which sound to play</param>
        /// <param name="callback">The method to call when the sound finishes playing</param>
        void PlayWithCallback(GameSound gameSound, EventHandler callback);
        /// <summary>
        /// Play a sound that relates to a particular game object
        /// </summary>
        /// <param name="gameSound">Indicates which sound to play</param>
        /// <param name="gameObject">The game object the sound relates to</param>
        /// <param name="centrePointProvider">The object that provides the co-ordinates of the centre of the screen view</param>
        void PlayForObject(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider);
        /// <summary>
        /// Play a sound that relates to a particular game object and trigger a callback upon its completion
        /// </summary>
        /// <param name="gameSound">Indicates which sound to play</param>
        /// <param name="gameObject">The game object the sound relates to</param>
        /// <param name="centrePointProvider">The object that provides the co-ordinates of the centre of the screen view</param>
        /// <param name="callback">The method to call when the sound finishes playing</param>
        void PlayForObjectWithCallback(GameSound gameSound, IGameObject gameObject, ICentrePointProvider centrePointProvider, EventHandler callback);

        /// <summary>
        /// Return volume to its unmuted level
        /// </summary>
        void Unmute();
        /// <summary>
        /// Mute the volume
        /// </summary>
        void Mute();
        /// <summary>
        /// Returns a value that indicates whether the volume is muted or not
        /// </summary>
        bool IsMuted { get; }
        /// <summary>
        /// Turn up the volume
        /// </summary>
        void TurnUpTheVolume();
        /// <summary>
        /// Turn down the volume
        /// </summary>
        void TurnDownTheVolume();

        /// <summary>
        /// Returns the instance of the sound library that is in use by this object
        /// </summary>
        SoundLibrary SoundLibrary { get; }
        /// <summary>
        /// Returns the instance of the active sound service that is in use by this object
        /// </summary>
        IActiveSoundService ActiveSoundService { get; }
        }
    }
