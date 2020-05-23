using System;
using System.Threading.Tasks;

namespace Ignitor
{
    public interface IStateNotifier<TContext, TId, TEntity> : IStateNotifier<TId, TEntity> { }

    public interface IStateNotifier<TId, TEntity>
    {
        IDisposable OnAdded(Func<TId, IImmutable<TEntity>, Task> callback, TId id = default);

        IDisposable OnUpdated(Func<TId, IImmutable<TEntity>, Task> callback, TId id = default);

        IDisposable OnRemoved(Func<TId, IImmutable<TEntity>, Task> callback, TId id = default);

        IDisposable OnChanged(Func<TId, IImmutable<TEntity>, Task> callback, TId id = default);
    }
}
