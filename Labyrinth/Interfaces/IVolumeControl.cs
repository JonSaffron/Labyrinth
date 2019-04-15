namespace Labyrinth
    {
    interface IVolumeControl
        {
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
        }
    }
