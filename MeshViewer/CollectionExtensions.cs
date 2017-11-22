using System;
using System.Collections.Generic;

namespace MeshViewer
{
    public static class CollectionExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> generator)
        {
            if (!dict.ContainsKey(key))
                return dict[key] = generator(key);

            return dict[key];
        }

        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                return false;

            dict.Add(key, value);
            return true;
        }
    }
}
