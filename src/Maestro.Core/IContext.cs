using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface IContext
	{
		event Action<IContext> Disposed;

		bool CanGetService<T>();
		bool CanGetService(Type type);

		T GetService<T>();
		object GetService(Type type);

		bool TryGetService<T>(out T instance);
		bool TryGetService(Type type, out object instance);

		IEnumerable<T> GetServices<T>();
		IEnumerable<object> GetServices(Type type);
	}
}