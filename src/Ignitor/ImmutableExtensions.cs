using Ignitor.Immutables;

namespace Ignitor
{
    public static class ImmutableExtensions
    {
        public static IImmutable<TImmutable> MakeImmutable<TImmutable>(this TImmutable value)
        {
            return new Immutable<TImmutable>(value);
        }
    }
}
