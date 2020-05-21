using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Ignitor.State
{
    internal class State : IState
    {
        private readonly IServiceProvider _services;

        public State(IServiceProvider services)
        {
            _services = services;
        }

        public Task<IImmutable<TEntity>> GetStateAsync<TId, TEntity>(
            TId id,
            Func<TId, CancellationToken, Task<TEntity>> defaultValue = null,
            CancellationToken cancellationToken = default) =>
                GetStateAsync(typeof(State), id, defaultValue, cancellationToken);

        public async Task<IImmutable<TEntity>> GetStateAsync<TId, TEntity>(
            Type context,
            TId id,
            Func<TId, CancellationToken, Task<TEntity>> defaultValue = null,
            CancellationToken cancellationToken = default)
        {
            var store = GetStore<TId, TEntity>(context);

            if (!store.ContainsKey(id))
            {
                if (defaultValue == null)
                    return default;

                store.TryAdd(id, await defaultValue.Invoke(id, cancellationToken));
            }

            return store[id];
        }

        public Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync<TId, TEntity>(
            Func<CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValue,
            Func<TEntity, TId> idSelector,
            CancellationToken cancellationToken = default) =>
                GetStateAsync(typeof(State), defaultValue, idSelector, cancellationToken);

        public async Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync<TId, TEntity>(
            Type context,
            Func<CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValue,
            Func<TEntity, TId> idSelector,
            CancellationToken cancellationToken = default)
        {
            var store = GetStore<TId, TEntity>(context);

            if (store.Count == 0 && defaultValue != null && idSelector != null)
            {
                var values = await defaultValue.Invoke(cancellationToken);

                await foreach (var value in values.WithCancellation(cancellationToken))
                {
                    var id = idSelector.Invoke(value);
                    store.AddOrUpdate(id, value);
                }
            }

            return store;
        }

        public Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync<TId, TEntity>(
            Func<CancellationToken, Task<IEnumerable<TEntity>>> defaultValue,
            Func<TEntity, TId> idSelector,
            CancellationToken cancellationToken = default) =>
                GetStateAsync(typeof(State), defaultValue, idSelector, cancellationToken);

        public async Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync<TId, TEntity>(
            Type context,
            Func<CancellationToken, Task<IEnumerable<TEntity>>> defaultValue,
            Func<TEntity, TId> idSelector,
            CancellationToken cancellationToken = default)
        {
            var store = GetStore<TId, TEntity>(context);

            if (store.Count == 0 && defaultValue != null && idSelector != null)
            {
                var values = await defaultValue.Invoke(cancellationToken);

                foreach (var value in values)
                {
                    var id = idSelector.Invoke(value);
                    store.AddOrUpdate(id, value);
                }
            }

            return store;
        }

        public IStateUpdater<TEntity> GetUpdater<TId, TEntity>(TId id) =>
            GetUpdater<TId, TEntity>(typeof(State), id);

        public IStateUpdater<TEntity> GetUpdater<TId, TEntity>(Type context, TId id)
        {
            var store = GetStore<TId, TEntity>(context);
            var notifier = GetNotifier<TId, TEntity>(context);

            return new SingleStateUpdater<TId, TEntity>(notifier, store, id);
        }

        public IStateUpdater<TId, TEntity> GetUpdater<TId, TEntity>() =>
            GetUpdater<TId, TEntity>(typeof(State));

        public IStateUpdater<TId, TEntity> GetUpdater<TId, TEntity>(Type context)
        {
            var store = GetStore<TId, TEntity>(context);
            var notifier = GetNotifier<TId, TEntity>(context);

            return new StateUpdater<TId, TEntity>(notifier, store);
        }

        public IStateNotifier<TId, TEntity> GetNotifier<TId, TEntity>() =>
            GetNotifier<TId, TEntity>(typeof(State));

        public IStateNotifier<TId, TEntity> GetNotifier<TId, TEntity>(Type context)
        {
            var serviceType = typeof(IStateNotifier<,,>).MakeGenericType(context, typeof(TId), typeof(TEntity));

            return (IStateNotifier<TId, TEntity>)_services.GetRequiredService(serviceType);
        }

        private IIgnitorStore<TId, TEntity> GetStore<TId, TEntity>(Type context)
        {
            var serviceType = typeof(IIgnitorStore<,,>).MakeGenericType(context, typeof(TId), typeof(TEntity));

            return (IIgnitorStore<TId, TEntity>)_services.GetRequiredService(serviceType);
        }
    }
}
