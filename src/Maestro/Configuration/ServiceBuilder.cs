using Maestro.FactoryProviders;
using Maestro.Internals;
using Maestro.Lifetimes;
using System;

namespace Maestro.Configuration
{
	internal class ServiceBuilder<TService> : IServiceBuilder, IServiceBuilder<TService>
	{
		public ServiceBuilder(Type serviceType, string name, Kernel kernel, ServiceRegistrationPolicy serviceRegistrationPolicy)
		{
			ServiceType = serviceType;
			Name = name;
			Kernel = kernel;
			ServiceRegistrationPolicy = serviceRegistrationPolicy;
		}

		internal Type ServiceType { get; }
		internal string Name { get; private set; }
		internal Kernel Kernel { get; }
		public ServiceRegistrationPolicy ServiceRegistrationPolicy { get; set; }

		IServiceBuilder IServiceBuilder.Named(string name)
		{
			Name = name;
			return this;
		}

		public void Instance(object instance)
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.ServiceDescriptors.Add(plugin, ServiceRegistrationPolicy);
		}

		public IFactoryInstanceBuilder<object> Factory(Func<object> factory)
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceBuilder<object> Factory(Func<Context, object> factory)
		{
			var plugin = CreatePlugin(Name, new FuncFactoryProvider(factory));
			return Kernel.ServiceDescriptors.Add(plugin, ServiceRegistrationPolicy)
				? new FactoryInstanceBuilder<object>(plugin)
				: null;
		}

		public ITypeInstanceBuilder<object> Type(Type type)
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(type, Name));
			return Kernel.ServiceDescriptors.Add(plugin, ServiceRegistrationPolicy)
				? new TypeInstanceBuilder<object>(plugin)
				: null;
		}

		public ITypeInstanceBuilder<object> Self()
		{
			return Type(ServiceType);
		}

		private ServiceDescriptor CreatePlugin(string name, IFactoryProvider factoryProvider)
		{
			return new ServiceDescriptor
			{
				Name = name,
				Type = ServiceType,
				FactoryProvider = factoryProvider,
				Lifetime = TransientLifetime.Instance
			};
		}

		IServiceBuilder<TService> IServiceBuilder<TService>.Named(string name)
		{
			Name = name;
			return this;
		}

		public void Instance<TInstance>(TInstance instance) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.ServiceDescriptors.Add(plugin, ServiceRegistrationPolicy);
		}

		public IFactoryInstanceBuilder<TInstance> Factory<TInstance>(Func<TInstance> factory) where TInstance : TService
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceBuilder<TInstance> Factory<TInstance>(Func<Context, TInstance> factory) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new FuncFactoryProvider(ctx => factory(ctx)));
			return Kernel.ServiceDescriptors.Add(plugin, ServiceRegistrationPolicy)
				? new FactoryInstanceBuilder<TInstance>(plugin)
				: null;
		}

		public ITypeInstanceBuilder<TInstance> Type<TInstance>() where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(typeof(TInstance), Name));
			return Kernel.ServiceDescriptors.Add(plugin, ServiceRegistrationPolicy)
				? new TypeInstanceBuilder<TInstance>(plugin)
				: null;
		}

		ITypeInstanceBuilder<TService> IServiceBuilder<TService>.Self()
		{
			return Type<TService>();
		}
	}
}