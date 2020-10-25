using System;
using static IsAwaitable.AwaitableInspector;

namespace IsAwaitable
{
    internal class AwaitableExpressionEvaluator
    {
        public static TypeEvaluation Evaluate(Type type)
        {
            if (!TryGetGetAwaiterMethod(type, out var getAwaiterMethod))
                return TypeEvaluation.NotAwaitable;

            var returnType = getAwaiterMethod.ReturnType;

            if (!ImplementsINotifyCompletion(returnType))
                return TypeEvaluation.NotAwaitable;

            if (!HasIsCompletedProperty(returnType))
                return TypeEvaluation.NotAwaitable;

            if (!TryGetGetResultMethod(returnType, out var getResultMethod))
                return TypeEvaluation.NotAwaitable;

            // GetResult() found!
            // If returns "void", it behaves like "Task" or "ValueTask".
            // If instead it returns something else, it behaves like "Task<T>" or "ValueTask<T>"

            return getResultMethod.ReturnType != typeof(void)
                ? TypeEvaluation.Awaitable
                : TypeEvaluation.AwaitableWithResult(getResultMethod.ReturnType);
        }
    }
}
