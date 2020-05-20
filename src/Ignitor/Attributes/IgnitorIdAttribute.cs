using System;

namespace Ignitor.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnitorIdAttribute : Attribute
    {
    }
}
