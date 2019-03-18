using System;

namespace Labyrinth.DataStructures
    {
    public class SimpleList<T>
        {
        private T[] _items;
        private static readonly T[]  EmptyArray = new T[0];        
        
        public int Length { get; private set; }

        public SimpleList(int capacity)
            {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            this._items = capacity == 0 ? EmptyArray : new T[capacity];
            }

        public int Capacity
            {
            get => this._items.Length;
            set
                {
                if (value < this.Length) 
                    {
                    throw new ArgumentOutOfRangeException(nameof(value));
                    }

                if (value == this._items.Length) 
                    return;

                if (value > 0) {
                    T[] newItems = new T[value];
                    if (this.Length > 0) 
                        {
                        Array.Copy(this._items, 0, newItems, 0, this.Length);
                        }
                    this._items = newItems;
                    }
                else 
                    {
                    this._items = EmptyArray;
                    }
                }
            }


        public void Add(T item) 
            {
            if (this.Length == this._items.Length)
                {
                EnsureCapacity(this.Length + 1);
                }
            this._items[this.Length] = item;
            this.Length += 1;
            }

        public void RemoveAt(int index)
            {
            if (index < 0 || index >= this.Length) 
                {
                throw new ArgumentOutOfRangeException(nameof(index));
                }

            var indexOfLastItem = this.Length - 1;
            if (indexOfLastItem == 0 || index == indexOfLastItem)
                {
                this._items[index] = default;
                }
            else
                {
                // remove item using swap and pop
                this._items[index] = this._items[indexOfLastItem];
                this._items[indexOfLastItem] = default;
                }
                
            this.Length--;
            }

        private void EnsureCapacity(int min) {
            const int maxArrayLength = 0X7FEFFFFF;

            if (_items.Length < min) {
                int newCapacity = _items.Length == 0 ? 100 : _items.Length + 100;
                // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                if ((uint)newCapacity > maxArrayLength) 
                    newCapacity = maxArrayLength;
                if (newCapacity < min)
                    newCapacity = min;
                Capacity = newCapacity;
                }
            }
   
        public int IndexOf(T item) 
            {
            return Array.IndexOf(this._items, item, 0, this.Length);
            }

        public void CopyTo(T[] array) 
            {
            // Delegate rest of error checking to Array.Copy.
            Array.Copy(this._items, 0, array, 0, this.Length);
            }
        }
    }
