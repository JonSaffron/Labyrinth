using System;
using System.Linq;
using Labyrinth.GameObjects;
using Labyrinth.Services.Input;
using Labyrinth.Services.WorldBuilding;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    public class ShootBoulderTest
        {
        [Test]
        public void TestPlayerShootsBoulder()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.None, FiringState.Pulse, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.DoNothingInstruction())
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl);
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("# b p");
            var w = g.World;

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }

            var list = w.GameObjects.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);
            }

        [Test]
        public void TestPlayerShootsAdjacentBoulder()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.None, FiringState.Pulse, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.DoNothingInstruction())
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl);
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("# bp ");
            var w = g.World;

            while (!pc.HasFinishedQueue || w.IsAnythingMoving())
                {
                g.Tick();
                }

            var list = w.GameObjects.GetItemsOnTile(new TilePos(1, 0)).ToList();
            Assert.IsNotEmpty(list);
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list[0] is Boulder);
            }

        [Test]
        public void TestPlayerCannotShootBoulder()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.None, FiringState.Pulse, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.DoNothingInstruction())
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl);
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("#bp ");
            var w = g.World;

            while (!pc.HasFinishedQueue)
                {
                g.Tick();
                }

            Assert.IsFalse(w.GameObjects.DoesShotExist());
            }

        [Test]
        public void TestPlayerCannotShootBoulder2()
            {
            var instructions = new[] 
                { 
                new PlayerController.TimedInstruction(TimeSpan.Zero, new PlayerController.Instruction(Direction.None, FiringState.Pulse, FiringState.None)),
                new PlayerController.TimedInstruction(TimeSpan.Zero, PlayerController.Instruction.DoNothingInstruction())
                };

            var pc = new PlayerController(instructions);
            var wl = new WorldLoaderForTest();
            var g = new Game1(pc, wl);
            g.Components.Add(new SuppressDrawComponent(g));
            g.LoadLevel("# bbp ");
            var w = g.World;

            while (!pc.HasFinishedQueue)
                {
                g.Tick();
                }

            Assert.IsFalse(w.GameObjects.DoesShotExist());
            }
        }
    }
