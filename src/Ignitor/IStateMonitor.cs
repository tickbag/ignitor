using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ignitor
{
    public interface IStateMonitor<TId, TEntity>
    {
        IDisposable OnAdded(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null);

        IDisposable OnUpdated(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null);

        IDisposable OnRemoved(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null);

        IDisposable OnChanged(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null);

        IDisposable OnAdded(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null);

        IDisposable OnUpdated(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null);

        IDisposable OnRemoved(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null);

        IDisposable OnChanged(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null);
    }
}
