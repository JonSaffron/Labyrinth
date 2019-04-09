using System;
using System.Linq;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.Input;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    public class PushAndBounceBackTest
        {
        [Test]
        public void TestPlayerPushesBoulder()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Move(Direction.Left))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadLevel("# bp#");

            g.RunTest();

            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(2, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Player);
            }

        [Test]
        public void TestPlayerBouncesBoulder()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Move(Direction.Left))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadLevel("#bp #");

            g.RunTest();

            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.IsEmpty(list);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(2, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(3, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Player);
            }

        [Test]
        public void TestPlayerHasNoSpaceToBounceBoulder()
            {
            var g = new GameForUnitTests();

            g.LoadLevel("#bp#");
            var p = GlobalServices.GameState.Player;

            Assert.IsFalse(p.CanMoveInDirection(Direction.Left));

            var boulder = (Boulder) GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ElementAt(0);
            boulder.PushOrBounce(p, Direction.Left);

            Assert.AreEqual(Direction.None, p.CurrentMovement.Direction);
            Assert.AreEqual(Direction.None, boulder.CurrentMovement.Direction);
            }

        [Test]
        public void TestPlayerBouncesOneBoulderAndEndsUpPushingAnother()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Move(Direction.Left))
                };

            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadLevel("#bpb #");

            g.RunTest();

            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.IsEmpty(list);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(2, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(3, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Player);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(4, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);
            }

        [Test]
        public void TestPlayerCannotBouncesOneBoulderBecauseAnotherBoulderBehindCannotMove()
            {
            var g = new GameForUnitTests();

            g.LoadLevel("#bpb#");
            var p = GlobalServices.GameState.Player;

            Assert.IsFalse(p.CanMoveInDirection(Direction.Left));

            var boulder1 = (Boulder) GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ElementAt(0);
            boulder1.PushOrBounce(p, Direction.Left);
            Assert.AreEqual(Direction.None, p.CurrentMovement.Direction);

            Assert.AreEqual(Direction.None, boulder1.CurrentMovement.Direction);
            }

        [Test]
        public void TestPlayerCannotMoveMultipleBoulders()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Move(Direction.Left))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadLevel("# bbp #");

            g.RunTest();

            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.IsEmpty(list);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(2, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(3, 0)).ToList();
            Assert.NotNull(list);
            Assert.IsEmpty(list);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(4, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(5, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Player);
            }


        }
    }
