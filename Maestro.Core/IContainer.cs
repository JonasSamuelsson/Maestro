using System;

namespace Maestro
{
	public interface IContainer
	{
		void Configure(Action<IContainerConfiguration> action);

		object Get(Type type, string name = null);
		T Get<T>(string name = null);
	}
}