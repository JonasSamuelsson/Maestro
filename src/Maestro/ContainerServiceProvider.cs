using System;

namespace Maestro
{
	internal class ContainerServiceProvider : IServiceProvider
	{
		private readonly IScopedContainer _container;

		public ContainerServiceProvider(IScopedContainer container)
		{
			_container = container;
		}

		public object GetService(Type serviceType)
		{
			// ReSharper disable once ConditionalTernaryEqualBranch
			return _container.TryGetService(serviceType, out var instance) ? instance : instance;
		}
	}
}