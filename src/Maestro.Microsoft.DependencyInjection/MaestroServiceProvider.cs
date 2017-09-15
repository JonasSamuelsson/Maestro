using Microsoft.Extensions.DependencyInjection;
using System;

namespace Maestro.Microsoft.DependencyInjection
{
	public sealed class MaestroServiceProvider : IServiceProvider, ISupportRequiredService
	{
		private readonly IContainer _container;

		public MaestroServiceProvider(IContainer container)
		{
			_container = container ?? throw new ArgumentNullException(nameof(container));
		}

		public object GetService(Type serviceType)
		{
			// ReSharper disable once ConditionalTernaryEqualBranch
			return _container.TryGetService(serviceType, out var instance) ? instance : instance;
		}

		public object GetRequiredService(Type serviceType)
		{
			return _container.GetService(serviceType);
		}
	}
}