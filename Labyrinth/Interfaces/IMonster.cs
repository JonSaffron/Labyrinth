namespace Labyrinth
    {
    /// <summary>
    /// Bit of a hack to enable score-keeping
    /// </summary>
    public interface IMonster
        {
        bool ShootsAtPlayer { get; }
        bool ShootsOnceProvoked { get; }
        bool IsStatic { get; }
        bool IsEgg { get; }
        }
    }
