using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Ignitor.State
{
    /// <summary>
    /// The Global Application State (GAS). This is the root, static object where all state scopes derive from.
    /// Injecting this into your Component or class gives you the root of the state tree.
    /// </summary>
    internal class State : IState
    {
        private readonly ConcurrentDictionary<string, IScopedState> _scopes = new ConcurrentDictionary<string, IScopedState>();

        private readonly IScopedStateFactory _scopedStateFactory;

        public State(IScopedStateFactory scopedStateFactory)
        {
            _scopedStateFactory = scopedStateFactory;
        }

        /// <inheritdoc/>
        public IScopedState<TId, TEntity> Scope<TId, TEntity>() =>
            Scope<TId, TEntity>(string.Empty);

        /// <inheritdoc/>
        public IScopedState<TId, TEntity> Scope<TId, TEntity>(object scope)
        {
            var internalScope = GetInternalScope<TId, TEntity>(scope);

            if (!_scopes.ContainsKey(internalScope))
            {
                _scopes.TryAdd(internalScope, _scopedStateFactory.CreateScope<TId, TEntity>(this));
            }

            return (IScopedState<TId, TEntity>)_scopes[internalScope];
        }

        /// <inheritdoc/>
        public bool HasScope<TId, TEntity>() =>
            HasScope<TId, TEntity>(string.Empty);

        /// <inheritdoc/>
        public bool HasScope<TId, TEntity>(object scope) =>
            _scopes.ContainsKey(GetInternalScope<TId, TEntity>(scope));

        /// <inheritdoc/>
        public void RemoveScope<TId, TEntity>() =>
            RemoveScope<TId, TEntity>(string.Empty);

        /// <inheritdoc/>
        public void RemoveScope<TId, TEntity>(object scope)
        {
            var internalScope = GetInternalScope<TId, TEntity>(scope);

            if (_scopes.ContainsKey(internalScope))
            {
                _scopes.Remove(internalScope, out var removedState);
                removedState?.Dispose();
            }
        }

        /// <summary>
        /// Calling Dispose on the root state object will clear the whole application state
        /// within that context.
        /// </summary>
        public void Dispose()
        {
            foreach (var scope in _scopes.Values)
            {
                scope?.Dispose();
            }

            _scopes.Clear();
        }

        private static string GetInternalScope<TId, TEntity>(object scope) =>
            $"{typeof(TId).Name}_{typeof(TEntity).Name}_{scope}";
    }

    /// <summary>
    /// The Global Application State (GAS) for this specific context type. This is the root, static object where all state scopes derive from.
    /// Injecting this into your Component or class gives you the root of the state tree.
    /// </summary>
    internal class State<TContext> : State, IState<TContext>
    {
        public State(IScopedStateFactory scopedStateFactory) : base(scopedStateFactory) { }
    }
}
