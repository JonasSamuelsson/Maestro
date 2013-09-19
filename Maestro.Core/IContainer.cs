using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface IContainer
	{
		void Configure(Action<IContainerConfiguration> action);

		object Get(Type type, string name = null);
		T Get<T>(string name = null);
		IEnumerable<object> GetAll(Type type);
		IEnumerable<T> GetAll<T>();

		string PrintConfiguration();
	}
}