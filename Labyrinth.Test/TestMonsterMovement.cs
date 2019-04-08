using System.Linq;
using Labyrinth.GameObjects.Motility;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    class TestMovement
        {
        [Test]
        public void TestPatrolPerimeterDirection()
            {
            Assert.IsTrue(PatrolPerimeter.GetPreferredDirections(Direction.Left, PatrolPerimeter.AttachmentToWall.FollowWallOnLeft).SequenceEqual(new[] { Direction.Down, Direction.Left, Direction.Up, Direction.Right }));
            Assert.IsTrue(PatrolPerimeter.GetPreferredDirections(Direction.Up, PatrolPerimeter.AttachmentToWall.FollowWallOnLeft).SequenceEqual(new[] { Direction.Left, Direction.Up, Direction.Right, Direction.Down }));
            Assert.IsTrue(PatrolPerimeter.GetPreferredDirections(Direction.Right, PatrolPerimeter.AttachmentToWall.FollowWallOnLeft).SequenceEqual(new[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left }));
            Assert.IsTrue(PatrolPerimeter.GetPreferredDirections(Direction.Down, PatrolPerimeter.AttachmentToWall.FollowWallOnLeft).SequenceEqual(new[] { Direction.Right, Direction.Down, Direction.Left, Direction.Up }));

            Assert.IsTrue(PatrolPerimeter.GetPreferredDirections(Direction.Left, PatrolPerimeter.AttachmentToWall.FollowWallOnRight).SequenceEqual(new [] { Direction.Up, Direction.Left, Direction.Down, Direction.Right}));
            Assert.IsTrue(PatrolPerimeter.GetPreferredDirections(Direction.Up, PatrolPerimeter.AttachmentToWall.FollowWallOnRight).SequenceEqual(new[] { Direction.Right, Direction.Up, Direction.Left, Direction.Down }));
            Assert.IsTrue(PatrolPerimeter.GetPreferredDirections(Direction.Right, PatrolPerimeter.AttachmentToWall.FollowWallOnRight).SequenceEqual(new[] { Direction.Down, Direction.Right, Direction.Up, Direction.Left }));
            Assert.IsTrue(PatrolPerimeter.GetPreferredDirections(Direction.Down, PatrolPerimeter.AttachmentToWall.FollowWallOnRight).SequenceEqual(new[] { Direction.Left, Direction.Down, Direction.Right, Direction.Up }));
            }
        }
    }
