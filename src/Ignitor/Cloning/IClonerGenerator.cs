using System;

namespace Ignitor
{
    /// <summary>
    /// Ignitor's highly-opinionated deep cloning compiler.
    /// The cloner compiles a dynamically constructed and specifically targeted expression tree down to IL code for execution,
    /// this allows the cloner to operate at speeds over 40x faster than standard reflection techniques.
    /// The cloner is critical for maintaining true immutability in the Ignitor state system.
    /// <para>
    /// NOTES:
    /// The cloner will:
    /// - NOT clone Interfaces or any object having Interfaces as fields rather than concrete implementations<br/>
    ///     This is due the fact the cloner has no idea what class lies behind the interface and so can't precompile the cloning algorithm<br/>
    /// - NOT clone Lists, Collections or Dictionaries - Use an array collection of data<br/>
    /// - Only shallow copy Structs - Don't put reference types in your Structs<br/>
    /// - initialise types with constructors with parameters as long as they only have simple type arguments<br/>
    /// - - If your constructor requires a parameter with a reference type, that type must use a parameterless constructor<br/>
    /// - NOT clone types with an object graph deeper than 5. This prevents slow cloning performance and infinite loops due to self-referencing.<br/>
    /// </para>
    /// </summary>
    internal interface IClonerGenerator<T> : IClonerGenerator
    {
        /// <summary>
        /// Gets a cloner function for the specified type
        /// </summary>
        /// <returns>The specifically targeted cloning function</returns>
        Func<T, T> GetCloner();
    }

    /// <summary>
    /// Represents a clone 
    /// </summary>
    internal interface IClonerGenerator { }
}
