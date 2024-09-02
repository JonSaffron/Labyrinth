using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Labyrinth.DataStructures;

// http://blog.two-cats.com/2014/06/a-star-example/
// http://theory.stanford.edu/~amitp/GameProgramming/index.html
// https://blogs.msdn.microsoft.com/ericlippert/2007/10/page/2/

namespace Labyrinth.Services.PathFinder
    {
    public class PathFinder
        {
        private readonly PriorityQueue<float, Path<TilePos>> _openNodes = new PriorityQueue<float, Path<TilePos>>();
        private readonly HashSet<Path<TilePos>> _closedNodes = new HashSet<Path<TilePos>>();
        private readonly SearchParameters _searchParameters;

        /// <summary>
        /// Create a new instance of PathFinder
        /// </summary>
        /// <param name="searchParameters">An instance of a SearchParameters object that defines the search to perform</param>
        public PathFinder(SearchParameters searchParameters)
            {
            if (searchParameters == null)
                throw new ArgumentNullException(nameof(searchParameters));
            if (searchParameters.CanBeOccupied == null)
                throw new ArgumentException("SearchParameters.CanBeOccupied must be set.");
            this._searchParameters = searchParameters;
            }

        /// <summary>
        /// Attempts to find a path from the start location to the end location based on the supplied SearchParameters
        /// </summary>
        /// <returns>True if a path can be found and a list of positions representing the path. If no path was found, false is returned and a null list.</returns>
        public bool TryFindPath([NotNullWhen(returnValue: true)] out IList<TilePos>? result)
            {
            // The start node is the first entry in the 'open' list
            this._openNodes.Enqueue(0, new Path<TilePos>(this._searchParameters.StartLocation));

            while (this._openNodes.Count != 0)
                {
                var path = this._openNodes.Dequeue();
                if (!path.IsViable)
                    continue;

                if (path.LastStep == this._searchParameters.EndLocation)
                    {
                    result = path.Reverse().Skip(1).ToList();
                    return true;
                    }

                this._closedNodes.Add(path);

                IEnumerable<TilePos> nextNodes = GetAdjacentWalkableNodes(path.LastStep);
                foreach (var nextNode in nextNodes)
                    {
                    var newPath = path.AddStep(nextNode, 1);
                    if (this._searchParameters.MaximumLengthOfPath.HasValue && newPath.Cost > this._searchParameters.MaximumLengthOfPath.Value)
                        continue;
                    
                    var pathYetToVisit = this._openNodes.Items.FirstOrDefault(item => item.LastStep == newPath.LastStep);
                    if (pathYetToVisit != null)
                        {
                        if (newPath.Cost >= pathYetToVisit.Cost)
                            // don't add the newPath as it's no better than one we've already found
                            continue;

                        // mark the path we've already queued as not worth investigating
                        pathYetToVisit.IsViable = false;
                        }

                    var pathAlreadyVisited = this._closedNodes.FirstOrDefault(item => item.LastStep == newPath.LastStep);
                    if (pathAlreadyVisited != null && newPath.Cost >= pathAlreadyVisited.Cost)
                        // don't add the newPath as it's no better than one we've already found
                        continue;

                    var estimatedCostToGoal = newPath.Cost + GetEstimatedTraversalCost(newPath.LastStep);
                    this._openNodes.Enqueue(estimatedCostToGoal, newPath);
                    }
                }

            // The method returns false if it's not possible to get to the target point
            result = null;
            return false;
            }

        /// <summary>
        /// Returns any nodes that are adjacent to <paramref name="fromNode"/> and may be considered to form the next step in the path
        /// </summary>
        /// <param name="fromNode">The node from which to return the next possible nodes in the path</param>
        /// <returns>A list of next possible nodes in the path</returns>
        private IEnumerable<TilePos> GetAdjacentWalkableNodes(TilePos fromNode)
            {
            IEnumerable<TilePos> nextLocations = GetAdjacentLocations(fromNode);

            foreach (var location in nextLocations)
                {
                // Ignore non-walkable nodes
                if (!this._searchParameters.CanBeOccupied!(location))
                    continue;

                yield return location;
                }
            }

        /// <summary>
        /// Returns the four locations immediately orthogonally adjacent to <paramref name="fromLocation"/>
        /// </summary>
        /// <param name="fromLocation">The location from which to return all adjacent points</param>
        /// <returns>The locations as an IEnumerable of Points</returns>
        private static IEnumerable<TilePos> GetAdjacentLocations(TilePos fromLocation)
            {
            var result = new List<TilePos>(4);
            if (fromLocation.X > 0)
                result.Add(fromLocation.GetPositionAfterOneMove(Direction.Left));
            if (fromLocation.Y > 0)
                result.Add(fromLocation.GetPositionAfterOneMove(Direction.Up));
            result.Add(fromLocation.GetPositionAfterOneMove(Direction.Right));
            result.Add(fromLocation.GetPositionAfterOneMove(Direction.Down));
            return result;
            }

        /// <summary>
        /// Gets the estimated distance between a specified point and the goal
        /// </summary>
        private float GetEstimatedTraversalCost(TilePos current)
            {
            var start = this._searchParameters.StartLocation;
            var goal = this._searchParameters.EndLocation;

            int deltaX = Math.Abs(goal.X - current.X);
            int deltaY = Math.Abs(goal.Y - current.Y);
            float result = (deltaX + deltaY);

            // add tie-breaker. This makes the heuristic non-applicable but should improve the routes taken
            int dx1 = current.X - goal.X;
            int dy1 = current.Y - goal.Y;
            int dx2 = start.X - goal.X;
            int dy2 = start.Y - goal.Y;
            int cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
            result += cross * 0.001f;

            return result;
            }
        }
    }
