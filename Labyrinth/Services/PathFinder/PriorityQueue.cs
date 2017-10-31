using System;
using System.Collections.Generic;
using System.Linq;

namespace Labyrinth.Services.PathFinder
    {
    /// <summary>
    /// Priority Queue implemented using a binary heap structure.
    /// </summary>
    /// <typeparam name="P">Type of the priority values</typeparam>
    /// <typeparam name="T">Type of the data items</typeparam>
    /// <remarks>Adapted from article at https://visualstudiomagazine.com/Articles/2012/11/01/Priority-Queues-with-C.aspx</remarks>
    // ReSharper disable once InconsistentNaming
    public class PriorityQueue<P, T> where P: IComparable<P>
        {
        private readonly List<KeyValuePair<P, T>> _data;

        public PriorityQueue()
            {
            this._data = new List<KeyValuePair<P, T>>();
            }

        public PriorityQueue(int initialCapacity)
            {
            this._data = new List<KeyValuePair<P, T>>(initialCapacity);
            }

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

        public IEnumerable<T> Items
            {
            get
                {
                var result = this._data.Select(item => item.Value);
                return result;
                }
            }

        public int Count => _data.Count;

        public override string ToString()
            {
            string s = "";
            for (int i = 0; i < _data.Count; ++i)
                s += _data[i] + " ";
            s += "count = " + _data.Count;
            return s;
            }

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
