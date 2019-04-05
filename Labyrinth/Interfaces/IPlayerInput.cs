namespace Labyrinth
    {
    public interface IPlayerInput
        {
        Direction Direction { get; }
        FiringState FireStatus1 { get; }
        FiringState FireStatus2 { get; }

        /// <summary>
        /// Gets player movement and fire action
        /// </summary>
        void Update();
        }
    }
