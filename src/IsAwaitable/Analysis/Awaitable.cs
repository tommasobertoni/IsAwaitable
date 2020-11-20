using System;
using System.Threading.Tasks;
using static IsAwaitable.Analysis.AwaitableInspector;

namespace IsAwaitable.Analysis
{
    /// <summary>
    /// Provides methods to inspect possible awaitables.
    /// </summary>
    public static class Awaitable
    {
        /// <summary>
        /// Returns an <seealso cref="AwaitableDescription"/> of the instance
        /// if it's awaitable, otherwise null.
        /// </summary>
        /// <param name="instance">The instance to be described.</param>
        /// <returns>An <c>AwaitableDescription</c>.</returns>
        public static AwaitableDescription? Describe(object instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            return Describe(instance.GetType());
        }

        /// <summary>
        /// Returns an <seealso cref="AwaitableDescription"/> of the type
        /// if it's awaitable, otherwise null.
        /// </summary>
        /// <typeparam name="T">The type to be described.</typeparam>
        /// <returns>An <c>AwaitableDescription</c>.</returns>
        public static AwaitableDescription? Describe<T>() =>
            Describe(typeof(T));

        /// <summary>
        /// Returns an <seealso cref="AwaitableDescription"/> of the type
        /// if it's awaitable, otherwise null.
        /// </summary>
        /// <param name="type">The type to be described.</param>
        /// <returns>An <c>AwaitableDescription</c>.</returns>
        public static AwaitableDescription? Describe(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (AwaitableDescriptionCache.TryGet(type, out var cached))
                return cached;

            if (!TryGetGetAwaiterMethod(type, out var getAwaiterMethod))
                return null;

            var awaiterType = getAwaiterMethod.ReturnType;

            if (!ImplementsINotifyCompletion(awaiterType))
                return null;

            if (!HasIsCompletedProperty(awaiterType, out var isCompletedProperty))
                return null;

            if (!TryGetGetResultMethod(awaiterType, out var getResultMethod))
                return null;

            var awaitableDescription = new AwaitableDescription(
                type,
                isKnownAwaitable: IsKnownAwaitable(type),
                resultType: getResultMethod.ReturnType,
                getAwaiterMethod,
                new AwaiterDescription(
                    awaiterType,
                    isCompletedProperty,
                    getResultMethod));

            AwaitableDescriptionCache.Add(type, awaitableDescription);

            return awaitableDescription;
        }

        private static bool IsKnownAwaitable(Type type)
        {
            // Known awaitable types are: Task, Task<>, ValueTask, ValueTask<>.

            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (typeof(Task<>) == genericTypeDefinition ||
                    typeof(ValueTask<>) == genericTypeDefinition)
                {
                    return true;
                }
            }

            var isKnownWithoutResult = typeof(Task) == type || typeof(ValueTask) == type;
            return isKnownWithoutResult;
        }
    }
}
