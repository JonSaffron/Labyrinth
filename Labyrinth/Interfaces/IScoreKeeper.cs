using System;
using Labyrinth.GameObjects;

namespace Labyrinth
    {
    public interface IScoreKeeper
        {
        void Reset();
        void EnemyShot(IMonster monster, int energyRemoved);
        void EnemyCrushed(IMonster monster, int energyRemoved);
        void CrystalTaken(IValuable valuable);

        decimal CurrentScore { get; }
        }
    }
