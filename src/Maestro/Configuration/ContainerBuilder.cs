using Maestro.Internals;
using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	internal class ContainerBuilder : IContainerBuilder
	{
		private readonly Container _container;

		internal ContainerBuilder(Container container)
		{
			_container = container;
		}

		public List<Func<Type, bool>> AutoResolveFilters => _container.Kernel.AutoResolveFilters;

		public IServiceBuilder Add(Type type) => Add(type, ServiceRegistrationPolicy.AddOrUpdate);

		public IServiceBuilder<TService> Add<TService>() => Add<TService>(typeof(TService), ServiceRegistrationPolicy.AddOrUpdate);

		public IServiceBuilder AddOrThrow(Type type) => Add(type, ServiceRegistrationPolicy.AddOrThrow);

		public IServiceBuilder<TService> AddOrThrow<TService>() => Add<TService>(typeof(TService), ServiceRegistrationPolicy.AddOrThrow);

		public IServiceBuilder TryAdd(Type type) => Add(type, ServiceRegistrationPolicy.TryAdd);

		public IServiceBuilder<TService> TryAdd<TService>() => Add<TService>(typeof(TService), ServiceRegistrationPolicy.TryAdd);

		private IServiceBuilder Add(Type type, ServiceRegistrationPolicy serviceRegistrationPolicy)
		{
			if (type == null) throw new ArgumentNullException();
			return new ServiceBuilder<object>(type, ServiceNames.Default, _container.Kernel, serviceRegistrationPolicy);
		}

		private IServiceBuilder<T> Add<T>(Type type, ServiceRegistrationPolicy serviceRegistrationPolicy)
		{
			if (type == null) throw new ArgumentNullException();
			return new ServiceBuilder<T>(type, ServiceNames.Default, _container.Kernel, serviceRegistrationPolicy);
		}

		public void Scan(Action<IScanner> action)
		{
			new Scanner(this).Execute(action);
		}
	}
}