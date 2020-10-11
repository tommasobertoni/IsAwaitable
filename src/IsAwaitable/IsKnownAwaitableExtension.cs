using System;
using System.Threading.Tasks;

namespace IsAwaitable
{
    internal static class IsKnownAwaitableExtension
    {
        internal static bool IsKnownAwaitable(this Type type, out bool withResult)
        {
            // Known awaitable types are: Task, Task<>, ValueTask, ValueTask<>.

            if (type.IsGenericType)
            {
                if (type.IsSubclassOfRawGeneric(typeof(Task<>)))
                {
                    withResult = true;
                    return true;
                }

                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (typeof(ValueTask<>) == genericTypeDefinition)
                {
                    withResult = true;
                    return true;
                }
            }

            withResult = false;

            if (typeof(Task).IsAssignableFrom(type) ||
                typeof(ValueTask) == type)
            {
                return true;
            }

            return false;
        }

        public static bool IsSubclassOfRawGeneric(this Type type, Type rawGenericType)
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
