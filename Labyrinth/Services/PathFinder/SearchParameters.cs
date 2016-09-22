
using System;

namespace Labyrinth.Services.PathFinder
    {
    /// <summary>
    /// Defines the parameters which will be used to find a path across a section of the map
    /// </summary>
    public class SearchParameters
        {
        /// <summary>
        /// Sets or returns the location to start the search from
        /// </summary>
        public TilePos StartLocation { get; set; }

        /// <summary>
        /// Sets or returns the target location
        /// </summary>
        public TilePos EndLocation { get; set; }
        
        /// <summary>
        /// Sets or returns a delegate that takes a co-ordinate and returns true when that location is walkable and false otherwise
        /// </summary>
        public Func<TilePos, bool> CanBeOccupied { get; set; }

        /// <summary>
        /// Sets or returns the maximum length of the path that can be travelled
        /// </summary>
        public int? MaximumLengthOfPath { get; set; }
        }
    }
