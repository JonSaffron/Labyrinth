using System;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    interface IScoreKeeper
        {
        void Reset();
        void EnemyShot(Monster monster, int energyRemoved);
        void EnemyCrushed(Monster monster, int energyRemoved);
        void CrystalTaken(Crystal crystal);
        [Obsolete] void AddToScore(int score);

        decimal CurrentScore { get; }
        }
    }
