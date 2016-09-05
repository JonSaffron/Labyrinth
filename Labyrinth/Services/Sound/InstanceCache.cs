using System;
using System.Linq;

namespace Labyrinth.Services.Sound
    {
    public class InstanceCache<T> : IDisposable where T: class 
        {
        private readonly Func<T> _createNewInstance;
        private readonly T[] _cache;
        protected readonly int Size;
        protected int Position;
        // ReSharper disable once MemberCanBePrivate.Global
        protected bool IsDisposed;

        public InstanceCache(int countOfEntries, Func<T> createNewInstance)
            {
            if (countOfEntries <= 0)
                throw new ArgumentOutOfRangeException("countOfEntries");

            this._createNewInstance = createNewInstance;
            this._cache = new T[countOfEntries];
            this.Size = countOfEntries;
            }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
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
            this.Position = (this.Position + 1) % this.Size;
            return result;
            }

        protected virtual T CreateNewInstance()
            {
            var result = this._createNewInstance();
            if (result == null)
                throw new InvalidOperationException();
            return result;
            }

        protected int Count
            {
            get
                {
                var result = this._cache.Count(item => item != null);
                return result;
                }
            }

        public void Dispose()
            {
            foreach (var item in this._cache.OfType<IDisposable>())
                {
                item.Dispose();
                }
            this.IsDisposed = true;
            }

        public override string ToString()
            {
            string result = string.Format("Caching {0} of {1} objects of type {2}", this.Count, this.Size, typeof(T).Name);
            return result;
            }
        }
    }
