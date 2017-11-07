using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Labyrinth.Services.PathFinder
    {
    /// <summary>
    /// An ordered list of items with an associated cost which is implemented as an immutable list for memory efficiency
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{" + nameof(ToString) + "()}")] 
    public class Path<T> : IEnumerable<T>, IComparable<Path<T>>
        {
        /// <summary>
        /// The last step along the path
        /// </summary>
        public T LastStep { get; }

        private readonly Path<T> _previousSteps;

        /// <summary>
        /// The actual cost of this path
        /// </summary>
        public int Cost { get; }

        /// <summary>
        /// Sets or returns whether this is a viable path
        /// </summary>
        public bool IsViable { get; set; }

        public Path(T start) : this(start, null, 0) 
            {
            // nothing to do
            }

        private Path(T lastStep, Path<T> previousSteps, int cost)
            {
            this.LastStep = lastStep;
            this._previousSteps = previousSteps;
            this.Cost = cost;
            this.IsViable = true;
            }

        public Path<T> AddStep(T step, int stepCost)
            {
            var result = new Path<T>(step, this, this.Cost + stepCost);
            return result;
            }

        public IEnumerator<T> GetEnumerator()
            {
            for (Path<T> p = this; p != null; p = p._previousSteps)
                yield return p.LastStep;
            }

        IEnumerator IEnumerable.GetEnumerator()
            {
            return this.GetEnumerator();
            }

        public int CompareTo(Path<T> other)
            {
            var result = other.Cost - this.Cost;
            return result;
            }

        public override string ToString()
            {
            var steps = string.Empty;
            var i = 0;
            for (var p = this; p != null; p = p._previousSteps)
                {
                i++;
                steps = p.LastStep + steps;
                }
            var result = this.IsViable ? "Viable" : "Not viable";
            result += " path " + i + " steps, cost=" + this.Cost + " " + steps;
            return result;
            }
        }
    }
