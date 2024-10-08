﻿using System;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.Services.Messages;
using Labyrinth.GameObjects;

namespace Labyrinth.Services.ScoreKeeper
    {
    internal class ScoreKeeper : IScoreKeeper, IDisposable
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
            this._score = 0m;
            }

        private void MonsterShot(MonsterShot monsterShot)
            {
            // no score from a rebound or from an enemy shot
            if (monsterShot.Munition is IStandardShot standardShot && (standardShot.HasRebounded || !(standardShot.Originator is IPlayer)))
                return;

            var energyRemoved = Math.Min(monsterShot.Monster.Energy, monsterShot.Munition.Energy);
            var increaseToScore = (energyRemoved >> 1) + 1;
            this._score += increaseToScore;
            }

        private void EnemyCrushed(MonsterCrushed monsterCrushed)
            {
            var doesMonsterScore = monsterCrushed.Monster.Properties.Get(GameObjectProperties.MonsterScoresWhenKilled);
            if (!doesMonsterScore)
                return;
            var increaseToScore = ((monsterCrushed.Monster.Energy >> 1) + 1) << 1;
            this._score += increaseToScore;
            }

        private void CrystalTaken(CrystalTaken crystalTaken)
            {
            this._score += crystalTaken.Crystal.Score;
            }

        public decimal CurrentScore => this._score * 10m;

        public void Dispose()
            {
            Messenger.Default.Unregister<MonsterShot>(this);
            Messenger.Default.Unregister<MonsterCrushed>(this);
            Messenger.Default.Unregister<CrystalTaken>(this);
            }
        }
    }
