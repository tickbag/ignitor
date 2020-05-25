using System;

namespace Ignitor
{
    /// <summary>
    /// An immutable version of a mutable type
    /// </summary>
    public interface IImmutable
    {
        /// <summary>
        /// Indicated if the base immutable type is an array
        /// </summary>
        bool IsArray { get; }

        /// <summary>
        /// Indicates if the base immutable type is a value type, struct or string
        /// </summary>
        bool IsValueType { get; }

        /// <summary>
        /// Indicates if the immutable value is null 
        /// </summary>
        bool IsNull { get; }

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
        /// Get the length of the array if this immutable has an array as a base type
        /// </summary>
        /// <returns>The length of the array</returns>
        int ArrayLength();

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
        /// Extracts a value from the internal immutable.
        /// Note this operates on an internal clone made at Immutable inception.
        /// If you modify this clone it will have no affect on the immurable,
        /// but could lead to unpredictable behaviour so please avoid doing that.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to access</typeparam>
        /// <param name="selector">Property value selector delegate function</param>
        /// <returns>Property value</returns>
        TValue Extract<TValue>(Func<TObj, TValue> selector);

        /// <summary>
        /// Convenience method to perform a simple predicate check on the contents of this immutable
        /// Note this operates on an internal clone made at Immutable inception.
        /// If you modify this clone it will have no affect on the immurable.
        /// </summary>
        /// <param name="predicate">The predicate expression</param>
        /// <returns>True if the predicate condition is met</returns>
        bool Check(Predicate<TObj> predicate);
    }
}
