using System.Threading;
using System.Threading.Tasks;

namespace Ignitor
{
    public interface IStateUpdater<TEntity>
    {
        Task UpdateAsync(TEntity newValue, CancellationToken ct = default);
        Task UpdateAsync(IImmutable<TEntity> newValue, CancellationToken ct = default);
        void Clear();
    }

    public interface IStateUpdater<TId, TEntity>
    {
        Task UpdateAsync(TId id, TEntity newValue, CancellationToken ct = default);
        Task UpdateAsync(TId id, IImmutable<TEntity> newValue, CancellationToken ct = default);
        void Clear();
    }
}
