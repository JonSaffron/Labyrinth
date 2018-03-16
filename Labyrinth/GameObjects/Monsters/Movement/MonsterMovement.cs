using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects.Movement
    {
    static class MonsterMovement
        {
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

        public static Direction RandomDirection()
            {
            int d = GlobalServices.Randomess.Next(256) & 3;
            switch (d)
                {
                case 0:
                    return Direction.Left;
                case 1:
                    return Direction.Right;
                case 2:
                    return Direction.Up;
                case 3:
                    return Direction.Down;
                default:
                    throw new InvalidOperationException();
                }
            }

        public static bool IsPlayerInWeaponSights(Monster m)
            {
            if (!GlobalServices.GameState.Player.IsAlive() || !IsPlayerInSameRoomAsMonster(m))
                return false;

            var result = (m.TilePosition.X == GlobalServices.GameState.Player.TilePosition.X)
                    ||  (m.TilePosition.Y == GlobalServices.GameState.Player.TilePosition.Y);
            return result;
            }

        public static bool IsPlayerNearby(Monster monster)
            {
            if (!GlobalServices.GameState.Player.IsAlive() || !monster.SightBoundary.IsPositionWithinBoundary(GlobalServices.GameState.Player.TilePosition))
                return false;
            var distance = Vector2.Distance(monster.Position, GlobalServices.GameState.Player.Position) / Constants.TileLength;
            var result = distance <= 20;
            return result;
            }

        public static bool IsPlayerInSameRoomAsMonster(Monster m)
            {
            Player p = GlobalServices.GameState.Player;
            if (!p.IsExtant)
                return false;

            var result = IsInSameRoom(p.Position, m.Position);
            return result;
            }

        private static bool IsInSameRoom(Vector2 p1, Vector2 p2)
            {
            Rectangle room1 = World.GetContainingRoom(p1);
            bool result = room1.ContainsPosition(p2);
            return result;
            }

        public static Direction UpdateDirectionWhereMovementBlocked(Monster m, Direction intendedDirection)
            {
            if (intendedDirection == Direction.None)
                intendedDirection = RandomDirection();

            TilePos tp = m.TilePosition;
            for (int i = 0; i < 3; i++, intendedDirection = GetNextDirection(intendedDirection))
                {
                TilePos potentiallyMovingTowardsTile = tp.GetPositionAfterOneMove(intendedDirection);
                Vector2 potentiallyMovingTowards = potentiallyMovingTowardsTile.ToPosition();

                if (m.ChangeRooms == ChangeRooms.StaysWithinRoom && !IsInSameRoom(m.Position, potentiallyMovingTowards))
                    continue;

                if (m.CanMoveInDirection(intendedDirection))
                    return intendedDirection;
                }

            return Direction.None;
            }

        public static Direction AlterDirection(Direction d)
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
        public static Direction DetermineDirectionTowardsPlayer(Monster m)
            {
            TilePos playerPos = TilePos.TilePosFromPosition(GlobalServices.GameState.Player.Position);
            int diffX = m.TilePosition.X - playerPos.X; 
            Direction result;
            if (diffX != 0)
                {
                result = diffX > 0 ? Direction.Left : Direction.Right;
                }
            else
                {
                int diffY = m.TilePosition.Y - playerPos.Y;
                result = diffY > 0 ? Direction.Up : Direction.Down;
                }
            return result;
            }
        }
    }
