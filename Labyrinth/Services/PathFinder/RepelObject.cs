﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Labyrinth.DataStructures;

namespace Labyrinth.Services.PathFinder
    {
    public class RepelObject
        {
        private readonly PriorityQueue<float, Path<TilePos>> _openNodes = new PriorityQueue<float, Path<TilePos>>();
        private readonly HashSet<Path<TilePos>> _closedNodes = new HashSet<Path<TilePos>>();
        private readonly RepelParameters _repelParameters;

        /// <summary>
        /// Create a new instance of PathFinder
        /// </summary>
        /// <param name="repelParameters">An instance of a RepelParameters object that defines the search to perform</param>
        public RepelObject(RepelParameters repelParameters)
            {
            if (repelParameters == null)
                throw new ArgumentNullException(nameof(repelParameters));
            if (repelParameters.CanBeOccupied == null)
                throw new ArgumentException("RepelParameters.CanBeOccupied must be set.");
            this._repelParameters = repelParameters;
            }

        /// <summary>
        /// Attempts to find a path from the start location to the end location based on the supplied SearchParameters
        /// </summary>
        /// <returns>A List of Points representing the path. If no path was found, the returned list is empty.</returns>
        public bool TryFindPath([NotNullWhen(returnValue: true)] out IList<TilePos>? result)
            {
            // The start node is the first entry in the 'open' list
            this._openNodes.Enqueue(0, new Path<TilePos>(this._repelParameters.StartLocation));

            while (this._openNodes.Count != 0)
                {
                var path = this._openNodes.Dequeue();
                if (!path.IsViable)
                    continue;

                var currentDistanceAwayFromRepelOrigin = ManhattanDistance(this._repelParameters.RepelLocation, path.LastStep);
                if (currentDistanceAwayFromRepelOrigin >= this._repelParameters.MinimumDistanceToMoveAway)
                    {
                    result = path.Reverse().Skip(1).ToList();
                    return true;
                    }

                this._closedNodes.Add(path);

                IEnumerable<TilePos> nextNodes = GetAdjacentWalkableNodes(path.LastStep);
                foreach (var nextNode in nextNodes)
                    {
                    var newPath = path.AddStep(nextNode, 1);
                    if (newPath.Cost > this._repelParameters.MaximumLengthOfPath)
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

                    var pathsAlreadyVisitedThatEndUpInTheSamePlace = this._closedNodes.Where(item => item.LastStep == newPath.LastStep);
                    if (pathsAlreadyVisitedThatEndUpInTheSamePlace.Any(p => p.Cost <= newPath.Cost))
                        // don't add the newPath as it's no better than one we've already found
                        continue;

                    var estimatedCostToGoal = -(ManhattanDistance(this._repelParameters.RepelLocation, nextNode)) + GetTieBreaker(nextNode);
                    this._openNodes.Enqueue(estimatedCostToGoal, newPath);
                    }
                }

            // The method returns false if no path can be found away from the object specified
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
                if (!this._repelParameters.CanBeOccupied!(location))
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
        private static int ManhattanDistance(TilePos tp1, TilePos tp2)
            {
            int deltaX = Math.Abs(tp1.X - tp2.X);
            int deltaY = Math.Abs(tp1.Y - tp2.Y);
            int result = (deltaX + deltaY);
            return result;
            }

        /// <summary>
        /// Gets a value that can be used to adjust the path so that it continues a line between the RepelLocation and the StartLocation
        /// </summary>
        /// <param name="current">The position to consider</param>
        /// <returns>A float value that measures closeness to the line between RepelLocation and StartLocation</returns>
        private float GetTieBreaker(TilePos current)
            {
            var start = this._repelParameters.StartLocation;
            var goal = this._repelParameters.RepelLocation;

            // add tie-breaker. This makes the heuristic non-applicable but should improve the routes taken
            int dx1 = start.X - goal.X;
            int dy1 = start.Y - goal.Y;
            int dx2 = current.X - goal.X;
            int dy2 = current.Y - goal.Y;
            int cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
            float result = cross * 0.001f;
            return result;
            }
        }
    }
