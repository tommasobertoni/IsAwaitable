using System;
using System.Threading.Tasks;

namespace IsAwaitable
{
    public static class IsKnownAwaitableExtension
    {
        public static bool IsKnownAwaitable(this object? instance)
        {
            if (instance is null)
                return false;

            var type = instance.GetType();
            return type.IsKnownAwaitable();
        }

        public static bool IsKnownAwaitable(this Type type)
        {
            var evaluation = GetEvaluationFor(type);
            
            return
                evaluation == TypeEvaluation.Awaitable ||
                evaluation == TypeEvaluation.AwaitableWithResult;
        }

        public static bool IsKnownAwaitableWithResult(this object? instance)
        {
            if (instance is null)
                return false;

            var type = instance.GetType();
            return type.IsKnownAwaitableWithResult();
        }

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
