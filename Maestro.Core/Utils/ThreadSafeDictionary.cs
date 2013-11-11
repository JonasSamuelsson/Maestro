using System;
using System.Collections;
using System.Collections.Generic;

namespace Maestro.Utils
{
	class ThreadSafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerable<TKey> Keys
		{
			get { return _dictionary.Keys; }
		}

		public void Add(TKey key, TValue value)
		{
			lock (this)
				_dictionary = new Dictionary<TKey, TValue>(_dictionary) { { key, value } };
		}

		public TValue Get(TKey key)
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

		public bool TryGet(TKey key, out TValue value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public void Clear()
		{
			lock (this)
				_dictionary = new Dictionary<TKey, TValue>();
		}
	}
}