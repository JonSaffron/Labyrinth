﻿using System;
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
            if (!GlobalServices.GameState.Player.IsAlive())
                return false;

            var absDiffX = Math.Abs(m.TilePosition.X - GlobalServices.GameState.Player.TilePosition.X);
            var absDiffY = Math.Abs(m.TilePosition.Y - GlobalServices.GameState.Player.TilePosition.Y);
            
            var result = 
                    (absDiffX == 0 && absDiffY <= Constants.RoomSizeInTiles.Y)
                ||
                    (absDiffY == 0 && absDiffX <= Constants.RoomSizeInTiles.X);
            return result;
            }

        public static bool IsPlayerNearby(Monster monster)
            {
            if (!GlobalServices.GameState.Player.IsAlive())
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

        public static bool IsInSameRoom(Vector2 p1, Vector2 p2)
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

        public static Direction DetermineDirectionTowardsPlayer(Monster m)
            {
            Vector2 diff = (m.Position - GlobalServices.GameState.Player.Position);
            var hMove = Math.Abs(diff.X);
            var vMove = Math.Abs(diff.Y);
            Direction result;
            if (hMove > vMove)
                {
                result = diff.X > 0 ? Direction.Left : Direction.Right;
                }
            else
                {
                result = diff.Y > 0 ? Direction.Up : Direction.Down;
                }
            return result;
            }

        public static Direction ContinueOrReverseWithinRoom(Monster monster, Direction currentDirection)
            {
            if (CanMoveWithinRoom(monster, currentDirection))
                return currentDirection;
            var reversed = currentDirection.Reversed();
            return reversed;
            }

        public static bool CanMoveWithinRoom(Monster monster, Direction direction)
            {
            if (!monster.CanMoveInDirection(direction))
                return false;

            TilePos tp = monster.TilePosition;
            TilePos pp = tp.GetPositionAfterOneMove(direction);
            Vector2 potentiallyMovingTowards = pp.ToPosition();
            bool isInSameRoom = IsInSameRoom(monster.Position, potentiallyMovingTowards);
            return isInSameRoom;
            }
        }
    }