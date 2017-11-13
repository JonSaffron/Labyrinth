using System;
using Labyrinth.Services.ScoreKeeper;
using NUnit.Framework;
using Moq;

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

            var scoreKeeper = new ScoreKeeper();
            Assert.Throws<ArgumentNullException>(() => scoreKeeper.EnemyShot(null, 99));
            scoreKeeper.EnemyShot(monster.Object, 10);

            Assert.AreEqual(60, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyShoots()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterShoots => monsterShoots.ShootsAtPlayer).Returns(true);

            var scoreKeeper = new ScoreKeeper();
            Assert.Throws<ArgumentNullException>(() => scoreKeeper.EnemyCrushed(null, 99));
            scoreKeeper.EnemyCrushed(monster.Object, 10);

            Assert.AreEqual(120, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyMoves()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterMoves => monsterMoves.IsStatic).Returns(false);

            var scoreKeeper = new ScoreKeeper();
            scoreKeeper.EnemyCrushed(monster.Object, 10);

            Assert.AreEqual(120, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyIsEgg()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterIsEgg => monsterIsEgg.ShootsAtPlayer).Returns(false);
            monster.Setup(monsterIsEgg => monsterIsEgg.IsStatic).Returns(true);
            monster.Setup(monsterIsEgg => monsterIsEgg.IsEgg).Returns(true);

            var scoreKeeper = new ScoreKeeper();
            scoreKeeper.EnemyCrushed(monster.Object, 10);

            Assert.AreEqual(120, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyIsNotDangerous()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterShoots => monsterShoots.ShootsAtPlayer).Returns(false);
            monster.Setup(monsterMoves => monsterMoves.IsStatic).Returns(true);
            monster.Setup(monsterIsEgg => monsterIsEgg.IsEgg).Returns(false);

            var scoreKeeper = new ScoreKeeper();
            scoreKeeper.EnemyCrushed(monster.Object, 10);

            Assert.AreEqual(0, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestValuableTaken()
            {
            var valuable = new Mock<IValuable>();
            valuable.Setup(valuableWithScore => valuableWithScore.Score).Returns(100);

            var scoreKeeper = new ScoreKeeper();
            Assert.Throws<ArgumentNullException>(() => scoreKeeper.CrystalTaken(null));
            scoreKeeper.CrystalTaken(valuable.Object);

            Assert.AreEqual(1000, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestReset()
            {
            var valuable = new Mock<IValuable>();
            valuable.Setup(valuableWithScore => valuableWithScore.Score).Returns(100);

            var scoreKeeper = new ScoreKeeper();
            scoreKeeper.CrystalTaken(valuable.Object);
            Assert.AreNotEqual(0, scoreKeeper.CurrentScore);
            scoreKeeper.Reset();

            Assert.AreEqual(0, scoreKeeper.CurrentScore);
            }
        }
    }
