using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IsAwaitable
{
    internal class AwaitableInspector
    {
        public static bool TryGetGetAwaiterMethod(
            Type type,
            [MaybeNullWhen(false)] out MethodInfo getAwaiterMethod)
        {
            getAwaiterMethod = type.GetMethod("GetAwaiter",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.InvokeMethod);

            if (getAwaiterMethod is { } &&
                !getAwaiterMethod.IsPrivate &&
                getAwaiterMethod.GetParameters().Length == 0)
            {
                return true;
            }

            // Find an extension method.

            var extensions = GetExtensionMethodsFor(type);

            if (!extensions.Any())
                return false;

            getAwaiterMethod = extensions.FirstOrDefault(m =>
                m.Name == "GetAwaiter" &&
                !m.IsPrivate &&
                m.GetParameters().Length == 1);

            if (getAwaiterMethod is { })
                return true;

            return false;
        }

        public static bool ImplementsINotifyCompletion(Type type)
        {
            var interfaces = type.GetInterfaces();
            return interfaces.Any(i => i == typeof(INotifyCompletion));
        }

        public static bool HasIsCompletedProperty(Type type)
        {
            var properties = type.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance);

            var isCompletedProperty = properties.FirstOrDefault(p =>
                p.CanRead &&
                p.Name == "IsCompleted" &&
                p.PropertyType == typeof(bool));

            return isCompletedProperty is { };
        }

        public static bool HasGetResultMethod(Type type, out bool withResult)
        {
            var method = type.GetMethod("GetResult",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.InvokeMethod);

            if (method is null || method.GetParameters().Length != 0)
            {
                withResult = false;
                return false;
            }

            // GetResult() found!
            // If returns "void", it behaves like "Task" or "ValueTask".
            // If instead it returns something else, it behaves like "Task<T>" or "ValueTask<T>"

            withResult = method.ReturnType != typeof(void);
            return true;
        }

        /// <summary>
        /// Source: https://stackoverflow.com/a/299526/3743963
        /// </summary>
        private static IReadOnlyList<MethodInfo> GetExtensionMethodsFor(Type targetType)
        {
            var query = from type in targetType.Assembly.DefinedTypes
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == targetType
                        select method;

            return query.ToArray();
        }
    }
}
