using System;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using Labyrinth.Services.Messages;
using Labyrinth.GameObjects;

namespace Labyrinth.Services.ScoreKeeper
    {
    class ScoreKeeper : IScoreKeeper
        {
        private decimal _score;

        public ScoreKeeper()
            {
            Messenger.Default.Register<MonsterShot>(this, MonsterShot);
            Messenger.Default.Register<MonsterCrushed>(this, EnemyCrushed);
            Messenger.Default.Register<CrystalTaken>(this, CrystalTaken);
            }

        public void Reset()
            {
            this._score = 0;
            }

        private void MonsterShot(MonsterShot monsterShot)
            {
            // no score from a rebound or from an enemy shot
            if (monsterShot.Shot.HasRebounded || !(shot.Originator is Player) || monster == null) 
                return;

            var energyRemoved = Math.Min(monsterShot.Monster.Energy, monsterShot.Shot.Energy );
            var increaseToScore = (energyRemoved >> 1) + 1;
            this._score += increaseToScore;
            }

        private void EnemyCrushed(MonsterCrushed monsterCrushed)
            {
            if (!IsMonsterDangerous(monsterCrushed.Monster))
                return;
            var increaseToScore = ((monsterCrushed.Monster.Energy >> 1) + 1) << 1;
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
            this._score += crystalTaken.Crystal.Score;
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
