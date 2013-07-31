using System;

namespace Maestro
{
	public interface IContainer
	{
		void Configure(Action<IContainerConfiguration> action);
	}
}