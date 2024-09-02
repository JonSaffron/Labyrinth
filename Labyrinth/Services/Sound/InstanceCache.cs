using System;
using System.Linq;

namespace Labyrinth.Services.Sound
    {
    public class InstanceCache<T> : IDisposable where T: class 
        {
        private readonly Func<T>? _createNewInstance;
        private readonly T?[] _cache;
        protected readonly int Size;
        protected int Position;
        // ReSharper disable once MemberCanBePrivate.Global
        protected bool IsDisposed;

        protected InstanceCache(int countOfEntries)
            {
            if (countOfEntries <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfEntries));
            this._cache = new T[countOfEntries];
            this.Size = countOfEntries;
            }

        public InstanceCache(int countOfEntries, Func<T> createNewInstance) : this(countOfEntries)
            {
            this._createNewInstance = createNewInstance ?? throw new ArgumentNullException(nameof(createNewInstance));
            }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        public virtual T GetNext()
            {
            if (this.IsDisposed)
                throw new ObjectDisposedException("InstanceCache");

            T? result = this._cache[this.Position];
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
            if (this._createNewInstance == null)
                throw new InvalidOperationException("CreateNewInstance property has not been set.");
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
            if (!this.IsDisposed)
                {
                for (int i = this.Size - 1; i >= 0; i--)
                    {
                    (this._cache[i] as IDisposable)?.Dispose();
                    this._cache[i] = null;
                    }
                GC.SuppressFinalize(this);

                this.IsDisposed = true;
                }
            }

        public override string ToString()
            {
            string result = $"Caching {this.Count} of {this.Size} objects of type {typeof(T).Name}";
            return result;
            }
        }
    }
