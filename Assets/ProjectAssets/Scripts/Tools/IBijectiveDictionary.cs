using System.Collections.Generic;

namespace Tools
{
	public interface IBijectiveDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
	{
		TValue this[TKey key] { get; set; }
		TKey this[TValue val] { get; set; }
		bool RemoveByKey(TKey key);
		bool RemoveByValue(TValue value);
		bool ContainsKey(TKey key);
		bool ContainsValue(TValue value);
		bool TryGetValue(TKey key, out TValue value);
		bool TryGetKey(TValue value, out TKey key);
	}

	public interface IReadOnlyBijectiveDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	{
		TValue this[TKey key] { get; }
		TKey this[TValue value] { get; }
		bool ContainsKey(TKey key);
		bool ContainsValue(TValue value);
		bool TryGetValue(TKey key, out TValue value);
		bool TryGetKey(TValue value, out TKey key);
	}
}