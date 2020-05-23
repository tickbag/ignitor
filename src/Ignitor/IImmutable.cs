using System;

namespace Ignitor
{
    /// <summary>
    /// A wrapper around a mutable type to provide immutability
    /// </summary>
    public interface IImmutable
    {
        /// <summary>
        /// Indexer for accessing the array of values where the root immutable object is an array
        /// </summary>
        /// <param name="arrayIndex">Index of the array item to access</param>
        /// <returns>An immutable representing the value at that array index</returns>
        IImmutable this[int arrayIndex] { get; }

        /// <summary>
        /// Access a reference type within the immutable object
        /// </summary>
        /// <param name="propertyName">The property name of the reference type</param>
        /// <returns>An Immutable representing the contents of the property</returns>
        IImmutable Ref(string propertyName);

        /// <summary>
        /// Access a reference type within the immutable object
        /// </summary>
        /// <typeparam name="TRef">The type of the referenced property</typeparam>
        /// <param name="propertyName">The property name of the reference type</param>
        /// <returns>An Immutable representing the contents of the property</returns>
        IImmutable<TRef> Ref<TRef>(string propertyName);

        /// <summary>
        /// Access an Array type using the specified generic type within the immutable object
        /// </summary>
        /// <typeparam name="TArray">The element type of the array</typeparam>
        /// <param name="propertyName">The property name of the array</param>
        /// <returns>An array of immutables representing the contents of the referenced array</returns>
        IImmutable<TArray>[] Array<TArray>(string propertyName);

        /// <summary>
        /// Emits a mutable copy of the immutable.
        /// Note that this will be a isolated instance and will not affect the contents of this immutable.
        /// </summary>
        /// <returns>A mutable copy of the immutable</returns>
        object Emit();

        /// <summary>
        /// Access a value type within the immutable object
        /// </summary>
        /// <param name="propertyName">The property name of the value type</param>
        /// <returns>The value of the property</returns>
        object Value(string propertyName);

        /// <summary>
        /// Access a value type within the immutable object
        /// </summary>
        /// <typeparam name="TValue">The type of the referenced value property</typeparam>
        /// <param name="propertyName">The property name of the value type</param>
        /// <returns>The value of the property</returns>
        TValue Value<TValue>(string propertyName);
    }

    /// <summary>
    /// A wrapper around a mutable type to provide immutability
    /// </summary>
    /// <typeparam name="TObj">The type being held immutable</typeparam>
    public interface IImmutable<TObj> : IImmutable, IEquatable<TObj>, IEquatable<IImmutable<TObj>>
    {
        /// <summary>
        /// Emits a mutable copy of the immutable.
        /// Note that this will be a isolated instance and will not affect the contents of this immutable.
        /// </summary>
        /// <returns>A mutable copy of the immutable</returns>
        new TObj Emit();

        /// <summary>
        /// Convenience method to perform a simple predicate check on the contents of this immutable
        /// </summary>
        /// <param name="predicate">The predicate expression</param>
        /// <returns>True if the predicate condition is met</returns>
        bool ValueCheck(Predicate<TObj> predicate);
    }
}
