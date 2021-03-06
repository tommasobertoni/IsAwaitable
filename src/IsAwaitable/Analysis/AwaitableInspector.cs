﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IsAwaitable.Analysis
{
    internal class AwaitableInspector
    {
        public static bool TryGetGetAwaiterMethod(
            Type type,
            [NotNullWhen(true)] out MethodInfo? getAwaiterMethod)
        {
            getAwaiterMethod = null;

            var match = type
                .GetMethods(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.InvokeMethod)
                .Where(m => m.Name == "GetAwaiter")
                .Where(m => m.GetParameters().Length == 0)
                .FirstOrDefault();

            if (match is not null)
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

        public static bool HasIsCompletedProperty(
            Type type,
            [NotNullWhen(true)] out PropertyInfo? isCompletedProperty)
        {
            var properties = type.GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance);

            isCompletedProperty = properties
                .Where(p => p.Name == "IsCompleted")
                .Where(p => p.PropertyType == typeof(bool))
                .Where(p => p.CanRead)
                .FirstOrDefault();

            return isCompletedProperty is not null;
        }

        public static bool TryGetGetResultMethod(
            Type type,
            [NotNullWhen(true)] out MethodInfo? getResultMethod)
        {
            getResultMethod = null;

            var match = type
                .GetMethods(
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.InvokeMethod)
                .Where(m => m.Name == "GetResult")
                .Where(m => m.GetParameters().Length == 0)
                .FirstOrDefault();

            if (match is null)
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
