using System;
using System.Reflection;
using Ignitor.Cloning;

namespace Ignitor.Immutables
{
    /// <summary>
    /// An immutable version of a mutable type
    /// </summary>
    internal class Immutable<TObj> : IImmutable<TObj>
    {
        private readonly Func<TObj, TObj> _cloner;

        private Type _type;

        private readonly TObj _state;
        private readonly TObj _clone;

        /// <summary>
        /// Construct a new Immutable from a mutable object
        /// </summary>
        /// <param name="value">The mutable object to encapsulate</param>
        public Immutable(TObj value)
        {
            _type = typeof(TObj);

            _state = value;

            _cloner = ClonerFactory.GetCloner<TObj>();

            _clone = _cloner(_state);
        }

        /// <summary>
        /// Indicated if the base immutable type is an array
        /// </summary>
        public bool IsArray { get => _type.IsArray; }

        /// <summary>
        /// Indicates if the base immutable type is a value type, struct or string
        /// </summary>
        public bool IsValueType { get => _type.IsValueType || _type == typeof(string); }

        /// <summary>
        /// Indicates if the immutable value is null 
        /// </summary>
        public bool IsNull { get => _state == null; }

        /// <summary>
        /// Indexer for accessing the array of values where the root immutable object is an array
        /// </summary>
        /// <param name="arrayIndex">Index of the array item to access</param>
        /// <returns>An immutable representing the value at that array index</returns>
        public IImmutable this[int arrayIndex]
        {
            get
            {
                if (!_type.IsArray)
                    throw new InvalidOperationException($"Immutable of type '{typeof(TObj).Name}' is not an array.");

                var arrayValue = _type.GetMethod("GetValue").Invoke(_state, new object[] { arrayIndex });

                return CreateImmutable(_type.GetElementType(), arrayValue);
            }
        }

        /// <summary>
        /// Emits a mutable copy of the immutable.
        /// Note that this will be a isolated instance and will not affect the contents of this immutable.
        /// </summary>
        /// <returns>A mutable copy of the immutable</returns>
        public TObj Emit()
        {
            return _cloner(_state);
        }

        /// <summary>
        /// Access a value type within the immutable object
        /// </summary>
        /// <param name="propertyName">The property name of the value type</param>
        /// <returns>The value of the property</returns>
        public object Value(string propertyName)
        {
            var property = GetPropertyInfo(propertyName);

            var propType = property.PropertyType;

            if (!propType.IsValueType && propType != typeof(string))
                throw new ArgumentException($"Property '{propertyName}' is not a immutable value type");

            return property.GetValue(_state);
        }

        /// <summary>
        /// Access a value type within the immutable object
        /// </summary>
        /// <typeparam name="TValue">The type of the referenced value property</typeparam>
        /// <param name="propertyName">The property name of the value type</param>
        /// <returns>The value of the property</returns>
        public TValue Value<TValue>(string propertyName) =>
            (TValue) Value(propertyName);

        /// <summary>
        /// Gets a value from the internal immutable.
        /// Note this operates on an internal clone made at Immutable inception.
        /// If you modify this clone it will have no affect on the immurable.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to access</typeparam>
        /// <param name="selector">Property value selector delegate function</param>
        /// <returns>Property value</returns>
        public TValue Extract<TValue>(Func<TObj, TValue> selector)
        {
            var cloner = ClonerFactory.GetCloner<TValue>();
            var value = selector.Invoke(_clone);
            return cloner(value);
        }

        /// <summary>
        /// Access a reference type within the immutable object
        /// </summary>
        /// <param name="propertyName">The property name of the reference type</param>
        /// <returns>An Immutable representing the contents of the property</returns>
        public IImmutable Ref(string propertyName)
        {
            var property = GetPropertyInfo(propertyName);

            var propValue = property.GetValue(_state);
            var result = CreateImmutable(property.PropertyType, propValue);

            return result;
        }

        /// <summary>
        /// Access a reference type within the immutable object
        /// </summary>
        /// <typeparam name="TRef">The type of the referenced property</typeparam>
        /// <param name="propertyName">The property name of the reference type</param>
        /// <returns>An Immutable representing the contents of the property</returns>
        public IImmutable<TRef> Ref<TRef>(string propertyName)
        {
            var property = GetPropertyInfo(propertyName);

            var propValue = (TRef)property.GetValue(_state);
            var result = new Immutable<TRef>(propValue);

            return result;
        }

        /// <summary>
        /// Access an Array type using the specified generic type within the immutable object
        /// </summary>
        /// <typeparam name="TArray">The element type of the array</typeparam>
        /// <param name="propertyName">The property name of the array</param>
        /// <returns>An array of immutables representing the contents of the referenced array</returns>
        public IImmutable<TArray>[] Array<TArray>(string propertyName)
        {
            var property = GetPropertyInfo(propertyName);

            if (!property.PropertyType.IsArray)
                throw new ArgumentException($"Property '{propertyName}' is not an Array type'");

            var array = (TArray[])property.GetValue(_state);
            var newArray = new IImmutable<TArray>[array.Length];

            for (var idx = 0; idx < array.Length; idx++)
            {
                newArray[idx] = new Immutable<TArray>(array[idx]);
            }

            return newArray;
        }

        /// <summary>
        /// Get the length of the array if this immutable has an array as a base type
        /// </summary>
        /// <returns>The length of the array</returns>
        public int ArrayLength()
        {
            if (!_type.IsArray)
                throw new InvalidOperationException($"Immutable of type '{typeof(TObj).Name}' is not an array.");

            return (int)_type.GetProperty("Length").GetValue(_state);
        }

        /// <summary>
        /// Convenience method to perform a simple predicate check on the contents of this immutable
        /// Note this operates on an internal clone made at Immutable inception.
        /// If you modify this clone it will have no affect on the immurable.
        /// </summary>
        /// <param name="predicate">The predicate expression</param>
        /// <returns>True if the predicate condition is met</returns>
        public bool Check(Predicate<TObj> predicate) =>
            predicate.Invoke(_clone);

        /// <inheritdoc/>
        public override int GetHashCode() =>
            _state.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() =>
            _state.ToString();

        /// <inheritdoc/>
        public bool Equals(TObj other) =>
            _state.Equals(other);

        /// <inheritdoc/>
        public bool Equals(IImmutable<TObj> other) =>
            ReferenceEquals(this, other) || other.Equals(_state);

        /// <inheritdoc/>
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj);

        /// <inheritdoc/>
        public static bool operator ==(Immutable<TObj> a, IImmutable<TObj> b) =>
            a.Equals(b);

        /// <inheritdoc/>
        public static bool operator !=(Immutable<TObj> a, IImmutable<TObj> b) =>
            !a.Equals(b);

        /// <inheritdoc/>
        public static bool operator ==(Immutable<TObj> a, TObj b) =>
            a.Equals(b);

        /// <inheritdoc/>
        public static bool operator !=(Immutable<TObj> a, TObj b) =>
            !a.Equals(b);

        /// <summary>
        /// Emits a mutable copy of the immutable.
        /// Note that this will be a isolated instance and will not affect the contents of this immutable.
        /// </summary>
        /// <returns>A mutable copy of the immutable</returns>
        object IImmutable.Emit() =>
            Emit();

        public static IImmutable CreateImmutable(Type type, object value)
        {
            var immutableType = typeof(Immutable<>).MakeGenericType(type);

            return (IImmutable)Activator.CreateInstance(immutableType, value);
        }

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            var property = typeof(TObj).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (property == null)
                throw new ArgumentException($"Property '{propertyName}' does not exist on this object");

            return property;
        }
    }
}
