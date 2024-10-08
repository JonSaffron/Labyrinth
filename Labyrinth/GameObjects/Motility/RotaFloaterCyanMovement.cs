using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    [UsedImplicitly]
    internal class RotaFloaterCyanMovement : StandardRolling
        {
        public RotaFloaterCyanMovement(Monster monster) : base(monster)
            {
            }
            
        public override ConfirmedDirection GetDirection()
            {
            var result = 
                ShouldMakeAnAggressiveMove(this.Monster) 
                    ? GetIntendedDirection(this.Monster) 
                    : base.GetDesiredDirection();
            return base.GetConfirmedDirection(result);
            }

        private PossibleDirection GetIntendedDirection(Monster monster)
            {
            TilePos tp = monster.TilePosition;
            TilePos playerPosition = GlobalServices.GameState.Player.TilePosition;
            int yDiff = tp.Y - playerPosition.Y;    // +ve and the monster is below, -ve and the monster is above
            int xDiff = tp.X - playerPosition.X;    // +ve and the monster is to the right, -ve and the monster is to the left

            var orientation = this.CurrentDirection.Orientation();
            if (orientation == Orientation.Horizontal)
                {
                if (yDiff > 0)
                    return PossibleDirection.Up;
                if (yDiff < 0)
                    return PossibleDirection.Down;
                }
            else if (orientation == Orientation.Vertical)
                {
                if (xDiff > 0)
                    return PossibleDirection.Left;
                if (xDiff < 0)
                    return PossibleDirection.Right;
                }

            return MonsterMovement.RandomDirection();
            }

        private static bool ShouldMakeAnAggressiveMove(Monster monster)
            {
            var result = GlobalServices.Randomness.Next(7) == 0 && monster.IsPlayerNearby();
            return result;
            }
        }
    }
