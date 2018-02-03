using System;
using Labyrinth.GameObjects.Actions;
using Labyrinth.Services.Messages;
using Labyrinth.Services.ScoreKeeper;
using NUnit.Framework;
using Moq;
using GalaSoft.MvvmLight.Messaging;
using Labyrinth.GameObjects;

// ReSharper disable AssignNullToNotNullAttribute

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestScoreKeeper
        {
        [Test]
        public void TestEnemyShot()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(m => m.Energy).Returns(5);
            var shot = new Mock<IShot>();
            shot.Setup(s => s.Energy).Returns(10);
            var player = new Mock<IPlayer>();
            shot.Setup(s => s.Originator).Returns(player.Object);

            var scoreKeeper = new ScoreKeeper();
            MonsterShot monsterShot;
            Assert.Throws<ArgumentNullException>(() => monsterShot = new MonsterShot(monster.Object, null));
            Assert.Throws<ArgumentNullException>(() => monsterShot = new MonsterShot(null, shot.Object));
            monsterShot = new MonsterShot(monster.Object, shot.Object);
            Messenger.Default.Send(monsterShot);

            Assert.AreEqual(30, scoreKeeper.CurrentScore);
            }

        ///////////////////////////////

        [Test]
        public void TestEnemyCrushedWhenEnemyShoots()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterShoots => monsterShoots.Energy).Returns(10);
            monster.Setup(monsterShoots => monsterShoots.HasBehaviour<ShootsAtPlayer>()).Returns(true);
            var boulder = new Mock<IGameObject>();

            var scoreKeeper = new ScoreKeeper();
            var monsterCrushed = new MonsterCrushed(monster.Object, boulder.Object);
            Messenger.Default.Send(monsterCrushed);

            Assert.AreEqual(120, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyMoves()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterShoots => monsterShoots.Energy).Returns(10);
            monster.Setup(monsterMoves => monsterMoves.IsStationary).Returns(false);
            var boulder = new Mock<IGameObject>();

            var scoreKeeper = new ScoreKeeper();
            var monsterCrushed = new MonsterCrushed(monster.Object, boulder.Object);
            Messenger.Default.Send(monsterCrushed);

            Assert.AreEqual(120, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyIsEgg()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterShoots => monsterShoots.Energy).Returns(10);
            monster.Setup(monsterIsEgg => monsterIsEgg.HasBehaviour<ShootsAtPlayer>()).Returns(false);
            monster.Setup(monsterIsEgg => monsterIsEgg.IsStationary).Returns(true);
            monster.Setup(monsterIsEgg => monsterIsEgg.IsEgg).Returns(true);
            var boulder = new Mock<IGameObject>();

            var scoreKeeper = new ScoreKeeper();
            var monsterCrushed = new MonsterCrushed(monster.Object, boulder.Object);
            Messenger.Default.Send(monsterCrushed);

            Assert.AreEqual(120, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyIsNotDangerous()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterShoots => monsterShoots.Energy).Returns(10);
            monster.Setup(monsterShoots => monsterShoots.HasBehaviour<ShootsAtPlayer>()).Returns(false);
            monster.Setup(monsterMoves => monsterMoves.IsStationary).Returns(true);
            monster.Setup(monsterIsEgg => monsterIsEgg.IsEgg).Returns(false);
            var boulder = new Mock<IGameObject>();

            var scoreKeeper = new ScoreKeeper();
            var monsterCrushed = new MonsterCrushed(monster.Object, boulder.Object);
            Messenger.Default.Send(monsterCrushed);

            Assert.AreEqual(0, scoreKeeper.CurrentScore);
            }

        ///////////////////////////////

        [Test]
        public void TestValuableTaken()
            {
            var valuable = new Mock<IValuable>();
            valuable.Setup(valuableWithScore => valuableWithScore.Score).Returns(100);

            var scoreKeeper = new ScoreKeeper();
            //Assert.Throws<ArgumentNullException>(() => scoreKeeper.CrystalTaken(null));
            //scoreKeeper.CrystalTaken(valuable.Object);

            Assert.AreEqual(1000, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestReset()
            {
            var valuable = new Mock<IValuable>();
            valuable.Setup(valuableWithScore => valuableWithScore.Score).Returns(100);

            var scoreKeeper = new ScoreKeeper();
            //scoreKeeper.CrystalTaken(valuable.Object);
            Assert.AreNotEqual(0, scoreKeeper.CurrentScore);
            scoreKeeper.Reset();

            Assert.AreEqual(0, scoreKeeper.CurrentScore);
            }
        }
    }
