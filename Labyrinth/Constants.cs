using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    static class Constants
        {
        /// <summary>
        /// Length of tile
        /// </summary>
        public const int TileLength = 32;

        /// <summary>
        /// Number of base movements a sprite can make per second
        /// </summary>
        private const int MovementsPerSecond = 20;

        /// <summary>
        /// Smallest move a sprite can make is a quarter of a tile
        /// </summary>
        public const int BaseDistance = TileLength / 4;

        /// <summary>
        /// Slowest move a sprite can make is a quarter of a tile, and 20 movements take place per second.
        /// Unit of measurement is therefore pixels per second.
        /// </summary>
        public const int BaseSpeed = BaseDistance * MovementsPerSecond;

        /// <summary>
        /// Speed when bouncing back
        /// </summary>
        public const int BounceBackSpeed = BaseSpeed * 3;

        /// <summary>
        /// The internal game clock ticks once every movement by a sprite
        /// Unit of measurement is seconds.
        /// </summary>
        public const float GameClockResolution = 1.0f / MovementsPerSecond;

        /// <summary>
        /// Size of tile in terms of a vector
        /// </summary>
        public static readonly Vector2 TileSize = new Vector2(TileLength, TileLength);

        /// <summary>
        /// Centre point of tile as a vector
        /// </summary>
        public static readonly Vector2 CentreOfTile = TileSize / 2;

        /// <summary>
        /// Rectanglular area of a tile
        /// </summary>
        public static readonly Rectangle TileRectangle = new Rectangle(0, 0, TileLength, TileLength);

        /// <summary>
        /// Half tile length
        /// </summary>
        public const int HalfTileLength = TileLength / 2;

        /// <summary>
        /// Size of a room measured in tiles
        /// </summary>
        public static readonly Vector2 RoomSizeInTiles = new Vector2(16, 10);

        /// <summary>
        /// Size of a room measured in pixels
        /// </summary>
        public static readonly Vector2 RoomSizeInPixels = new Vector2(RoomSizeInTiles.X * TileLength, RoomSizeInTiles.Y * TileLength);

        /// <summary>
        /// Magnification factor whlist in a window
        /// </summary>
        public const int ZoomWhilstWindowed = 2;
        }
    }
