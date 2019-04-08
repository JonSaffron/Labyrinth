using System;
using JetBrains.Annotations;

namespace Labyrinth.Services.Messages
    {
    public class WorldLoaderProgress
        {
        public readonly string Message;

        public WorldLoaderProgress([NotNull] string message)
            {
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
            }
        }
    }
