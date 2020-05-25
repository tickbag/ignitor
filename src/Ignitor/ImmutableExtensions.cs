using Ignitor.Immutables;

namespace Ignitor
{
    /// <summary>
    /// Object extension methods to provide Immutable data types
    /// </summary>
    public static class ImmutableExtensions
    {
        /// <summary>
        /// Make this object immutable by wrapping it in an <see cref="IImmutable">IImmutable</see> interface.
        /// </summary>
        /// <typeparam name="TImmutable">Type to make immutable</typeparam>
        /// <param name="value">Object to make immutable</param>
        /// <returns>An Immutable version of the object</returns>
        public static IImmutable<TImmutable> MakeImmutable<TImmutable>(this TImmutable value)
        {
            return new Immutable<TImmutable>(value);
        }
    }
}
