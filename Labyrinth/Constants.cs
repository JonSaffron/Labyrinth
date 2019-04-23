using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    static class Constants
        {
        /// <summary>
        /// Length of tile in pixels
        /// </summary>
        public const int TileLength = 32;

        /// <summary>
        /// Smallest unit of movement that a sprite can make is a quarter of a tile
        /// </summary>
        public const int BaseDistance = TileLength / 4;

        /// <summary>
        /// Number of base distances a sprite can make per second
        /// </summary>
        /// <remarks>This is a speed value in terms of base distances</remarks>
        public const int BaseDistancesMovedPerSecond = 20;

        /// <summary>
        /// Slowest move a sprite can make is a quarter of a tile, and x movements take place per second.
        /// Unit of measurement is therefore pixels per second.
        /// </summary>
        public const int BaseSpeed = BaseDistancesMovedPerSecond * BaseDistance;

        /// <summary>
        /// Speed of a pushed object
        /// </summary>
        public const int PushSpeed = (int) (BaseSpeed * 2.5);

        /// <summary>
        /// Speed when bouncing back
        /// </summary>
        public const int BounceBackSpeed = BaseSpeed * 3;

        /// <summary>
        /// The internal game clock ticks once every base distance by a sprite moving at base speed
        /// Unit of measurement is seconds.
        /// </summary>
        public const float GameClockResolution = 1.0f / BaseDistancesMovedPerSecond;

        /// <summary>
        /// Size of tile in terms of a vector
        /// </summary>
        public static readonly Vector2 TileSize = new Vector2(TileLength, TileLength);

        /// <summary>
        /// Centre point of tile as a vector
        /// </summary>
        public static readonly Vector2 CentreOfTile = TileSize / 2;

        /// <summary>
        /// Rectangular area of a tile
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
        /// Size of a room measured in pixels 512 x 320
        /// </summary>
        public static readonly Vector2 RoomSizeInPixels = new Vector2(RoomSizeInTiles.X * TileLength, RoomSizeInTiles.Y * TileLength);
        }
    }
