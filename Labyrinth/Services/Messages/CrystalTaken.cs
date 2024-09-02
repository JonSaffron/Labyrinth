using System;

namespace Labyrinth.Services.Messages
    {
    internal class CrystalTaken
        {
        public readonly IValuable Crystal;

        public CrystalTaken(IValuable crystal)
            {
            this.Crystal = crystal ?? throw new ArgumentNullException(nameof(crystal));
            }
        }
    }
