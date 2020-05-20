using System.Threading.Tasks;

namespace Ignitor.State
{
    internal class StateUpdater<TId, TEntity> : IStateUpdater<TId, TEntity>
    {
        private readonly IIgnitorStore<TId, TEntity> _store;

        public StateUpdater(IIgnitorStore<TId, TEntity> store)
        {
            _store = store;
        }

        public async Task UpdateAsync(TId id, TEntity newValue)
        {
            if (newValue == null)
            {
                _store.Remove(id);
            }
            else
            {
                _store.AddOrUpdate(id, newValue);
            }
        }

        public void Clear()
        {
            _store.Clear();
        }
    }

    internal class SingleStateUpdater<TId, TEntity> : IStateUpdater<TEntity>
    {
        private readonly IIgnitorStore<TId, TEntity> _store;
        private readonly TId _id;

        public SingleStateUpdater(IIgnitorStore<TId, TEntity> store, TId id)
        {
            _store = store;
            _id = id;
        }

        public async Task UpdateAsync(TEntity newValue)
        {
            if (newValue == null)
            {
                _store.Remove(_id);
            }
            else
            {
                _store.AddOrUpdate(_id, newValue);
            }
        }

        public void Clear()
        {
            _store.Clear();
        }
    }
}
