using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Labyrinth.Services.PathFinder;
using NUnit.Framework;

namespace Labyrinth.Test
    {
    [TestFixture]
    public class RepelObjectTests
        {
        private bool[,] _map;
        private RepelParameters _repelParameters;

        [SetUp]
        public void Initialize()
            {
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■
            //  ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■
            //  ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■ ■
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ ■ ■ □ □ □ □ □ □ □ □ □ □ □ □ □ □ □

            this._map = new bool[48, 30];
            for (int y = 0; y < this._map.GetLength(1); y++)
                for (int x = 0; x < this._map.GetLength(0); x++)
                    _map[x, y] = true;

            for (int y = 9; y <= 10; y++)
                for (int x = 0; x < this._map.GetLength(0); x++)
                    _map[x, y] = false;

            for (int y = 19; y <= 20; y++)
                for (int x = 0; x < this._map.GetLength(0); x++)
                    _map[x, y] = false;

            for (int x = 15; x <= 16; x++)
                for (int y = 0; y < this._map.GetLength(1); y++)
                    _map[x, y] = false;

            for (int x = 31; x <= 32; x++)
                for (int y = 0; y < this._map.GetLength(1); y++)
                    _map[x, y] = false;

            this._repelParameters = new RepelParameters();
            this._repelParameters.CanBeOccupied = CanOccupyPositionOnMap;
            }

        private bool CanOccupyPositionOnMap(TilePos tp)
            {
            if (tp.X < 0 || tp.Y < 0)
                return false;
            if (tp.X >= this._map.GetLength(0) || tp.Y >= this._map.GetLength(1))
                return false;
            var result = this._map[tp.X, tp.Y];
            return result;
            }

        private void OutputRoute(IList<TilePos> path)
            {
            var newMap = new char[this._map.GetLength(0), this._map.GetLength(1)];
            for (int y = 0; y < this._map.GetLength(1); y++)
                {
                for (int x = 0; x < this._map.GetLength(0); x++)
                    {
                    newMap[x, y] = this._map[x, y] ? '·' : '■';
                    }
                }
            newMap[this._repelParameters.RepelLocation.X, this._repelParameters.RepelLocation.Y] = 'R';
            newMap[this._repelParameters.StartLocation.X, this._repelParameters.StartLocation.Y] = 'S';
            if (path != null)
                {
                foreach (var location in path)
                    {
                    newMap[location.X, location.Y] = '+';
                    }
                if (path.Any())
                    newMap[path.Last().X, path.Last().Y] = 'F';
                }

            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < newMap.GetLength(1); y++)
                {
                for (int x = 0; x < newMap.GetLength(0); x++)
                    {
                    sb.Append(newMap[x, y]);
                    }
                sb.AppendLine();
                }
            System.Diagnostics.Trace.Write(sb.ToString());
            Console.Write(sb.ToString());
            }

        private void OpenNorthWall()
            {
            this._map[23, 9] = true;
            this._map[24, 9] = true;
            this._map[23, 10] = true;
            this._map[24, 10] = true;
            }

        private void OpenSouthWall()
            {
            this._map[23, 19] = true;
            this._map[24, 19] = true;
            this._map[23, 20] = true;
            this._map[24, 20] = true;
            }

        private void OpenWestWall()
            {
            this._map[15, 14] = true;
            this._map[15, 15] = true;
            this._map[16, 14] = true;
            this._map[16, 15] = true;
            }

        private void OpenEastWall()
            {
            this._map[31, 14] = true;
            this._map[31, 15] = true;
            this._map[32, 14] = true;
            this._map[32, 15] = true;
            }

        private void AddCentralIsland()
            {
            this._map[23, 14] = false;
            this._map[23, 15] = false;
            this._map[24, 14] = false;
            this._map[24, 15] = false;
            }

        private void AddTopCentralPairOfIslands()
            {
            this._map[23, 2] = false;
            this._map[23, 3] = false;
            this._map[24, 2] = false;
            this._map[24, 3] = false;
            this._map[23, 6] = false;
            this._map[23, 7] = false;
            this._map[24, 6] = false;
            this._map[24, 7] = false;
            }

        [Test]
        public void TestAllExitsAvailableAndMonsterToNorthThenMonsterMovesUpwards()
            {
            // Arrange
            OpenNorthWall();
            OpenEastWall();
            OpenWestWall();
            OpenSouthWall();
            AddCentralIsland();
            AddTopCentralPairOfIslands();
            this._repelParameters.StartLocation = new TilePos(20, 13);
            this._repelParameters.RepelLocation = new TilePos(20, 15);
            this._repelParameters.MaximumLengthOfPath = 24;
            this._repelParameters.MinimumDistanceToMoveAway = 12;
            RepelObject pathFinder = new RepelObject(this._repelParameters);

            // Act
            IList<TilePos> path;
            var result = pathFinder.TryFindPath(out path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(path.Any());
            OutputRoute(path);
            }

        [Test]
        public void TestNoExitsAvailableAndMonsterToNorthThenMonsterCannotMoveAway()
            {
            // Arrange
            AddCentralIsland();
            this._repelParameters.StartLocation = new TilePos(20, 13);
            this._repelParameters.RepelLocation = new TilePos(20, 15);
            this._repelParameters.MaximumLengthOfPath = 24;
            this._repelParameters.MinimumDistanceToMoveAway = 16;
            RepelObject pathFinder = new RepelObject(this._repelParameters);

            // Act
            IList<TilePos> path;
            var result = pathFinder.TryFindPath(out path);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(path);
            OutputRoute(null);
            }

        [Test]
        public void TestObstacleBetweenObjects()
            {
            // Arrange
            AddCentralIsland();
            OpenEastWall();
            this._repelParameters.StartLocation = new TilePos(26, 14);
            this._repelParameters.RepelLocation = new TilePos(20, 14);
            this._repelParameters.MaximumLengthOfPath = 24;
            this._repelParameters.MinimumDistanceToMoveAway = 16;
            RepelObject pathFinder = new RepelObject(this._repelParameters);

            // Act
            IList<TilePos> path;
            var result = pathFinder.TryFindPath(out path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(path.Any());
            OutputRoute(path);
            }

        [Test]
        public void TestMoveAroundObstacle()
            {
            // Arrange
            AddCentralIsland();
            OpenEastWall();
            this._repelParameters.StartLocation = new TilePos(21, 14);
            this._repelParameters.RepelLocation = new TilePos(20, 14);
            this._repelParameters.MaximumLengthOfPath = 24;
            this._repelParameters.MinimumDistanceToMoveAway = 16;
            RepelObject pathFinder = new RepelObject(this._repelParameters);

            // Act
            IList<TilePos> path;
            var result = pathFinder.TryFindPath(out path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(path.Any());
            OutputRoute(path);
            }

        [Test]
        public void TestNoNeedToMove()
            {
            // Arrange
            OpenNorthWall();
            this._repelParameters.StartLocation = new TilePos(20, 4);
            this._repelParameters.RepelLocation = new TilePos(20, 18);
            this._repelParameters.MaximumLengthOfPath = 24;
            this._repelParameters.MinimumDistanceToMoveAway = 12;
            RepelObject pathFinder = new RepelObject(this._repelParameters);

            // Act
            IList<TilePos> path;
            var result = pathFinder.TryFindPath(out path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(path.Any());
            OutputRoute(path);
            }

        [Test]
        public void TestCantMove()
            {
            // Arrange
            OpenNorthWall();
            OpenEastWall();
            OpenWestWall();
            OpenSouthWall();
            this._repelParameters.StartLocation = new TilePos(20, 13);
            this._map[this._repelParameters.StartLocation.X - 1, this._repelParameters.StartLocation.Y] = false;
            this._map[this._repelParameters.StartLocation.X + 1, this._repelParameters.StartLocation.Y] = false;
            this._map[this._repelParameters.StartLocation.X, this._repelParameters.StartLocation.Y - 1] = false;
            this._map[this._repelParameters.StartLocation.X, this._repelParameters.StartLocation.Y + 1] = false;
            this._repelParameters.RepelLocation = new TilePos(20, 15);
            this._repelParameters.MaximumLengthOfPath = 24;
            this._repelParameters.MinimumDistanceToMoveAway = 12;
            RepelObject pathFinder = new RepelObject(this._repelParameters);

            // Act
            IList<TilePos> path;
            var result = pathFinder.TryFindPath(out path);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(path);
            OutputRoute(null);
            }

        [Test]
        public void TestOnlyExitIsSouthAndMonsterToNorthThenMonsterMovesDownPastRepelLocation()
            {
            // Arrange
            OpenSouthWall();
            AddCentralIsland();
            AddTopCentralPairOfIslands();
            this._repelParameters.StartLocation = new TilePos(20, 13);
            this._repelParameters.RepelLocation = new TilePos(20, 15);
            this._repelParameters.MaximumLengthOfPath = 24;
            this._repelParameters.MinimumDistanceToMoveAway = 12;
            RepelObject pathFinder = new RepelObject(this._repelParameters);

            // Act
            IList<TilePos> path;
            var result = pathFinder.TryFindPath(out path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(path.Any());
            OutputRoute(path);
            }
 
        [Test]
        public void TestRepelLocationIsSameAsStartLocation()
            {
            // Arrange
            OpenNorthWall();
            OpenEastWall();
            OpenWestWall();
            OpenSouthWall();
            AddCentralIsland();
            AddTopCentralPairOfIslands();
            this._repelParameters.StartLocation = new TilePos(20, 15);
            this._repelParameters.RepelLocation = new TilePos(20, 15);
            this._repelParameters.MaximumLengthOfPath = 24;
            this._repelParameters.MinimumDistanceToMoveAway = 16;
            RepelObject pathFinder = new RepelObject(this._repelParameters);

            // Act
            IList<TilePos> path;
            var result = pathFinder.TryFindPath(out path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(path.Any());
            OutputRoute(path);
            }

        [Test]
        public void TestConstructor()
            {
            Assert.Throws<ArgumentNullException>(() => new RepelObject(null));
            Assert.Throws<ArgumentException>(() => new RepelObject(new RepelParameters()));
            }

        [Test]
        public void TestMaxPathLength()
            {
            this._map = new bool[10,1];
            for (int i = 0; i < 10; i++)
                this._map[i, 0] = true;
            this._repelParameters.MaximumLengthOfPath = 5;
            this._repelParameters.StartLocation = new TilePos(0, 0);
            this._repelParameters.MinimumDistanceToMoveAway = 8;
            RepelObject pathFinder = new RepelObject(this._repelParameters);

            // Act
            IList<TilePos> path;
            var result = pathFinder.TryFindPath(out path);

            // Assert
            Assert.IsFalse(result);
            }

        [Test]
        public void TestRepelParameters()
            {
            var rp = new RepelParameters();
            rp.MaximumLengthOfPath = 10;
            Assert.AreEqual(10, rp.MaximumLengthOfPath);
            Assert.Throws<ArgumentOutOfRangeException>(() => rp.MaximumLengthOfPath = -1);

            rp.MinimumDistanceToMoveAway = 20;
            Assert.AreEqual(20, rp.MinimumDistanceToMoveAway);
            Assert.Throws<ArgumentOutOfRangeException>(() => rp.MinimumDistanceToMoveAway = -1);
            }
        }
    }
