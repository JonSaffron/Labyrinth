using System;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.Services.Messages;
using Labyrinth.GameObjects;

namespace Labyrinth.Services.ScoreKeeper
    {
    class ScoreKeeper : IScoreKeeper, IDisposable
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
            if (monsterShot.Shot.HasRebounded || !(monsterShot.Shot.Originator is IPlayer)) 
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
            // todo check isextant
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

        public void Dispose()
            {
            Messenger.Default.Unregister<MonsterShot>(this);
            Messenger.Default.Unregister<MonsterCrushed>(this);
            Messenger.Default.Unregister<CrystalTaken>(this);
            }
        }
    }
