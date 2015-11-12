using System;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects.Movement
    {
    class FullPursuit : IMonsterMovement
        {
        public virtual Direction DetermineDirection(Monster monster)
            {
            Direction result = DetermineDirectionTowardsPlayer(monster);
            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);
            if (result == Direction.None)
                return result;

            TilePos potentiallyMovingTowards = monster.TilePosition.GetPositionAfterOneMove(result);
            if (potentiallyMovingTowards == GlobalServices.GameState.Player.TilePosition)
                result = MonsterMovement.AlterDirection(result);

            result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, result);
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