using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Labyrinth.Services;
using Labyrinth.Services.WorldBuilding;

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestStandardRandom
        {
        [Test]
        public void BasicTest()
            {
            var r = new StandardRandom();
            for (byte i = 0; i < 255; i++)
                {
                r.Test(i);
                }

            for (int i = 0; i <= 1000; i++)
                {
                r.Next(i);
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

            Assert.Throws<ArgumentNullException>(() => r.DiceRoll(null));
            for (int i = 1; i <= 10; i++)
                {
                for (int j = 1; j <= 10; j++)
                    {
                    var dr = new DiceRoll($"{i}D{j}");
                    r.DiceRoll(dr);
                    }
                }
            }
        }
    }
