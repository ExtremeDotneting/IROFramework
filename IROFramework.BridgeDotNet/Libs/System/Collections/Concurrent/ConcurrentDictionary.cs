using System;
using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    public class ConcurrentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public ConcurrentDictionary()
        {
        }

        public ConcurrentDictionary(int capacity) : base(capacity)
        {
        }

        public ConcurrentDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        {
        }

        public ConcurrentDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }

        public ConcurrentDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
        {
        }

        public ConcurrentDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>true if the key/value pair was added to the </returns>
        public bool TryAdd(TKey key, TValue value)
        {
            this[key] = value;
            return true;
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> addFunc)
        {
            if (!ContainsKey(key))
            {
                this[key]=addFunc(key);
            }
            return this[key];
            
        }
    }
}
