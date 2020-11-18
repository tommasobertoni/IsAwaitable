using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace IsAwaitable
{
    internal class Cache<T>
    {
        private static readonly ConcurrentDictionary<Type, T> _cache = new();

        public static bool TryGet(
            Type type,
            [MaybeNullWhen(false)] out T value)
        {
            if (_cache.TryGetValue(type, out value))
                return true;

            return false;
        }

        public static void Add(Type type, T value) =>
            _cache.TryAdd(type, value);

        public static void Clear() =>
            _cache.Clear();
    }
}
