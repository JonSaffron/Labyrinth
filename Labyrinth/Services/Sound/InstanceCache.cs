using System;
using System.Linq;

namespace Labyrinth.Services.Sound
    {
    public class InstanceCache<T> : IDisposable where T: class 
        {
        private readonly Func<T> _createNewInstance;
        private readonly T[] _cache;
        private readonly int _size;
        protected int Position;
        protected bool IsDisposed;

        public InstanceCache(int countOfEntries, Func<T> createNewInstance)
            {
            if (countOfEntries <= 0)
                throw new ArgumentOutOfRangeException("countOfEntries");

            this._createNewInstance = createNewInstance;
            this._cache = new T[countOfEntries];
            this._size = countOfEntries;
            }

        public virtual T GetNext()
            {
            if (this.IsDisposed)
                throw new ObjectDisposedException("InstanceCache");

            T result = this._cache[this.Position];
            if (result == null) 
                {
                result = CreateNewInstance();
                this._cache[this.Position] = result;
                }
            this.Position = (this.Position + 1) % this._size;
            return result;
            }

        protected virtual T CreateNewInstance()
            {
            var result = this._createNewInstance();
            if (result == null)
                throw new InvalidOperationException();
            return result;
            }

        public void Dispose()
            {
            foreach (var item in this._cache.OfType<IDisposable>())
                {
                item.Dispose();
                }
            this.IsDisposed = true;
            }
        }
    }
