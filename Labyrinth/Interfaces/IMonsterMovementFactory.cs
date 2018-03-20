using Labyrinth.GameObjects;

namespace Labyrinth
    {
    internal interface IMonsterMovementFactory
        {
        IMonsterMotion StandardPatrolling(Monster monster, Direction initialDirection);
        IMonsterMotion StandardRolling(Monster monster, Direction initialDirection);
        IMonsterMotion FullPursuit(Monster monster);
        IMonsterMotion Cautious(Monster monster);
        IMonsterMotion SemiAggressive(Monster monster);
        IMonsterMotion Placid(Monster monster);
        IMonsterMotion KillerCubeRedMovement(Monster monster);
        IMonsterMotion RotaFloaterCyanMovement(Monster monster);
        IMonsterMotion PatrolPerimiter(Monster monster, Direction initialDirection);
        }
    }
