using System;

namespace Ignitor.Attributes
{
    /// <summary>
    /// Indicates to the Ignitor framework which property represents the primary Id/Key of the model.
    /// This is used for bulk loading items into state, and storing them under the correct Id.
    /// It can be overridden by using the '.WithId()' method on a scoped state object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IgnitorIdAttribute : Attribute { }
}
