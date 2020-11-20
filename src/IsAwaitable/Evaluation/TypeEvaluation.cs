using System;
using IsAwaitable.Analysis;

namespace IsAwaitable
{
    internal class TypeEvaluation
    {
        public static readonly TypeEvaluation NotAwaitable = new(isAwaitable: false);

        public static TypeEvaluation From(AwaitableDescription descriptor) => new(descriptor);

        public bool IsAwaitable { get; }

        public bool IsAwaitableWithResult { get; }

        public bool IsKnownAwaitable { get; }

        public bool IsKnownAwaitableWithResult => IsAwaitableWithResult && IsKnownAwaitable;

        /// <exception cref="System.InvalidOperationException">
        /// Thrown when <see cref="IsAwaitableWithResult" /> returns <c>false</c>. />
        /// </exception>
        public Type ResultType
        {
            get
            {
                if (_resultType is null)
                    throw new InvalidOperationException("Type doesn't have a result.");

                return _resultType;
            }
        }

        private readonly Type? _resultType;

        private TypeEvaluation(bool isAwaitable)
        {
            IsAwaitable = isAwaitable;
            IsAwaitableWithResult = false;
            _resultType = null;
        }

        private TypeEvaluation(AwaitableDescription descriptor)
        {
            IsAwaitable = true;
            IsKnownAwaitable = descriptor.IsKnownAwaitable;

            if (descriptor.ResultType != typeof(void))
            {
                IsAwaitableWithResult = true;
                _resultType = descriptor.ResultType;
            }
        }
    }
}
