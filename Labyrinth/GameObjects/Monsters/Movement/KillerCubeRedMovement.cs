namespace Labyrinth.GameObjects.Movement
    {
    class KillerCubeRedMovement : StandardRolling
        {
        public override Direction DetermineDirection(Monster monster)
            {
            var intendedDirection = 
                ShouldMakeAnAggressiveMove(monster)
                    ? GetIntendedDirection(monster)
                    : base.DetermineDirection(monster);

            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            this.CurrentDirection = result;
            return result;
            }

        private new Direction GetIntendedDirection(Monster monster)
            {
            TilePos tp = monster.TilePosition;
                
            Direction newDirection;
            TilePos playerPosition = GlobalServices.GameState.Player.TilePosition;
            int yDiff = tp.Y - playerPosition.Y;    // +ve and the player is above, -ve and the player is below
            int xDiff = tp.X - playerPosition.X;    // +ve and the player is to the left, -ve and the player is to the right
                
            // if on the same row or column as the player then will be at risk of being shot
            if ((xDiff == 0 || yDiff == 0) && ShouldMakeMoveToAvoidTrouble())
                newDirection = GetRandomPerpendicularDirection(this.CurrentDirection);
            else if ((this.CurrentDirection == Direction.Left && xDiff <= -5) || (this.CurrentDirection == Direction.Right && xDiff >= 5))
                {
                newDirection = yDiff > 0 ? Direction.Up : Direction.Down;
                }
            else if ((this.CurrentDirection == Direction.Up && yDiff <= -5) || (this.CurrentDirection == Direction.Down && yDiff >= 5))
                {
                newDirection = xDiff > 0 ? Direction.Left : Direction.Right;
                }
            else if (GlobalServices.Randomess.Next(16) == 0)
                {
                newDirection = GetRandomPerpendicularDirection(this.CurrentDirection);
                }
            else
                newDirection = this.CurrentDirection;
                
            return newDirection;
            }

        private static bool ShouldMakeMoveToAvoidTrouble()
            {
            return GlobalServices.Randomess.Next(8) == 0; 
            }

        private static Direction GetRandomPerpendicularDirection(Direction currentDirection)
            {
            if (currentDirection.IsHorizontal())
                return GlobalServices.Randomess.Next(2) == 0 ? Direction.Up : Direction.Down;
            if (currentDirection.IsVertical())
                return GlobalServices.Randomess.Next(2) == 0 ? Direction.Left : Direction.Right;
            return MonsterMovement.RandomDirection();
            }

        private static bool ShouldMakeAnAggressiveMove(Monster monster)
            {
            var result = MonsterMovement.IsPlayerNearby(monster);
            return result;
            }
        }
    }
