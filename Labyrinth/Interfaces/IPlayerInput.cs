using Labyrinth.DataStructures;

namespace Labyrinth
    {
    public interface IPlayerInput
        {
        /// <summary>
        /// Gets player movement and fire action
        /// </summary>
        PlayerControl Update();
        }
    }
