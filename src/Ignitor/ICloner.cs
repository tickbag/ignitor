using System;

namespace Ignitor
{
    public interface ICloner<T> : ICloner
    {
        Func<T, T> GetCloner();
    }

    public interface ICloner { }
}
