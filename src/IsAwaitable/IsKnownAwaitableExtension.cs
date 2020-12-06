using System;
using System.Diagnostics.CodeAnalysis;
using IsAwaitable.Analysis;

namespace IsAwaitable
{
    /// <summary>
    /// Extension methods determining if instances or types math a known awaitable (type):
    /// Task, Task&lt;T&gt;, ValueTask, ValueTask&lt;T&gt;.
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
            var description = Awaitable.Describe(type);

            if (description is null || !description.IsKnownAwaitable)
                return false;

            return true;
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
        public static bool IsKnownAwaitableWithResult(this object? instance) =>
            instance.IsKnownAwaitableWithResult(out _);

        /// <summary>
        /// Determines whether the given instance is a known awaitable that
        /// returns a result: Task&lt;T&gt;, ValueTask&lt;T&gt;.
        /// </summary>
        /// <param name="instance">The instance to be inspected.</param>
        /// <param name="resultType">
        /// The type of the result of the <c>await</c> operation.
        /// Null when the <paramref name="instance" /> is not an awaitable
        /// that returns a result.
        /// </param>
        /// <returns>
        /// <c>true</c> when the instance is a known awaitable that returns
        /// a result, or <c>false</c> when it isn't or is null.
        /// </returns>
        public static bool IsKnownAwaitableWithResult(
            this object? instance,
            [NotNullWhen(true)] out Type? resultType)
        {
            if (instance is null)
            {
                resultType = null;
                return false;
            }

            var type = instance.GetType();
            return type.IsKnownAwaitableWithResult(out resultType);
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
        public static bool IsKnownAwaitableWithResult(this Type type) =>
            type.IsKnownAwaitableWithResult(out _);

        /// <summary>
        /// Determines whether the given <c>Type</c> is a known awaitable type
        /// that returns a result: Task&lt;T&gt;, ValueTask&lt;T&gt;.
        /// </summary>
        /// <param name="type">The <c>Type</c> to be inspected.</param>
        /// <param name="resultType">
        /// The type of the result of the <c>await</c> operation.
        /// Null when the <paramref name="type" /> is not an awaitable
        /// that returns a result.
        /// </param>
        /// <returns>
        /// <c>true</c> when the type matches one of the known awaitable
        /// types that return a result, or <c>false</c> when it doesn't.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the type parameter is null.
        /// </exception>
        public static bool IsKnownAwaitableWithResult(
            this Type type,
            [NotNullWhen(true)] out Type? resultType)
        {
            var evaluation = TypeEvaluationProvider.GetEvaluationFor(type);

            if (evaluation.IsKnownAwaitable &&
                evaluation.IsAwaitableWithResult)
            {
                resultType = evaluation.ResultType;
                return true;
            }

            resultType = null;
            return false;
        }
    }
}
