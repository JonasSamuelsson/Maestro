using Maestro.FactoryProviders;
using Maestro.Internals;
using System;

namespace Maestro.Configuration
{
	internal class ServiceExpression<TService> : IServiceExpression, IServiceExpression<TService>
	{
		public ServiceExpression(Type serviceType, string name, Kernel kernel, bool throwIfDuplicate)
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

		public IFactoryInstanceExpression<object> Factory(Func<object> factory)
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceExpression<object> Factory(Func<Context, object> factory)
		{
			var plugin = CreatePlugin(Name, new FuncFactoryProvider(factory));
			return Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate)
				? new FactoryInstanceExpression<object>(plugin)
				: null;
		}

		public ITypeInstanceExpression<object> Type(Type type)
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(type, Name));
			return Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate)
				? new TypeInstanceExpression<object>(plugin)
				: null;
		}

		public ITypeInstanceExpression<object> Self()
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
				Lifetime = Kernel.Settings.LifetimeFactory()
			};
		}

		public void Instance<TInstance>(TInstance instance) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate);
		}

		public IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<TInstance> factory) where TInstance : TService
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<Context, TInstance> factory) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new FuncFactoryProvider(ctx => factory(ctx)));
			return Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate)
				? new FactoryInstanceExpression<TInstance>(plugin)
				: null;
		}

		public ITypeInstanceExpression<TInstance> Type<TInstance>() where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(typeof(TInstance), Name));
			return Kernel.ServiceDescriptors.Add(plugin, ThrowIfDuplicate)
				? new TypeInstanceExpression<TInstance>(plugin)
				: null;
		}

		ITypeInstanceExpression<TService> IServiceExpression<TService>.Self()
		{
			return Type<TService>();
		}
	}
}