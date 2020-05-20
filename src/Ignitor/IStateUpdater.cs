using System;
using System.Threading.Tasks;

namespace Ignitor
{
    public interface IStateUpdater<TEntity>
    {
        Task UpdateAsync(TEntity newValue);
        void Clear();
    }

    public interface IStateUpdater<TId, TEntity>
    {
        Task UpdateAsync(TId id, TEntity newValue);
        void Clear();
    }
}
