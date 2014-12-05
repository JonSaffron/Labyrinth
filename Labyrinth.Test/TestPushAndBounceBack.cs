using System.Linq;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    public class TestPushAndBounceBack
        {
        [Test]
        public void TestPlayerPushesBoulder()
            {
            var g = new Game1(new NullSoundLibrary());
            var wl = new WorldLoaderForTest("# bp#");
            var w = new World(g, wl);

            Assert.IsTrue(w.Player.TestCanMoveTo(Direction.Left));

            var boulder = (Boulder) w.GetItemsOnTile(new TilePos(2, 0)).ElementAt(0);
            boulder.PushOrBounce(w.Player, Direction.Left);
            Assert.AreEqual(Direction.None, w.Player.Direction);

            Assert.AreEqual(Direction.Left, boulder.Direction);
            Assert.IsTrue(boulder.MovingTowards == new TilePos(1, 0).ToPosition());
            }

        [Test]
        public void TestPlayerBouncesBoulder()
            {
            var g = new Game1(new NullSoundLibrary());
            var wl = new WorldLoaderForTest("#bp #");
            var w = new World(g, wl);

            Assert.IsTrue(w.Player.TestCanMoveTo(Direction.Left));

            var boulder = (Boulder) w.GetItemsOnTile(new TilePos(1, 0)).ElementAt(0);
            boulder.PushOrBounce(w.Player, Direction.Left);
            Assert.AreEqual(Direction.Right, w.Player.Direction);

            Assert.AreEqual(Direction.Right, boulder.Direction);
            Assert.IsTrue(boulder.MovingTowards == new TilePos(2, 0).ToPosition());
            }

        [Test]
        public void TestPlayerHasNoSpaceToBounceBoulder()
            {
            var g = new Game1(new NullSoundLibrary());
            var wl = new WorldLoaderForTest("#bp#");
            var w = new World(g, wl);

            Assert.IsFalse(w.Player.TestCanMoveTo(Direction.Left));

            var boulder = (Boulder) w.GetItemsOnTile(new TilePos(1, 0)).ElementAt(0);
            boulder.PushOrBounce(w.Player, Direction.Left);

            Assert.AreEqual(Direction.None, w.Player.Direction);
            Assert.AreEqual(Direction.None, boulder.Direction);
            }

        [Test]
        public void TestPlayerBouncesOneBoulderAndEndsUpPushingAnother()
            {
            var g = new Game1(new NullSoundLibrary());
            var wl = new WorldLoaderForTest("#bpb #");
            var w = new World(g, wl);

            Assert.IsTrue(w.Player.TestCanMoveTo(Direction.Left));

            var boulder1 = (Boulder) w.GetItemsOnTile(new TilePos(1, 0)).ElementAt(0);
            boulder1.PushOrBounce(w.Player, Direction.Left);
            Assert.AreEqual(Direction.Right, w.Player.Direction);

            Assert.AreEqual(Direction.Right, boulder1.Direction);
            Assert.IsTrue(boulder1.MovingTowards == new TilePos(2, 0).ToPosition());

            var boulder2 = (Boulder) w.GetItemsOnTile(new TilePos(3, 0)).ElementAt(0);
            boulder2.PushOrBounce(w.Player, w.Player.Direction);

            Assert.AreEqual(Direction.Right, boulder2.Direction);
            Assert.IsTrue(boulder2.MovingTowards == new TilePos(4, 0).ToPosition());
            }

        [Test]
        public void TestPlayerCannotBouncesOneBoulderBecauseAnotherBoulderBehindCannotMove()
            {
            var g = new Game1(new NullSoundLibrary());
            var wl = new WorldLoaderForTest("#bpb#");
            var w = new World(g, wl);

            Assert.IsFalse(w.Player.TestCanMoveTo(Direction.Left));

            var boulder1 = (Boulder) w.GetItemsOnTile(new TilePos(1, 0)).ElementAt(0);
            boulder1.PushOrBounce(w.Player, Direction.Left);
            Assert.AreEqual(Direction.None, w.Player.Direction);

            Assert.AreEqual(Direction.None, boulder1.Direction);
            }
        }
    }
