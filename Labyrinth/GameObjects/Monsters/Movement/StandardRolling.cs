using Labyrinth.Services.WorldBuilding;
using Microsoft.Xna.Framework;

namespace Labyrinth.GameObjects.Movement
    {
    // todo: this should not inherit from standardpatrolling

    class StandardRolling : StandardPatrolling
        {
        public StandardRolling(Direction initialDirection) : base(initialDirection)
            {
            }

        public StandardRolling()
            {
            }

        public override Direction DetermineDirection(Monster monster)
            {
            var intendedDirection = GetIntendedDirection(monster);
            var result = MonsterMovement.UpdateDirectionWhereMovementBlocked(monster, intendedDirection);
            this.CurrentDirection = result;
            return result;
            }

        private Direction GetIntendedDirection(Monster monster)
            {
            if (this.CurrentDirection == Direction.None)
                this.CurrentDirection = MonsterMovement.RandomDirection();
            bool changeDirection = GlobalServices.Randomess.Test(7);
            var result = changeDirection ? MonsterMovement.AlterDirection(this.CurrentDirection) : this.GetIntendedDirectionForPatrol(monster);
            return result;
            }

        protected static bool IsPlayerInSight(Monster monster)
            {
            if (!GlobalServices.GameState.Player.IsAlive())
                return false;
            var distance = Vector2.Distance(monster.Position, GlobalServices.GameState.Player.Position) / Constants.TileLength;
            var result = distance <= 20;
            return result;
            }
        }
    }
