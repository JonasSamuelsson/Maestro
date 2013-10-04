using System;
using System.Collections.Generic;

namespace Maestro
{
	internal interface IThreadSafeDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		IEnumerable<TKey> Keys { get; }
 
		void Add(TKey key, TValue value);
		TValue Get(TKey key);
		TValue GetOrAdd(TKey key, Func<TKey, TValue> factory);
		bool TryGet(TKey key, out TValue value);
		void Clear();
	}
}