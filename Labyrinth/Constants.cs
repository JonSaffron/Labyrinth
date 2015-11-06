using Labyrinth.Services.WorldBuilding;

namespace Labyrinth
    {
    static class Constants
        {
        // Number of base movements a sprite can make per second
        private const int MovementsPerSecond = 20;

        // Smallest move a sprite can make is a quarter of a tile
        public const int BaseDistance = Tile.Width / 4;

        // Slowest move a sprite can make is a quarter of a tile, and 20 movements take place per second.
        // Unit of measurement is therefore pixels per second.
        public const int BaseSpeed = BaseDistance * MovementsPerSecond;

        // Speed when bouncing back
        public const int BounceBackSpeed = BaseSpeed * 3;

        // The internal game clock ticks once every movement by a sprite
        // Unit of measurement is seconds.
        public const float GameClockResolution = 1.0f / MovementsPerSecond;

        private const int RoomWidthInTiles = 16;
        private const int RoomHeightInTiles = 10;

        public const int RoomWidthInPixels = RoomWidthInTiles * Tile.Width;
        public const int RoomHeightInPixels = RoomHeightInTiles * Tile.Height;
        public const int ZoomWhilstWindowed = 2;


        }
    }
