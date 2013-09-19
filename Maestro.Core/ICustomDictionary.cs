using System;
using System.Collections.Generic;

namespace Maestro
{
	internal interface ICustomDictionary<T> : IEnumerable<KeyValuePair<Type, T>>
	{
		T GetOrAdd(Type type);
		bool TryGet(Type type, out T pipelineEngine);
		void Clear();
	}
}