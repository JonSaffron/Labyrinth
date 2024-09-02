namespace Labyrinth.Services.ScoreKeeper
    {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class NullScoreKeeper : IScoreKeeper
        {
        public void Reset()
            {
            // do nothing
            }

        public decimal CurrentScore => 0;
        }
    }
