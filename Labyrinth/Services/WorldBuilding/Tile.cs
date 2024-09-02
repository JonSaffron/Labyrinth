using System;

namespace Labyrinth.Services.WorldBuilding
    {
    /// <summary>
    /// Stores the floor texture and other meta data for a tile in the world
    /// </summary>
    public readonly struct Tile
        {
        public readonly string TextureName;
        public readonly int WorldAreaId;

        public Tile(string textureName, int worldAreaId)
            {
            this.TextureName = textureName ?? throw new ArgumentNullException(nameof(textureName));
            this.WorldAreaId = worldAreaId;
            }
        }
    }
