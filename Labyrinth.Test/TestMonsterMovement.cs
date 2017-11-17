using System;
using System.Linq;
using Labyrinth.Services.Input;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestMovement
        {
        [Test]
        public void Test()
            {
            var services = new UnitTestServices();
            var g = new Game1(services);

            var instructions = new[]
                {
                new PlayerController.TimedInstruction(TimeSpan.Zero,
                    PlayerController.Instruction.DoNothingInstruction()),
                new PlayerController.TimedInstruction(TimeSpan.FromMilliseconds(4000),
                    PlayerController.Instruction.DoNothingInstruction())
                };
            services.PlayerController.Enqueue(instructions);

            g.LoadLevel("#pm             #");

            while (!services.PlayerController.HasFinishedQueue || Helpers.IsAnythingMoving())
                {
                g.Tick();
                }

            var monster = GlobalServices.GameState.GetSurvivingInteractiveItems().OfType<DummyMonster>().Single();
            foreach (var item in monster.Log)
                {
                Console.WriteLine(item.Ticks + " " + (item.Ticks/60.0) + ": " + item.Position + " " + TilePos.TilePosFromPosition(item.Position));
                }
            Console.WriteLine("(ends)");
            }
        }
    }
