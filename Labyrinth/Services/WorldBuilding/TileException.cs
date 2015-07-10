using System;

namespace Labyrinth.Services.WorldBuilding
    {
    class TileException : Exception
        {
        public TileExceptionType Type {get; private set;}

        public TileException(TileExceptionType type, string message) : base(message)
            {
            this.Type = type;
            }

        public TileException(TileException te, string message) : base(string.Format("{0}: {1}", message, te.Message))
            {
            this.Type = te.Type;
            }
        }
    }
