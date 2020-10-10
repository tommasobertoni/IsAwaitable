using IsAwaitable;
using static IsAwaitable.AwaitableInspector;

namespace System.Threading.Tasks
{
    /// <summary>
    /// (from the c# language specification)
    /// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#awaitable-expressions
    /// 
    /// An expression t is awaitable if one of the following holds:
    ///   - t is of compile time type dynamic
    ///   
    ///   - t has an accessible instance or extension method called GetAwaiter
    ///     with no parameters and no type parameters, and a return type A
    ///     for which all of the following hold:
    ///     
    ///     - A implements the interface System.Runtime.CompilerServices.INotifyCompletion
    ///     
    ///     - A has an accessible, readable instance property IsCompleted of type bool
    ///
    ///     - A has an accessible instance method GetResult with no parameters
    ///       and no type parameters
    /// </summary>
    public static class IsAwaitableExtension
    {
        public static bool IsAwaitable(this object? instance)
        {
            if (instance is null)
                return false;

            var type = instance.GetType();
            return type.IsAwaitable();
        }

        public static bool IsAwaitable(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (IsKnownAwaitable(type))
                return true;

            if (EvaluationCache.TryGet(type, out bool isAwaitable))
                return isAwaitable;

            isAwaitable = EvaluateIfTypeIsAwaitable(type);

            EvaluationCache.Add(type, isAwaitable);

            return isAwaitable;
        }

        private static bool IsKnownAwaitable(Type type)
        {
            return
                typeof(Task).IsAssignableFrom(type) ||
                typeof(ValueTask).IsAssignableFrom(type) ||
                typeof(ValueTask<>).IsAssignableFrom(type);
        }

        private static bool EvaluateIfTypeIsAwaitable(Type type)
        {
            if (!TryGetGetAwaiterMethod(type, out var getAwaiterMethod))
                return false;

            var returnType = getAwaiterMethod.ReturnType;

            if (!ImplementsINotifyCompletion(returnType))
                return false;

            if (!HasIsCompletedProperty(returnType))
                return false;

            if (!HasGetResultMethod(returnType))
                return false;

            return true;
        }
    }
}
