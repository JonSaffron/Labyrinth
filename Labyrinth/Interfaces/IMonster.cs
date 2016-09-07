namespace Labyrinth
    {
    /// <summary>
    /// Bit of a hack to enable score-keeping
    /// </summary>
    public interface IMonster
        {
        MonsterShootBehaviour ShootBehaviour { get; }
        bool IsStill { get; }
        bool IsEgg { get; }
        }
    }
