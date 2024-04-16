
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tools
{
    public class BijectiveDictionary<TKey, TValue> : IBijectiveDictionary<TKey, TValue>, IReadOnlyBijectiveDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _forward = new();
        private readonly Dictionary<TValue, TKey> _reverse = new();

        public int Count => _forward.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (_forward.ContainsKey(key) || _reverse.ContainsKey(value))
            {
                throw new ArgumentException("Duplicate key or value.");
            }

            _forward[key] = value;
            _reverse[value] = key;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _forward.TryGetValue(item.Key, out TValue value) &&
                EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        public bool RemoveByKey(TKey key)
        {
            if (_forward.Remove(key, out var value))
            {
                _reverse.Remove(value);
                return true;
            }

            return false;
        }

        public bool RemoveByValue(TValue value)
        {
            if (_reverse.Remove(value, out var key))
            {
                _forward.Remove(key);
                return true;
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return _forward.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _reverse.ContainsKey(value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < _forward.Count)
            {
                throw new ArgumentException(
                    "The number of elements in the source dictionary is greater than the available space from arrayIndex to the end of the destination array.");
            }

            foreach (var pair in _forward)
            {
                array[arrayIndex++] = pair;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }
        
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                return _forward.Remove(item.Key) && _reverse.Remove(item.Value);
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _forward.TryGetValue(key, out value);
        }

        public bool TryGetKey(TValue value, out TKey key)
        {
            return _reverse.TryGetValue(value, out key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TValue this[TKey key]
        {
            get => _forward[key];
            set
            {
                if (_forward.TryGetValue(key, out var oldValue))
                {
                    _reverse.Remove(oldValue);
                }

                if (_reverse.ContainsKey(value) && !EqualityComparer<TKey>.Default.Equals(_reverse[value], key))
                {
                    throw new ArgumentException("The value is already associated with another key.");
                }

                _forward[key] = value;
                _reverse[value] = key;
            }
        }

        public TKey this[TValue valueKey]
        {
            get => _reverse[valueKey];
            set
            {
                if (_reverse.TryGetValue(valueKey, out var oldKey))
                {
                    _forward.Remove(oldKey);
                }

                if (_forward.ContainsKey(value) && !EqualityComparer<TValue>.Default.Equals(_forward[value], valueKey))
                {
                    throw new ArgumentException("The key is already associated with another valueKey.");
                }

                _reverse[valueKey] = value;
                _forward[value] = valueKey;
            }
        }
    }
}
