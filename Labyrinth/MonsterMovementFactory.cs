using Labyrinth.GameObjects;
using Labyrinth.GameObjects.Movement;

namespace Labyrinth
    {
    class MonsterMovementFactory : IMonsterMovementFactory
        {
        public IMonsterMotion StandardPatrolling(Monster monster, Direction initialDirection)
            {
            var result = new StandardPatrolling(monster, initialDirection);
            return result;
            }

        public IMonsterMotion StandardRolling(Monster monster, Direction initialDirection)
            {
            var result = initialDirection != Direction.None ? new StandardRolling(monster, initialDirection) : new StandardRolling(monster);
            return result;
            }

        public IMonsterMotion FullPursuit(Monster monster)
            {
            var result = new FullPursuit(monster);
            return result;
            }

        public IMonsterMotion Cautious(Monster monster)
            {
            var result = new CautiousPursuit(monster);
            return result;
            }

        public IMonsterMotion SemiAggressive(Monster monster)
            {
            var result = new SemiAggressive(monster);
            return result;
            }

        public IMonsterMotion Placid(Monster monster)
            {
            var result = new Placid(monster);
            return result;
            }

        public IMonsterMotion KillerCubeRedMovement(Monster monster)
            {
            var result = new KillerCubeRedMovement(monster);
            return result;
            }

        public IMonsterMotion RotaFloaterCyanMovement(Monster monster)
            {
            var result = new RotaFloatCyanMovement(monster);
            return result;
            }
        }
    }
