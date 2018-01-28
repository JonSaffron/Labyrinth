using System;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Labyrinth.Services.Messages;
using Labyrinth.GameObjects;

namespace Labyrinth.Services.ScoreKeeper
    {
    class ScoreKeeper
        {
        private decimal _score;

        public ScoreKeeper()
            {
            Messenger.Default.Register<MonsterShot>(this, EnemyShot);
            Messenger.Default.Register<MonsterCrushed>(this, EnemyCrushed);
            Messenger.Default.Register<CrystalTaken>(this, CrystalTaken);
            }

        public void Reset()
            {
            this._score = 0;
            }

        private void EnemyShot(MonsterShot monsterKilledByShot)
            {
            EnemyShot(monsterKilledByShot.Monster, monsterKilledByShot.EnergyRemoved);
            }

        public void EnemyShot([NotNull] IMonster monster, int energyRemoved)
            {
            if (monster == null)
                throw new ArgumentNullException(nameof(monster));

            var increaseToScore = (energyRemoved >> 1) + 1;
            this._score += increaseToScore;
            }

        private void EnemyCrushed(MonsterCrushed monsterCrushed)
            {
            EnemyCrushed(monsterCrushed.Monster, monsterCrushed.EnergyRemoved);
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

        private void CrystalTaken(CrystalTaken crystalTaken)
            {
            CrystalTaken(crystalTaken.Crystal);
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
