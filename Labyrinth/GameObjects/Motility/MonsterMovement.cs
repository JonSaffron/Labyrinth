using System;
using Microsoft.Xna.Framework;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    static class MonsterMovement
        {
        private static readonly Direction[] Directions = { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
        
        public static PossibleDirection RandomDirection()
            {
            int d = GlobalServices.Randomness.Next(256) & 3;
            return new PossibleDirection(Directions[d]);
            }

        public static bool IsPlayerInWeaponSights(this Monster m)
            {
            Player p = GlobalServices.GameState.Player;
            if (!p.IsAlive() || !IsPlayerInSameRoom(m))
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
            if (monster.SightBoundary == null)
                throw new InvalidOperationException("SightBoundary has not been set for monster");

            var playerTilePos = p.TilePosition;
            if (!monster.SightBoundary.IsPositionWithinBoundary(playerTilePos))
                return false;
            var distanceSquared = TilePos.DistanceSquared(playerTilePos, monster.TilePosition);
            var result = distanceSquared <= radiusSquared;
            return result;
            }

        public static bool IsPlayerInSameRoom(this IMonster m)
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
        public static bool ConfirmDirectionToMoveIn(this Monster m, IDirectionChosen intendedDirection, out ConfirmedDirection feasibleDirection)
            {
            if (intendedDirection.Direction == Direction.None)
                {
                feasibleDirection = ConfirmedDirection.None;
                return true;
                }

            if (intendedDirection is ConfirmedDirection confirmedDirection)
                {
                feasibleDirection = confirmedDirection;
                return true;
                }

            if (m.CanMoveInDirection(intendedDirection.Direction))
                {
                feasibleDirection = intendedDirection.Confirm();
                return true;
                }

            var directionToTry = new PossibleDirection(intendedDirection.Direction);
            for (int i = 0; i < 3; i++)
                {
                directionToTry = GetNextDirection(directionToTry);
                if (m.CanMoveInDirection(directionToTry.Direction))
                    {
                    feasibleDirection = directionToTry.Confirm();
                    return false;
                    }
                }

            feasibleDirection = ConfirmedDirection.None;
            return false;
            }

        private static PossibleDirection GetNextDirection(PossibleDirection d)
            {
            switch (d.Direction)
                {
                case Direction.Left:
                    return PossibleDirection.Right;
                case Direction.Right:
                    return PossibleDirection.Up;
                case Direction.Up:
                    return PossibleDirection.Down;
                case Direction.Down:
                    return PossibleDirection.Left;
                default:
                    throw new InvalidOperationException();
                }
            }

        public static PossibleDirection AlterDirectionByVeeringAway(Direction d)
            {
            switch (d)
                {
                case Direction.Left:
                    return PossibleDirection.Up;
                case Direction.Right:
                    return PossibleDirection.Down;
                case Direction.Up:
                    return PossibleDirection.Left;
                case Direction.Down:
                    return PossibleDirection.Right;
                default:
                    throw new InvalidOperationException();
                }
            }

        /// <summary>
        /// This works in the same way to the original code. Movement to the right or left will be made in preference to up/down.
        /// </summary>
        /// <param name="m">The monster to determine the movement for</param>
        /// <returns>The direction that brings the monster closer to the player</returns>
        /// <remarks>
        /// For left/right movement, the direction of travel is confirmed as safe.
        /// For up/down movement, the direction of travel is not necessarily safe.
        /// This will never return Direction.None
        /// </remarks>
        public static IDirectionChosen DetermineDirectionTowardsPlayer(this Monster m)
            {
            TilePos playerPos = GlobalServices.GameState.Player.TilePosition;
            int diffX = m.TilePosition.X - playerPos.X;
            if (diffX != 0)
                {
                var result = diffX > 0 ? PossibleDirection.Left : PossibleDirection.Right;

                if (m.CanMoveInDirection(result.Direction))
                    return result.Confirm();
                }

            int diffY = m.TilePosition.Y - playerPos.Y;
            return diffY > 0 ? PossibleDirection.Up : PossibleDirection.Down;
            }

        /// <summary>
        /// This works in the same way to the original code. Movement to the right or left will be made in preference to up/down.
        /// </summary>
        /// <param name="m">The monster to determine the movement for</param>
        /// <returns>The direction that brings the monster away from the player</returns>
        /// <remarks>
        /// For left/right movement, the direction of travel is confirmed as safe.
        /// For up/down movement, the direction of travel is not necessarily safe.
        /// This will never return Direction.None
        /// </remarks>
        public static IDirectionChosen DetermineDirectionAwayFromPlayer(this Monster m)
            {
            TilePos playerPos = GlobalServices.GameState.Player.TilePosition;
            int diffX = m.TilePosition.X - playerPos.X;
            if (diffX != 0)
                {
                var result = diffX > 0 ? PossibleDirection.Right : PossibleDirection.Left;

                if (m.CanMoveInDirection(result.Direction))
                    return result.Confirm();
                }

            int diffY = m.TilePosition.Y - playerPos.Y;
            return diffY > 0 ? PossibleDirection.Down : PossibleDirection.Up;
            }
        }
    }
