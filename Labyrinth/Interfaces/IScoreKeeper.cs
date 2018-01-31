
namespace Labyrinth
    {
    public interface IScoreKeeper
        {
        void Reset();
        decimal CurrentScore { get; }
        }
    }
