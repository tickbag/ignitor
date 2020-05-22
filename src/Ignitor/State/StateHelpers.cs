using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ignitor.Attributes;

namespace Ignitor.State
{
    internal class StateHelpers
    {
        internal class StateHelper<TId, TEntity> : IState<TId, TEntity>
        {
            private static readonly string _noIdErrorMessage = $"No Id has been found or defined on the '{typeof(TEntity).Name}' type.";

            private readonly IState _state;
            private readonly PropertyInfo _idProperty;

            protected Type _context;

            public StateHelper(IState state)
            {
                _context = typeof(State);

                _state = state;

                _idProperty = GetIdProperty();
            }

            public async Task<IImmutable<TEntity>> GetStateAsync(TId id, Func<TId, CancellationToken, Task<TEntity>> defaultValue = null, CancellationToken cancellationToken = default) =>
                await _state.GetStateAsync(_context, id, defaultValue, cancellationToken);

            public async Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync(Func<CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValue,
                CancellationToken cancellationToken = default)
            {
                _ = _idProperty ?? throw new InvalidOperationException(_noIdErrorMessage);

                return await _state.GetStateAsync(_context, defaultValue, (entity) => (TId)_idProperty.GetValue(entity), cancellationToken);
            }

            public async Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetStateAsync(Func<CancellationToken, Task<IEnumerable<TEntity>>> defaultValue = null,
                CancellationToken cancellationToken = default)
            {
                _ = _idProperty ?? throw new InvalidOperationException(_noIdErrorMessage);

                return await _state.GetStateAsync(_context, defaultValue, (entity) => (TId)_idProperty.GetValue(entity), cancellationToken);
            }

            public IStateUpdater<TEntity> GetUpdater(TId id) =>
                _state.GetUpdater<TId, TEntity>(_context, id);

            public IStateUpdater<TId, TEntity> GetUpdater() =>
                _state.GetUpdater<TId, TEntity>(_context);

            public IStateNotifier<TId, TEntity> GetNotifier() =>
                _state.GetNotifier<TId, TEntity>(_context);

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
                    {
                        return prop;
                    }
                }

                return firstId;
            }
        }

        internal class StateHelper<TContext, TId, TEntity> : StateHelper<TId, TEntity>, IState<TContext, TId, TEntity>
        {
            public StateHelper(IState state) : base(state)
            {
                _context = typeof(TContext);
            }
        }
    }
}
