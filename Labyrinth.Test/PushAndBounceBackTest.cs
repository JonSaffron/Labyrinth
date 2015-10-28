using System;
using System.Linq;
using Labyrinth.GameObjects;
using Labyrinth.Services.Input;
using Labyrinth.Services.Sound;
using Labyrinth.Services.WorldBuilding;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    public class PushAndBounceBackTest
        {
        [Test]
        public void TestPlayerPushesBoulder()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.Left, FiringState.None, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.FromMilliseconds(200), PlayerController.Instruction.DoNothingInstruction())
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl, new NullSoundPlayer());
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("# bp#");
            var w = g.World;

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }

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
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.Left, FiringState.None, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.FromMilliseconds(200), PlayerController.Instruction.DoNothingInstruction())
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl, new NullSoundPlayer());
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("#bp #");
            var w = g.World;

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }

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
            var pc = new PlayerController();
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl, new NullSoundPlayer());
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("#bp#");
            var w = g.World;

            Assert.IsFalse(w.Player.CanMoveInDirection(Direction.Left));

            var boulder = (Boulder) GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ElementAt(0);
            boulder.PushOrBounce(w.Player, Direction.Left);

            Assert.AreEqual(Direction.None, w.Player.Direction);
            Assert.AreEqual(Direction.None, boulder.Direction);
            }

        [Test]
        public void TestPlayerBouncesOneBoulderAndEndsUpPushingAnother()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.Left, FiringState.None, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.FromMilliseconds(200), PlayerController.Instruction.DoNothingInstruction())
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl, new NullSoundPlayer());
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("#bpb #");
            var w = g.World;

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }

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
            var pc = new PlayerController();
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl, new NullSoundPlayer());
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("#bpb#");
            var w = g.World;

            Assert.IsFalse(w.Player.CanMoveInDirection(Direction.Left));

            var boulder1 = (Boulder) GlobalServices.GameState.GetItemsOnTile(new TilePos(1, 0)).ElementAt(0);
            boulder1.PushOrBounce(w.Player, Direction.Left);
            Assert.AreEqual(Direction.None, w.Player.Direction);

            Assert.AreEqual(Direction.None, boulder1.Direction);
            }

        [Test]
        public void TestPlayerCannotMoveMultipleBoulders()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.Left, FiringState.None, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.FromMilliseconds(200), PlayerController.Instruction.DoNothingInstruction())
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl, new NullSoundPlayer());
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("# bbp #");
            var w = g.World;

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }

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
