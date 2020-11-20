using System;
using IsAwaitable.Analysis;

namespace IsAwaitable
{
    internal class TypeEvaluationProvider
    {
        public static TypeEvaluation GetEvaluationFor(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (TypeEvaluationCache.TryGet(type, out var evaluation))
                return evaluation;

            var description = Awaitable.Describe(type);

            evaluation = description is null
                ? TypeEvaluation.NotAwaitable
                : TypeEvaluation.From(description);

            TypeEvaluationCache.Add(type, evaluation);

            return evaluation;
        }
    }
}
