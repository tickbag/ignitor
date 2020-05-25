using System.Threading;
using System.Threading.Tasks;

namespace Ignitor
{
    /// <summary>
    /// Provides write access to a specifc data item in the state.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data</typeparam>
    public interface IStateUpdater<TEntity>
    {
        /// <summary>
        /// Update or Add a specific entity in the state.
        /// If you pass a Null as the new value, the entity at that location in the state will be removed.
        /// </summary>
        /// <param name="newValue">New value for the entity</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>An awaitable task</returns>
        Task UpdateAsync(TEntity newValue, CancellationToken ct = default);

        /// <summary>
        /// Update or Add a specific entity in the state.
        /// If you pass a Null as the new value, the entity at that location in the state will be removed.
        /// </summary>
        /// <param name="newValue">An IImmutable of the new value for the entity</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>An awaitable task</returns>
        Task UpdateAsync(IImmutable<TEntity> newValue, CancellationToken ct = default);
    }

    /// <summary>
    /// Provides write access to data in the state.
    /// Use this Updater if you need to write to many different items in the state.
    /// </summary>
    /// <typeparam name="TId">The type of the Id for the data</typeparam>
    /// <typeparam name="TEntity">The type of the data</typeparam>
    public interface IStateUpdater<TId, TEntity>
    {
        /// <summary>
        /// Update or Add an entity at the given id in the state.
        /// If you pass a Null as the new value, the entity at that location in the state will be removed.
        /// </summary>
        /// <param name="id">Id of the entity to effect</param>
        /// <param name="newValue">New value for the entity</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>An awaitable task</returns>
        Task UpdateAsync(TId id, TEntity newValue, CancellationToken ct = default);

        /// <summary>
        /// Update or Add an entity at the given id in the state.
        /// If you pass a Null as the new value, the entity at that location in the state will be removed.
        /// </summary>
        /// <param name="id">Id of the entity to effect</param>
        /// <param name="newValue">An IImmutable of the new value for the entity</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>An awaitable task</returns>
        Task UpdateAsync(TId id, IImmutable<TEntity> newValue, CancellationToken ct = default);

        /// <summary>
        /// Remove all data from the state.
        /// </summary>
        void Clear();
    }
}
