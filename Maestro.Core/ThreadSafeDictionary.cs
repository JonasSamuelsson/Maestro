using System;
using System.Collections;
using System.Collections.Generic;

namespace Maestro
{
	class ThreadSafeDictionary<T> : IThreadSafeDictionary<T>
	{
		private Dictionary<Type, T> _dictionary;
		private readonly Func<Type, T> _factory;

		public ThreadSafeDictionary(Func<Type, T> factory)
		{
			_dictionary = new Dictionary<Type, T>();
			_factory = factory;
		}

		public IEnumerator<KeyValuePair<Type, T>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T GetOrAdd(Type type)
		{
			T value;
			if (_dictionary.TryGetValue(type, out value)) return value;
			lock (this)
				if (!_dictionary.TryGetValue(type, out value))
				{
					value = _factory(type);
					_dictionary = new Dictionary<Type, T>(_dictionary) { { type, value } };
				}
			return value;
		}

		public bool TryGet(Type type, out T value)
		{
			return _dictionary.TryGetValue(type, out value);
		}

		public void Clear()
		{
			lock (this) _dictionary = new Dictionary<Type, T>();
		}
	}
}