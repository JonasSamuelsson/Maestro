using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Utils
{
	class ThreadSafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private readonly ConcurrentDictionary<TKey, TValue> _dictionary;

		public ThreadSafeDictionary() : this(Enumerable.Empty<KeyValuePair<TKey, TValue>>()) { }

		public ThreadSafeDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		{
			_dictionary = new ConcurrentDictionary<TKey, TValue>(dictionary);
		}

		public int Count => _dictionary.Count;
		public IEnumerable<TKey> Keys => _dictionary.Keys;
		public IEnumerable<TValue> Values => _dictionary.Values;

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(TKey key, TValue value)
		{
			if (_dictionary.TryAdd(key, value)) return;
			throw new InvalidOperationException($"Duplicate key '{key}'.");
		}

		public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
		{
			return _dictionary.GetOrAdd(key, factory);
		}

		public TValue GetOrAdd(TKey key, Func<TValue> factory)
		{
			return GetOrAdd(key, _ => factory());
		}

		public bool TryGet(TKey key, out TValue value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public void AddOrUpdate(TKey key, TValue value)
		{
			_dictionary.AddOrUpdate(key, value, (_, __) => value);
		}

		public bool TryAdd(TKey key, TValue value)
		{
			return _dictionary.TryAdd(key, value);
		}
	}
}