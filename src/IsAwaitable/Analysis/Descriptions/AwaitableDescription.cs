using System;
using System.Reflection;

namespace IsAwaitable.Analysis
{
    /// <summary>
    /// Describes an awaitable type and
    /// what allows it to be awaitable.
    /// </summary>
    public class AwaitableDescription
    {
        /// <summary>
        /// The awaitable type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The type of the result of the <c>await</c> operation.
        /// </summary>
        public Type ResultType { get; }

        /// <summary>
        /// The type is a known awaitable (Task, Task&lt;T&gt;, ValueTask, ValueTask&lt;T&gt;).
        /// </summary>
        public bool IsKnownAwaitable { get; }

        /// <summary>
        /// The <c>GetAwaiter</c> method of this type.
        /// </summary>
        public MethodInfo GetAwaiterMethod { get; }

        /// <summary>
        /// The descriptor for the awaiter type.
        /// </summary>
        public AwaiterDescription AwaiterDescriptor { get; }

        internal AwaitableDescription(
            Type type,
            bool isKnownAwaitable,
            Type resultType,
            MethodInfo getAwaiterMethod,
            AwaiterDescription awaiterDescriptor)
        {
            Type = type;
            IsKnownAwaitable = isKnownAwaitable;
            ResultType = resultType;
            GetAwaiterMethod = getAwaiterMethod;
            AwaiterDescriptor = awaiterDescriptor;
        }
    }
}
