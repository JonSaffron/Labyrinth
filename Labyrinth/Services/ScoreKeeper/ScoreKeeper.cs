using System;

namespace Labyrinth.Services.ScoreKeeper
    {
    class ScoreKeeper : IScoreKeeper
        {
        private decimal _score;

        public void Reset()
            {
            this._score = 0;
            }

        public void EnemyShot(IMonster monster, int energyRemoved)
            {
            if (monster == null)
                throw new ArgumentNullException("monster");

            var increaseToScore = (energyRemoved >> 1) + 1;
            this._score += increaseToScore;
            }

        public void EnemyCrushed(IMonster monster, int energyRemoved)
            {
            if (monster == null)
                throw new ArgumentNullException("monster");

            if (!IsMonsterDangerous(monster))
                return;
            var increaseToScore = ((energyRemoved >> 1) + 1) << 1;
            this._score += increaseToScore;
            }

        private static bool IsMonsterDangerous(IMonster monster)
            {
            if (monster.MonsterShootBehaviour != MonsterShootBehaviour.None)
                return true;
            if (!monster.IsStill)
                return true;
            if (monster.IsEgg)
                return true;
            return false;
            }

        public void CrystalTaken(IValuable crystal)
            {
            if (crystal == null)
                throw new ArgumentNullException("crystal");
            this._score += crystal.Score;
            }

        public decimal CurrentScore
            {
            get
                {
                var result = this._score * 10;
                return result;
                }
            }
        }
    }
