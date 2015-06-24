using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class PluginExpression<T> : IDefaultPluginExpression, IPluginExpression, IDefaultPluginExpression<T>, IPluginExpression<T>
	{
		public PluginExpression(Type type, string name, Kernel kernel, DefaultSettings defaultSettings)
		{
			Type = type;
			Name = name;
			Kernel = kernel;
			DefaultSettings = defaultSettings;
		}

		internal Type Type { get; }
		internal string Name { get; set; }
		internal Kernel Kernel { get; }
		internal DefaultSettings DefaultSettings { get; set; }

		public void Use(object instance)
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin);
		}

		public IFactoryInstanceExpression<object> Use(Func<object> factory)
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(_ => factory()));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<object>(plugin);
		}

		public IFactoryInstanceExpression<object> Use(Func<IContext, object> factory)
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(factory));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<object>(plugin);
		}

		public ITypeInstanceExpression<object> Use(Type type)
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(type));
			Kernel.Add(plugin);
			return new TypeInstanceExpression<object>(plugin);
		}

		public void Add(object instance)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new InstanceFactoryProvider(instance));
			Kernel.Add(plugin);
		}

		public IFactoryInstanceExpression<object> Add(Func<object> factory)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new LambdaFactoryProvider(_ => factory()));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<object>(plugin);
		}

		public IFactoryInstanceExpression<object> Add(Func<IContext, object> factory)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new LambdaFactoryProvider(factory));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<object>(plugin);
		}

		public ITypeInstanceExpression<object> Add(Type type)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new TypeFactoryProvider(type));
			Kernel.Add(plugin);
			return new TypeInstanceExpression<object>(plugin);
		}

		public void Use(T instance)
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin);
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<TInstance> factory)
			 where TInstance : T
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(_ => factory()));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> factory)
			where TInstance : T
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(ctx => factory(ctx)));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public ITypeInstanceExpression<TInstance> Use<TInstance>()
			 where TInstance : T
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(typeof(TInstance)));
			Kernel.Add(plugin);
			return new TypeInstanceExpression<TInstance>(plugin);
		}

		public void Add(T instance)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new InstanceFactoryProvider(instance));
			Kernel.Add(plugin);
		}

		public IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<TInstance> factory)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new LambdaFactoryProvider(_ => factory()));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<IContext, TInstance> factory)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new LambdaFactoryProvider(ctx => factory(ctx)));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public ITypeInstanceExpression<TInstance> Add<TInstance>()
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new TypeFactoryProvider(typeof(TInstance)));
			Kernel.Add(plugin);
			return new TypeInstanceExpression<TInstance>(plugin);
		}

		private Plugin CreatePlugin(string name, IFactoryProvider factoryProvider)
		{
			return new Plugin
			{
				Name = name,
				Type = Type,
				FactoryProvider = factoryProvider,
				Lifetime = DefaultSettings.GetLifetime()
			};
		}
	}
}