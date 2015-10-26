using System;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    public interface IScoreKeeper
        {
        void Reset();
        void EnemyShot(Monster monster, int energyRemoved);
        void EnemyCrushed(Monster monster, int energyRemoved);
        void CrystalTaken(Crystal crystal);

        decimal CurrentScore { get; }
        }
    }
