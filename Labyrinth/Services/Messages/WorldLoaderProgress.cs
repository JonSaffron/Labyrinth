using System;

namespace Labyrinth.Services.Messages
    {
    public class WorldLoaderProgress
        {
        public readonly string Message;

        public WorldLoaderProgress(string message)
            {
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
            }
        }
    }
