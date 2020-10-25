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
            [NotNullWhen(true)] out MethodInfo? getAwaiterMethod)
        {
            getAwaiterMethod = null;

            var match = type.GetMethod("GetAwaiter",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.InvokeMethod);

            if (match is { } &&
                !match.IsPrivate &&
                match.GetParameters().Length == 0)
            {
                getAwaiterMethod = match;
                return true;
            }

            // Find an extension method.

            var extensions = ListExtensionMethodsFor(type);

            if (!extensions.Any())
                return false;

            var extensionMatch = extensions.FirstOrDefault(m =>
                m.Name == "GetAwaiter" &&
                !m.IsPrivate &&
                m.GetParameters().Length == 1);

            if (extensionMatch is null)
                return false;

            getAwaiterMethod = extensionMatch;
            return true;
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

            var isCompletedProperty = properties
                .Where(p => p.Name == "IsCompleted")
                .Where(p => p.PropertyType == typeof(bool))
                .Where(p => p.CanRead)
                .FirstOrDefault();

            return isCompletedProperty is { };
        }

        public static bool TryGetGetResultMethod(
            Type type,
            [NotNullWhen(true)] out MethodInfo? getResultMethod)
        {
            getResultMethod = null;

            var match = type.GetMethod("GetResult",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.InvokeMethod);

            if (match is null || match.GetParameters().Length != 0)
                return false;

            getResultMethod = match;
            return true;
        }

        private static IReadOnlyList<MethodInfo> ListExtensionMethodsFor(Type targetType)
        {
            // Source: https://stackoverflow.com/a/299526/3743963

            var query =
                from type in targetType.Assembly.DefinedTypes
                where type.IsSealed && !type.IsGenericType && !type.IsNested
                from method in type.GetMethods(
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where method.IsDefined(typeof(ExtensionAttribute), false)
                where method.GetParameters()[0].ParameterType == targetType
                select method;

            return query.ToArray();
        }
    }
}
