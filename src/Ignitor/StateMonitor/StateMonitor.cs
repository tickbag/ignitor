using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ignitor.StateMonitor
{
    /// <summary>
    /// Instance for registering state change callbacks and signal state change on a scoped state.
    /// </summary>
    /// <typeparam name="TId">Type for the key/id of the state data</typeparam>
    /// <typeparam name="TEntity">Type of the state data</typeparam>
    internal class StateMonitor<TId, TEntity> : IStateMonitor<TId, TEntity>, IStateSignaling<TId, TEntity>
    {
        private readonly List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)> _addedSubscribers =
            new List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)>();

        private readonly List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)> _updatedSubscribers =
            new List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)>();

        private readonly List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)> _removedSubscribers =
            new List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)>();

        private readonly List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)> _changedSubscribers =
            new List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)>();

        /// <inheritdoc/>
        public IDisposable OnAdded(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null) =>
            Subscribe(callback, id, _addedSubscribers, unsubscribe);

        /// <inheritdoc/>
        public IDisposable OnUpdated(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null) =>
            Subscribe(callback, id, _updatedSubscribers, unsubscribe);

        /// <inheritdoc/>
        public IDisposable OnRemoved(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null) =>
            Subscribe(callback, id, _removedSubscribers, unsubscribe);

        /// <inheritdoc/>
        public IDisposable OnChanged(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null) =>
            Subscribe(callback, id, _changedSubscribers, unsubscribe);

        /// <inheritdoc/>
        public IDisposable OnAdded(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null) =>
            Subscribe(callback, default, _addedSubscribers, unsubscribe);

        /// <inheritdoc/>
        public IDisposable OnUpdated(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null) =>
            Subscribe(callback, default, _updatedSubscribers, unsubscribe);

        /// <inheritdoc/>
        public IDisposable OnRemoved(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null) =>
            Subscribe(callback, default, _removedSubscribers, unsubscribe);

        /// <inheritdoc/>
        public IDisposable OnChanged(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null) =>
            Subscribe(callback, default, _changedSubscribers, unsubscribe);

        /// <inheritdoc/>
        public Task StateItemAddedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct) =>
            NotifySubscribers(id, value, _addedSubscribers, ct);

        /// <inheritdoc/>
        public Task StateItemUpdatedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct) =>
            NotifySubscribers(id, value, _updatedSubscribers, ct);

        /// <inheritdoc/>
        public Task StateItemRemovedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct) =>
            NotifySubscribers(id, value, _removedSubscribers, ct);

        /// <inheritdoc/>
        public Task StateChangedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct) =>
            NotifySubscribers(id, value, _changedSubscribers, ct);

        private static IDisposable Subscribe(
            Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback,
            TId id,
            List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)> subscribers,
            IDisposable unsubscribe)
        {
            if (subscribers.Contains((callback, id)))
            {
                subscribers.Remove((callback, id));

                if (unsubscribe != null)
                    unsubscribe.Dispose();
            }

            subscribers.Add((callback, id));

            return new Unsubscriber(subscribers, (callback, id));
        }

        private static async Task NotifySubscribers(TId id, IImmutable<TEntity> value, List<(
            Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)> subscribers,
            CancellationToken ct)
        {
            foreach (var subscriber in subscribers.Where(s => s.Item2.Equals(default(TId)) || s.Item2.Equals(id)).Select(s => s.Item1))
            {
                await subscriber.Invoke(id, value, ct);
            }
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)> _subscribers;
            private readonly (Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId) _subscriber;

            public Unsubscriber(List<(Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId)> subscribers, (Func<TId, IImmutable<TEntity>, CancellationToken, Task>, TId) subscriber)
            {
                _subscribers = subscribers;
                _subscriber = subscriber;
            }

            public void Dispose()
            {
                if (_subscribers != null)
                    _subscribers.Remove(_subscriber);
            }
        }
    }
}
