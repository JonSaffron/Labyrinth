namespace Labyrinth
    {
    public interface IServiceSetup
        {
        IWorldLoader WorldLoader { get; }
        IScoreKeeper ScoreKeeper { get; }
        void Setup(Game1 game);
        }
    }
