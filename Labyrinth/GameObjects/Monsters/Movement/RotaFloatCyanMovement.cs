namespace Labyrinth.GameObjects.Movement
    {
    class RotaFloatCyanMovement : StandardRolling
        {
        public override Direction DetermineDirection(Monster monster)
            {
            if (!IsPlayerInSight(monster) || this.CurrentDirection == Direction.None || !ShouldMakeAnAggressiveMove())
                return base.DetermineDirection(monster);

            var intendedDirection = GetIntendedDirection(monster);
            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            this.CurrentDirection = result;
            return result;
            }

        private Direction GetIntendedDirection(Monster monster)
            {
            TilePos tp = monster.TilePosition;
            TilePos playerPosition = GlobalServices.GameState.Player.TilePosition;
            int yDiff = tp.Y - playerPosition.Y;    // +ve and the monster is below, -ve and the monster is above
            int xDiff = tp.X - playerPosition.X;    // +ve and the monster is to the right, -ve and the monster is to the left

            Direction newDirection = this.CurrentDirection;
            if (this.CurrentDirection.IsHorizontal())
                {
                if (yDiff > 0)
                    newDirection = Direction.Up;
                else if (yDiff < 0)
                    newDirection = Direction.Down;
                }
            else if (this.CurrentDirection.IsVertical())
                {
                if (xDiff > 0)
                    newDirection = Direction.Left;
                else if (xDiff < 0)
                    newDirection = Direction.Right;
                }

            return newDirection;
            }

        private static bool ShouldMakeAnAggressiveMove()
            {
            return GlobalServices.Randomess.Next(7) == 0; 
            }
        }
    }