using System.Collections.Generic;
using System.Linq;
using Labyrinth.Services.PathFinder;
using NUnit.Framework;

namespace Labyrinth.Test
{
    [TestFixture]
    public class PathFinderTests
    {
        private bool[,] _map;
        private SearchParameters _searchParameters;

        [SetUp]
        public void Initialize()
        {
            //  □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □
            //  □ S □ □ □ F □
            //  □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □
            
            this._map = new bool[7, 5];
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 7; x++)
                    _map[x, y] = true;

        this._searchParameters = new SearchParameters();
        this._searchParameters.StartLocation = new TilePos(1, 2);
        this._searchParameters.EndLocation = new TilePos(5, 2);
        this._searchParameters.CanBeOccupied = CanOccupyPositionOnMap;
        }

        private bool CanOccupyPositionOnMap(TilePos tp)
            {
            if (tp.X >= this._map.GetLength(0) || tp.Y >= this._map.GetLength(1))
                return false;
            var result = this._map[tp.X, tp.Y];
            return result;
            }

        /// <summary>
        /// Create an L-shaped wall between S and F
        /// </summary>
        private void AddWallWithGap()
        {
            //  □ □ □ ■ □ □ □
            //  □ □ □ ■ □ □ □
            //  □ S □ ■ □ F □
            //  □ □ □ ■ ■ □ □
            //  □ □ □ □ □ □ □

            // Path: 1,2 ; 2,1 ; 3,0 ; 4,0 ; 5,1 ; 5,2

            this._map[3, 4] = false;
            this._map[3, 3] = false;
            this._map[3, 2] = false;
            this._map[3, 1] = false;
            this._map[4, 1] = false;
        }

        /// <summary>
        /// Create a closed barrier between S and F
        /// </summary>
        private void AddWallWithoutGap()
        {
            //  □ □ □ ■ □ □ □
            //  □ □ □ ■ □ □ □
            //  □ S □ ■ □ F □
            //  □ □ □ ■ □ □ □
            //  □ □ □ ■ □ □ □

            // No path

            this._map[3, 4] = false;
            this._map[3, 3] = false;
            this._map[3, 2] = false;
            this._map[3, 1] = false;
            this._map[3, 0] = false;
        }

        [Test]
        public void Test_WithoutWalls_CanFindPath()
        {
            // Arrange
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            List<TilePos> path = pathFinder.FindPath();

            // Assert
            Assert.IsNotNull(path);
            Assert.IsTrue(path.Any());
            Assert.AreEqual(4, path.Count);
        }

        [Test]
        public void Test_WithOpenWall_CanFindPathAroundWall()
        {
            // Arrange
            AddWallWithGap();
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            List<TilePos> path = pathFinder.FindPath();

            // Assert
            Assert.IsNotNull(path);
            Assert.IsTrue(path.Any());
            Assert.AreEqual(8, path.Count);
        }

        [Test]
        public void Test_WithClosedWall_CannotFindPath()
        {
            // Arrange
            AddWallWithoutGap();
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            List<TilePos> path = pathFinder.FindPath();

            // Assert
            Assert.IsNotNull(path);
            Assert.IsFalse(path.Any());
        }
    }
}
