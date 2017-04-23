using System;
using Microsoft.Extensions.DependencyInjection;

namespace Maestro.Microsoft.DependencyInjection
{
	public class MaestroServiceProvider : IServiceProvider, ISupportRequiredService
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

		public object GetRequiredService(Type serviceType)
		{
			return _container.GetService(serviceType);
		}
	}
}