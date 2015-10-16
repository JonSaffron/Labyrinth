using System;
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

        public void EnemyShot(Monster monster, int energyRemoved)
            {
            if (monster == null)
                throw new ArgumentNullException("monster");

            var increaseToScore = (energyRemoved >> 1) + 1;
            this._score += increaseToScore;
            }

        public void EnemyCrushed(Monster monster, int energyRemoved)
            {
            if (monster == null)
                throw new ArgumentNullException("monster");

            // todo: perhaps monsters that are not moving and don't shoot and are not eggs
            if (monster is DeathCube)
                return;
            var increaseToScore = ((energyRemoved >> 1) + 1) << 1;
            this._score += increaseToScore;
            }

        public void CrystalTaken(Crystal crystal)
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
