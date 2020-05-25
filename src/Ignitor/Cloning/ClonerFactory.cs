using System;
using System.Collections.Concurrent;

namespace Ignitor.Cloning
{
    /// <summary>
    /// Static factory responsible for getting and caching type cloners in the Ignitor ecosystme.
    /// <para>
    /// Cloners get only be generated for types following specific rules:
    /// - No Interface. Only use concrete tyoes.<br/>
    /// - No List, Collection or Dictionary types. Use native Array's instead.<br/>
    /// - Parameterless constructors only<br/>
    /// - No deep object graphs (objects in object). The maximum tree depth is fixed a 5 levels deep.<br/>
    /// </para>
    /// Note: Structs are only shallow copied.
    /// </summary>
    public static class ClonerFactory
    {
        private static ConcurrentDictionary<Type, IClonerGenerator> _cloners = new ConcurrentDictionary<Type, IClonerGenerator>();

        /// <summary>
        /// Get a cloning function for the specified type. If the type has not been encountered before a new cloner will be generated and cached.
        /// </summary>
        /// <typeparam name="TObj">Type to get a cloning functon for</typeparam>
        /// <returns>The cloning function</returns>
        public static Func<TObj, TObj> GetCloner<TObj>()
        {
            var type = typeof(TObj);

            return ((IClonerGenerator<TObj>)_cloners.GetOrAdd(type, (t) => new ClonerGenerator<TObj>())).GetCloner();
        }

        /// <summary>
        /// Clear the cloning function cache.
        /// </summary>
        public static void Clear()
        {
            _cloners.Clear();
        }
    }
}
