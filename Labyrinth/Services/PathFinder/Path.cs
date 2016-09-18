using System.Collections;
using System.Collections.Generic;

namespace Labyrinth.Services.PathFinder
    {
    class ImmutableStack<T> : IEnumerable<T>
        {
        public T LastStep { get; private set; }
        public ImmutableStack<T> PreviousSteps { get; private set; }
        public double TotalCost { get; private set; }

        public ImmutableStack(T start) : this(start, null, 0) 
            {
            }

        private ImmutableStack(T lastStep, ImmutableStack<T> previousSteps, double totalCost)
            {
            this.LastStep = lastStep;
            this.PreviousSteps = previousSteps;
            this.TotalCost = totalCost;
            }

        public ImmutableStack<T> AddStep(T step, double stepCost)
            {
            var result = new ImmutableStack<T>(step, this, this.TotalCost + stepCost);
            return result;
            }

        public IEnumerator<T> GetEnumerator()
            {
            for (ImmutableStack<T> p = this; p != null; p = p.PreviousSteps)
                yield return p.LastStep;
            }

        IEnumerator IEnumerable.GetEnumerator()
            {
            return this.GetEnumerator();
            }
        }
    }
