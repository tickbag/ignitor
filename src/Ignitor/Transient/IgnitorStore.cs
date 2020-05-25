using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Ignitor.Transient
{
    /// <summary>
    /// An Ignitor non-persistent, immutable store. This implements an IReadOnlyDictionary interface to provide data access.
    /// All data is stored as an IImmutable type as it is inserted/updated.
    /// Modification of data in the store is via specialised methods based on a Dictionary interface.
    /// </summary>
    /// <typeparam name="TId">Type to key the stored data on</typeparam>
    /// <typeparam name="TEntity">Type of data to store. This type will be wrapped in an IImmutable interface by the store.</typeparam>
    internal class IgnitorStore<TId, TEntity> : IReadOnlyDictionary<TId, IImmutable<TEntity>>, IIgnitorStore<TId, TEntity>
    {
        private readonly ConcurrentDictionary<TId, IImmutable<TEntity>> _store = new ConcurrentDictionary<TId, IImmutable<TEntity>>();

        /// <inheritdoc/>
        public IImmutable<TEntity> this[TId key]
        {
            get
            {
                return _store[key];
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TId> Keys => _store.Keys;

        /// <inheritdoc/>
        public IEnumerable<IImmutable<TEntity>> Values => _store.Values;

        /// <inheritdoc/>
        public int Count => _store.Count;

        /// <inheritdoc/>
        public void Clear() =>
            _store.Clear();

        /// <inheritdoc/>
        public bool ContainsKey(TId key) =>
            _store.ContainsKey(key);

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TId, IImmutable<TEntity>>> GetEnumerator() =>
            _store.GetEnumerator();

        /// <inheritdoc/>
        public void Remove(TId id) =>
            _store.TryRemove(id, out _);

        /// <inheritdoc/>
        public void AddOrUpdate(TId id, TEntity value)
        {
            var immutable = value.MakeImmutable();
            _store.AddOrUpdate(id, immutable, (a, b) => immutable);
        }

        /// <inheritdoc/>
        public void AddOrUpdate(TId id, IImmutable<TEntity> value) =>
            _store.AddOrUpdate(id, value, (a, b) => value);

        /// <inheritdoc/>
        public bool TryAdd(TId id, TEntity value)
        {
            var immutable = value.MakeImmutable();
            return _store.TryAdd(id, immutable);
        }

        /// <inheritdoc/>
        public bool TryAdd(TId id, IImmutable<TEntity> value) =>
            _store.TryAdd(id, value);

        /// <inheritdoc/>
        public bool TryGetValue(TId key, out IImmutable<TEntity> value) =>
            _store.TryGetValue(key, out value);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
