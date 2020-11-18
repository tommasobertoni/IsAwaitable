using System;
using System.Reflection;

namespace IsAwaitable.Analysis
{
    /// <summary>
    /// Describes an awaiter type and
    /// what allows it to be invoked
    /// by an awaitable expression.
    /// </summary>
    public class AwaiterDescription
    {
        /// <summary>
        /// The awaiter type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The <c>IsCompletedProperty</c> property of this type.
        /// </summary>
        public PropertyInfo IsCompletedProperty { get; }

        /// <summary>
        /// The <c>GetResultMethod</c> method of this type.
        /// </summary>
        public MethodInfo GetResultMethod { get; }

        internal AwaiterDescription(
            Type type,
            PropertyInfo isCompletedProperty,
            MethodInfo getResultMethod)
        {
            Type = type;
            IsCompletedProperty = isCompletedProperty;
            GetResultMethod = getResultMethod;
        }
    }
}
