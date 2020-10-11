﻿using System;
using System.Collections.Concurrent;

namespace IsAwaitable
{
    internal class EvaluationCache
    {
        private static readonly ConcurrentDictionary<Type, TypeEvaluation> _cache =
            new ConcurrentDictionary<Type, TypeEvaluation>();

        public static bool TryGet(Type type, out TypeEvaluation evaluation)
        {
            if (_cache.TryGetValue(type, out evaluation))
                return true;

            return false;
        }

        public static void Add(Type type, TypeEvaluation evaluation) =>
            _cache.TryAdd(type, evaluation);

        public static void Clear() =>
            _cache.Clear();
    }
}
