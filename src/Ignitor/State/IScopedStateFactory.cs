namespace Ignitor.State
{
    /// <summary>
    /// Generates instances of scoped state
    /// </summary>
    internal interface IScopedStateFactory
    {
        /// <summary>
        /// Create a new scoped state instance
        /// </summary>
        /// <typeparam name="TId">Type of the id/key for the new scoped state</typeparam>
        /// <typeparam name="TEntity">Type of the data entity for the new scoped state</typeparam>
        /// <param name="parentState">Parent state of the new scoped state</param>
        /// <returns>A new scoped state instance</returns>
        IScopedState<TId, TEntity> CreateScope<TId, TEntity>(IState parentState);
    }
}
