using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.DataStructures;
using Labyrinth.GameObjects;
using Labyrinth.Services.Input;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestMortonCode
        {
/*
        [Test]
        public void TestBackAndForth()
            {
            for (int x = 0; x < 256; x++)
                {
                for (int y = 0; y < 40; y++)
                    {
                    var tp = new TilePos(x, y);
                    var mc = tp.MortonCode;
                    var tp2 = TilePos.FromMortonCode(mc);
                    Assert.AreEqual(tp, tp2);
                    }
                }
            }
*/
        [Test]
        public void TestTilePosToPosition()
            {
            for (int x = 0; x <= 1000; x++)
                {
                for (int y = 0; y <= 1000; y++)
                    {
                    var tp = new TilePos(x, y);
                    var p = tp.ToPosition();
                    var tp2 = TilePos.TilePosFromPosition(p);
                    Assert.AreEqual(tp, tp2);
                    }
                }
            }

        [Test]
        public void VerifyThatTheListOfItemsMovesWhenGameObjectMovesToAnAdjacentEmptySpace()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                {
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Move(Direction.Right))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadWorld("p ");
            var p = GlobalServices.GameState.Player;

            g.RunTest();

            Assert.IsEmpty(GlobalServices.GameState.GetItemsOnTile(new TilePos(0, 0)));
            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.IsTrue(list.SequenceEqual(new List<StaticItem> {p}));
            }

        [Test]
        public void VerifyThatTheListOfItemsIsSuitablyAdjustedWhenGameObjectMovesToAnAdjacentOccupiedSpace()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                {
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Move(Direction.Right))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadWorld("pg");

            g.RunTest();
            
            Assert.IsEmpty(GlobalServices.GameState.GetItemsOnTile(new TilePos(0, 0)));
            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list[0] is Grave);
            Assert.IsTrue(list[1] is Player);
            }

        [Test]
        public void VerifyThatListOfItemsIsSuitablyAdjustedWhenGameObjectMovesFromAnAdditionalOccupiedSpaceToFreeSpace()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                {
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Move(Direction.Right))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadWorld("p ");
            GlobalServices.GameState.AddGrave(new TilePos(0, 0));

            g.RunTest();

            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(0, 0)).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Grave);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Player);
            }

        [Test]
        public void VerifyThatListOfItemsIsSuitablyAdjustedWhenGameObjectMovesFromAnAdditionalOccupiedSpaceToAnotherOccupiedSpace()
            {
            var g = new GameForUnitTests();

            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.Move(Direction.Right))
                };
            g.UnitTestServices.PlayerController.Enqueue(instructions);

            g.LoadWorld("pg");
            GlobalServices.GameState.AddGrave(new TilePos(0, 0));

            g.RunTest();

            var list = GlobalServices.GameState.GetItemsOnTile(new TilePos(0, 0)).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Grave);

            list = GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list[0] is Grave);
            Assert.IsTrue(list[1] is Player);
            }
        }
    }
