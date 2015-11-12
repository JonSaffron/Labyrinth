using System;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects.Movement
    {
    class KillerCubeRedMovement : StandardRolling
        {
        public override Direction DetermineDirection(Monster monster)
            {
            if (!IsPlayerInSight(monster) || this.CurrentDirection == Direction.None)
                return base.DetermineDirection(monster);

            TilePos tp = monster.TilePosition;
                
            Direction newDirection;
            Player p = GlobalServices.GameState.Player;
            TilePos playerPosition = p.TilePosition;
            int yDiff = tp.Y - playerPosition.Y;    // +ve and the player is above, -ve and the player is below
            int xDiff = tp.X - playerPosition.X;    // +ve and the player is to the left, -ve and the player is to the right
                
            // if on the same row or column as the player then will be at risk of being shot
            if ((xDiff == 0 || yDiff == 0) && ShouldMakeMove())
                newDirection = GetRandomPerpendicularDirection(this.CurrentDirection);
            else if ((this.CurrentDirection == Direction.Left && xDiff <= -5) || (this.CurrentDirection == Direction.Right && xDiff >= 5))
                {
                newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                }
            else if ((this.CurrentDirection == Direction.Up && yDiff <= -5) || (this.CurrentDirection == Direction.Down && yDiff >= 5))
                {
                newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                }
            else if (MonsterMovement.MonsterRandom.Next(16) == 0)
                {
                newDirection = GetRandomPerpendicularDirection(this.CurrentDirection);
                }
            else
                newDirection = this.CurrentDirection;

            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, newDirection);

            return result;
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
