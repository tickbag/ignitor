using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ignitor.State
{
    /// <summary>
    /// Generates instances of scoped state
    /// </summary>
    internal class ScopedStateFactory : IScopedStateFactory
    {
        private readonly IServiceProvider _services;

        public ScopedStateFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc/>
        public IScopedState<TId, TEntity> CreateScope<TId, TEntity>(IState parentState)
        {
            var store = _services.GetRequiredService<IIgnitorStore<TId, TEntity>>();
            var monitor = _services.GetRequiredService<IStateMonitor<TId, TEntity>>();

            return new ScopedState<TId, TEntity>(parentState, this, store, monitor);
        }
    }
}
