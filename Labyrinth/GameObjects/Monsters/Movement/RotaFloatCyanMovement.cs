namespace Labyrinth.GameObjects.Movement
    {
    class RotaFloatCyanMovement : StandardRolling
        {
        public override Direction DetermineDirection(Monster monster)
            {
            var intendedDirection = 
                ShouldMakeAnAggressiveMove(monster) 
                    ? GetIntendedDirection(monster) 
                    : base.GetIntendedDirection(monster);

            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            this.CurrentDirection = result;
            return result;
            }

        private new Direction GetIntendedDirection(Monster monster)
            {
            TilePos tp = monster.TilePosition;
            TilePos playerPosition = GlobalServices.GameState.Player.TilePosition;
            int yDiff = tp.Y - playerPosition.Y;    // +ve and the monster is below, -ve and the monster is above
            int xDiff = tp.X - playerPosition.X;    // +ve and the monster is to the right, -ve and the monster is to the left

            var orientation = this.CurrentDirection.Orientation();
            if (orientation == Orientation.Horizontal)
                {
                if (yDiff > 0)
                    return Direction.Up;
                if (yDiff < 0)
                    return Direction.Down;
                }
            else if (orientation == Orientation.Vertical)
                {
                if (xDiff > 0)
                    return Direction.Left;
                if (xDiff < 0)
                    return Direction.Right;
                }

            return MonsterMovement.RandomDirection();
            }

        private static bool ShouldMakeAnAggressiveMove(Monster monster)
            {
            var result = GlobalServices.Randomess.Next(7) == 0 && MonsterMovement.IsPlayerNearby(monster);
            return result;
            }
        }
    }
