using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface IContext
	{
		int ConfigVersion { get; }
		long ContextId { get; }
		string Name { get; }
		ITypeStack TypeStack { get; }

		bool CanGet(Type type);
		bool CanGet<T>();
		object Get(Type type);
		T Get<T>();
		IEnumerable<object> GetAll(Type type);
		IEnumerable<T> GetAll<T>();

		event Action Disposed;
	}
}