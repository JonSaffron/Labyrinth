using System;
using Labyrinth.GameObjects;
using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth
    {
    class FullPursuit : IMonsterMovement
        {
        public virtual Direction DetermineDirection(Monster monster)
            {
            Direction result = DetermineDirectionTowardsPlayer(monster);
            if (result == Direction.None)
                result = MonsterMovement.RandomDirection();
            
            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);

            Vector2 potentiallyMovingTowards = monster.TilePosition.GetPositionAfterOneMove(result).ToPosition();
            Vector2 diff = (potentiallyMovingTowards - GlobalServices.GameState.Player.Position) / Tile.Size;
            float tilesToPlayer = Math.Min(Math.Abs(diff.X), Math.Abs(diff.Y));
            if (tilesToPlayer <= 2)
                result = MonsterMovement.AlterDirection(result);

            return result;
            }

        protected static Direction DetermineDirectionTowardsPlayer(Monster m)
            {
            Vector2 diff = (m.Position - GlobalServices.GameState.Player.Position);
            double hMove = MonsterMovement.MonsterRandom.NextDouble() * Math.Abs(diff.X);
            double vMove = MonsterMovement.MonsterRandom.NextDouble() * Math.Abs(diff.Y);
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
        }
    }