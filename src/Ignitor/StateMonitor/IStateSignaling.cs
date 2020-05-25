using System.Threading;
using System.Threading.Tasks;

namespace Ignitor.StateMonitor
{
    /// <summary>
    /// Interface for signalling state changes to the state monitor.
    /// </summary>
    /// <typeparam name="TId">Type of the Id used for the state</typeparam>
    /// <typeparam name="TEntity">Type of the entity used in the state</typeparam>
    internal interface IStateSignaling<TId, TEntity>
    {
        /// <summary>
        /// Signals that an item has been added to the state.
        /// </summary>
        /// <param name="id">Id of the item that was added</param>
        /// <param name="value">IImmutable of the entity that was added</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>An awaitable task</returns>
        Task StateItemAddedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct);

        /// <summary>
        /// Signals that an item has been updated in the state.
        /// </summary>
        /// <param name="id">Id of the item that was updated</param>
        /// <param name="value">IImmutable of the entity that was updated in its new state</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>An awaitable task</returns>
        Task StateItemUpdatedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct);

        /// <summary>
        /// Signals that an item has been removed from the state.
        /// </summary>
        /// <param name="id">Id of the item that was removed</param>
        /// <param name="value">IImmutable of the entity that was removed</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>An awaitable task</returns>
        Task StateItemRemovedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct);

        /// <summary>
        /// Signals that the state has changed in some way.
        /// </summary>
        /// <param name="id">Id of the item that was changed</param>
        /// <param name="value">IImmutable of the entity that was changed</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>An awaitable task</returns>
        Task StateChangedAsync(TId id, IImmutable<TEntity> value, CancellationToken ct);
    }
}
