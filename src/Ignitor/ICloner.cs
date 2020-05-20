using System;

namespace Ignitor
{
    public interface ICloner<T>
    {
        Func<T, T> GetCloner();
    }
}
