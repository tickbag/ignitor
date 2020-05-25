using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ignitor
{
    /// <summary>
    /// Targeting interface for the IScopedState interface. See <see cref="IScopedState{TId, TEntity}"/> for more information.
    /// </summary>
    public interface IScopedState : IState, IDisposable { }

    /// <summary>
    /// Represents an individual block (or scope) of state data. This scope must be unique by Id type, Data type and scope name (if provided).
    /// This also controls access to the state via <see cref="GetAsync(TId, CancellationToken)">GetAsync</see> and <see cref="Updater(TId)">Updater</see> methods,
    /// and also monitoring of the state via the <see cref="Monitor">Monitor</see> method.
    /// <para>
    /// Scoped state is hierarchical and can have multiple child scopes and a single parent scope.<br/>
    /// Top level scopes have the Global Application Scope (GAS) as their parent.<br/>
    /// Scoped state is usually accessed via the GAS, and can thus be accessed from anywhere in the application if the hierarchy is known.<br/>
    /// If a scoped state is Disposed, all its child scopes are also Disposed.<br/>
    /// It is also possible to directly inject an IScopedState into a class/Component. In this instance the state will only exist for as long as the class/Component.
    /// It's lifetime will be limited and it will not accessible from anywhere else. In other words, It will be locally scoped.<br/>
    /// </para>
    /// </summary>
    /// <typeparam name="TId">Type to key/id the state data on</typeparam>
    /// <typeparam name="TEntity">Type of the state data</typeparam>
    public interface IScopedState<TId, TEntity> : IScopedState
    {
        /// <summary>
        /// Get data stored in this scoped state. Data is always returned in an immutable state.
        /// </summary>
        /// <param name="id">Id of the data to retrieve within the state</param>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>IImmutable of the data for that Id or Null if no data is present for that Id and
        /// no <see cref="Fuel(Func{IState, TId, CancellationToken, Task{TEntity}})">Fuel</see> has been defined</returns>
        Task<IImmutable<TEntity>> GetAsync(TId id, CancellationToken ct = default);

        /// <summary>
        /// Get all data stored in this scoped state. This will return a read-only dictionary of immutable objects.
        /// </summary>
        /// <param name="ct">Cancellation token for the request</param>
        /// <returns>A read-only dictionary containing immutable objects representing all the data in this scoped state. If no data is in the state,
        /// and no <see cref="Fuel(Func{IState, CancellationToken, Task{IEnumerable{TEntity}}})">Fuel</see> has been defined, this will return an empty dictionary.</returns>
        Task<IReadOnlyDictionary<TId, IImmutable<TEntity>>> GetAsync(CancellationToken ct = default);

        /// <summary>
        /// Get the state monitor which handles the registration of state change callbacks
        /// </summary>
        /// <returns>The state change monitor</returns>
        IStateMonitor<TId, TEntity> Monitor();

        /// <summary>
        /// Get the state updater which is responsible for updating, adding or deleting data in this scoped state.
        /// </summary>
        /// <returns>The state updater</returns>
        IStateUpdater<TId, TEntity> Updater();

        /// <summary>
        /// Get the state updater which is responsible for updating, adding or deleting data in this scoped state.
        /// </summary>
        /// <param name="id">The Id of the state data the updater should be responsible for</param>
        /// <returns>The state updater for the given Id</returns>
        IStateUpdater<TEntity> Updater(TId id);

        /// <summary>
        /// Registers a callback to load fuel (data) into the scoped state. The callback is only call when the state is read for the first time.
        /// The provided callback function is registered against the scoped state, the returned scoped state is not a new, mutated version,
        /// it's just the same one you called this method on and is provided for chaining (building) purposes.
        /// </summary>
        /// <param name="defaultValues">The callback function to load data, should return an IAsyncEnumerable of the data</param>
        /// <returns>The current scope state object again for convience in chaining methods</returns>
        IScopedState<TId, TEntity> Fuel(Func<IState, CancellationToken, Task<IAsyncEnumerable<TEntity>>> defaultValues);

        /// <summary>
        /// Registers a callback to load fuel (data) into the scoped state. The callback is only call when the state is read for the first time.
        /// The provided callback function is registered against the scoped state, the returned scoped state is not a new, mutated version,
        /// it's just the same one you called this method on and is provided for chaining (building) purposes.
        /// </summary>
        /// <param name="defaultValues">The callback function to load data, should return an IEnumerable of the data</param>
        /// <returns>The current scope state object again for convience in chaining methods</returns>
        IScopedState<TId, TEntity> Fuel(Func<IState, CancellationToken, Task<IEnumerable<TEntity>>> defaultValues);

        /// <summary>
        /// Registers a callback to load fuel (data) into the scoped state. The callback is only call when the state is read for the first time.
        /// The provided callback function is registered against the scoped state, the returned scoped state is not a new, mutated version,
        /// it's just the same one you called this method on and is provided for chaining (building) purposes.
        /// </summary>
        /// <param name="defaultValue">The callback function to load data, should return an single data object based on the provided Id</param>
        /// <returns>The current scope state object again for convience in chaining methods</returns>
        IScopedState<TId, TEntity> Fuel(Func<IState, TId, CancellationToken, Task<TEntity>> defaultValue);

        /// <summary>
        /// A simple delegate used to specify how to generate the Id for each item of data being loaded into the state.
        /// This will override the IgnitorId attribute and the auto Id matching algorithm, and give you full control over
        /// data keying as it's loaded. Note that this isn't used for single item data loads where Id was requested by the
        /// consumer as in that case the Id was provided by the consumer.
        /// </summary>
        /// <param name="idSelector">The Id selector delegate function. Provides a copy of the entity being loaded and expects the Id to be returned.</param>
        /// <returns>The current scope state object again for convience in chaining methods</returns>
        IScopedState<TId, TEntity> WithId(Func<TEntity, TId> idSelector);
    }
}
