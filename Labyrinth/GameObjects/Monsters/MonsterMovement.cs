using System;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    static class MonsterMovement
        {
        public static readonly Random MonsterRandom = new Random();

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

        public static Direction RandomDirection()
            {
            int d = MonsterRandom.Next(256) & 3;
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

        public static Direction GetRandomPerpendicularDirection(Direction d)
            {
            switch (d)
                {
                case Direction.Left:
                case Direction.Right:
                    return MonsterRandom.Next(2) == 0 ? Direction.Up : Direction.Down;
                case Direction.Up:
                case Direction.Down:
                    return MonsterRandom.Next(2) == 0 ? Direction.Left : Direction.Right;
                default:
                    throw new InvalidOperationException();
                }
            }

        public static bool IsPlayerInSight(Monster m)
            {
            Vector2 diff = (m.Position - GlobalServices.GameState.Player.Position) / Tile.Size;
            bool result;
            if (Math.Abs(diff.X) >= Math.Abs(diff.Y))
                {
                float tilesDistance = Math.Abs(diff.X) / Tile.Width;
                result = tilesDistance <= Constants.RoomWidthInPixels * 2;
                }
            else
                {
                float tilesDistance = Math.Abs(diff.Y) / Tile.Height;
                result = tilesDistance <= Constants.RoomHeightInPixels * 2;
                }
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

        public static Direction UpdateDirectionWhereMovementBlocked(Monster m, Direction d)
            {
            Direction intendedDirection = d;
            
            TilePos tp = m.TilePosition;
            do
                {
                TilePos potentiallyMovingTowardsTile = tp.GetPositionAfterOneMove(d);
                Vector2 potentiallyMovingTowards = potentiallyMovingTowardsTile.ToPosition();
                
                if (m.ChangeRooms == ChangeRooms.StaysWithinRoom && !IsInSameRoom(m.Position, potentiallyMovingTowards))
                    {
                    d = GetNextDirection(d);
                    continue;
                    }
                
                if (GlobalServices.GameState.CanTileBeOccupied(potentiallyMovingTowardsTile, true))
                    {
                    return d;
                    }                
                
                d = GetNextDirection(d);
                } while (d != intendedDirection);
            
            return Direction.None;
            }
        
        public static bool IsInSameRoom(Vector2 p1, Vector2 p2)
            {
            Rectangle room1 = World.GetContainingRoom(p1);
            bool result = room1.ContainsPosition(p2);
            return result;
            }
        }
    }
