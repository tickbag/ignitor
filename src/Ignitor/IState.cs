using System;

namespace Ignitor
{
    public interface IState : IDisposable
    {
        IScopedState<TId, TEntity> Ignite<TId, TEntity>();
        IScopedState<TId, TEntity> Ignite<TId, TEntity>(object scope);
    }

    public interface IState<TContext> : IState { }
}
