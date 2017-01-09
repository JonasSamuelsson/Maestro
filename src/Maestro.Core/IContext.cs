using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface IContext
	{
		event Action<IContext> Disposed;

		bool CanGetService<T>(string name = null);
		bool CanGetService(Type type, string name = null);

		T GetService<T>(string name = null);
		object GetService(Type type, string name = null);

		bool TryGetService<T>(out T instance);
		bool TryGetService<T>(string name, out T instance);
		bool TryGetService(Type type, out object instance);
		bool TryGetService(Type type, string name, out object instance);

		IEnumerable<T> GetServices<T>();
		IEnumerable<object> GetServices(Type type);
	}
}