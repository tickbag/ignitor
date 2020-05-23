using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ignitor.Notifier
{
    internal class StateNotifier<TContext, TId, TEntity> : IStateNotifier<TContext, TId, TEntity>, IStateSignaling<TId, TEntity>
    {
        private readonly List<(Func<TId, IImmutable<TEntity>, Task>, TId)> _addedSubscribers = new List<(Func<TId, IImmutable<TEntity>, Task>, TId)>();
        private readonly List<(Func<TId, IImmutable<TEntity>, Task>, TId)> _updatedSubscribers = new List<(Func<TId, IImmutable<TEntity>, Task>, TId)>();
        private readonly List<(Func<TId, IImmutable<TEntity>, Task>, TId)> _removedSubscribers = new List<(Func<TId, IImmutable<TEntity>, Task>, TId)>();
        private readonly List<(Func<TId, IImmutable<TEntity>, Task>, TId)> _changedSubscribers = new List<(Func<TId, IImmutable<TEntity>, Task>, TId)>();

        public IDisposable OnAdded(Func<TId, IImmutable<TEntity>, Task> callback, TId id = default) =>
            Subscribe(callback, id, _addedSubscribers);

        public IDisposable OnUpdated(Func<TId, IImmutable<TEntity>, Task> callback, TId id = default) =>
            Subscribe(callback, id, _updatedSubscribers);

        public IDisposable OnRemoved(Func<TId, IImmutable<TEntity>, Task> callback, TId id = default) =>
            Subscribe(callback, id, _removedSubscribers);

        public IDisposable OnChanged(Func<TId, IImmutable<TEntity>, Task> callback, TId id = default) =>
            Subscribe(callback, id, _changedSubscribers);

        public Task StateItemAddedAsync(TId id, IImmutable<TEntity> value) =>
            NotifySubscribers(id, value, _addedSubscribers);

        public Task StateItemUpdatedAsync(TId id, IImmutable<TEntity> value) =>
            NotifySubscribers(id, value, _updatedSubscribers);

        public Task StateItemRemovedAsync(TId id, IImmutable<TEntity> value) =>
            NotifySubscribers(id, value, _removedSubscribers);

        public Task StateChangedAsync(TId id, IImmutable<TEntity> value) =>
            NotifySubscribers(id, value, _changedSubscribers);

        private IDisposable Subscribe(Func<TId, IImmutable<TEntity>, Task> callback, TId id, List<(Func<TId, IImmutable<TEntity>, Task>, TId)> subscribers)
        {
            if (subscribers.Contains((callback, id)))
            {
                subscribers.Remove((callback, id));
            }

            subscribers.Add((callback, id));

            return new Unsubscriber(subscribers, (callback, id));
        }

        private async Task NotifySubscribers(TId id, IImmutable<TEntity> value, List<(Func<TId, IImmutable<TEntity>, Task>, TId)> subscribers)
        {
            foreach (var subscriber in subscribers.Where(s => s.Item2.Equals(default(TId)) || s.Item2.Equals(id)).Select(s => s.Item1))
            {
                await subscriber.Invoke(id, value);
            }
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<(Func<TId, IImmutable<TEntity>, Task>, TId)> _subscribers;
            private readonly (Func<TId, IImmutable<TEntity>, Task>, TId) _subscriber;

            public Unsubscriber(List<(Func<TId, IImmutable<TEntity>, Task>, TId)> subscribers, (Func<TId, IImmutable<TEntity>, Task>, TId) subscriber)
            {
                _subscribers = subscribers;
                _subscriber = subscriber;
            }

            public void Dispose()
            {
                if (_subscribers != null)
                {
                    _subscribers.Remove(_subscriber);
                }
            }
        }
    }
}
