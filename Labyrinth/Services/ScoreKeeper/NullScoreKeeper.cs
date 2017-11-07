﻿namespace Labyrinth.Services.ScoreKeeper
    {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    class NullScoreKeeper : IScoreKeeper
        {
        public void Reset()
            {
            // do nothing
            }

        public void EnemyShot(IMonster monster, int energyRemoved)
            {
            // do nothing
            }

        public void EnemyCrushed(IMonster monster, int energyRemoved)
            {
            // do nothing
            }

        public void CrystalTaken(IValuable crystal)
            {
            // do nothing
            }

        public decimal CurrentScore => 0;
        }
    }
