using System;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    public struct TilePos
        {
        public readonly int X;
        public readonly int Y;
        
        /// <summary>
        /// Constructs a new TilePos from the specified co-ordinates
        /// </summary>
        /// <param name="x">The X co-ordinate</param>
        /// <param name="y">The Y co-ordinate</param>
        public TilePos(int x, int y)
            {
            this.X = x;
            this.Y = y;
            }
        
        /// <summary>
        /// Returns a TilePos object corresponding to the specified co-ordinate
        /// </summary>
        /// <param name="position">The position within the world. For a GameObject, pass in the co-ordinates of the object's origin.</param>
        /// <returns>A TilePos that contains the specified co-ordinate</returns>
        public static TilePos TilePosFromPosition(Vector2 position)
            {
            var intx = (int)position.X / Constants.TileLength;
            var inty = (int)position.Y / Constants.TileLength;

            var result = new TilePos(intx, inty);
            return result;
            }
        
        /// <summary>
        /// Determines if two TilePos structures are equivalent
        /// </summary>
        /// <param name="first">The first TilePos</param>
        /// <param name="second">The second TilePos</param>
        /// <returns>True if the co-ordinates are the same in both TilePos structures, false otherwise</returns>
        public static bool operator ==(TilePos first, TilePos second)
            {
            return (first.X == second.X) && (first.Y == second.Y);
            }

        /// <summary>
        /// Determines if two TilePos structures are not equivalent
        /// </summary>
        /// <param name="first">The first TilePos</param>
        /// <param name="second">The second TilePos</param>
        /// <returns>True if either co-ordinate differs in the two TilePos structures, false otherwise</returns>
        public static bool operator !=(TilePos first, TilePos second)
            {
            return (first.X != second.X) || (first.Y != second.Y);
            }

        /// <summary>
        /// Determines if another TilePos structure is equivalent to this instance
        /// </summary>
        /// <param name="obj">The TilePos structure to compare against</param>
        /// <returns>True if the co-ordinates are the same in both TilePos structures, false otherwise</returns>
        public override bool Equals(object obj)
            {
            if (!(obj is TilePos))
                {
                return false;
                }
            var tp = (TilePos)obj;
            return tp == this;
            }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
            {
            var result = (this.Y & 0xFFFF << 0x10) & this.X & 0xFFFF;
            return result;
            }
        
        /// <summary>
        /// Returns a new TilePos instance after having moved 1 unit in the specified direction from this instance
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        /// <returns>A new TilePos instance</returns>
        /// <remarks>The specified direction cannot be Direction.None</remarks>
        internal TilePos GetPositionAfterOneMove(Direction direction)
            {
            return GetPositionAfterMoving(direction, 1);
            }

        /// <summary>
        /// Returns a new TilePos instance after having moved the specified number of units in the specified direction from this instance
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        /// <param name="countOfMoves">The number of units to move</param>
        /// <returns>A new TilePos instance</returns>
        /// <remarks>The specified direction cannot be Direction.None, and countOfMoves cannot be negative.</remarks>
        internal TilePos GetPositionAfterMoving(Direction direction, int countOfMoves)
            {
            if (countOfMoves <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfMoves));

            switch (direction)
                {
                case Direction.Left:
                    return new TilePos(X - countOfMoves, Y);
                case Direction.Right:
                    return new TilePos(X + countOfMoves, Y);
                case Direction.Up:
                    return new TilePos(X, Y - countOfMoves);
                case Direction.Down:
                    return new TilePos(X, Y + countOfMoves);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
                }
            }

        /// <summary>
        /// Returns the co-ordinates of the middle of the specified Tile
        /// </summary>
        /// <returns>A Vector2 structure for the middle of the tile</returns>
        //todo does this need to be refactored? not every object will have its origin in the middle of tile.
        public Vector2 ToPosition()
            {
            int x = this.X * Constants.TileLength + Constants.HalfTileLength;
            int y = this.Y * Constants.TileLength + Constants.HalfTileLength;
            var result = new Vector2(x, y);
            return result;
            }
        
        /// <summary>
        /// Returns a rectangle around this instance
        /// </summary>
        /// <param name="radius">The radius of tiles around this instance</param>
        /// <returns>A new TileRect structure with this TilePos instance at its centre</returns>
        public TileRect GetRectAroundPosition(int radius)
            {
            if (radius < 0)
                throw new ArgumentOutOfRangeException(nameof(radius));
            var topLeft = new TilePos(this.X - radius, this.Y - radius);
            var diameter = radius * 2 + 1;
            var result = new TileRect(topLeft, diameter, diameter);
            return result;
            }

        /// <summary>
        /// Returns the Morton Code for this instance
        /// </summary>
        /// <returns>A signed integer value</returns>
        public int MortonCode
            {
            get
                {
                var result = (Part1By1(this.Y) << 1) + Part1By1(this.X);
                return result;
                }
            }

        private static int Part1By1(int x)
            {
            x &= 0x0000ffff;                  // x = ---- ---- ---- ---- fedc ba98 7654 3210
            x = (x ^ (x <<  8)) & 0x00ff00ff; // x = ---- ---- fedc ba98 ---- ---- 7654 3210
            x = (x ^ (x <<  4)) & 0x0f0f0f0f; // x = ---- fedc ---- ba98 ---- 7654 ---- 3210
            x = (x ^ (x <<  2)) & 0x33333333; // x = --fe --dc --ba --98 --76 --54 --32 --10
            x = (x ^ (x <<  1)) & 0x55555555; // x = -f-e -d-c -b-a -9-8 -7-6 -5-4 -3-2 -1-0
            return x;
            }

        /// <summary>
        /// Constructs a TilePos instances from the specified Morton Code
        /// </summary>
        /// <param name="mortonCode">The Morton Code value</param>
        /// <returns>A new TilePos instance</returns>
        public static TilePos FromMortonCode(int mortonCode)
            {
            if (mortonCode < 0)
                throw new ArgumentOutOfRangeException(nameof(mortonCode));

            int x = Compact1By1(mortonCode >> 0);
            int y = Compact1By1(mortonCode >> 1);
            var result = new TilePos(x, y);
            return result;
            }

        private static int Compact1By1(int x)
            {
            x &= 0x55555555;                  // x = -f-e -d-c -b-a -9-8 -7-6 -5-4 -3-2 -1-0
            x = (x ^ (x >>  1)) & 0x33333333; // x = --fe --dc --ba --98 --76 --54 --32 --10
            x = (x ^ (x >>  2)) & 0x0f0f0f0f; // x = ---- fedc ---- ba98 ---- 7654 ---- 3210
            x = (x ^ (x >>  4)) & 0x00ff00ff; // x = ---- ---- fedc ba98 ---- ---- 7654 3210
            x = (x ^ (x >>  8)) & 0x0000ffff; // x = ---- ---- ---- ---- fedc ba98 7654 3210
            return x;
            }

        /// <summary>
        /// Returns a textual representation of this instance
        /// </summary>
        /// <returns>A string describing this instance</returns>
        public override string ToString()
            {
            return string.Format("({0}, {1})", this.X, this.Y);
            }
        }
    }
