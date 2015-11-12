using System;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects.Movement
    {
    class KillerCubeRedMovement : StandardRolling
        {
        public override Direction DetermineDirection(Monster monster)
            {
            if (!IsPlayerInSight(monster))
                return base.DetermineDirection(monster);

            TilePos tp = monster.TilePosition;
                
            Direction newDirection = Direction.None;
            Player p = GlobalServices.GameState.Player;
            if (true)
                {
                TilePos playerPosition = p.TilePosition;
                int yDiff = tp.Y - playerPosition.Y;
                int xDiff = tp.X - playerPosition.X;
                
                if (this.CurrentDirection.IsHorizontal())
                    {
                    // Check if travelling on same row as player
                    if (yDiff == 0 && ShouldMakeMove())
                        {
                        // Move aside
                        newDirection = GetRandomPerpendicularDirection(this.CurrentDirection);
                        }
                    else if ((monster.Direction == Direction.Left && xDiff <= -5) || (monster.Direction == Direction.Right && xDiff >= 5))
                        {
                        newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                        }
                    }
                else if (this.CurrentDirection.IsVertical())
                    {
                    if (xDiff == 0 && ShouldMakeMove())
                        {
                        // Move aside
                        newDirection = GetRandomPerpendicularDirection(this.CurrentDirection);
                        }
                    else if ((monster.Direction == Direction.Up && yDiff <= -5) || (monster.Direction == Direction.Down && yDiff >= 5))
                        {
                        newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                        }
                    }

                if (newDirection != Direction.None)
                    {
                    TilePos pp = tp.GetPositionAfterOneMove(newDirection);
                    if (!GlobalServices.GameState.CanTileBeOccupied(pp, true))
                        newDirection = Direction.None;
                    }
                }

            if (newDirection == Direction.None)
                {                    
                if (monster.Direction == Direction.None)
                    {
                    newDirection = MonsterMovement.RandomDirection();
                    }
                else if (!isCurrentlyMovingTowardsFreeSpace)
                    {
                    // tend to bounce backwards
                    newDirection = MonsterMovement.MonsterRandom.Next(5) == 0 ? GetRandomPerpendicularDirection(monster.Direction) : monster.Direction.Reversed();
                    }
                else if (MonsterMovement.MonsterRandom.Next(16) == 0)
                    {
                    newDirection = GetRandomPerpendicularDirection(monster.Direction);
                    }
                else
                    {
                    newDirection = monster.Direction;
                    }
                }
            return newDirection;
            }

        private static bool IsPlayerInSight(Monster monster)
            {
            var distance = Vector2.Distance(monster.Position, GlobalServices.GameState.Player.Position) / Tile.Width;
            var result = distance <= 16;
            return result;
            }

        private static bool ShouldMakeMove()
            {
            return MonsterMovement.MonsterRandom.Next(8) == 0; 
            }

        private static Direction GetRandomPerpendicularDirection(Direction currentDirection)
            {
            if (currentDirection.IsHorizontal())
                return MonsterMovement.MonsterRandom.Next(2) == 0 ? Direction.Up : Direction.Down;
            if (currentDirection.IsVertical())
                return MonsterMovement.MonsterRandom.Next(2) == 0 ? Direction.Left : Direction.Right;
            throw new ArgumentOutOfRangeException("currentDirection");
            }
        }
    }
