using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ignitor.Cloning
{
    /// <summary>
    /// Ignitor's highly-opinionated deep cloning compiler.
    /// The cloner compiles a dynamically constructed and specifically targeted expression tree down to IL code for execution,
    /// this allows the cloner to operate at speeds over 40x faster than standard reflection techniques.
    /// The cloner is critical for maintaining true immutability in the Ignitor state system.
    /// 
    /// NOTES:
    /// The cloner will:
    /// - NOT clone Interfaces or any object having Interfaces as fields rather than concrete implementations
    ///     This is due the fact the cloner has no idea what class lies behind the interface and so can't precompile the cloning algorithm
    /// - NOT clone Lists, Collections or Dictionaries - Use an array collection of data
    /// - Only shallow copy Structs - Don't put reference types in your Structs
    /// - initialise types with constructors with parameters as long as they only have simple type arguments
    /// - - If your constructor requires a parameter with a reference type, that type must use a parameterless constructor
    /// - NOT clone types with an object graph deeper than 5. This prevents slow cloning performance and infinite loops due to self-referencing.
    /// </summary>
    internal class Cloner<T> : ICloner<T>
    {
        /// <summary>
        /// Maximum object graph depth to clone. Any higher than this value and the cloner build function will throw an exception.
        /// </summary>
        private const int MaxObjectGraphDepth = 5;

        /// <summary>
        /// Static cache of the compiled cloning function
        /// </summary>
        private static Func<T, T> _compiledCloner;

        /// <summary>
        /// Gets a cloner function for the specified type
        /// </summary>
        /// <returns>The specifically targeted cloning function</returns>
        public Func<T, T> GetCloner()
        {
            _compiledCloner ??= CompileCloner();
            return _compiledCloner;
        }

        /// <summary>
        /// Creates a cloner function specifically targeted to the type requested.
        /// </summary>
        /// <returns>A specifically trageted deep cloning function</returns>
        private static Func<T, T> CompileCloner()
        {
            var isValueType = typeof(T).IsValueType || typeof(T) == typeof(string);
            return isValueType ? (source) => source : CreateClonerExpression().Compile();
        }

        #region Cloner Expression builder code - HERE BE DRAGONS!
        /// <summary>
        /// Creates a cloner function for the specified generic type
        /// </summary>
        /// <returns>An uncompiled cloner function for the specifed generic type</returns>
        private static Expression<Func<T, T>> CreateClonerExpression()
        {
            var source = Expression.Parameter(typeof(T), "sourceObj");

            var block = BuildObjectCloner<T>(source, 1);

            return Expression.Lambda<Func<T, T>>(block, source);
        }

        /// <summary>
        /// Calls the BuildObjectCloner generic method based on a provideed type
        /// </summary>
        /// <param name="type">Type to use for the generic call</param>
        /// <param name="sourceObj">The source object parameter to pass on to the BuildObjectCloner method</param>
        /// <param name="depth">The current object graph depth</param>
        /// <returns>A block expression representing the clone procedure for that object type</returns>
        private static BlockExpression BuildObjectClonerByType(Type type, ParameterExpression sourceObj, int depth)
        {
            var method = typeof(Cloner<T>).GetMethod(nameof(Cloner<object>.BuildObjectCloner), BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo generic = method.MakeGenericMethod(type);

            return (BlockExpression)generic.Invoke(null, new object[] { sourceObj, depth + 1 });
        }

        /// <summary>
        /// Builds an object cloner for the specifed generic type. This is the main entry point for each traversal of the object graph.
        /// </summary>
        /// <typeparam name="TObj">Type to build cloner fragment for</typeparam>
        /// <param name="sourceObj">Source object parameter</param>
        /// <param name="depth">Current object graph depth</param>
        /// <returns>A block expression that handles cloning the type specified and all it's descendant objects with the graph</returns>
        private static BlockExpression BuildObjectCloner<TObj>(ParameterExpression sourceObj, int depth)
        {
            var type = typeof(TObj);

            if (depth > MaxObjectGraphDepth)
                throw new InvalidOperationException($"'{type.Name}' - Maximum Object graph depth exceeded. Please flatten your object structure to continue.");

            var source = Expression.Parameter(type, type.Name + "Source");
            var sourceVar = Expression.Assign(source, sourceObj);

            var result = Expression.Parameter(type, type.Name + "Result");

            var exitLabel = Expression.Label(type, Guid.NewGuid().ToString());

            var nullValue = Expression.Constant(null, type);
            var nullCheck = Expression.IfThen(Expression.Equal(source, nullValue), Expression.Return(exitLabel, nullValue));

            var copyExpression = new List<Expression>();
            copyExpression.Add(sourceVar);
            copyExpression.Add(nullCheck);

            switch (type)
            {
                case Type arrType when arrType.IsArray:
                    copyExpression.Add(HandleArrayObject(arrType.GetElementType(), source, result, depth));
                    break;
                case Type valType when valType.IsValueType || valType == typeof(string):
                    copyExpression.Add(Expression.Assign(result, source));
                    break;
                default:
                    copyExpression.AddRange(HandleReferenceTypeObject(type, source, result, depth));
                    break;
            }

            copyExpression.Add(Expression.Label(exitLabel, result));

            var block = Expression.Block(
                type,
                new[] { result, source },
                copyExpression);

            return block;
        }

        private static IEnumerable<Expression> HandleReferenceTypeObject(Type type, ParameterExpression source, ParameterExpression result, int depth)
        {
            if (type.IsInterface)
            {
                throw new InvalidOperationException($"'{source.Type.Name}' - Unable to clone interfaces. Use concrete types and implementations instead.");
            }
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"'{source.Type.Name}' - Unable to clone types derived from IDictionary. Consider moving these objects to their own state.");
            }
            if (typeof(IList).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"'{source.Type.Name}' - Unable to clone types derived from IList. Consider using an array instead. ");
            }

            var copyExpression = new List<Expression>();

            var constructors = type.GetConstructors();
            var ctor = constructors.Length > 0 ? constructors[0] : throw new ArgumentException("Object has no constructor.");
            var ctorParams = ctor.GetParameters();

            Expression newObj;
            if (ctorParams.Length > 0)
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var ctorArgs = new List<Expression>();
                foreach (var ctorParam in ctorParams)
                {
                    var matchedProp = properties.FirstOrDefault(p => p.Name.Equals(ctorParam.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (matchedProp != null)
                    {
                        ctorArgs.Add(Expression.Property(source, matchedProp.GetGetMethod()));
                    }
                    else
                    {
                        ctorArgs.Add(Expression.New(ctorParam.ParameterType));
                    }
                }
                newObj = Expression.New(ctor, ctorArgs);
            }
            else
            {
                newObj = Expression.New(ctor);
            }

            copyExpression.Add(Expression.Assign(result, newObj));

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {

                if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                {
                    copyExpression.Add(Expression.Assign(Expression.Field(result, field), Expression.Field(source, field)));
                }
                else
                {
                    copyExpression.Add(HandleObjectGraphTraversal(field, source, result, depth));
                }
            }

            return copyExpression;
        }

        private static BlockExpression HandleObjectGraphTraversal(FieldInfo field, ParameterExpression source, ParameterExpression result, int depth)
        {
            var sourceField = Expression.Field(source, field);
            var targetField = Expression.Field(result, field);

            var expressions = new List<Expression>();

            var srcObj = Expression.Parameter(field.FieldType, field.FieldType.Name + "SourceCall1");
            expressions.Add(Expression.Assign(srcObj, sourceField));
            var fragment = BuildObjectClonerByType(field.FieldType, srcObj, depth);
            expressions.Add(Expression.Assign(targetField, fragment));

            return Expression.Block(new[] { srcObj }, expressions);
        }

        private static BlockExpression HandleArrayObject(Type elementType, ParameterExpression source, ParameterExpression result, int depth)
        {
            var arrayLength = Expression.ArrayLength(source);
            var newArray = Expression.NewArrayBounds(elementType, arrayLength);

            var arrayIndex = Expression.Variable(typeof(int), elementType.Name + "ArrayIndex");
            var arrayValue = Expression.Variable(elementType, elementType.Name + "ArrayValue");

            var srcParam = Expression.Parameter(elementType, elementType.Name + "SourceCall2");

            var breakLabel = Expression.Label();

            var arrayCopyExpr = new List<Expression>();
            arrayCopyExpr.Add(Expression.IfThen(Expression.GreaterThanOrEqual(arrayIndex, arrayLength), Expression.Break(breakLabel)));
            arrayCopyExpr.Add(Expression.Assign(arrayValue, Expression.ArrayAccess(source, arrayIndex)));
            if (!elementType.IsValueType)
            {
                arrayCopyExpr.Add(Expression.Assign(srcParam, arrayValue));
                var fragment = BuildObjectClonerByType(elementType, srcParam, depth);
                arrayCopyExpr.Add(Expression.Assign(arrayValue, fragment));
            }
            arrayCopyExpr.Add(Expression.Assign(Expression.ArrayAccess(result, arrayIndex), arrayValue));
            arrayCopyExpr.Add(Expression.AddAssign(arrayIndex, Expression.Constant(1)));

            var copyLoopBlock = Expression.Block(
                new[] { arrayValue, srcParam },
                arrayCopyExpr);

            var copyLoop = Expression.Loop(copyLoopBlock, breakLabel);

            var arrayCopy = new List<Expression>();
            arrayCopy.Add(Expression.Assign(arrayIndex, Expression.Constant(0)));
            arrayCopy.Add(Expression.Assign(result, newArray));
            arrayCopy.Add(copyLoop);

            return Expression.Block(
                new[] { arrayIndex },
                arrayCopy);
        }
        #endregion
    }
}
