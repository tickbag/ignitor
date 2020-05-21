using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ignitor
{
    public interface IState
    {
        // Async
        Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync<TId, TEntity>(Type context,
            Func<CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValue,
            Func<TEntity, TId> idSelector,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync<TId, TEntity>(Func<CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValue,
            Func<TEntity, TId> idSelector,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync<TId, TEntity>(Type context,
            Func<CancellationToken, Task<IEnumerable<TEntity>>> defaultValue,
            Func<TEntity, TId> idSelector,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync<TId, TEntity>(Func<CancellationToken, Task<IEnumerable<TEntity>>> defaultValue,
            Func<TEntity, TId> idSelector,
            CancellationToken cancellationToken = default);

        Task<IImmutable<TEntity>> GetStateAsync<TId, TEntity>(TId id, Func<TId, CancellationToken, Task<TEntity>> defaultValue, CancellationToken cancellationToken = default);

        Task<IImmutable<TEntity>> GetStateAsync<TId, TEntity>(Type context, TId id, Func<TId, CancellationToken, Task<TEntity>> defaultValue, CancellationToken cancellationToken = default);

        IStateUpdater<TId, TEntity> GetUpdater<TId, TEntity>();

        IStateUpdater<TEntity> GetUpdater<TId, TEntity>(TId id);

        IStateUpdater<TId, TEntity> GetUpdater<TId, TEntity>(Type context);

        IStateUpdater<TEntity> GetUpdater<TId, TEntity>(Type context, TId id);

        IStateNotifier<TId, TEntity> GetNotifier<TId, TEntity>();

        IStateNotifier<TId, TEntity> GetNotifier<TId, TEntity>(Type context);
    }

    public interface IState<TId, TEntity>
    {
        Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync(Func<CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValue, CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync(Func<CancellationToken, Task<IEnumerable<TEntity>>> defaultValue = null, CancellationToken cancellationToken = default);

        Task<IImmutable<TEntity>> GetStateAsync(TId id, Func<TId, CancellationToken, Task<TEntity>> defaultValue = null, CancellationToken cancellationToken = default);

        IStateUpdater<TId, TEntity> GetUpdater();

        IStateUpdater<TEntity> GetUpdater(TId id);

        IStateNotifier<TId, TEntity> GetNotifier();
    }

    public interface IState<TContext, TId, TEntity> : IState<TId, TEntity> { }
}
