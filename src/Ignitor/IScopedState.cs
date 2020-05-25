using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ignitor
{
    public interface IScopedState : IState, IDisposable { }

    public interface IScopedState<TId, TEntity> : IScopedState
    {
        Task<IImmutable<TEntity>> GetAsync(TId id, CancellationToken ct = default);
        Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetAsync(CancellationToken ct = default);
        IStateMonitor<TId, TEntity> Monitor();
        IStateUpdater<TId, TEntity> Updater();
        IStateUpdater<TEntity> Updater(TId id);
        IScopedState<TId, TEntity> Fuel(Func<IState, CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValues);
        IScopedState<TId, TEntity> Fuel(Func<IState, CancellationToken, Task<IEnumerable<TEntity>>> defaultValues);
        IScopedState<TId, TEntity> Fuel(Func<IState, TId, CancellationToken, Task<TEntity>> defaultValue);
        IScopedState<TId, TEntity> WithId(Func<TEntity, TId> idSelector);
    }
}
