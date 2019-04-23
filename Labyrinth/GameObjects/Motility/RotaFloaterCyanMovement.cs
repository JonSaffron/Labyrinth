using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    class RotaFloaterCyanMovement : StandardRolling
        {
        public RotaFloaterCyanMovement([NotNull] Monster monster) : base(monster)
            {
            }
            
        public override Direction GetDirection()
            {
            var result = 
                ShouldMakeAnAggressiveMove(this.Monster) 
                    ? GetIntendedDirection(this.Monster) 
                    : base.GetDesiredDirection();
            return base.GetConfirmedSafeDirection(result);
            }

        private SelectedDirection GetIntendedDirection(Monster monster)
            {
            TilePos tp = monster.TilePosition;
            TilePos playerPosition = GlobalServices.GameState.Player.TilePosition;
            int yDiff = tp.Y - playerPosition.Y;    // +ve and the monster is below, -ve and the monster is above
            int xDiff = tp.X - playerPosition.X;    // +ve and the monster is to the right, -ve and the monster is to the left

            var orientation = this.CurrentDirection.Orientation();
            if (orientation == Orientation.Horizontal)
                {
                if (yDiff > 0)
                    return SelectedDirection.UnsafeDirection(Direction.Up);
                if (yDiff < 0)
                    return SelectedDirection.UnsafeDirection(Direction.Down);
                }
            else if (orientation == Orientation.Vertical)
                {
                if (xDiff > 0)
                    return SelectedDirection.UnsafeDirection(Direction.Left);
                if (xDiff < 0)
                    return SelectedDirection.UnsafeDirection(Direction.Right);
                }

            return MonsterMovement.RandomDirection();
            }

        private static bool ShouldMakeAnAggressiveMove(Monster monster)
            {
            var result = GlobalServices.Randomness.Next(7) == 0 && MonsterMovement.IsPlayerNearby(monster);
            return result;
            }
        }
    }
