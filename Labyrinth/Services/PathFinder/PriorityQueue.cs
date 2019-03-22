using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Labyrinth.Services.PathFinder
    {
    /// <summary>
    /// Priority Queue implemented using a binary heap structure.
    /// </summary>
    /// <typeparam name="P">Type of the priority values</typeparam>
    /// <typeparam name="T">Type of the data items</typeparam>
    /// <remarks>Adapted from article at https://visualstudiomagazine.com/Articles/2012/11/01/Priority-Queues-with-C.aspx</remarks>
    [DebuggerDisplay("{" + nameof(ToString) + "()}")]
    // ReSharper disable once InconsistentNaming
    public class PriorityQueue<P, T> where P: IComparable<P>
        {
        private readonly List<KeyValuePair<P, T>> _data;

        /// <summary>
        /// Instantiate a new priority queue with the default capacity
        /// </summary>
        public PriorityQueue()
            {
            this._data = new List<KeyValuePair<P, T>>();
            }

        /// <summary>
        /// Instantiate a new priority queue with the specified initial capacity
        /// </summary>
        public PriorityQueue(int initialCapacity)
            {
            this._data = new List<KeyValuePair<P, T>>(initialCapacity);
            }

        /// <summary>
        /// Adds a new item to the queue
        /// </summary>
        /// <param name="priority">The priority of the item</param>
        /// <param name="item">The item to be queued</param>
        public void Enqueue(P priority, T item)
            {
            this._data.Add(new KeyValuePair<P, T>(priority, item));

            int childIndex = _data.Count - 1; // child index; start at end
            while (childIndex > 0)
                {
                int parentIndex = (childIndex - 1) / 2; // parent index
                if (this._data[childIndex].Key.CompareTo(this._data[parentIndex].Key) >= 0) 
                    break; // child item is larger than (or equal) parent so we're done
                KeyValuePair<P, T> tmp = this._data[childIndex]; 
                this._data[childIndex] = this._data[parentIndex]; 
                this._data[parentIndex] = tmp;
                childIndex = parentIndex;
                }
            }

        /// <summary>
        /// Removes and returns the item at the top of the queue
        /// </summary>
        /// <returns>The item with the lowest priority score</returns>
        [MustUseReturnValue]
        public T Dequeue()
            {
            if (this._data.Count == 0)
                throw new InvalidOperationException("Queue is empty");

            int lastIndex = _data.Count - 1; // last index (before removal)
            KeyValuePair<P, T> frontItem = this._data[0];   // fetch the front
            this._data[0] = this._data[lastIndex];
            this._data.RemoveAt(lastIndex);

            lastIndex -= 1; // last index (after removal)
            int parentIndex = 0; // parent index. start at front of pq
            while (true)
                {
                int leftChildOfParentIndex = parentIndex * 2 + 1; // left child index of parent
                if (leftChildOfParentIndex > lastIndex) 
                    break;  // no children so done

                int rightChildOfParentIndex = leftChildOfParentIndex + 1;     // right child
                if (rightChildOfParentIndex <= lastIndex && this._data[rightChildOfParentIndex].Key.CompareTo(this._data[leftChildOfParentIndex].Key) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    leftChildOfParentIndex = rightChildOfParentIndex;

                if (this._data[parentIndex].Key.CompareTo(this._data[leftChildOfParentIndex].Key) <= 0) 
                    break; // parent is smaller than (or equal to) smallest child so done

                KeyValuePair<P, T> tmp = this._data[parentIndex]; 
                this._data[parentIndex] = this._data[leftChildOfParentIndex]; 
                this._data[leftChildOfParentIndex] = tmp; // swap parent and child
                parentIndex = leftChildOfParentIndex;
                }
            return frontItem.Value;
            }

        /// <summary>
        /// Returns all the items in the queue in no particular order.
        /// </summary>
        public IEnumerable<T> Items
            {
            get
                {
                var result = this._data.Select(item => item.Value);
                return result;
                }
            }

        /// <summary>
        /// Returns true if the queue is empty, false otherwise
        /// </summary>
        public bool IsEmpty => this._data.Count == 0;

        /// <summary>
        /// Returns a count of all the items currently in the queue.
        /// </summary>
        public int Count => this._data.Count;

        /// <summary>
        /// Returns a string representation of the queue contents
        /// </summary>
        /// <returns>A string value that contains the default representation of the queue items</returns>
        public override string ToString()
            {
            var c = this._data.Count;
            if (c == 0)
                return "(empty)";
            string s = $"{c} item" + (c != 1 ? "s" : string.Empty);
            for (int i = 0; i < c; i++)
                s += $"{(i == 0 ? ":" : ",")} {this._data[i]}";
            s += ".";
            return s;
            }

        /// <summary>
        /// Checks whether the queue is correctly constructed
        /// </summary>
        /// <returns>True is the queue's binary heap is consistent, otherwise false.</returns>
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public bool IsConsistent()
            {
            // is the heap property true for all data?
            if (this._data.Count == 0) 
                return true;

            int lastIndex = _data.Count - 1; // last index
            for (int parentIndex = 0; parentIndex < _data.Count; parentIndex++) // each parent index
                {
                int leftChildIndex = 2 * parentIndex + 1; // left child index
                int rightChildIndex = 2 * parentIndex + 2; // right child index

                if (leftChildIndex <= lastIndex && this._data[parentIndex].Key.CompareTo(this._data[leftChildIndex].Key) > 0) 
                    return false; // if lc exists and it's greater than parent then bad.

                if (rightChildIndex <= lastIndex && this._data[parentIndex].Key.CompareTo(this._data[rightChildIndex].Key) > 0) 
                    return false; // check the right child too.
                }

            return true; // passed all checks
            }
        }
    }
