using System;
using System.Collections.Generic;
using System.Linq;
using Labyrinth.GameObjects;
using Labyrinth.Services.Input;
using Labyrinth.Services.WorldBuilding;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestMortonCode
        {
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
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.Right, FiringState.None, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.FromMilliseconds(100), new PlayerController.Instruction(Direction.Right, FiringState.None, FiringState.None))
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl);
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("p ");
            var w = g.World;

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }

            Assert.IsEmpty(w.GameObjects.GetItemsOnTile(new TilePos(0, 0)));
            var list = w.GameObjects.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.IsTrue(list.SequenceEqual(new List<StaticItem> {w.Player}));
            }

        [Test]
        public void VerifyThatTheListOfItemsIsSuitablyAdjustedWhenGameObjectMovesToAnAdjacentOccupiedSpace()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.Right, FiringState.None, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.FromMilliseconds(100), new PlayerController.Instruction(Direction.Right, FiringState.None, FiringState.None))
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl);
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("pg");
            var w = g.World;

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }
            
            Assert.IsEmpty(w.GameObjects.GetItemsOnTile(new TilePos(0, 0)));
            var list = w.GameObjects.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list[0] is Grave);
            Assert.IsTrue(list[1] is Player);
            }

        [Test]
        public void VerifyThatListOfItemsIsSuitablyAdjustedWhenGameObjectMovesFromAnAdditionalOccupiedSpaceToFreeSpace()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.Right, FiringState.None, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.FromMilliseconds(100), new PlayerController.Instruction(Direction.Right, FiringState.None, FiringState.None))
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl);
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("p ");
            var w = g.World;
            w.AddGrave(new TilePos(0, 0));

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }

            var list = w.GameObjects.GetItemsOnTile(new TilePos(0, 0)).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Grave);

            list = w.GameObjects.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Player);
            }

        [Test]
        public void VerifyThatListOfItemsIsSuitablyAdjustedWhenGameObjectMovesFromAnAdditionalOccupiedSpaceToAnotherOccupiedSpace()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.Right, FiringState.None, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.FromMilliseconds(100), new PlayerController.Instruction(Direction.Right, FiringState.None, FiringState.None))
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl);
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("pg");
            var w = g.World;
            w.AddGrave(new TilePos(0, 0));

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }

            var list = w.GameObjects.GetItemsOnTile(new TilePos(0, 0)).ToList();
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Grave);

            list = w.GameObjects.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.NotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list[0] is Grave);
            Assert.IsTrue(list[1] is Player);
            }
        }
    }
