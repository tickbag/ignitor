using System.Collections.Generic;

namespace Ignitor
{
    public interface IIgnitorStore<TId, TEntity> : IReadOnlyDictionary<TId, IImmutable<TEntity>>
    {
        void Remove(TId id);

        void Clear();

        bool TryAdd(TId id, TEntity value);

        bool TryAdd(TId id, IImmutable<TEntity> value);

        void AddOrUpdate(TId id, TEntity value);

        void AddOrUpdate(TId id, IImmutable<TEntity> value);
    }
}
