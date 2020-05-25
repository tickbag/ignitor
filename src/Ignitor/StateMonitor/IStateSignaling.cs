using System.Threading;
using System.Threading.Tasks;

namespace Ignitor.StateMonitor
{
    internal interface IStateSignaling<TId, TEntity>
    {
        Task StateItemAddedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct);
        Task StateItemUpdatedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct);
        Task StateItemRemovedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct);
        Task StateChangedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct);
    }
}
