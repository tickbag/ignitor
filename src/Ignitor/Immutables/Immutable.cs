using System;
using System.Reflection;
using Ignitor.Cloning;

namespace Ignitor.Immutables
{
    internal class Immutable<TObj> : IImmutable<TObj>
    {
        private readonly Func<TObj, TObj> _cloner;

        private TObj _state;

        public Immutable(TObj value)
        {
            _state = value;

            var isValueType = typeof(TObj).IsValueType || typeof(TObj) == typeof(string);
            _cloner = isValueType ? (source) => source : CloneFactory.GetCloner<TObj>();
        }

        public IImmutable this[string propertyName]
        {
            get
            {
                return Ref(propertyName);
            }
        }

        public TObj Emit()
        {
            return _cloner(_state);
        }

        public object Value(string propertyName)
        {
            var property = GetPropertyInfo(propertyName);

            var propType = property.PropertyType;

            if (!propType.IsValueType && propType != typeof(string))
                throw new ArgumentException($"Property '{propertyName}' is not a immutable value type");

            return property.GetValue(_state);
        }

        public IImmutable Ref(string propertyName)
        {
            var property = GetPropertyInfo(propertyName);

            var propValue = property.GetValue(_state);
            var result = CreateImmutable(property.PropertyType, propValue);

            return result;
        }

        public IImmutable<TArray>[] Array<TArray>(string propertyName)
        {
            var property = GetPropertyInfo(propertyName);

            if (!property.PropertyType.IsArray)
                throw new ArgumentException($"Property '{propertyName} is not an Array type'");

            var array = (TArray[])property.GetValue(_state);
            var newArray = new IImmutable<TArray>[array.Length];

            for (var idx = 0; idx < array.Length; idx++)
            {
                newArray[idx] = new Immutable<TArray>(array[idx]);
            }

            return newArray;
        }

        public IImmutable<TRef> Ref<TRef>(string propertyName)
        {
            var property = GetPropertyInfo(propertyName);

            var propValue = (TRef)property.GetValue(_state);
            var result = new Immutable<TRef>(propValue);

            return result;
        }

        public TValue Value<TValue>(string propertyName) =>
            (TValue)Value(propertyName);

        public bool ValueCheck(Predicate<TObj> predicate) =>
            predicate.Invoke(_state);

        public override int GetHashCode() =>
            _state.GetHashCode();

        public override string ToString() =>
            _state.ToString();

        public bool Equals(TObj other) =>
            _state.Equals(other);

        public bool Equals(IImmutable<TObj> other) =>
            ReferenceEquals(this, other) || other.Equals(_state);

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj);

        public static bool operator ==(Immutable<TObj> a, IImmutable<TObj> b) =>
            a.Equals(b);

        public static bool operator !=(Immutable<TObj> a, IImmutable<TObj> b) =>
            !a.Equals(b);

        public static bool operator ==(Immutable<TObj> a, TObj b) =>
            a.Equals(b);

        public static bool operator !=(Immutable<TObj> a, TObj b) =>
            !a.Equals(b);

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

        object IImmutable.Emit() =>
            Emit();
    }
}
