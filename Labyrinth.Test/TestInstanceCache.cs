using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.Services.Sound;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestInstanceCache
        {
        [Test]
        public void CannotConstructACacheWithNoEntries()
            {
            // ReSharper disable once NotAccessedVariable
            InstanceCache<string> x;
            Assert.Throws<ArgumentOutOfRangeException>(() => x = new InstanceCache<string>(0, () => string.Empty));
            }

        [Test]
        public void CannotConstructACacheWithNegativeEntries()
            {
            // ReSharper disable once NotAccessedVariable
            InstanceCache<string> x;
            Assert.Throws<ArgumentOutOfRangeException>(() => x = new InstanceCache<string>(-10, () => string.Empty));
            }

        [Test]
        public void TheInstanceCreatorMustReturnAValue()
            {
            var cache = new InstanceCache<string>(1, () => null);

            Assert.Throws<InvalidOperationException>(() => cache.GetNext());
            }

        private static string UniqueStringCreator()
            {
            var result = string.Format("Test{0}Test", Guid.NewGuid());
            return result;
            }

        [Test]
        public void ACacheWithASingleEntryShouldAlwaysReturnThatEntry()
            {
            var cache = new InstanceCache<string>(1, UniqueStringCreator);

            var firstInstance = cache.GetNext();

            for (int i = 0; i < 100; i++)
                Assert.AreEqual(firstInstance, cache.GetNext());
            }

        [Test]
        public void ACacheWithThreeeEntriesShouldAlwaysReturnThreeDistinctValues()
            {
            var cache = new InstanceCache<string>(3, UniqueStringCreator);

            var list = new List<string>();
            for (int i = 0; i < 100; i++)
                list.Add(cache.GetNext());

            int countOfDistinctEntries = list.Distinct().Count();
            Assert.AreEqual(countOfDistinctEntries, 3);
            }
        }
    }
