using System;
using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    internal class KillerCubeRedMovement : StandardRolling
        {
        public KillerCubeRedMovement(Monster monster) : base(monster)
            {
            }

        public override ConfirmedDirection GetDirection()
            {
            var direction = ShouldMakeAnAggressiveMove() ? GetIntendedDirection() : base.GetDesiredDirection();
            var result = base.GetConfirmedDirection(direction);
            return result;
            }

        private PossibleDirection GetIntendedDirection()
            {
            TilePos tp = this.Monster.TilePosition;
                
            PossibleDirection newDirection;
            TilePos playerPosition = GlobalServices.GameState.Player.TilePosition;
            int yDiff = tp.Y - playerPosition.Y;    // +ve and the player is above, -ve and the player is below
            int xDiff = tp.X - playerPosition.X;    // +ve and the player is to the left, -ve and the player is to the right
                
            // if on the same row or column as the player then will be at risk of being shot
            if ((xDiff == 0 || yDiff == 0) && ShouldMakeMoveToAvoidTrouble())
                {
                newDirection = GetRandomPerpendicularDirection(this.CurrentDirection);
                }
            else if ((this.CurrentDirection == Direction.Left && xDiff <= -5) || (this.CurrentDirection == Direction.Right && xDiff >= 5))
                {
                newDirection = yDiff > 0 ? PossibleDirection.Up : PossibleDirection.Down;
                }
            else if ((this.CurrentDirection == Direction.Up && yDiff <= -5) || (this.CurrentDirection == Direction.Down && yDiff >= 5))
                {
                newDirection = xDiff > 0 ? PossibleDirection.Left : PossibleDirection.Right;
                }
            else if (GlobalServices.Randomness.Next(16) == 0)
                {
                newDirection = GetRandomPerpendicularDirection(this.CurrentDirection);
                }
            else
                newDirection = new PossibleDirection(this.CurrentDirection);
                
            return newDirection;
            }

        private static bool ShouldMakeMoveToAvoidTrouble()
            {
            return GlobalServices.Randomness.Next(8) == 0; 
            }

        private static PossibleDirection GetRandomPerpendicularDirection(Direction currentDirection)
            {
            if (currentDirection == Direction.None)
                return MonsterMovement.RandomDirection();

            bool whichWay = GlobalServices.Randomness.Next(2) == 0;
            var orientation = currentDirection.Orientation();
            switch (orientation)
                {
                case Orientation.Horizontal:
                    return whichWay ? PossibleDirection.Up : PossibleDirection.Down;
                case Orientation.Vertical:
                    return whichWay ? PossibleDirection.Left : PossibleDirection.Right;
                }
            throw new InvalidOperationException();
            }

        private bool ShouldMakeAnAggressiveMove()
            {
            return this.Monster.IsPlayerNearby();
            }
        }
    }
