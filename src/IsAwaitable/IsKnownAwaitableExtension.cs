using System;
using System.Threading.Tasks;

namespace IsAwaitable
{
    /// <summary>
    /// Extension methods determining if instances or types math a known awaitable (type):
    /// Task, Task&lt;T&gt;, ValueTask, ValueTask&lt;T&gt;.
    /// Publicly available, but mainly intended to be used by the
    /// <see cref="System.Threading.Tasks.IsAwaitableExtension"/>.
    /// </summary>
    public static class IsKnownAwaitableExtension
    {
        /// <summary>
        /// Determines whether the given instance is a known awaitable:
        /// Task, Task&lt;T&gt;, ValueTask, ValueTask&lt;T&gt;.
        /// </summary>
        /// <param name="instance">The instance to be inspected.</param>
        /// <returns>
        /// <c>true</c> when the instance is a known awaitable,
        /// or <c>false</c> when it isn't or is null.
        /// </returns>
        public static bool IsKnownAwaitable(this object? instance)
        {
            if (instance is null)
                return false;

            var type = instance.GetType();
            return type.IsKnownAwaitable();
        }

        /// <summary>
        /// Determines whether the given <c>Type</c> is a known awaitable type:
        /// Task, Task&lt;T&gt;, ValueTask, ValueTask&lt;T&gt;.
        /// </summary>
        /// <param name="type">The <c>Type</c> to be inspected.</param>
        /// <returns>
        /// <c>true</c> when the type matches one of the known awaitable
        /// types, or <c>false</c> when it doesn't.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the type parameter is null.
        /// </exception>
        public static bool IsKnownAwaitable(this Type type)
        {
            var evaluation = GetEvaluationFor(type);
            
            return
                evaluation == TypeEvaluation.Awaitable ||
                evaluation == TypeEvaluation.AwaitableWithResult;
        }

        /// <summary>
        /// Determines whether the given instance is a known awaitable that
        /// returns a result: Task&lt;T&gt;, ValueTask&lt;T&gt;.
        /// </summary>
        /// <param name="instance">The instance to be inspected.</param>
        /// <returns>
        /// <c>true</c> when the instance is a known awaitable that returns
        /// a result, or <c>false</c> when it isn't or is null.
        /// </returns>
        public static bool IsKnownAwaitableWithResult(this object? instance)
        {
            if (instance is null)
                return false;

            var type = instance.GetType();
            return type.IsKnownAwaitableWithResult();
        }

        /// <summary>
        /// Determines whether the given <c>Type</c> is a known awaitable type
        /// that returns a result: Task&lt;T&gt;, ValueTask&lt;T&gt;.
        /// </summary>
        /// <param name="type">The <c>Type</c> to be inspected.</param>
        /// <returns>
        /// <c>true</c> when the type matches one of the known awaitable
        /// types that return a result, or <c>false</c> when it doesn't.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the type parameter is null.
        /// </exception>
        public static bool IsKnownAwaitableWithResult(this Type type)
        {
            var evaluation = GetEvaluationFor(type);
            return evaluation == TypeEvaluation.AwaitableWithResult;
        }

        internal static TypeEvaluation GetEvaluationFor(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            // Known awaitable types are: Task, Task<>, ValueTask, ValueTask<>.

            if (type.IsGenericType)
            {
                if (type.IsSubclassOfRawGeneric(typeof(Task<>)))
                    return TypeEvaluation.AwaitableWithResult;

                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (typeof(ValueTask<>) == genericTypeDefinition)
                    return TypeEvaluation.AwaitableWithResult;
            }

            if (typeof(Task).IsAssignableFrom(type) ||
                typeof(ValueTask) == type)
                return TypeEvaluation.Awaitable;

            return TypeEvaluation.NotAwaitable;
        }

        private static bool IsSubclassOfRawGeneric(this Type type, Type rawGenericType)
        {
            var toCheck = type;

            while (toCheck is { } && toCheck != typeof(object))
            {
                var target = toCheck.IsGenericType
                    ? toCheck.GetGenericTypeDefinition()
                    : toCheck;
                
                if (rawGenericType == target)
                    return true;

                toCheck = toCheck.BaseType;
            }

            return false;
        }
    }
}
