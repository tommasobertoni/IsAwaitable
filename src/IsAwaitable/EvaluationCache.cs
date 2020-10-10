using System;
using System.Collections.Generic;

namespace IsAwaitable
{
    internal class EvaluationCache
    {
        private static readonly Dictionary<Type, bool> _cache =
            new Dictionary<Type, bool>();

        public static bool TryGet(Type type, out bool isAwaitable) =>
            _cache.TryGetValue(type, out isAwaitable);

        public static void Add(Type type, bool isAwaitable) =>
            _cache.TryAdd(type, isAwaitable);
    }
}
