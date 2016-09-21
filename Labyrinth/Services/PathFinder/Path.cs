using System.Collections;
using System.Collections.Generic;

namespace Labyrinth.Services.PathFinder
    {
    class Path<T> : IEnumerable<T>
        {
        public T LastStep { get; private set; }
        public Path<T> PreviousSteps { get; private set; }
        public int TotalCost { get; private set; }

        public Path(T start) : this(start, null, 0) 
            {
            // nothing to do
            }

        private Path(T lastStep, Path<T> previousSteps, int totalCost)
            {
            this.LastStep = lastStep;
            this.PreviousSteps = previousSteps;
            this.TotalCost = totalCost;
            }

        public Path<T> AddStep(T step, int stepCost)
            {
            var result = new Path<T>(step, this, this.TotalCost + stepCost);
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
        }
    }
