using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    static class Constants
        {
        // Length of tile
        public const int TileLength = 32;

        // Number of base movements a sprite can make per second
        private const int MovementsPerSecond = 20;

        // Smallest move a sprite can make is a quarter of a tile
        public const int BaseDistance = TileLength / 4;

        // Slowest move a sprite can make is a quarter of a tile, and 20 movements take place per second.
        // Unit of measurement is therefore pixels per second.
        public const int BaseSpeed = BaseDistance * MovementsPerSecond;

        // Speed when bouncing back
        public const int BounceBackSpeed = BaseSpeed * 3;

        // The internal game clock ticks once every movement by a sprite
        // Unit of measurement is seconds.
        public const float GameClockResolution = 1.0f / MovementsPerSecond;

        // Size of tile in terms of a vector
        public static readonly Vector2 TileSize = new Vector2(TileLength, TileLength);

        public static readonly Vector2 CentreOfTile = TileSize / 2;

        public static readonly Rectangle TileRectangle = new Rectangle(0, 0, TileLength, TileLength);

        public const int RoomWidthInTiles = 16;
        public const int RoomHeightInTiles = 10;

        public const int RoomWidthInPixels = RoomWidthInTiles * TileLength;
        public const int RoomHeightInPixels = RoomHeightInTiles * TileLength;
        public const int ZoomWhilstWindowed = 2;
        }
    }
