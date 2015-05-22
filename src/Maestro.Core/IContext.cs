using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface IContext
	{
		bool CanGet<T>();
		bool CanGet(Type type);

		T Get<T>();
		object Get(Type type);

		bool TryGet<T>(out T instance);
		bool TryGet(Type type, out object instance);

		IEnumerable<T> GetAll<T>();
		IEnumerable<object> GetAll(Type type);
	}
}