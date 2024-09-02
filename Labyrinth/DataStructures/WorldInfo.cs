using System;

namespace Labyrinth.DataStructures
    {
    internal class WorldInfo
        {
        public readonly string File;
        public readonly string Name;
        public readonly string Difficulty;

        public WorldInfo(string file, string name, string difficulty)
            {
            this.File = file ?? throw new ArgumentNullException(nameof(file));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Difficulty = difficulty ?? throw new ArgumentNullException(nameof(difficulty));
            }
        }
    }
