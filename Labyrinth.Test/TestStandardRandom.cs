using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Labyrinth.Services;
using Labyrinth.Services.WorldBuilding;

// ReSharper disable ObjectCreationAsStatement
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable MustUseReturnValue

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestStandardRandom
        {
        [Test]
        public void BasicTest()
            {
            var r = new StandardRandom();
            Assert.IsTrue(r.Test(0));
            Assert.DoesNotThrow(() => r.Test(0b1));
            Assert.DoesNotThrow(() => r.Test(0b11));
            Assert.DoesNotThrow(() => r.Test(0b111));
            Assert.DoesNotThrow(() => r.Test(0b1111));
            Assert.DoesNotThrow(() => r.Test(0b1_1111));
            Assert.DoesNotThrow(() => r.Test(0b11_1111));
            Assert.DoesNotThrow(() => r.Test(0b111_1111));
            Assert.DoesNotThrow(() => r.Test(0b1111_1111));

            for (int i = 0; i <= 1000; i++)
                {
                var t = r.Next(i);
                Assert.GreaterOrEqual(t, 0);
                if (i == 0)
                    Assert.AreEqual(0, i);
                else
                    Assert.Less(t, i);
                }
            }

        [Test]
        public void IsSequenceRepeatable()
            {
            var seed = new Random().Next();

            var r1 = new StandardRandom(seed);
            var referenceSequence = new List<int>();
            for (int i = 0; i <= 1000; i++)
                referenceSequence.Add(r1.Next(100000));

            var r2 = new StandardRandom(seed);
            var compareSequence = new List<int>();
            for (int i = 0; i <= 1000; i++)
                compareSequence.Add(r2.Next(100000));

            Assert.IsTrue(referenceSequence.SequenceEqual(compareSequence));
            }

        [Test]
        public void TestTestResults()
            {
            var seed = new Random().Next();

            var r1 = new StandardRandom(seed);
            var referenceSequence = new List<int>();
            for (int i = 0; i <= 1000; i++)
                {
                referenceSequence.Add(r1.Next(256));
                }

            var r2 = new StandardRandom(seed);
            for (int i = 0; i <= 1000; i++)
                {
                bool t = referenceSequence[i] == 0;
                bool testResult = r2.Test(0xFF);
                Assert.IsTrue(testResult == t);
                }
            }

        [Test]
        public void TestDiceRoll()
            {
            var r = new StandardRandom();

            Assert.Throws<ArgumentOutOfRangeException>(() => r.DiceRoll(new DiceRoll()));
            for (int i = 1; i <= 10; i++)
                {
                for (int j = 1; j <= 10; j++)
                    {
                    var dr = new DiceRoll(i, j);
                    var t = r.DiceRoll(dr);
                    Assert.GreaterOrEqual(t, i);
                    Assert.LessOrEqual(t, i * j);
                    }
                }
            }

        [Test]
        public void TestDiceRollConstructor()
            {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DiceRoll(0, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DiceRoll(10, 0));
            Assert.DoesNotThrow(() => new DiceRoll(1, 1));

            Assert.Throws<FormatException>(() => new DiceRoll(""));
            Assert.Throws<FormatException>(() => new DiceRoll("D"));
            Assert.Throws<FormatException>(() => new DiceRoll("0D"));
            Assert.Throws<FormatException>(() => new DiceRoll("D0"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DiceRoll("0D0"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DiceRoll("0D10"));
            Assert.Throws<ArgumentOutOfRangeException>(() => new DiceRoll("10D0"));
            Assert.DoesNotThrow(() => new DiceRoll("1D1"));
            }
        }
    }
