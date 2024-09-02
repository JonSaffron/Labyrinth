using System;

namespace Labyrinth.DataStructures
    {
    internal class WorldStartParameters
        {
        public readonly int CountOfLives;
        public readonly string WorldToLoad;

        public WorldStartParameters(int countOfLives, string worldToLoad)
            {
            this.CountOfLives = countOfLives;
            this.WorldToLoad = worldToLoad ?? throw new ArgumentNullException(nameof(worldToLoad));
            }
        }
    }
