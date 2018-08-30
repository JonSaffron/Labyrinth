using Microsoft.Xna.Framework.Graphics;

namespace Labyrinth.Services.WorldBuilding
    {
    /// <summary>
    /// Stores the floor texture and other meta data for a tile in the world
    /// </summary>
    public readonly struct Tile
        {
        public readonly Texture2D Floor;
        public readonly int WorldAreaId;

        public Tile(Texture2D floor, int worldAreaId)
            {
            this.Floor = floor;
            this.WorldAreaId = worldAreaId;
            }
        }
    }
