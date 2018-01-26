using System;
using JetBrains.Annotations;
using Labyrinth.GameObjects;

namespace Labyrinth.Services.ScoreKeeper
    {
    class ScoreKeeper : IScoreKeeper
        {
        private decimal _score;

        public void Reset()
            {
            this._score = 0;
            }

        public void EnemyShot([NotNull] IMonster monster, int energyRemoved)
            {
            if (monster == null)
                throw new ArgumentNullException(nameof(monster));

            var increaseToScore = (energyRemoved >> 1) + 1;
            this._score += increaseToScore;
            }

        public void EnemyCrushed([NotNull] IMonster monster, int energyRemoved)
            {
            if (monster == null)
                throw new ArgumentNullException(nameof(monster));

            if (!IsMonsterDangerous(monster))
                return;
            var increaseToScore = ((energyRemoved >> 1) + 1) << 1;
            this._score += increaseToScore;
            }

        private static bool IsMonsterDangerous(IMonster monster)
            {
            if (monster.IsArmed())
                return true;
            if (!monster.IsStationary)
                return true;
            if (monster.IsEgg)
                return true;
            return false;
            }

        public void CrystalTaken([NotNull] IValuable crystal)
            {
            if (crystal == null)
                throw new ArgumentNullException(nameof(crystal));
            this._score += crystal.Score;
            }

        public decimal CurrentScore
            {
            get
                {
                var result = this._score * 10m;
                return result;
                }
            }
        }
    }
