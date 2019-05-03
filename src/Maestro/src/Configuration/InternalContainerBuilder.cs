using Maestro.Internals;
using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	internal class InternalContainerBuilder : IContainerBuilder
	{
		private readonly Container _container;

		internal InternalContainerBuilder(Container container)
		{
			_container = container;
		}

		public ICollection<Func<Type, bool>> AutoResolveFilters => _container.Kernel.AutoResolveFilters;
		public ICollection<IInstanceTypeProvider> InstanceTypeProviders => _container.Kernel.InstanceTypeProviders;

		public IServiceBuilder Add(Type type) => Add<object>(type);

		public IServiceBuilder<TService> Add<TService>() => Add<TService>(typeof(TService));

		public IServiceBuilder AddOrThrow(Type type) => TryAdd(type) ?? throw CreateDuplicateServiceRegistrationException(type);

		public IServiceBuilder<TService> AddOrThrow<TService>() => TryAdd<TService>() ?? throw CreateDuplicateServiceRegistrationException(typeof(TService));

		public IServiceBuilder TryAdd(Type type) => TryAdd<object>(type);

		public IServiceBuilder<TService> TryAdd<TService>() => TryAdd<TService>(typeof(TService));

		public void Scan(Action<IScanner> action)
		{
			new Scanner(this).Execute(action);
		}

		private ServiceBuilder<T> Add<T>(Type type)
		{
			return new ServiceBuilder<T>(type, Add);
		}

		private void Add(ServiceDescriptor serviceDescriptor)
		{
			_container.Kernel.ServiceRegistry.Add(serviceDescriptor);
		}

		private ServiceBuilder<T> TryAdd<T>(Type type)
		{
			return _container.Kernel.ServiceRegistry.Contains(type) ? null : Add<T>(type);
		}

		private static DuplicateServiceRegistrationException CreateDuplicateServiceRegistrationException(Type type)
		{
			var message = $"Service of type '{type.ToFormattedString()}' has already been registered.";
			return new DuplicateServiceRegistrationException(message);
		}
	}
}