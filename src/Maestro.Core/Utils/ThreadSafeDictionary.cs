using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Maestro.Utils
{
	class ThreadSafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private ImmutableDictionary<TKey, TValue> _dictionary;

		public ThreadSafeDictionary() : this(Enumerable.Empty<KeyValuePair<TKey, TValue>>()) { }

		public ThreadSafeDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		{
			_dictionary = dictionary.ToImmutableDictionary(x => x.Key, x => x.Value);
		}

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
		    {
		        _dictionary = _dictionary.Add(key, value);
		    }
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
					_dictionary = _dictionary.Add(key, value);
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
		    {
		        _dictionary = _dictionary.Clear();
		    }
		}

		public void Remove(TKey key)
		{
			lock (this)
			{
			    _dictionary = _dictionary.Remove(key);
			}
		}

		public void Set(TKey key, TValue value)
		{
			lock (this)
			{
			    _dictionary = _dictionary.SetItem(key, value);
			}
		}
	}
}