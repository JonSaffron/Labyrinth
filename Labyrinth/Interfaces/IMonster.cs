namespace Labyrinth
    {
    public interface IMonster
        {
        MonsterShootBehaviour MonsterShootBehaviour { get; }
        bool IsStill { get; }
        bool IsEgg { get; }
        }
    }
