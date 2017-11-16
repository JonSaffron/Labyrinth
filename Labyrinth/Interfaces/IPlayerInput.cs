using Microsoft.Xna.Framework;

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
        // todo this doesn't need the gameTime parameter (usually)
        void ProcessInput(GameTime gameTime);
        }
    }
