using System;

namespace Ignitor
{
    /// <summary>
    /// The Global Application State (GAS). This is the root, static object where all state scopes derive from.
    /// Injecting this into your Component or class gives you the root of the state tree.
    /// </summary>
    public interface IState : IDisposable
    {
        /// <summary>
        /// Get or create a scoped state for the specified Id and Entity types.
        /// A scope only contains data for the Id and Entity requested, however each scope may contain zero, 
        /// one or more child scopes with different Id and Entity types.
        /// </summary>
        /// <typeparam name="TId">Type to key/id the state data on</typeparam>
        /// <typeparam name="TEntity">Type of the state data</typeparam>
        /// <returns>A scoped state for the Id and Entity type requested</returns>
        IScopedState<TId, TEntity> Scope<TId, TEntity>();

        /// <summary>
        /// Get or create a scoped state for the specified Id and Entity types and an additional scope identifier (such as a string or Guid).
        /// A scope only contains data for the Id and Entity requested, however each scope may contain zero, 
        /// one or more child scopes with different Id and Entity types.
        /// </summary>
        /// <typeparam name="TId">Type to key/id the state data on</typeparam>
        /// <typeparam name="TEntity">Type of the state data</typeparam>
        /// <param name="scope">An additional scoping parameter such as a Guid or string, ToString is called on whatever object is provided</param>
        /// <returns>A scoped state for the Id and Entity type requested</returns>
        IScopedState<TId, TEntity> Scope<TId, TEntity>(object scope);
    }

    /// <summary>
    /// Represents a root Global Application State for a specific context, based on the type supplied.
    /// The type simply provides seperation of root state trees and can be any type although it is logical
    /// to use a type related to the scopes that will be contained in this context.
    /// </summary>
    /// <typeparam name="TContext">Context type for this GAS</typeparam>
    public interface IState<TContext> : IState { }
}
