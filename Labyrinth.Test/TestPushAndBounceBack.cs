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

            Assert.IsTrue(w.Player.CanMoveTo(Direction.Left));

            var boulder = (Boulder) w.GetItemsOnTile(new TilePos(2, 0)).ElementAt(0);
            boulder.PushOrBounce(w.Player, Direction.Left);

            Assert.AreEqual(boulder.Direction, Direction.Left);
            Assert.IsTrue(boulder.MovingTowards == new TilePos(1, 0).ToPosition());
            }
        }
    }
