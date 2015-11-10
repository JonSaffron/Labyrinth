using System;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    class RotaFloatCyanMovement : StandardRolling
        {
        public override Direction DetermineDirection(Monster monster)
            {
            var p = GlobalServices.GameState.Player;
            if (!p.IsExtant)
                {
                return base.DetermineDirection(monster);
                }
            
            TilePos tp = monster.TilePosition;
            Direction newDirection = Direction.None;
            TilePos playerPosition = p.TilePosition;
            int yDiff = tp.Y - playerPosition.Y;
            int xDiff = tp.X - playerPosition.X;
            if (monster.Direction == Direction.Left || monster.Direction == Direction.Right)
                {
                if (Math.Abs(tp.X - playerPosition.X) == 1 && Math.Abs(yDiff) > 3 && Math.Abs(yDiff) <= 16 && ShouldMakeAnAggressiveMove())
                    {
                    newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                    }
                else if ((tp.X - playerPosition.X) == 0 && Math.Abs(yDiff) <= 3 && ShouldMakeAnAggressiveMove())
                    {
                    newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                    }
                }
            else if (monster.Direction == Direction.Up || monster.Direction == Direction.Down)
                {
                if (Math.Abs(tp.Y - playerPosition.Y) == 1 && Math.Abs(xDiff) > 3 && Math.Abs(xDiff) <= 16 && ShouldMakeAnAggressiveMove())
                    {
                    newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                    }
                else if ((tp.Y - playerPosition.Y) == 0 && Math.Abs(xDiff) <= 3 && ShouldMakeAnAggressiveMove())
                    {
                    newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                    }
                }
            if (newDirection != Direction.None)
                {
                TilePos pp = tp.GetPositionAfterOneMove(newDirection);
                if (GlobalServices.GameState.CanTileBeOccupied(pp, true))
                    {
                    return newDirection;
                    }
                }

            return base.DetermineDirection(monster);
            }

        private static bool ShouldMakeAnAggressiveMove()
            {
            return MonsterMovement.MonsterRandom.Next(3) == 0; 
            }
        }
    }