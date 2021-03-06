﻿using System;
using Labyrinth.Services.Messages;
using Labyrinth.Services.ScoreKeeper;
using NUnit.Framework;
using Moq;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;

// ReSharper disable ObjectCreationAsStatement
// ReSharper disable AssignNullToNotNullAttribute

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestScoreKeeper
        {
        [Test]
        public void TestInvalidMonsterShotMessage()
            {
            var monster = new Mock<IMonster>();
            var shot = new Mock<IMunition>();

            Assert.Throws<ArgumentNullException>(() => new MonsterShot(monster.Object, null));
            Assert.Throws<ArgumentNullException>(() => new MonsterShot(null, shot.Object));
            }
        
        [Test]
        public void TestEnemyShotByPlayer()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(m => m.Energy).Returns(5);
            var shot = new Mock<IMunition>();
            var player = new Mock<IPlayer>();
            shot.Setup(s => s.Originator).Returns(player.Object);

            var expected = new[] { 10, 10, 20, 20, 30, 30, 30, 30, 30, 30, 30 };
            for (int i = 0; i <= 10; i++)
                {
                shot.Setup(s => s.Energy).Returns(i);
                
                using (var scoreKeeper = new ScoreKeeper())
                    {
                    MonsterShot monsterShot = new MonsterShot(monster.Object, shot.Object);
                    Messenger.Default.Send(monsterShot);
                    Assert.AreEqual(expected[i], scoreKeeper.CurrentScore);
                    }
                }
            }

        [Test]
        public void TestEnemyShotByOtherMonster()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(m => m.Energy).Returns(5);
            var shot = new Mock<IStandardShot>();
            var otherMonster = new Mock<IMonster>();
            shot.Setup(s => s.Originator).Returns(otherMonster.Object);
            shot.Setup(s => s.Energy).Returns(10);
                
            using (var scoreKeeper = new ScoreKeeper())
                {
                MonsterShot monsterShot = new MonsterShot(monster.Object, shot.Object);
                Messenger.Default.Send(monsterShot);
                Assert.AreEqual(0, scoreKeeper.CurrentScore);
                }
            }

        [Test]
        public void TestEnemyShotByRebound()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(m => m.Energy).Returns(5);
            var shot = new Mock<IStandardShot>();
            var player = new Mock<IPlayer>();
            shot.Setup(s => s.Originator).Returns(player.Object);
            shot.Setup(s => s.Energy).Returns(10);
            shot.Setup(s => s.HasRebounded).Returns(true);
                
            using (var scoreKeeper = new ScoreKeeper())
                {
                MonsterShot monsterShot = new MonsterShot(monster.Object, shot.Object);
                Messenger.Default.Send(monsterShot);
                Assert.AreEqual(0, scoreKeeper.CurrentScore);
                }
            }
        
        ///////////////////////////////

        [Test]
        public void TestInvalidMonsterCrushedMessage()
            {
            var monster = new Mock<IMonster>();
            var boulder = new Mock<IGameObject>();

            Assert.Throws<ArgumentNullException>(() => new MonsterCrushed(monster.Object, null));
            Assert.Throws<ArgumentNullException>(() => new MonsterCrushed(null, boulder.Object));
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyScores()
            {
            var propertyBag = new PropertyBag();
            propertyBag.Set(GameObjectProperties.MonsterScoresWhenKilled, true);

            var monster = new Mock<IMonster>();
            monster.Setup(m => m.Energy).Returns(5);
            monster.Setup(m => m.Properties).Returns(propertyBag);
            var boulder = new Mock<IGameObject>();

            using (var scoreKeeper = new ScoreKeeper())
                {
                var monsterCrushed = new MonsterCrushed(monster.Object, boulder.Object);
                Messenger.Default.Send(monsterCrushed);

                Assert.AreEqual(60, scoreKeeper.CurrentScore);
                }
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyDoesNotScore()
            {
            var propertyBag = new PropertyBag();
            propertyBag.Set(GameObjectProperties.MonsterScoresWhenKilled, false);

            var monster = new Mock<IMonster>();
            monster.Setup(monsterShoots => monsterShoots.Energy).Returns(10);
            monster.Setup(m => m.Properties).Returns(propertyBag);
            var boulder = new Mock<IGameObject>();

            using (var scoreKeeper = new ScoreKeeper())
                {
                var monsterCrushed = new MonsterCrushed(monster.Object, boulder.Object);
                Messenger.Default.Send(monsterCrushed);

                Assert.AreEqual(0, scoreKeeper.CurrentScore);
                }
            }

        ///////////////////////////////

        [Test]
        public void TestInvalidValuableTakenMessage()
            {
            Assert.Throws<ArgumentNullException>(() => new CrystalTaken(null));
            }

        [Test]
        public void TestValuableTaken()
            {
            var valuable = new Mock<IValuable>();
            valuable.Setup(valuableWithScore => valuableWithScore.Score).Returns(100);

            using (var scoreKeeper = new ScoreKeeper())
                {
                var crystalTaken = new CrystalTaken(valuable.Object);
                Messenger.Default.Send(crystalTaken);

                Assert.AreEqual(1000, scoreKeeper.CurrentScore);
                }
            }

        ///////////////////////////////

        [Test]
        public void TestReset()
            {
            var valuable = new Mock<IValuable>();
            valuable.Setup(valuableWithScore => valuableWithScore.Score).Returns(100);

            using (var scoreKeeper = new ScoreKeeper())
                {
                var crystalTaken = new CrystalTaken(valuable.Object);
                Messenger.Default.Send(crystalTaken);

                Assert.AreNotEqual(0, scoreKeeper.CurrentScore);
                scoreKeeper.Reset();

                Assert.AreEqual(0, scoreKeeper.CurrentScore);
                }
            }
        }
    }
