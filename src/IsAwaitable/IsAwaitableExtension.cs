using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    /// <summary>
    /// (from the c# language specification)
    /// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#awaitable-expressions
    /// 
    /// An expression t is awaitable if one of the following holds:
    ///   - t is of compile time type dynamic
    ///   
    ///   - t has an accessible instance or extension method called GetAwaiter
    ///     with no parameters and no type parameters, and a return type A
    ///     for which all of the following hold:
    ///     
    ///     - A implements the interface System.Runtime.CompilerServices.INotifyCompletion
    ///     
    ///     - A has an accessible, readable instance property IsCompleted of type bool
    ///
    ///     - A has an accessible instance method GetResult with no parameters
    ///       and no type parameters
    /// </summary>
    public static class IsAwaitableExtension
    {
        public static bool IsAwaitable(this object instance)
        {
            if (instance is null)
                return false;

            var type = instance.GetType();

            return type.IsAwaitable();
        }

        public static bool IsAwaitable(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (IsKnownAwaitable(type))
                return true;

            if (!TryGetGetAwaiterMethod(type, out var getAwaiterMethod))
                return false;

            var returnType = getAwaiterMethod.ReturnType;

            if (!ImplementsINotifyCompletion(returnType))
                return false;

            if (!HasIsCompletedProperty(returnType))
                return false;

            if (!HasGetResultMethod(returnType))
                return false;

            return true;
        }

        private static bool TryGetGetAwaiterMethod(
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

        private static bool ImplementsINotifyCompletion(Type type)
        {
            var interfaces = type.GetInterfaces();
            return interfaces.Any(i => i == typeof(INotifyCompletion));
        }

        private static bool HasIsCompletedProperty(Type type)
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

        private static bool HasGetResultMethod(Type type)
        {
            var method = type.GetMethod("GetResult",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.InvokeMethod);

            return method is { } && method.GetParameters().Length == 0;
        }

        private static bool IsKnownAwaitable(Type type)
        {
            return
                typeof(Task).IsAssignableFrom(type) ||
                typeof(ValueTask).IsAssignableFrom(type) ||
                typeof(ValueTask<>).IsAssignableFrom(type);
        }
    }
}
