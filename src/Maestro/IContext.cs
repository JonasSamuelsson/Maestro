using System;
using System.Collections.Generic;
using Maestro.Internals;

namespace Maestro
{
	public interface IContext
	{
		event Action<IContext> Disposed;

		bool CanGetService<T>(string name = ServiceNames.Default);
		bool CanGetService(Type type, string name = ServiceNames.Default);

		T GetService<T>(string name = null);
		object GetService(Type type, string name = ServiceNames.Default);

		bool TryGetService<T>(out T instance);
		bool TryGetService<T>(string name, out T instance);
		bool TryGetService(Type type, out object instance);
		bool TryGetService(Type type, string name, out object instance);

		IEnumerable<T> GetServices<T>();
		IEnumerable<object> GetServices(Type type);
	}
}