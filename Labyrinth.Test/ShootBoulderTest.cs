using System;
using System.Linq;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.Input;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    public class ShootBoulderTest
        {
        [Test]
        public void TestPlayerShootsBoulder()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Fire(Direction.Left))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadLevel("# b p");

            g.RunTest();

            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);
            }

        [Test]
        public void TestPlayerShootsAdjacentBoulder()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Fire(Direction.Left))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadLevel("# bp ");

            g.RunTest();

            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);
            }

        [Test]
        public void TestPlayerCannotShootBoulder1()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Fire(Direction.Left))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadLevel("#bp ");

            g.RunTest();

            Assert.IsFalse(GlobalServices.GameState.DoesShotExist());
            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);
            }

        [Test]
        public void TestPlayerCannotShootBoulder2()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Fire(Direction.Left))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadLevel("#b p ");

            g.RunTest();

            Assert.IsFalse(GlobalServices.GameState.DoesShotExist());
            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.IsTrue(list.OfType<Boulder>().Any());
            }

        [Test]
        public void TestPlayerCannotShootBoulder3()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Fire(Direction.Left))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadLevel("# bbp ");

            g.RunTest();

            Assert.IsFalse(GlobalServices.GameState.DoesShotExist());
            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(2, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);
            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(3, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);
            }
        }
    }
