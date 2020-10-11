using System;
using System.Collections.Concurrent;

namespace IsAwaitable
{
    internal class EvaluationCache
    {
        private static readonly ConcurrentDictionary<Type, bool> _cache =
            new ConcurrentDictionary<Type, bool>();

        public static bool TryGet(Type type, out bool isAwaitable) =>
            _cache.TryGetValue(type, out isAwaitable);

        public static void Add(Type type, bool isAwaitable) =>
            _cache.TryAdd(type, isAwaitable);

        public static void Clear() =>
            _cache.Clear();
    }
}
