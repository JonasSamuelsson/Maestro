using System;
using System.Collections.Generic;

namespace Maestro
{
	internal interface IThreadSafeDictionary<T> : IEnumerable<KeyValuePair<Type, T>>
	{
		T GetOrAdd(Type type);
		bool TryGet(Type type, out T value);
		void Clear();
	}
}