using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ignitor.Notifier
{
    internal class StateNotifier<TContext, TId, TEntity> : IStateNotifier<TContext, TId, TEntity>, IStateSignaling<TId, TEntity>
    {
        private readonly List<Func<TId, IImmutable<TEntity>, Task>> _addedSubscribers = new List<Func<TId, IImmutable<TEntity>, Task>>();
        private readonly List<Func<TId, IImmutable<TEntity>, Task>> _updatedSubscribers = new List<Func<TId, IImmutable<TEntity>, Task>>();
        private readonly List<Func<TId, IImmutable<TEntity>, Task>> _removedSubscribers = new List<Func<TId, IImmutable<TEntity>, Task>>();
        private readonly List<Func<TId, IImmutable<TEntity>, Task>> _changedSubscribers = new List<Func<TId, IImmutable<TEntity>, Task>>();

        public IDisposable OnAdded(Func<TId, IImmutable<TEntity>, Task> callback) =>
            Subscribe(callback, _addedSubscribers);

        public IDisposable OnUpdated(Func<TId, IImmutable<TEntity>, Task> callback) =>
            Subscribe(callback, _updatedSubscribers);

        public IDisposable OnRemoved(Func<TId, IImmutable<TEntity>, Task> callback) =>
            Subscribe(callback, _removedSubscribers);

        public IDisposable OnChanged(Func<TId, IImmutable<TEntity>, Task> callback) =>
            Subscribe(callback, _changedSubscribers);

        public Task StateItemAddedAsync(TId id, IImmutable<TEntity> value) =>
            NotifySubscribers(id, value, _addedSubscribers);

        public Task StateItemUpdatedAsync(TId id, IImmutable<TEntity> value) =>
            NotifySubscribers(id, value, _updatedSubscribers);

        public Task StateItemRemovedAsync(TId id, IImmutable<TEntity> value) =>
            NotifySubscribers(id, value, _removedSubscribers);

        public Task StateChangedAsync(TId id, IImmutable<TEntity> value) =>
            NotifySubscribers(id, value, _changedSubscribers);

        private IDisposable Subscribe(Func<TId, IImmutable<TEntity>, Task> callback, List<Func<TId, IImmutable<TEntity>, Task>> subscribers)
        {
            if (subscribers.Contains(callback))
            {
                subscribers.Remove(callback);
            }

            subscribers.Add(callback);

            return new Unsubscriber(subscribers, callback);
        }

        private async Task NotifySubscribers(TId id, IImmutable<TEntity> value, List<Func<TId, IImmutable<TEntity>, Task>> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                await subscriber.Invoke(id, value);
            }
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<Func<TId, IImmutable<TEntity>, Task>> _subscribers;
            private readonly Func<TId, IImmutable<TEntity>, Task> _subscriber;

            public Unsubscriber(List<Func<TId, IImmutable<TEntity>, Task>> subscribers, Func<TId, IImmutable<TEntity>, Task> subscriber)
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
