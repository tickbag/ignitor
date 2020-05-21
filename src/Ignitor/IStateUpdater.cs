using System;
using System.Threading.Tasks;

namespace Ignitor
{
    public interface IStateUpdater<TEntity>
    {
        Task UpdateAsync(TEntity newValue);
        Task UpdateAsync(IImmutable<TEntity> newValue);
        void Clear();
    }

    public interface IStateUpdater<TId, TEntity>
    {
        Task UpdateAsync(TId id, TEntity newValue);
        Task UpdateAsync(TId id, IImmutable<TEntity> newValue);
        void Clear();
    }
}
