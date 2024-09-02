using System;
using Labyrinth.DataStructures;

namespace Labyrinth.Services.PathFinder
    {
    public class RepelParameters
        {
        private int _maximumLengthOfPath;
        private int _minimumDistanceToMoveAway;

        /// <summary>
        /// Sets or returns the location to start any resulting path from
        /// </summary>
        public TilePos StartLocation { get; set; }

        /// <summary>
        /// Sets or returns the location to move away from
        /// </summary>
        public TilePos RepelLocation { get; set; }

        /// <summary>
        /// Sets or returns a delegate that takes a co-ordinate and returns true when that location is walkable and false otherwise
        /// </summary>
        public Func<TilePos, bool>? CanBeOccupied { get; set; }

        /// <summary>
        /// Sets or returns the maximum length of the path that can be traveled
        /// </summary>
        public int MaximumLengthOfPath 
            { 
            get => this._maximumLengthOfPath;
            set
                {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Cannot be negative.");
                this._maximumLengthOfPath = value;
                }
            }

        /// <summary>
        /// Sets or returns the distance the object must move away from the RepelLocation
        /// </summary>
        public int MinimumDistanceToMoveAway 
            { 
            get => this._minimumDistanceToMoveAway;
            set
                {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Cannot be negative.");
                this._minimumDistanceToMoveAway = value;
                }
            }
        }
    }
