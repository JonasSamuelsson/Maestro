using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface IContext
	{
		int ConfigId { get; }
		long RequestId { get; }
		string Name { get; }
		ITypeStack TypeStack { get; }
		Guid ContainerId { get; }

		bool CanGet(Type type);
		bool CanGet<T>();
		object Get(Type type);
		T Get<T>();
		IEnumerable<object> GetAll(Type type);
		IEnumerable<T> GetAll<T>(); 

		event Action Disposed;
		event Action<Guid> ContainerDisposed;
	}
}