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
            var evaluation = Evaluate(type);
            return
                evaluation == TypeEvaluation.Awaitable ||
                evaluation == TypeEvaluation.AwaitableWithResult;
        }

        public static bool IsAwaitableWithResult(this object? instance)
        {
            if (instance is null)
                return false;

            var type = instance.GetType();
            return type.IsAwaitableWithResult();
        }

        public static bool IsAwaitableWithResult(this Type type)
        {
            var evaluation = Evaluate(type);
            return evaluation == TypeEvaluation.AwaitableWithResult;
        }

        private static TypeEvaluation Evaluate(Type type)
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
