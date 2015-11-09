using Labyrinth.GameObjects;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class KillerCubeRedMovement : IMonsterMovement
        {
        public Direction DetermineDirection(Monster monster)
            {
            TilePos tp = monster.TilePosition;
            bool isCurrentlyMovingTowardsFreeSpace;
            if (monster.Direction == Direction.None) 
                isCurrentlyMovingTowardsFreeSpace = false;
            else
                {
                TilePos pp = tp.GetPositionAfterOneMove(monster.Direction);
                isCurrentlyMovingTowardsFreeSpace = GlobalServices.GameState.CanTileBeOccupied(pp, true);
                
                if (isCurrentlyMovingTowardsFreeSpace)
                    {
                    Vector2 newPos = pp.ToPosition();
                    if (!MonsterMovement.IsInSameRoom(monster.Position, newPos) && MonsterMovement.MonsterRandom.Next(4) != 0)
                        isCurrentlyMovingTowardsFreeSpace = false;
                    }
                }
                
            Direction newDirection = Direction.None;
            Player p = GlobalServices.GameState.Player;
            if (p != null)
                {
                TilePos playerPosition = p.TilePosition;
                int yDiff = tp.Y - playerPosition.Y;
                int xDiff = tp.X - playerPosition.X;
                
                if (xDiff > 16 || yDiff > 16)
                    {
                    // player is out of sight
                    }
                else if (monster.Direction == Direction.Left || monster.Direction == Direction.Right)
                    {
                    // Check if travelling on same row as player
                    if (yDiff == 0 && ShouldMakeMove())
                        {
                        // Move aside
                        newDirection = MonsterMovement.MonsterRandom.Next(2) == 0 ? Direction.Up : Direction.Down;
                        }
                    else if ((monster.Direction == Direction.Left && xDiff <= -5) || (monster.Direction == Direction.Right && xDiff >= 5))
                        {
                        newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                        }
                    }
                else if (monster.Direction == Direction.Up || monster.Direction == Direction.Down)
                    {
                    if (xDiff == 0 && ShouldMakeMove())
                        {
                        // Move aside
                        newDirection = MonsterMovement.MonsterRandom.Next(2) == 0 ? Direction.Left : Direction.Right;
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
                    newDirection = MonsterMovement.MonsterRandom.Next(5) == 0 ? MonsterMovement.GetRandomPerpendicularDirection(monster.Direction) : monster.Direction.Reversed();
                    }
                else if (MonsterMovement.MonsterRandom.Next(16) == 0)
                    {
                    newDirection = MonsterMovement.GetRandomPerpendicularDirection(monster.Direction);
                    }
                else
                    {
                    newDirection = monster.Direction;
                    }
                }
            return newDirection;
            }

        private static bool ShouldMakeMove()
            {
            return MonsterMovement.MonsterRandom.Next(8) == 0; 
            }
        }
    }