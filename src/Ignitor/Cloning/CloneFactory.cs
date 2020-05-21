using System;
using System.Collections.Concurrent;

namespace Ignitor.Cloning
{
    public static class CloneFactory
    {
        private static ConcurrentDictionary<Type, ICloner> _cloners = new ConcurrentDictionary<Type, ICloner>();

        public static Func<TObj, TObj> GetCloner<TObj>()
        {
            var type = typeof(TObj);

            return ((ICloner<TObj>)_cloners.GetOrAdd(type, (t) => new Cloner<TObj>())).GetCloner();
        }
    }
}
