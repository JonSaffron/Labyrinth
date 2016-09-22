using System;
using System.Collections;
using System.Collections.Generic;

namespace Labyrinth.Services.PathFinder
    {
    class Path<T> : IEnumerable<T>, IComparable<Path<T>>
        {
        public T LastStep { get; private set; }

        public Path<T> PreviousSteps { get; private set; }

        /// <summary>
        /// The actual cost of this path
        /// </summary>
        public int Cost { get; private set; }

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
            this.PreviousSteps = previousSteps;
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
            for (Path<T> p = this; p != null; p = p.PreviousSteps)
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
            var result = this.IsViable ? "Viable" : "Not viable";
            var steps = string.Empty;
            var i = 0;
            for (var p = this; p != null; p = this.PreviousSteps)
                {
                i++;
                steps += p.LastStep;
                }
            result += " path of " + i + " steps " + steps;
            return result;
            }
        }
    }
