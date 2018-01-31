using System;
using JetBrains.Annotations;

namespace Labyrinth.Services.Messages
    {
    class CrystalTaken
        {
        public readonly IValuable Crystal;

        public CrystalTaken([NotNull] IValuable crystal)
            {
            this.Crystal = crystal ?? throw new ArgumentNullException(nameof(crystal));
            }
        }
    }
