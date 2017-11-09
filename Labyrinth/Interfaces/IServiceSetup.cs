namespace Labyrinth
    {
    public interface IServiceSetup
        {
        IWorldLoader WorldLoader { get; }
        void Setup(Game1 game);
        }
    }
