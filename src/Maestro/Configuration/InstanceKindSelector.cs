using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.Configuration
{
	internal class InstanceKindSelector<TService> : IInstanceKindSelector, IInstanceKindSelector<TService>
	{
		public InstanceKindSelector(Type serviceType, string name, Kernel kernel, DefaultSettings defaultSettings, bool throwIfDuplicate)
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

		public IFactoryInstanceConfigurator<object> Factory(Func<object> factory)
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceConfigurator<object> Factory(Func<IContext, object> factory)
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(factory));
			return Kernel.Add(plugin, ThrowIfDuplicate)
				? new FactoryInstanceConfigurator<object>(plugin)
				: null;
		}

		public ITypeInstanceConfigurator<object> Type(Type type)
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(type, Name));
			return Kernel.Add(plugin, ThrowIfDuplicate)
				? new TypeInstanceConfigurator<object>(plugin)
				: null;
		}

		public ITypeInstanceConfigurator<object> Self()
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
				Lifetime = DefaultSettings.LifetimeFactory()
			};
		}

		public void Instance<TInstance>(TInstance instance) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin, ThrowIfDuplicate);
		}

		public IFactoryInstanceConfigurator<TInstance> Factory<TInstance>(Func<TInstance> factory) where TInstance : TService
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceConfigurator<TInstance> Factory<TInstance>(Func<IContext, TInstance> factory) where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(ctx => factory(ctx)));
			return Kernel.Add(plugin, ThrowIfDuplicate)
				? new FactoryInstanceConfigurator<TInstance>(plugin)
				: null;
		}

		public ITypeInstanceConfigurator<TInstance> Type<TInstance>() where TInstance : TService
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(typeof(TInstance), Name));
			return Kernel.Add(plugin, ThrowIfDuplicate)
				? new TypeInstanceConfigurator<TInstance>(plugin)
				: null;
		}

		ITypeInstanceConfigurator<TService> IInstanceKindSelector<TService>.Self()
		{
			return Type<TService>();
		}
	}
}