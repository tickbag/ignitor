using System;

namespace Ignitor
{
    public interface IImmutable
    {
        IImmutable this[string propertyName] { get; }

        IImmutable Ref(string propertyName);
        IImmutable<TRef> Ref<TRef>(string propertyName);
        IImmutable<TArray>[] Array<TArray>(string propertyName);
        object Emit();
        object Value(string propertyName);
        TValue Value<TValue>(string propertyName);
    }

    public interface IImmutable<TObj> : IImmutable, IEquatable<TObj>, IEquatable<IImmutable<TObj>>
    {
        new TObj Emit();
        bool ValueCheck(Predicate<TObj> predicate);
    }
}
