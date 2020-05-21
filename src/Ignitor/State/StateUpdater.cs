using System.Threading.Tasks;
using Ignitor.Notifier;

namespace Ignitor.State
{
    internal class StateUpdater<TId, TEntity> : IStateUpdater<TId, TEntity>
    {
        private readonly IIgnitorStore<TId, TEntity> _store;
        private readonly IStateSignaling<TId, TEntity> _notifier;

        public StateUpdater(IStateNotifier<TId, TEntity> notifier, IIgnitorStore<TId, TEntity> store)
        {
            _notifier = (IStateSignaling<TId, TEntity>)notifier;
            _store = store;
        }

        public Task UpdateAsync(TId id, TEntity newValue)
        {
            IImmutable<TEntity> immutableValue = newValue != null ? newValue.MakeImmutable() : null;
            return UpdateAsync(id, immutableValue);
        }

        public async Task UpdateAsync(TId id, IImmutable<TEntity> newValue)
        {
            if (newValue == null)
            {
                if (_store.ContainsKey(id))
                {
                    newValue = _store[id];
                    _store.Remove(id);
                    await _notifier.StateItemRemovedAsync(id, newValue);
                }
            }
            else
            {
                if (_store.ContainsKey(id))
                {
                    _store.AddOrUpdate(id, newValue);
                    await _notifier.StateItemUpdatedAsync(id, newValue);
                }
                else
                {
                    _store.TryAdd(id, newValue);
                    await _notifier.StateItemAddedAsync(id, newValue);
                }
            }

            await _notifier.StateChangedAsync(id, newValue);
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

        public SingleStateUpdater(IStateNotifier<TId, TEntity> notifier, IIgnitorStore<TId, TEntity> store, TId id)
        {
            _notifier = (IStateSignaling<TId, TEntity>)notifier;
            _store = store;
            _id = id;
        }

        public Task UpdateAsync(TEntity newValue)
        {
            IImmutable<TEntity> immutableValue = newValue != null ? newValue.MakeImmutable() : null;
            return UpdateAsync(immutableValue);
        }

        public async Task UpdateAsync(IImmutable<TEntity> newValue)
        {
            if (newValue == null)
            {
                if (_store.ContainsKey(_id))
                {
                    newValue = _store[_id];
                    _store.Remove(_id);
                    await _notifier.StateItemRemovedAsync(_id, newValue);
                }
            }
            else
            {
                if (_store.ContainsKey(_id))
                {
                    _store.AddOrUpdate(_id, newValue);
                    await _notifier.StateItemUpdatedAsync(_id, newValue);
                }
                else
                {
                    _store.TryAdd(_id, newValue);
                    await _notifier.StateItemAddedAsync(_id, newValue);
                }
            }

            await _notifier.StateChangedAsync(_id, newValue);
        }

        public void Clear()
        {
            _store.Clear();
        }
    }
}
