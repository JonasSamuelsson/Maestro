using System;

namespace Maestro.Microsoft.DependencyInjection
{
	internal class MaestroServiceProvider : IServiceProvider
	{
		private readonly IContainer _container;

		public MaestroServiceProvider(IContainer container)
		{
			_container = container;
		}

		public object GetService(Type serviceType)
		{
			object instance;
			return _container.TryGetService(serviceType, out instance)
				? instance
				: null;
		}
	}
}