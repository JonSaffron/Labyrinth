using Labyrinth.GameObjects;

namespace Labyrinth.Services.ScoreKeeper
    {
    class NullScoreKeeper : IScoreKeeper
        {
        public void Reset()
            {
            // do nothing
            }

        public void EnemyShot(Monster monster, int energyRemoved)
            {
            // do nothing
            }

        public void EnemyCrushed(Monster monster, int energyRemoved)
            {
            // do nothing
            }

        public void CrystalTaken(Crystal crystal)
            {
            // do nothing
            }

        public decimal CurrentScore
            {
            get
                {
                return 0;
                }
            }
        }
    }
