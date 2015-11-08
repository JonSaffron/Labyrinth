using System;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects
    {
    static class MonsterMovement
        {
        public static readonly Random MonsterRandom = new Random();

        [Obsolete]
        public static Direction DetermineDirectionStandardPatrolling(Monster m)
            {
            if (m.Direction == Direction.None)
                throw new InvalidOperationException("Direction must be set for patrolling.");

            TilePos tp = m.TilePosition;
            TilePos pp = tp.GetPositionAfterOneMove(m.Direction);
            bool isCurrentlyMovingTowardsFreeSpace = GlobalServices.GameState.CanTileBeOccupied(pp, true);
            Vector2 potentiallyMovingTowards = pp.ToPosition();
            bool isInSameRoom = IsInSameRoom(m.Position, potentiallyMovingTowards);
            bool canContinueMovingInTheSameDirection = isCurrentlyMovingTowardsFreeSpace && isInSameRoom;
            var result = canContinueMovingInTheSameDirection ? m.Direction : m.Direction.Reversed();
            System.Diagnostics.Trace.WriteLine(string.Format("{0} {1} {2} {3} {4}", tp, pp, isCurrentlyMovingTowardsFreeSpace, isInSameRoom, result));
            return result;
            }
        
        [Obsolete]
        public static Direction DetermineDirectionRolling(Monster m)
            {
            if (m.Direction == Direction.None)
                return RandomDirection();
            
            bool changeDirection = (MonsterRandom.Next(256) & 7) == 0;
            var result = changeDirection ? AlterDirection(m.Direction) : DetermineDirectionStandardPatrolling(m);
            return result;
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

        public static Direction RandomDirection(Monster m)
            {
            var result = RandomDirection();
            return result;
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

        public static Direction DetermineDirectionCautious(Monster m)
            {
            Direction result;
            if (IsCautious(m))
                {
                bool alterDirection = m.Direction != Direction.None && (MonsterRandom.Next(256) & 3) == 0;
                if (alterDirection)
                    result = AlterDirection(m.Direction);
                else
                    {
                    var d = DetermineDirectionTowardsPlayer(m);
                    result = (d == Direction.None) ? RandomDirection() : d.Reversed();
                    }
                }
            else
                {
                if ((MonsterRandom.Next(256) & 7) == 0)
                    {
                    result = RandomDirection();
                    }
                else
                    {
                    result = DetermineDirectionTowardsPlayer(m);
                    if (result == Direction.None)
                        result = RandomDirection();
                    }
                }
            return result;
            }
        
        public static Direction DetermineDirectionFullPursuit(Monster m)
            {
            Direction result = DetermineDirectionTowardsPlayer(m);
            if (result == Direction.None)
                result = RandomDirection();
            
            result = UpdateDirectionWhereMovementBlocked(m, result);

            Vector2 potentiallyMovingTowards = m.TilePosition.GetPositionAfterOneMove(result).ToPosition();
            Vector2 diff = (potentiallyMovingTowards - GlobalServices.GameState.Player.Position) / Tile.Size;
            float tilesToPlayer = Math.Min(Math.Abs(diff.X), Math.Abs(diff.Y));
            if (tilesToPlayer <= 2)
                result = AlterDirection(result);

            return result;
            }
        
        public static Direction DetermineDirectionSemiAggressive(Monster m)
            {
            bool makeAggressiveMove = (MonsterRandom.Next(256) & 1) == 0;
            if (m.ChangeRooms != ChangeRooms.FollowsPlayer)
                makeAggressiveMove &= IsPlayerInSameRoomAsMonster(m);
            var shouldFollowPlayer = makeAggressiveMove && IsPlayerInSight(m);
            var result = shouldFollowPlayer ? DetermineDirectionTowardsPlayer(m) : RandomDirection();
            return result;
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

        public static Direction DetermineDirectionTowardsPlayer(Monster m)
            {
            Vector2 diff = (m.Position - GlobalServices.GameState.Player.Position);
            double hMove = MonsterRandom.NextDouble() * Math.Abs(diff.X);
            double vMove = MonsterRandom.NextDouble() * Math.Abs(diff.Y);
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

        public static bool IsPlayerInSameRoomAsMonster(Monster m)
            {
            Player p = GlobalServices.GameState.Player;
            if (!p.IsExtant)
                return false;
            
            var result = IsInSameRoom(p.Position, m.Position);
            return result;
            }
            
        private static bool IsCautious(Monster m)
            {
            int compareTo = m.Energy << 2;
            bool result = GlobalServices.GameState.Player.Energy > compareTo;
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
