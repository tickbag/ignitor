using System;
using System.Collections.Concurrent;

namespace Ignitor.State
{
    internal class State : IState
    {
        private readonly ConcurrentDictionary<string, IScopedState> _scopes = new ConcurrentDictionary<string, IScopedState>();

        private readonly IServiceProvider _services;

        public State(IServiceProvider services)
        {
            _services = services;
        }

        public IScopedState<TId, TEntity> Ignite<TId, TEntity>() =>
            Ignite<TId, TEntity>(string.Empty);

        public IScopedState<TId, TEntity> Ignite<TId, TEntity>(object scope)
        {
            var internalScope = $"{typeof(TId).Name}_{typeof(TEntity).Name}_{scope}";

            if (!_scopes.ContainsKey(internalScope))
            {
                _scopes.TryAdd(internalScope, CreateScope<TId, TEntity>());
            }

            return (IScopedState<TId, TEntity>)_scopes[internalScope];
        }

        public void Dispose()
        {
            foreach (var scope in _scopes.Values)
            {
                scope.Dispose();
            }
        }

        private IScopedState<TId, TEntity> CreateScope<TId, TEntity>()
        {
            Console.WriteLine($"State - Create new scope for <{typeof(TId).Name}, {typeof(TEntity).Name}>");
            return new ScopedState<TId, TEntity>(_services, this);
        }
    }

    internal class State<TContext> : State, IState<TContext>
    {
        public State(IServiceProvider services) : base(services) { }
    }
}
