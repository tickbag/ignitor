using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ignitor
{
    /// <summary>
    /// Interface for registering state change callbacks against a scoped state.
    /// </summary>
    /// <typeparam name="TId">Type for the key/id of the state data</typeparam>
    /// <typeparam name="TEntity">Type of the state data</typeparam>
    public interface IStateMonitor<TId, TEntity>
    {
        /// <summary>
        /// Register a callback to execute when data is added to the state.
        /// </summary>
        /// <param name="callback">The callback delegate to execute</param>
        /// <param name="id">The entity Id to receive this state change on</param>
        /// <param name="unsubscribe">The unsubscribe instance from the last time the registeration was called.
        /// Useful if you're registering this in a routine that is called reptitively, such as the OnParameterSet Component methods in Blazor.
        /// The registeration routine will unsubscribe the old registration and provide a new one.</param>
        /// <returns>An unsubscribe instance used for removing this registeration.
        /// Please ensure this is called when you no longer need the callback or your Component is unloaded otherwise memory leaks could occur.</returns>
        IDisposable OnAdded(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null);

        /// <summary>
        /// Register a callback to execute when existing data is updated in the state.
        /// </summary>
        /// <param name="callback">The callback delegate to execute</param>
        /// <param name="id">The entity Id to receive this state change on</param>
        /// <param name="unsubscribe">The unsubscribe instance from the last time the registeration was called.
        /// Useful if you're registering this in a routine that is called reptitively, such as the OnParameterSet Component methods in Blazor.
        /// The registeration routine will unsubscribe the old registration and provide a new one.</param>
        /// <returns>An unsubscribe instance used for removing this registeration.
        /// Please ensure this is called when you no longer need the callback or your Component is unloaded otherwise memory leaks could occur.</returns>
        IDisposable OnUpdated(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null);

        /// <summary>
        /// Register a callback to execute when data is removed from the state.
        /// </summary>
        /// <param name="callback">The callback delegate to execute</param>
        /// <param name="id">The entity Id to receive this state change on</param>
        /// <param name="unsubscribe">The unsubscribe instance from the last time the registeration was called.
        /// Useful if you're registering this in a routine that is called reptitively, such as the OnParameterSet Component methods in Blazor.
        /// The registeration routine will unsubscribe the old registration and provide a new one.</param>
        /// <returns>An unsubscribe instance used for removing this registeration.
        /// Please ensure this is called when you no longer need the callback or your Component is unloaded otherwise memory leaks could occur.</returns>
        IDisposable OnRemoved(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null);

        /// <summary>
        /// Register a callback to execute when the data in the state is changed in anyway.
        /// </summary>
        /// <param name="callback">The callback delegate to execute</param>
        /// <param name="id">The entity Id to receive this state change on</param>
        /// <param name="unsubscribe">The unsubscribe instance from the last time the registeration was called.
        /// Useful if you're registering this in a routine that is called reptitively, such as the OnParameterSet Component methods in Blazor.
        /// The registeration routine will unsubscribe the old registration and provide a new one.</param>
        /// <returns>An unsubscribe instance used for removing this registeration.
        /// Please ensure this is called when you no longer need the callback or your Component is unloaded otherwise memory leaks could occur.</returns>
        IDisposable OnChanged(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, TId id, IDisposable unsubscribe = null);

        /// <summary>
        /// Register a callback to execute when data is added to the state.
        /// </summary>
        /// <param name="callback">The callback delegate to execute</param>
        /// <param name="unsubscribe">The unsubscribe instance from the last time the registeration was called.
        /// Useful if you're registering this in a routine that is called reptitively, such as the OnParameterSet Component methods in Blazor.
        /// The registeration routine will unsubscribe the old registration and provide a new one.</param>
        /// <returns>An unsubscribe instance used for removing this registeration.
        /// Please ensure this is called when you no longer need the callback or your Component is unloaded otherwise memory leaks could occur.</returns>
        IDisposable OnAdded(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null);

        /// <summary>
        /// Register a callback to execute when existing data is updated in the state.
        /// </summary>
        /// <param name="callback">The callback delegate to execute</param>
        /// <param name="unsubscribe">The unsubscribe instance from the last time the registeration was called.
        /// Useful if you're registering this in a routine that is called reptitively, such as the OnParameterSet Component methods in Blazor.
        /// The registeration routine will unsubscribe the old registration and provide a new one.</param>
        /// <returns>An unsubscribe instance used for removing this registeration.
        /// Please ensure this is called when you no longer need the callback or your Component is unloaded otherwise memory leaks could occur.</returns>
        IDisposable OnUpdated(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null);

        /// <summary>
        /// Register a callback to execute when data is removed from the state.
        /// </summary>
        /// <param name="callback">The callback delegate to execute</param>
        /// <param name="unsubscribe">The unsubscribe instance from the last time the registeration was called.
        /// Useful if you're registering this in a routine that is called reptitively, such as the OnParameterSet Component methods in Blazor.
        /// The registeration routine will unsubscribe the old registration and provide a new one.</param>
        /// <returns>An unsubscribe instance used for removing this registeration.
        /// Please ensure this is called when you no longer need the callback or your Component is unloaded otherwise memory leaks could occur.</returns>
        IDisposable OnRemoved(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null);

        /// <summary>
        /// Register a callback to execute when the data in the state is changed in anyway.
        /// </summary>
        /// <param name="callback">The callback delegate to execute</param>
        /// <param name="unsubscribe">The unsubscribe instance from the last time the registeration was called.
        /// Useful if you're registering this in a routine that is called reptitively, such as the OnParameterSet Component methods in Blazor.
        /// The registeration routine will unsubscribe the old registration and provide a new one.</param>
        /// <returns>An unsubscribe instance used for removing this registeration.
        /// Please ensure this is called when you no longer need the callback or your Component is unloaded otherwise memory leaks could occur.</returns>
        IDisposable OnChanged(Func<TId, IImmutable<TEntity>, CancellationToken, Task> callback, IDisposable unsubscribe = null);
    }
}
