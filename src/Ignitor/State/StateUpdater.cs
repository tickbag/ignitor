using System.Threading;
using System.Threading.Tasks;
using Ignitor.StateMonitor;

namespace Ignitor.State
{
    internal class StateUpdater<TId, TEntity> : IStateUpdater<TId, TEntity>
    {
        private readonly IIgnitorStore<TId, TEntity> _store;
        private readonly IStateSignaling<TId, TEntity> _notifier;

        public StateUpdater(IStateMonitor<TId, TEntity> notifier, IIgnitorStore<TId, TEntity> store)
        {
            _notifier = (IStateSignaling<TId, TEntity>)notifier;
            _store = store;
        }

        public Task UpdateAsync(TId id, TEntity newValue, CancellationToken ct = default)
        {
            var immutableValue = newValue != null ? newValue.MakeImmutable() : null;
            return UpdateAsync(id, immutableValue);
        }

        public async Task UpdateAsync(TId id, IImmutable<TEntity> newValue, CancellationToken ct = default)
        {
            if (newValue == null)
            {
                if (_store.ContainsKey(id))
                {
                    newValue = _store[id];
                    _store.Remove(id);
                    await _notifier.StateItemRemovedAsync(id, newValue, ct);
                }
            }
            else
            {
                if (_store.ContainsKey(id))
                {
                    _store.AddOrUpdate(id, newValue);
                    await _notifier.StateItemUpdatedAsync(id, newValue, ct);
                }
                else
                {
                    _store.TryAdd(id, newValue);
                    await _notifier.StateItemAddedAsync(id, newValue, ct);
                }
            }

            await _notifier.StateChangedAsync(id, newValue, ct);
        }

        public void Clear()
        {
            _store.Clear();
        }
    }

    internal class SingleStateUpdater<TId, TEntity> : IStateUpdater<TEntity>
    {
        private readonly IStateSignaling<TId, TEntity> _notifier;
        private readonly IIgnitorStore<TId, TEntity> _store;
        private readonly TId _id;

        public SingleStateUpdater(IStateMonitor<TId, TEntity> notifier, IIgnitorStore<TId, TEntity> store, TId id)
        {
            _notifier = (IStateSignaling<TId, TEntity>)notifier;
            _store = store;
            _id = id;
        }

        public Task UpdateAsync(TEntity newValue, CancellationToken ct = default)
        {
            var immutableValue = newValue != null ? newValue.MakeImmutable() : null;
            return UpdateAsync(immutableValue);
        }

        public async Task UpdateAsync(IImmutable<TEntity> newValue, CancellationToken ct = default)
        {
            if (newValue == null)
            {
                if (_store.ContainsKey(_id))
                {
                    newValue = _store[_id];
                    _store.Remove(_id);
                    await _notifier.StateItemRemovedAsync(_id, newValue, ct);
                }
            }
            else
            {
                if (_store.ContainsKey(_id))
                {
                    _store.AddOrUpdate(_id, newValue);
                    await _notifier.StateItemUpdatedAsync(_id, newValue, ct);
                }
                else
                {
                    _store.TryAdd(_id, newValue);
                    await _notifier.StateItemAddedAsync(_id, newValue, ct);
                }
            }

            await _notifier.StateChangedAsync(_id, newValue, ct);
        }

        public void Clear()
        {
            _store.Clear();
        }
    }
}
