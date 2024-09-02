using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    internal class PlayerIndexEventArgs : EventArgs
        {
        /// <summary>
        /// Constructs the event arguments
        /// </summary>
        /// <param name="playerIndex">Specifies the index of the player who triggered this event</param>
        public PlayerIndexEventArgs(PlayerIndex playerIndex)
            {
            this.PlayerIndex = playerIndex;
            }

        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public PlayerIndex PlayerIndex { get; }
        }
    }
