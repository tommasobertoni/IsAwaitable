using System;

namespace IsAwaitable
{
    internal class TypeEvaluation
    {
        public static readonly TypeEvaluation NotAwaitable = new TypeEvaluation(false);

        public static readonly TypeEvaluation Awaitable = new TypeEvaluation(true);

        public static TypeEvaluation AwaitableWithResult(Type resultType) =>
            new TypeEvaluation(true, resultType);

        public bool IsAwaitable { get; }

        public bool IsAwaitableWithResult { get; }

        /// <exception cref="System.InvalidOperationException">
        /// Thrown when <see cref="IsAwaitableWithResult" /> returns <c>false</c>. />
        /// </exception>
        public Type ResultType
        {
            get
            {
                if (_resultType is null)
                    throw new InvalidOperationException("Type doesn't return a type.");

                return _resultType;
            }
        }

        private readonly Type? _resultType;

        private TypeEvaluation(
            bool isAwaitable,
            Type? resultType = null)
        {
            if (resultType == typeof(void))
                throw new ArgumentException("Result type can't be 'void'.");

            IsAwaitable = isAwaitable;
            _resultType = resultType;

            IsAwaitableWithResult = isAwaitable && !(_resultType is null);
        }
    }
}
