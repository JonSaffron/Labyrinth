using System;

namespace Labyrinth.Services.Messages
    {
    public class WorldStatus
        {
        public readonly string Message;

        public WorldStatus(string message)
            {
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
            }
        }
    }
