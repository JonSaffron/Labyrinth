using System;
using Microsoft.Xna.Framework;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    static class MonsterMovement
        {
        private static readonly Direction[] Directions = { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
        
        public static Direction RandomDirection()
            {
            int d = GlobalServices.Randomness.Next(256) & 3;
            return Directions[d];
            }

        public static bool IsPlayerInWeaponSights(this Monster m)
            {
            Player p = GlobalServices.GameState.Player;
            if (!p.IsAlive() || !IsPlayerInSameRoomAsMonster(m))
                return false;

            var result = (m.TilePosition.X == p.TilePosition.X)
                      || (m.TilePosition.Y == p.TilePosition.Y);
            return result;
            }

        public static bool IsPlayerNearby(this Monster monster)
            {
            const int radiusSquared = 20 * 20;

            Player p = GlobalServices.GameState.Player;
            if (!p.IsAlive())
                return false;
            var playerTilePos = p.TilePosition;
            if (!monster.SightBoundary.IsPositionWithinBoundary(playerTilePos))
                return false;
            var distanceSquared = TilePos.DistanceSquared(playerTilePos, monster.TilePosition);
            var result = distanceSquared <= radiusSquared;
            return result;
            }

        public static bool IsPlayerInSameRoomAsMonster(this IMonster m)
            {
            Player p = GlobalServices.GameState.Player;
            if (!p.IsAlive())
                return false;

            Rectangle playerRoom = World.GetContainingRoom(p.Position);
            bool result = playerRoom.ContainsPosition(m.Position);
            return result;
            }

        /// <summary>
        /// Checks whether the monster can move in the specified direction.
        /// If it can't, it will try other directions in turn until it finds a direction that is available.
        /// </summary>
        /// <param name="m">The monster that is moving</param>
        /// <param name="intendedDirection">The direction the monster hopes to move in</param>
        /// <param name="feasibleDirection">The direction the monster can move in, or Direction.None if it's unable to move</param>
        /// <returns>True if the monster can move in the intendedDirection, or False otherwise</returns>
        public static bool ConfirmDirectionToMoveIn(this Monster m, Direction intendedDirection, out Direction feasibleDirection)
            {
            if (intendedDirection == Direction.None)
                throw new ArgumentOutOfRangeException(nameof(intendedDirection), "Cannot specify Direction.None.");

            if (m.CanMoveInDirection(intendedDirection))
                {
                feasibleDirection = intendedDirection;
                return true;
                }

            var directionToTry = intendedDirection;
            for (int i = 0; i < 3; i++)
                {
                directionToTry = GetNextDirection(directionToTry);
                if (m.CanMoveInDirection(directionToTry))
                    {
                    feasibleDirection = directionToTry;
                    return false;
                    }
                }

            feasibleDirection = Direction.None;
            return false;
            }

        private static Direction GetNextDirection(Direction d)
            {
            switch (d)
                {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Left;
                default:
                    throw new InvalidOperationException();
                }
            }

        public static Direction AlterDirectionByVeeringAway(Direction d)
            {
            switch (d)
                {
                case Direction.Left:
                    return Direction.Up;
                case Direction.Right:
                    return Direction.Down;
                case Direction.Up:
                    return Direction.Left;
                case Direction.Down:
                    return Direction.Right;
                default:
                    throw new InvalidOperationException();
                }
            }

        /// <summary>
        /// This works in the same way to the original code. Movement to the right or left will be made in preference to up/down.
        /// </summary>
        /// <param name="m">The monster to determine the movement for</param>
        /// <returns>The direction that brings the monster closer to the player</returns>
        /// <remarks>This will never return Direction.None</remarks>
        public static Direction DetermineDirectionTowardsPlayer(this Monster m)
            {
            TilePos playerPos = GlobalServices.GameState.Player.TilePosition;
            int diffX = m.TilePosition.X - playerPos.X; 
            Direction result;
            if (diffX != 0)
                {
                result = diffX > 0 ? Direction.Left : Direction.Right;

                if (m.CanMoveInDirection(result))
                    return result;
                }

            int diffY = m.TilePosition.Y - playerPos.Y;
            result = diffY > 0 ? Direction.Up : Direction.Down;
            return result;
            }

        public static Direction DetermineDirectionAwayFromPlayer(this Monster m)
            {
            TilePos playerPos = GlobalServices.GameState.Player.TilePosition;
            int diffX = m.TilePosition.X - playerPos.X;
            Direction result;
            if (diffX != 0)
                {
                result = diffX > 0 ? Direction.Right : Direction.Left;

                if (m.CanMoveInDirection(result))
                    return result;
                }

            int diffY = m.TilePosition.Y - playerPos.Y;
            result = diffY > 0 ? Direction.Down : Direction.Up;
            return result;
            }
        }
    }
