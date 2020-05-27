using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ignitor.Attributes;

namespace Ignitor.State
{
    /// <summary>
    /// Represents an individual block (or scope) of state data. This scope must be unique by Id type, Data type and scope name (if provided).
    /// This also controls access to the state via <see cref="GetAsync(TId, CancellationToken)">GetAsync</see> and <see cref="Updater(TId)">Updater</see> methods,
    /// and also monitoring of the state via the <see cref="Monitor">Monitor</see> method.
    /// <para>
    /// Scoped state is hierarchical and can have multiple child scopes and a single parent scope.<br/>
    /// Top level scopes have the Global Application Scope (GAS) as their parent.<br/>
    /// Scoped state is usually accessed via the GAS, and can thus be accessed from anywhere in the application if the hierarchy is known.<br/>
    /// If a scoped state is Disposed, all its child scopes are also Disposed.<br/>
    /// It is also possible to directly inject an IScopedState into a class/Component. In this instance the state will only exist for as long as the class/Component.
    /// It's lifetime will be limited and it will not accessible from anywhere else. In other words, It will be locally scoped.<br/>
    /// </para>
    /// </summary>
    /// <typeparam name="TId">Type to key/id the state data on</typeparam>
    /// <typeparam name="TEntity">Type of the state data</typeparam>
    internal class ScopedState<TId, TEntity> : State, IScopedState<TId, TEntity>
    {
        private readonly IState _parentState;

        private readonly IIgnitorStore<TId, TEntity> _store;
        private readonly IStateMonitor<TId, TEntity> _monitor;

        private readonly PropertyInfo _idProperty;

        private Func<TEntity, TId> _idSelector;

        private Func<IState, TId, CancellationToken, Task<TEntity>> _defaultValueLoader = null;
        private Func<IState, CancellationToken, Task<IAsyncEnumerable<TEntity>>> _defaultAsyncValuesLoader = null;
        private Func<IState, CancellationToken, Task<IEnumerable<TEntity>>> _defaultValuesLoader = null;

        /// <summary>
        /// Scoped State constructor.
        /// This should be called by either the State object or the Dependency Injection system.
        /// </summary>
        /// <param name="parentState">The parent state to this scoped state</param>
        /// <param name="scopedStateFactory">The scope state factory</param>
        /// <param name="store">The dta store for this scoped state</param>
        /// <param name="monitor">The state monitor service for this scoped state</param>
        public ScopedState(IState parentState,
            IScopedStateFactory scopedStateFactory,
            IIgnitorStore<TId, TEntity> store,
            IStateMonitor<TId, TEntity> monitor) :
            base(scopedStateFactory)
        {
            _parentState = parentState;

            _store = store;
            _monitor = monitor;

            // Id selector defaults
            _idProperty = GetIdProperty();
            _idSelector = (entity) => (TId)_idProperty.GetValue(entity);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetAsync(CancellationToken ct = default)
        {
            if (_store.Count > 0 || (_defaultValuesLoader == null && _defaultAsyncValuesLoader == null))
                return _store;

            return _defaultAsyncValuesLoader != null ? await GetUsingAsyncEnumerable(ct) : await GetUsingEnumeratable(ct);
        }

        /// <inheritdoc/>
        public IStateMonitor<TId, TEntity> Monitor() =>
            _monitor;

        /// <inheritdoc/>
        public IStateUpdater<TEntity> Updater(TId id) =>
            new SingleStateUpdater<TId, TEntity>(_monitor, _store, id);

        /// <inheritdoc/>
        public IStateUpdater<TId, TEntity> Updater() =>
            new StateUpdater<TId, TEntity>(_monitor, _store);

        /// <inheritdoc/>
        public IScopedState<TId, TEntity> Fuel(Func<IState, CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValues)
        {
            _defaultAsyncValuesLoader = defaultValues;

            return this;
        }

        /// <inheritdoc/>
        public IScopedState<TId, TEntity> Fuel(Func<IState, CancellationToken, Task<IEnumerable<TEntity>>> defaultValues)
        {
            _defaultValuesLoader = defaultValues;

            return this;
        }

        /// <inheritdoc/>
        public IScopedState<TId, TEntity> Fuel(Func<IState, TId, CancellationToken, Task<TEntity>> defaultValue)
        {
            _defaultValueLoader = defaultValue;

            return this;
        }

        /// <inheritdoc/>
        public IScopedState<TId, TEntity> WithId(Func<TEntity, TId> idSelector)
        {
            _idSelector = idSelector;

            return this;
        }

        /// <inheritdoc/>
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
