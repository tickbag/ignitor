using System.Collections.Generic;

namespace Ignitor
{
    /// <summary>
    /// An Ignitor non-persistent, immutable store. This implements an IReadOnlyDictionary interface to provide data access.
    /// All data is stored as an IImmutable type as it is inserted/updated.
    /// Modification of data in the store is via specialised methods based on a Dictionary interface.
    /// </summary>
    /// <typeparam name="TId">Type to key the stored data on</typeparam>
    /// <typeparam name="TEntity">Type of data to store. This type will be wrapped in an IImmutable interface by the store.</typeparam>
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public interface IIgnitorStore<TId, TEntity> : IReadOnlyDictionary<TId, IImmutable<TEntity>>
    {
        /// <summary>
        /// Remove an item of data from the store
        /// </summary>
        /// <param name="id">Id of the item to remove</param>
        void Remove(TId id);

        /// <summary>
        /// Clear the entire store of data
        /// </summary>
        void Clear();

        /// <summary>
        /// Try to add data to the store.
        /// </summary>
        /// <param name="id">Id of the data to add</param>
        /// <param name="value">Data to add. It will be wrapped in an IImmutable before insertion</param>
        /// <returns>true if the data was added, false if the Id is already in use</returns>
        bool TryAdd(TId id, TEntity value);

        /// <summary>
        /// Try to add data to the store.
        /// </summary>
        /// <param name="id">Id of the data to add</param>
        /// <param name="value">Immutable data to add</param>
        /// <returns>true if the data was added, false if the Id is already in use</returns>
        bool TryAdd(TId id, IImmutable<TEntity> value);

        /// <summary>
        /// Add or update data in the store.
        /// </summary>
        /// <param name="id">Id of the data to add or update</param>
        /// <param name="value">Data to add or update. It will be wrapped in an IImmutable before update</param>
        void AddOrUpdate(TId id, TEntity value);

        /// <summary>
        /// Add or update data in the store.
        /// </summary>
        /// <param name="id">Id of the data to add or update</param>
        /// <param name="value">Immutable data to add or update</param>
        void AddOrUpdate(TId id, IImmutable<TEntity> value);
    }
#pragma warning restore CA1710 // Identifiers should have correct suffix
}
