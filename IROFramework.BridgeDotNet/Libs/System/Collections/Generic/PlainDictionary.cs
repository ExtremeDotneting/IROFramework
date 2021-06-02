using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge;

namespace System.Collections.Generic
{
    /// <summary>
    /// Designed specially for Bridge.Net.
    /// Use js object fields to store keys.
    /// </summary>
    public class PlainDictionary<TKey, TValue> : 
        IDictionary<TKey, TValue>,
        IDictionary,
        IReadOnlyDictionary<TKey, TValue>
    {
        public int Count { get; private set; }

        public bool IsReadOnly => false;

        /// <summary>
        /// Note that here is saved duplicates.
        /// </summary>
        public ICollection<TKey> Keys { get; } = new List<TKey>();

        public ICollection<TValue> Values
        {
            get
            {
                var values = new List<TValue>();
                foreach (var key in Keys)
                {
                    values.Add(this[key]);
                }
                return values;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in Keys)
            {
                var newItem = new KeyValuePair<TKey, TValue>(
                    key,
                    this[key]
                );
                yield return newItem;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this[item.Key] = item.Value;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            var keysClone = Keys.ToArray();
            foreach (var key in keysClone)
            {
                Keys.Remove(key);
                Remove(key);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new Exception(
                    $"{nameof(PlainDictionary<TKey, TValue>)} already contains value with key '{KeyToString(key)}'");
            }
            this[key] = value;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGetValue(key, out var value);
        }

        public bool Remove(TKey key)
        {
            if (ContainsKey(key))
            {
                var keyStr = KeyToString(key);
                Keys.Remove(key);
                Script.Delete(this, keyStr);
                Count--;
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                value = this[key];
                return true;
            }
            catch
            {
                value = default(TValue);
                return false;
            }
        }

        public TValue this[TKey key]
        {
            get => GetDictValue<TValue>(key);
            set => SetDictValue(key, value);
        }

        object IDictionary.this[object key]
        {
            get => GetDictValue<TValue>((TKey)key);
            set => SetDictValue((TKey)key, value);
        }

        void SetDictValue<TInnerValue>(TKey key, TInnerValue value)
        {
            var incCount = !ContainsKey(key);
            var keyStr = KeyToString(key);
            Script.Set(this, keyStr, value);
            Keys.Add(key);
            if (incCount)
            {
                Count++;
            }
        }

        TInnerValue GetDictValue<TInnerValue>(TKey key)
        {
            var keyStr = KeyToString(key);
            return Script.Get<TInnerValue>(this, keyStr);
        }

        string KeyToString(object key)
        {
            if (key is string str)
            {
                return str;
            }
            else if (key is IFormattable)
            {
                return key.ToString();
            }
            else
            {
                return "HashKey_" + key.GetHashCode();
            }
        }

        #region Other collections stuff
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys { get; }
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { get; }
        ICollection IDictionary.Values { get; }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        
        object ICollection.SyncRoot
        {
            get { return this; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        bool IDictionary.IsFixedSize { get; }

        ICollection IDictionary.Keys { get; }
        
        bool IDictionary.Contains(object key)
        {
            throw new NotImplementedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void IDictionary.Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        void IDictionary.Remove(object key)
        {
            throw new NotImplementedException();
        }
      
        #endregion
    }
}
