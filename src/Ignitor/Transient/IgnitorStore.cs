using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Ignitor.Immutables;

namespace Ignitor.Transient
{
    internal class IgnitorStore<TContext, TId, TEntity> : IReadOnlyDictionary<TId, IImmutable<TEntity>>, IIgnitorStore<TContext, TId, TEntity>
    {
        private readonly ConcurrentDictionary<TId, IImmutable<TEntity>> _store = new ConcurrentDictionary<TId, IImmutable<TEntity>>();

        private readonly IServiceProvider _services;

        public IgnitorStore(IServiceProvider services)
        {
            _services = services;
        }

        public IImmutable<TEntity> this[TId key]
        {
            get
            {
                return _store[key];
            }
        }

        public IEnumerable<TId> Keys => _store.Keys;

        public IEnumerable<IImmutable<TEntity>> Values => _store.Values;

        public int Count => _store.Count;

        public void Clear() =>
            _store.Clear();

        public bool ContainsKey(TId key) =>
            _store.ContainsKey(key);

        public IEnumerator<KeyValuePair<TId, IImmutable<TEntity>>> GetEnumerator() =>
            _store.GetEnumerator();

        public void Remove(TId id) =>
            _store.TryRemove(id, out _);

        public void AddOrUpdate(TId id, TEntity value)
        {
            var immutable = new Immutable<TEntity>(_services, value);
            _store.AddOrUpdate(id, immutable, (a, b) => immutable);
        }

        public bool TryAdd(TId id, TEntity value)
        {
            var immutable = new Immutable<TEntity>(_services, value);
            return _store.TryAdd(id, immutable);
        }

        public bool TryAdd(TId id, IImmutable<TEntity> value) =>
            _store.TryAdd(id, value);

        public bool TryGetValue(TId key, out IImmutable<TEntity> value) =>
            _store.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
