using System;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth
    {
    public interface ISoundEffectInstance : IDisposable
        {
        /// <summary>
        /// Starts playing the sound. This should be from the beginning
        /// </summary>
        void Play();

        /// <summary>
        /// Stops playing the sound. This happens almost immediately.
        /// </summary>
        void Stop();

        /// <summary>
        /// Stops playing the sound, and marks it as needing to be restarted as soon as it stops.
        /// </summary>
        void Restart();

        /// <summary>
        /// Returns the unique name of this SoundEffectInstance
        /// </summary>
        string InstanceName { get; }

        /// <summary>
        /// Gets whether the sound is currently playing, paused, or stopped
        /// </summary>
        SoundState State { get; }

        /// <summary>
        /// Indicates that the sound should be restarted.
        /// </summary>
        /// <remarks>This can be set when a new ActiveSound instance wants to play the sound, but it is already being played by an older instance</remarks>
        bool IsSetToRestart { get; }

        /// <summary>
        /// Pan value ranging from -1.0 (left speaker) to 0.0 (centered), 1.0 (right speaker). Values outside of this range will throw an exception.
        /// </summary>
        float Pan { get; set; }

        /// <summary>
        /// Volume, ranging from 0.0 (silence) to 1.0 (full volume). Volume during playback is scaled by SoundEffect.MasterVolume.
        /// </summary>
        float Volume { get; set; }
        }
    }
