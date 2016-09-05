namespace Labyrinth
    {
    public interface IActiveSound
        {
        /// <summary>
        /// Returns the xna SoundEffectInstance object associated with the object
        /// </summary>
        ISoundEffectInstance SoundEffectInstance { get; }

        /// <summary>
        /// Returns a value that indicates whether the sound has finished playing
        /// </summary>
        bool HasFinishedPlaying { get; }

        /// <summary>
        /// Starts playing the sound, or restarts playing it if it is already playing
        /// </summary>
        void Play();

        /// <summary>
        /// Stops playing the play
        /// </summary>
        void Stop();

        /// <summary>
        /// Allows the sound to adjust its volume and panning
        /// </summary>
        void Update();
        }
    }
