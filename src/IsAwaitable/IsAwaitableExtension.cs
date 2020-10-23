using IsAwaitable;
using static IsAwaitable.AwaitableInspector;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Extension methods determining if instances or types can be awaited.
    /// </summary>
    /// <remarks>
    /// <para>
    /// (from the c# language specification)
    /// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#awaitable-expressions
    /// </para>
    /// <para>An expression t is awaitable if one of the following holds:</para>
    /// <para>- <c>t</c> is of compile time type <c>dynamic</c></para>
    /// <para>- <c>t</c> has an accessible instance or extension method called <c>GetAwaiter</c>
    ///     with no parameters and no type parameters, and a return type <c>A</c>
    ///     for which all of the following hold:
    /// </para>
    /// <para>-- <c>A</c> implements the interface <c>System.Runtime.CompilerServices.INotifyCompletion</c></para>
    /// <para>-- <c>A</c> has an accessible, readable instance property <c>IsCompleted</c> of type <c>bool</c></para>
    /// <para>-- <c>A</c> has an accessible instance method <c>GetResult</c> with no parameters and no type parameters</para>
    /// </remarks>
    public static class IsAwaitableExtension
    {
        /// <summary>
        /// Determines whether the given instance can be awaited.
        /// </summary>
        /// <param name="instance">The instance to be inspected.</param>
        /// <returns>
        /// <c>true</c> when the instance can be awaited,
        /// or <c>false</c> when it can't or is null.
        /// </returns>
        /// <example>
        /// <code>
        /// object instance = argument;
        /// if (instance.IsAwaitable())
        ///     await (dynamic) instance;
        /// </code>
        /// </example>
        public static bool IsAwaitable(this object? instance)
        {
            if (instance is null)
                return false;

            var type = instance.GetType();
            return type.IsAwaitable();
        }

        /// <summary>
        /// Determines whether instances of the given <c>Type</c> can be awaited.
        /// </summary>
        /// <param name="type">The <c>Type</c> to be inspected.</param>
        /// <returns>
        /// <c>true</c> when the type matches the language specification for
        /// awaitable expressions, or <c>false</c> when it doesn't.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the type parameter is null.
        /// </exception>
        /// <example>
        /// <code>
        /// T instance = argument;
        /// if (typeof(T).IsAwaitable())
        ///     await (dynamic) instance;
        /// </code>
        /// </example>
        public static bool IsAwaitable(this Type type)
        {
            var evaluation = GetEvaluationFor(type);
            
            return
                evaluation == TypeEvaluation.Awaitable ||
                evaluation == TypeEvaluation.AwaitableWithResult;
        }

        /// <summary>
        /// Determines whether the given instance can be awaited
        /// and the operation will return a result.
        /// </summary>
        /// <param name="instance">The instance to be inspected.</param>
        /// <returns>
        /// <c>true</c> when the instance can be awaited and returns a result,
        /// or <c>false</c> when it can't or is null.
        /// </returns>
        /// <example>
        /// <code>
        /// object instance = argument;
        /// if (instance.IsAwaitableWithResult())
        /// {
        ///     var result = await (dynamic)instance;
        /// }
        /// </code>
        /// </example>
        public static bool IsAwaitableWithResult(this object? instance)
        {
            if (instance is null)
                return false;

            var type = instance.GetType();
            return type.IsAwaitableWithResult();
        }

        /// <summary>
        /// Determines whether instances of the given <c>Type</c> can be awaited
        /// and the operation will return a result.
        /// </summary>
        /// <param name="type">The <c>Type</c> to be inspected.</param>
        /// <returns>
        /// <c>true</c> when the type matches the language specification for
        /// awaitable expressions returning a result, or <c>false</c> when it doesn't.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the type parameter is null.
        /// </exception>
        /// <example>
        /// <code>
        /// T instance = argument;
        /// if (typeof(T).IsAwaitableWithResult())
        /// {
        ///     var result = await (dynamic)instance;
        /// }
        /// </code>
        /// </example>
        public static bool IsAwaitableWithResult(this Type type)
        {
            var evaluation = GetEvaluationFor(type);
            return evaluation == TypeEvaluation.AwaitableWithResult;
        }

        private static TypeEvaluation GetEvaluationFor(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsKnownAwaitableWithResult())
                return TypeEvaluation.AwaitableWithResult;

            if (type.IsKnownAwaitable())
                return TypeEvaluation.Awaitable;

            if (EvaluationCache.TryGet(type, out var evaluation))
                return evaluation;

            evaluation = InspectAndEvaluateIfTypeIsAwaitable(type);

            EvaluationCache.Add(type, evaluation);

            return evaluation;
        }

        private static TypeEvaluation InspectAndEvaluateIfTypeIsAwaitable(Type type)
        {
            if (!TryGetGetAwaiterMethod(type, out var getAwaiterMethod))
                return TypeEvaluation.NotAwaitable;

            var returnType = getAwaiterMethod.ReturnType;

            if (!ImplementsINotifyCompletion(returnType))
                return TypeEvaluation.NotAwaitable;

            if (!HasIsCompletedProperty(returnType))
                return TypeEvaluation.NotAwaitable;

            if (!HasGetResultMethod(returnType, out var withResult))
                return TypeEvaluation.NotAwaitable;

            return withResult
                ? TypeEvaluation.AwaitableWithResult
                : TypeEvaluation.Awaitable;
        }
    }
}
