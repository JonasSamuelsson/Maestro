using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Utils
{
	class ThreadSafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private Dictionary<TKey, TValue> _dictionary;

		public ThreadSafeDictionary() : this(Enumerable.Empty<KeyValuePair<TKey, TValue>>()) { }

		public ThreadSafeDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		{
			_dictionary = dictionary.ToDictionary(x => x.Key, x => x.Value);
		}

		public int Count => _dictionary.Count;
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
			lock (this)
				_dictionary = new Dictionary<TKey, TValue>(_dictionary) { { key, value } };
		}

		public TValue GetValue(TKey key)
		{
			return _dictionary[key];
		}

		public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
		{
			TValue value;
			if (_dictionary.TryGetValue(key, out value)) return value;
			lock (this)
				if (!_dictionary.TryGetValue(key, out value))
				{
					value = factory(key);
					_dictionary = new Dictionary<TKey, TValue>(_dictionary) { { key, value } };
				}
			return value;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public void Clear()
		{
			lock (this)
				_dictionary = new Dictionary<TKey, TValue>();
		}

		public void Remove(TKey key)
		{
			lock (this)
			{
				if (!_dictionary.ContainsKey(key)) return;
				var dictionary = new Dictionary<TKey, TValue>(_dictionary);
				dictionary.Remove(key);
				_dictionary = dictionary;
			}
		}

		public void Set(TKey key, TValue value)
		{
			lock (this)
			{
				_dictionary = new Dictionary<TKey, TValue>(_dictionary) { [key] = value };
			}
		}
	}
}