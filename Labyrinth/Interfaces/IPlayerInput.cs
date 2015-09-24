using Labyrinth.Services.Input;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public interface IPlayerInput
        {
        GameInput GameInput { get; set; }
        Direction Direction { get; }
        FiringState FireStatus1 { get; }
        FiringState FireStatus2 { get; }

        /// <summary>
        /// Gets player movement and fire action
        /// </summary>
        void ProcessInput(GameTime gameTime);
        }
    }
