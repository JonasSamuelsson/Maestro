using Maestro.FactoryProviders;
using Maestro.Internals;
using System;
using Maestro.Lifetimes;

namespace Maestro.Configuration
{
	internal class ServiceBuilder<TService> : IServiceBuilder, IServiceBuilder<TService>
	{
		public ServiceBuilder(Type serviceType, string name, Kernel kernel, bool throwIfDuplicate)
		{
			ServiceType = serviceType;
			Name = name;
			Kernel = kernel;
			ThrowIfDuplicate = throwIfDuplicate;
		}

		internal Type ServiceType { get; }
		internal string Name { get; set; }
		internal Kernel Kernel { get; }
		public bool ThrowIfDuplicate { get; set; }

		public void Instance(object instance)
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate);
		}

		public IFactoryInstanceBuilder<object> Factory(Func<object> factory)
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceBuilder<object> Factory(Func<Context, object> factory)
		{
			var plugin = CreatePlugin(Name, new FuncFactoryProvider(factory));
			return Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate)
				? new FactoryInstanceBuilder<object>(plugin)
				: null;
		}

		public ITypeInstanceBuilder<object> Type(Type type)
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(type, Name));
			return Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate)
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

		public void Instance<TInstance>(TInstance instance) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate);
		}

		public IFactoryInstanceBuilder<TInstance> Factory<TInstance>(Func<TInstance> factory) where TInstance : TService
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceBuilder<TInstance> Factory<TInstance>(Func<Context, TInstance> factory) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new FuncFactoryProvider(ctx => factory(ctx)));
			return Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate)
				? new FactoryInstanceBuilder<TInstance>(plugin)
				: null;
		}

		public ITypeInstanceBuilder<TInstance> Type<TInstance>() where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(typeof(TInstance), Name));
			return Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate)
				? new TypeInstanceBuilder<TInstance>(plugin)
				: null;
		}

		ITypeInstanceBuilder<TService> IServiceBuilder<TService>.Self()
		{
			return Type<TService>();
		}
	}
}