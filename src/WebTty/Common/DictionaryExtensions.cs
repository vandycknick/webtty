using System;
using System.Collections.Generic;

namespace WebTty.Common
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key, Func<TValue> valueCreator)
        {
            if (!dictionary.TryGetValue(key, out TValue value))
            {
                value = valueCreator();
                dictionary.Add(key, value);
            }
            return value;
        }
    }
}
