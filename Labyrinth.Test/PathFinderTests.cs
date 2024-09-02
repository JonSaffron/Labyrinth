using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Labyrinth.DataStructures;
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
        public void Initialise()
            {
            //  □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □
            //  □ S □ □ □ F □
            //  □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □
            //  □ □ □ □ □ □ □
            
            this._map = new bool[7, 7];
            for (int y = 0; y < this._map.GetLength(1); y++)
                for (int x = 0; x < this._map.GetLength(0); x++)
                    _map[x, y] = true;

            this._searchParameters = new SearchParameters
                {
                StartLocation = new TilePos(1, 2),
                EndLocation = new TilePos(5, 2),
                CanBeOccupied = CanOccupyPositionOnMap
                };
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

        /// <summary>
        /// Create an L-shaped wall between S and F
        /// </summary>
        private void AddWallWithGap()
            {
            //  □ □ □ □ □ □ □
            //  □ □ □ ■ ■ □ □
            //  □ S □ ■ □ F □
            //  □ □ □ ■ □ □ □
            //  □ □ □ ■ □ □ □
            //  □ □ □ ■ □ □ □
            //  □ □ □ ■ □ □ □

            // Path: 1,2 ; 2,1 ; 3,0 ; 4,0 ; 5,1 ; 5,2

            this._map[3, 1] = false;
            this._map[4, 1] = false;
            this._map[3, 2] = false;
            this._map[3, 3] = false;
            this._map[3, 4] = false;
            this._map[3, 5] = false;
            this._map[3, 6] = false;
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
            //  □ □ □ ■ □ □ □
            //  □ □ □ ■ □ □ □

            // No path

            this._map[3, 0] = false;
            this._map[3, 1] = false;
            this._map[3, 2] = false;
            this._map[3, 3] = false;
            this._map[3, 4] = false;
            this._map[3, 5] = false;
            this._map[3, 6] = false;
            }

        /// <summary>
        /// Create a closed barrier between S and F
        /// </summary>
        private void AddConcaveObstacle()
            {
            //  □ □ □ □ □ □ F
            //  □ ■ ■ ■ ■ ■ □
            //  □ ■ □ □ □ ■ □
            //  □ ■ □ □ □ ■ □
            //  □ □ □ □ □ ■ □
            //  □ S □ ■ ■ ■ □
            //  □ □ □ □ □ □ □

            // No path

            this._map[1, 3] = false;
            this._map[1, 2] = false;
            this._map[1, 1] = false;
            this._map[2, 1] = false;
            this._map[3, 1] = false;
            this._map[4, 1] = false;
            this._map[5, 1] = false;
            this._map[5, 2] = false;
            this._map[5, 3] = false;
            this._map[5, 4] = false;
            this._map[5, 5] = false;
            this._map[4, 5] = false;
            this._map[3, 5] = false;
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
            newMap[this._searchParameters.StartLocation.X, this._searchParameters.StartLocation.Y] = 'S';
            if (path != null)
                {
                foreach (var location in path)
                    {
                    newMap[location.X, location.Y] = '+';
                    }
                }
            newMap[this._searchParameters.EndLocation.X, this._searchParameters.EndLocation.Y] = (this._searchParameters.StartLocation == this._searchParameters.EndLocation) ? 'X' : 'F';

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

        [Test]
        public void Test_WithoutWalls_CanFindPath()
            {
            // Arrange
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            var result = pathFinder.TryFindPath(out var path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(path);
            Assert.IsTrue(path.Any());
            Assert.AreEqual(4, path.Count);
            Assert.AreEqual(new TilePos(2, 2), path[0]);
            Assert.AreEqual(new TilePos(3, 2), path[1]);
            Assert.AreEqual(new TilePos(4, 2), path[2]);
            Assert.AreEqual(new TilePos(5, 2), path[3]);
            OutputRoute(path);
            }

        [Test]
        public void Test_WithOpenWall_CanFindPathAroundWall()
            {
            // Arrange
            AddWallWithGap();
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            var result = pathFinder.TryFindPath(out var path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(path);
            Assert.IsTrue(path.Any());
            Assert.AreEqual(8, path.Count);
            OutputRoute(path);
            }

        [Test]
        public void Test_WithClosedWall_CannotFindPath()
            {
            // Arrange
            AddWallWithoutGap();
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            var result = pathFinder.TryFindPath(out var path);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(path);
            OutputRoute(null);
            }

        [Test]
        public void Test_StartAndEndInSamePlace_IsOkay()
            {
            // Arrange
            this._searchParameters.StartLocation = new TilePos(1, 1);
            this._searchParameters.EndLocation = new TilePos(1, 1);
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            var result = pathFinder.TryFindPath(out var path);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(!path.Any());
            OutputRoute(path);
            }

        [Test]
        public void Test_MaximumRouteLengthIsNotLongEnough_IsOkay()
            {
            // Arrange
            this._searchParameters.StartLocation = new TilePos(1, 1);
            this._searchParameters.EndLocation = new TilePos(5, 3);
            this._searchParameters.MaximumLengthOfPath = 5;
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            var result = pathFinder.TryFindPath(out var path);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(path);
            OutputRoute(null);
            }

        [Test]
        public void Test_MaximumRouteLengthIsJustEnough_IsOkay()
            {
            // Arrange
            this._searchParameters.StartLocation = new TilePos(1, 1);
            this._searchParameters.EndLocation = new TilePos(5, 3);
            this._searchParameters.MaximumLengthOfPath = 6;
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            var result = pathFinder.TryFindPath(out var path);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(this._searchParameters.MaximumLengthOfPath.Value, path.Count);
            OutputRoute(path);
            }

        [Test]
        public void Test_ConcaveObstacle_CanRouteAround()
            {
            // Arrange
            AddConcaveObstacle();
            this._searchParameters.StartLocation = new TilePos(1, 5);
            this._searchParameters.EndLocation = new TilePos(6, 0);
            PathFinder pathFinder = new PathFinder(_searchParameters);

            // Act
            var result = pathFinder.TryFindPath(out var path);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(12, path.Count);
            OutputRoute(path);
            }

        [Test]
        public void TestPath()
            {
            var path1 = new Path<char>('A').AddStep('B', 1).AddStep('C', 2).AddStep('D', 3);
            Assert.AreEqual(6, path1.Cost);
            Assert.IsTrue('D' == path1.LastStep);
            Assert.IsTrue(path1.IsViable);
            Assert.IsTrue(path1.ToString().StartsWith("Viable path 4 steps, cost=6"));
            path1.IsViable = false;
            Assert.IsFalse(path1.IsViable);
            Assert.IsTrue(path1.ToString().StartsWith("Not viable path 4 steps, cost=6"));
            var enumerator = ((IEnumerable) path1).GetEnumerator();
            var disposable = enumerator as IDisposable;
            var s = string.Empty;
            while (enumerator.MoveNext())
                {
                // ReSharper disable once PossibleNullReferenceException
                s += (char) enumerator.Current;
                }
            Assert.IsTrue("DCBA" == s);
            disposable?.Dispose();

            Assert.Positive(path1.CompareTo(path1.AddStep('E', 4)));
            Assert.Negative(path1.AddStep('E', 4).CompareTo(path1));
            Assert.Zero(path1.CompareTo(path1.AddStep(' ', 0)));
            }
        }
    }
