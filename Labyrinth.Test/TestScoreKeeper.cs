using Labyrinth.Services.ScoreKeeper;
using NUnit.Framework;
using Moq;

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
            scoreKeeper.EnemyShot(monster.Object, 10);

            Assert.AreEqual(60, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyShoots()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterShoots => monsterShoots.MonsterShootBehaviour).Returns(MonsterShootBehaviour.ShootsImmediately);

            var scoreKeeper = new ScoreKeeper();
            scoreKeeper.EnemyCrushed(monster.Object, 10);

            Assert.AreEqual(120, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyMoves()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterMoves => monsterMoves.IsStill).Returns(false);

            var scoreKeeper = new ScoreKeeper();
            scoreKeeper.EnemyCrushed(monster.Object, 10);

            Assert.AreEqual(120, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyIsEgg()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterIsEgg => monsterIsEgg.IsEgg).Returns(true);

            var scoreKeeper = new ScoreKeeper();
            scoreKeeper.EnemyCrushed(monster.Object, 10);

            Assert.AreEqual(120, scoreKeeper.CurrentScore);
            }

        [Test]
        public void TestEnemyCrushedWhenEnemyIsNotDangerous()
            {
            var monster = new Mock<IMonster>();
            monster.Setup(monsterShoots => monsterShoots.MonsterShootBehaviour).Returns(MonsterShootBehaviour.None);
            monster.Setup(monsterMoves => monsterMoves.IsStill).Returns(true);
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
