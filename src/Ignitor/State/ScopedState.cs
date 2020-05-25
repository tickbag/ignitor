using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ignitor.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Ignitor.State
{
    internal class ScopedState<TId, TEntity> : State, IScopedState<TId, TEntity>
    {
        private readonly IIgnitorStore<TId, TEntity> _store;
        private readonly IStateMonitor<TId, TEntity> _monitor;

        private readonly PropertyInfo _idProperty;

        private readonly IServiceProvider _services;
        private readonly IState _parentState;

        private Func<IState, TId, CancellationToken, Task<TEntity>> _defaultValueLoader = null;
        private Func<IState, CancellationToken, Task<IAsyncEnumerable<TEntity>>> _defaultAsyncValuesLoader = null;
        private Func<IState, CancellationToken, Task<IEnumerable<TEntity>>> _defaultValuesLoader = null;

        private Func<TEntity, TId> _idSelector;

        public ScopedState(IServiceProvider services, IState parentState) :
            base(services)
        {
            _services = services;
            _parentState = parentState;

            _store = services.GetRequiredService<IIgnitorStore<TId, TEntity>>();
            _monitor = _services.GetRequiredService<IStateMonitor<TId, TEntity>>();

            // Id selector defaults
            _idProperty = GetIdProperty();
            _idSelector = (entity) => (TId)_idProperty.GetValue(entity);
        }

        public async Task<IImmutable<TEntity>> GetAsync(TId id, CancellationToken ct = default)
        {
            if (!_store.ContainsKey(id))
            {
                if (_defaultValueLoader == null)
                    return default;

                _store.TryAdd(id, await _defaultValueLoader.Invoke(_parentState, id, ct));
            }

            return _store[id];
        }

        public async Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetAsync(CancellationToken ct = default)
        {
            if (_store.Count > 0 || (_defaultValuesLoader == null && _defaultAsyncValuesLoader == null))
                return _store;

            return _defaultAsyncValuesLoader != null ? await GetUsingAsyncEnumerable(ct) : await GetUsingEnumeratable(ct);
        }

        public IStateMonitor<TId, TEntity> Monitor() =>
            _monitor;

        public IStateUpdater<TEntity> Updater(TId id) =>
            new SingleStateUpdater<TId, TEntity>(_monitor, _store, id);

        public IStateUpdater<TId, TEntity> Updater() =>
            new StateUpdater<TId, TEntity>(_monitor, _store);

        public IScopedState<TId, TEntity> Fuel(Func<IState, CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValues)
        {
            _defaultAsyncValuesLoader = defaultValues;

            return this;
        }

        public IScopedState<TId, TEntity> Fuel(Func<IState, CancellationToken, Task<IEnumerable<TEntity>>> defaultValues)
        {
            _defaultValuesLoader = defaultValues;

            return this;
        }

        public IScopedState<TId, TEntity> Fuel(Func<IState, TId, CancellationToken, Task<TEntity>> defaultValue)
        {
            _defaultValueLoader = defaultValue;

            return this;
        }

        public IScopedState<TId, TEntity> WithId(Func<TEntity, TId> idSelector)
        {
            _idSelector = idSelector;

            return this;
        }

        public new void Dispose()
        {
            base.Dispose();

            _store.Clear();
        }

        private async Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetUsingAsyncEnumerable(CancellationToken ct)
        {
            var values = await _defaultAsyncValuesLoader.Invoke(_parentState, ct);

            await foreach (var value in values.WithCancellation(ct))
            {
                var id = _idSelector.Invoke(value);
                _store.AddOrUpdate(id, value);
            }

            return _store;
        }

        private async Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetUsingEnumeratable(CancellationToken ct)
        {
            var values = await _defaultValuesLoader.Invoke(_parentState, ct);

            foreach (var value in values)
            {
                var id = _idSelector.Invoke(value);
                _store.AddOrUpdate(id, value);
            }

            return _store;
        }

        private static PropertyInfo GetIdProperty()
        {
            PropertyInfo firstId = null;

            if (typeof(TEntity).IsValueType || typeof(TEntity) == typeof(string) || typeof(TEntity).IsArray)
                return null;

            var props = typeof(TEntity).GetProperties();
            foreach (var prop in props)
            {
                if (firstId == null && prop.Name.EndsWith("Id", StringComparison.InvariantCulture) &&
                    prop.PropertyType == typeof(TId))
                {
                    firstId = prop;
                }

                var idAttribute = prop.GetCustomAttribute<IgnitorIdAttribute>(true);
                if (idAttribute != null)
                    return prop;
            }

            return firstId;
        }
    }
}
