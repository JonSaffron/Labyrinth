namespace Labyrinth.Services.ScoreKeeper
    {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    class NullScoreKeeper : IScoreKeeper
        {
        public void Reset()
            {
            // do nothing
            }

        public decimal CurrentScore => 0;
        }
    }
