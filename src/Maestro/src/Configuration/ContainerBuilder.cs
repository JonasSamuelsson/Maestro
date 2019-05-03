using Maestro.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Configuration
{
	public class ContainerBuilder : IContainerBuilder
	{
		private readonly List<ServiceDescriptor> _serviceDescriptors = new List<ServiceDescriptor>();

		public ICollection<Func<Type, bool>> AutoResolveFilters { get; } = new List<Func<Type, bool>>();
		public ICollection<IInstanceTypeProvider> InstanceTypeProviders { get; } = new List<IInstanceTypeProvider>();

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
			_serviceDescriptors.Add(serviceDescriptor);
		}

		private ServiceBuilder<T> TryAdd<T>(Type type)
		{
			return _serviceDescriptors.Any(x => x.Type == type) ? null : Add<T>(type);
		}

		private static DuplicateServiceRegistrationException CreateDuplicateServiceRegistrationException(Type type)
		{
			var message = $"Service of type '{type.ToFormattedString()}' has already been registered.";
			return new DuplicateServiceRegistrationException(message);
		}

		public IContainer BuildContainer()
		{
			var container = new Container();
			AutoResolveFilters.ForEach(x => container.Kernel.AutoResolveFilters.Add(x));
			InstanceTypeProviders.ForEach(x => container.Kernel.InstanceTypeProviders.Add(x));
			_serviceDescriptors.ForEach(x => container.Kernel.ServiceRegistry.Add(x));
			return container;
		}
	}
}