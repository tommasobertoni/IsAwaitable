using System;
using System.Threading.Tasks;

namespace IsAwaitable
{
    internal class KnownAwaitableEvaluator
    {
        public static TypeEvaluation Evaluate(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            // Known awaitable types are: Task, Task<>, ValueTask, ValueTask<>.

            if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (typeof(Task<>) == genericTypeDefinition ||
                    typeof(ValueTask<>) == genericTypeDefinition)
                {
                    var resultType = type.GetGenericArguments()[0];
                    return TypeEvaluation.AwaitableWithResult(resultType);
                }
            }

            if (typeof(Task) == type || typeof(ValueTask) == type)
                return TypeEvaluation.Awaitable;

            return TypeEvaluation.NotAwaitable;
        }
    }
}
