namespace Labyrinth.Services.Display
    {
    /// <summary>
    /// Describes the screen transition state.
    /// </summary>
    public enum ScreenState
        {
        /// <summary>
        /// The screen is currently in the process of being shown
        /// </summary>
        TransitionOn,

        /// <summary>
        /// The screen is currently on top and receiving input
        /// </summary>
        Active,

        /// <summary>
        /// The screen is currently in the process of being removed
        /// </summary>
        TransitionOff,

        /// <summary>
        /// The screen is currently obscured by another
        /// </summary>
        Hidden
        }
    }
