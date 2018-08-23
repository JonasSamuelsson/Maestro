using Maestro.FactoryProviders;
using Maestro.Internals;
using Maestro.Lifetimes;
using System;

namespace Maestro.Configuration
{
	internal class ServiceBuilder<TService> : IServiceBuilder, IServiceBuilder<TService>
	{
		private readonly Type _serviceType;
		private readonly Action<ServiceDescriptor> _addService;
		private string _name;

		public ServiceBuilder(Type serviceType, Action<ServiceDescriptor> addService)
		{
			_serviceType = serviceType;
			_addService = addService;
			_name = ServiceNames.Default;
		}

		IServiceBuilder IServiceBuilder.Named(string name)
		{
			_name = name;
			return this;
		}

		public void Instance(object instance)
		{
			var serviceDescriptor = CreateServiceDescriptor(new InstanceFactoryProvider(instance));
			_addService(serviceDescriptor);
		}

		public IFactoryInstanceBuilder<object> Factory(Func<object> factory)
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceBuilder<object> Factory(Func<Context, object> factory)
		{
			var serviceDescriptor = CreateServiceDescriptor(new FuncFactoryProvider(factory));
			_addService(serviceDescriptor);
			return new FactoryInstanceBuilder<object>(serviceDescriptor);
		}

		public ITypeInstanceBuilder<object> Type(Type type)
		{
			var serviceDescriptor = CreateServiceDescriptor(new TypeFactoryProvider(type, _name));
			_addService(serviceDescriptor);
			return new TypeInstanceBuilder<object>(serviceDescriptor);
		}

		public ITypeInstanceBuilder<object> Self()
		{
			return Type(_serviceType);
		}

		IServiceBuilder<TService> IServiceBuilder<TService>.Named(string name)
		{
			_name = name;
			return this;
		}

		public void Instance<TInstance>(TInstance instance) where TInstance : TService
		{
			var serviceDescriptor = CreateServiceDescriptor(new InstanceFactoryProvider(instance));
			_addService(serviceDescriptor);
		}

		public IFactoryInstanceBuilder<TInstance> Factory<TInstance>(Func<TInstance> factory) where TInstance : TService
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceBuilder<TInstance> Factory<TInstance>(Func<Context, TInstance> factory) where TInstance : TService
		{
			var serviceDescriptor = CreateServiceDescriptor(new FuncFactoryProvider(ctx => factory(ctx)));
			_addService(serviceDescriptor);
			return new FactoryInstanceBuilder<TInstance>(serviceDescriptor);
		}

		public ITypeInstanceBuilder<TInstance> Type<TInstance>() where TInstance : TService
		{
			var serviceDescriptor = CreateServiceDescriptor(new TypeFactoryProvider(typeof(TInstance), _name));
			_addService(serviceDescriptor);
			return new TypeInstanceBuilder<TInstance>(serviceDescriptor);
		}

		ITypeInstanceBuilder<TService> IServiceBuilder<TService>.Self()
		{
			return Type<TService>();
		}

		private ServiceDescriptor CreateServiceDescriptor(IFactoryProvider factoryProvider)
		{
			return new ServiceDescriptor
			{
				Name = _name,
				Type = _serviceType,
				FactoryProvider = factoryProvider,
				Lifetime = TransientLifetime.Instance
			};
		}
	}
}