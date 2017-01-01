using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.Configuration
{
	internal class ServiceInstanceExpression<TService> : IServiceInstanceExpression, IServiceInstanceExpression<TService>
	{
		public ServiceInstanceExpression(Type serviceType, string name, Kernel kernel, DefaultSettings defaultSettings, bool throwIfDuplicate)
		{
			ServiceType = serviceType;
			Name = name;
			Kernel = kernel;
			DefaultSettings = defaultSettings;
			ThrowIfDuplicate = throwIfDuplicate;
		}

		internal Type ServiceType { get; }
		internal string Name { get; set; }
		internal Kernel Kernel { get; }
		internal DefaultSettings DefaultSettings { get; set; }
		public bool ThrowIfDuplicate { get; set; }

		public void Instance(object instance)
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin, ThrowIfDuplicate);
		}

		public IFactoryInstanceExpression<object> Factory(Func<object> factory)
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceExpression<object> Factory(Func<IContext, object> factory)
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(factory));
			return Kernel.Add(plugin, ThrowIfDuplicate)
				? new FactoryInstanceExpression<object>(plugin)
				: null;
		}

		public ITypeInstanceExpression<object> Type(Type type)
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(type));
			return Kernel.Add(plugin, ThrowIfDuplicate)
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
				Lifetime = DefaultSettings.GetLifetime()
			};
		}

		public void Instance<TInstance>(TInstance instance) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin, ThrowIfDuplicate);
		}

		public IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<TInstance> factory) where TInstance : TService
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<IContext, TInstance> factory) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(ctx => factory(ctx)));
			return Kernel.Add(plugin, ThrowIfDuplicate)
				? new FactoryInstanceExpression<TInstance>(plugin)
				: null;
		}

		public ITypeInstanceExpression<TInstance> Type<TInstance>() where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(typeof(TInstance)));
			return Kernel.Add(plugin, ThrowIfDuplicate)
				? new TypeInstanceExpression<TInstance>(plugin)
				: null;
		}

		ITypeInstanceExpression<TService> IServiceInstanceExpression<TService>.Self()
		{
			return Type<TService>();
		}
	}
}