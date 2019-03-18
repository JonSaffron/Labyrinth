using JetBrains.Annotations;
using Labyrinth.DataStructures;

namespace Labyrinth.GameObjects.Motility
    {
    class RotaFloaterCyanMovement : StandardRolling
        {
        public RotaFloaterCyanMovement([NotNull] Monster monster) : base(monster)
            {
            }
            
        private Direction GetIntendedDirection(Monster monster)
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
        public override Direction DetermineDirection()
            {
            var result = 
                ShouldMakeAnAggressiveMove(this.Monster) 
                    ? GetIntendedDirection(this.Monster) 
                    : base.DetermineDirection();
            return result;
            }

        private static bool ShouldMakeAnAggressiveMove(Monster monster)
            {
            var result = GlobalServices.Randomness.Next(7) == 0 && MonsterMovement.IsPlayerNearby(monster);
            return result;
            }
        }
    }
