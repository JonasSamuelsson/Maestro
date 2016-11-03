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
			Use(instance, true);
		}

		public IFactoryInstanceExpression<object> Use(Func<object> factory)
		{
			return Use(factory, true);
		}

		public IFactoryInstanceExpression<object> Use(Func<IContext, object> factory)
		{
			return Use(factory, true);
		}

		public ITypeInstanceExpression<object> Use(Type type)
		{
			return Use(type, true);
		}

		public void TryUse(object instance)
		{
			Use(instance, false);
		}

		public IFactoryInstanceExpression<object> TryUse(Func<object> factory)
		{
			return Use(factory, false);
		}

		public IFactoryInstanceExpression<object> TryUse(Func<IContext, object> factory)
		{
			return Use(factory, false);
		}

		public ITypeInstanceExpression<object> TryUse(Type type)
		{
			return Use(type, false);
		}

		private void Use(object instance, bool throwIfDuplicate)
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin, throwIfDuplicate);
		}

		private IFactoryInstanceExpression<object> Use(Func<object> factory, bool throwIfDuplicate)
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(_ => factory()));
			return Kernel.Add(plugin, throwIfDuplicate)
				? new FactoryInstanceExpression<object>(plugin)
				: null;
		}

		private IFactoryInstanceExpression<object> Use(Func<IContext, object> factory, bool throwIfDuplicate)
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(factory));
			return Kernel.Add(plugin, throwIfDuplicate)
				? new FactoryInstanceExpression<object>(plugin)
				: null;
		}

		private ITypeInstanceExpression<object> Use(Type type, bool throwIfDuplicate)
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(type));
			return Kernel.Add(plugin, throwIfDuplicate)
				? new TypeInstanceExpression<object>(plugin)
				: null;
		}

		public void Add(object instance)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new InstanceFactoryProvider(instance));
			Kernel.Add(plugin, true);
		}

		public IFactoryInstanceExpression<object> Add(Func<object> factory)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new LambdaFactoryProvider(_ => factory()));
			Kernel.Add(plugin, true);
			return new FactoryInstanceExpression<object>(plugin);
		}

		public IFactoryInstanceExpression<object> Add(Func<IContext, object> factory)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new LambdaFactoryProvider(factory));
			Kernel.Add(plugin, true);
			return new FactoryInstanceExpression<object>(plugin);
		}

		public ITypeInstanceExpression<object> Add(Type type)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new TypeFactoryProvider(type));
			Kernel.Add(plugin, true);
			return new TypeInstanceExpression<object>(plugin);
		}

		public void Use(T instance)
		{
			Use(instance, true);
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<TInstance> factory)
			 where TInstance : T
		{
			return Use(factory, true);
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> factory)
			where TInstance : T
		{
			return Use(factory, true);
		}

		public ITypeInstanceExpression<TInstance> Use<TInstance>()
			 where TInstance : T
		{
			return Use<TInstance>(true);
		}

		public void TryUse(T instance)
		{
			Use(instance, false);
		}

		public IFactoryInstanceExpression<TInstance> TryUse<TInstance>(Func<TInstance> factory) where TInstance : T
		{
			return Use(factory, false);
		}

		public IFactoryInstanceExpression<TInstance> TryUse<TInstance>(Func<IContext, TInstance> factory) where TInstance : T
		{
			return Use(factory, false);
		}

		public ITypeInstanceExpression<TInstance> TryUse<TInstance>() where TInstance : T
		{
			return Use<TInstance>(false);
		}

		public void Use(T instance, bool throwIfDuplicate)
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin, throwIfDuplicate);
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<TInstance> factory, bool throwIfDuplicate)
			 where TInstance : T
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(_ => factory()));
			return Kernel.Add(plugin, throwIfDuplicate)
				? new FactoryInstanceExpression<TInstance>(plugin)
				: null;
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> factory, bool throwIfDuplicate)
			where TInstance : T
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(ctx => factory(ctx)));
			return Kernel.Add(plugin, throwIfDuplicate)
				? new FactoryInstanceExpression<TInstance>(plugin)
				: null;
		}

		public ITypeInstanceExpression<TInstance> Use<TInstance>(bool throwIfDuplicate)
			 where TInstance : T
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(typeof(TInstance)));
			return Kernel.Add(plugin, throwIfDuplicate)
				? new TypeInstanceExpression<TInstance>(plugin)
				: null;
		}

		public void Add(T instance)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new InstanceFactoryProvider(instance));
			Kernel.Add(plugin, true);
		}

		public IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<TInstance> factory)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new LambdaFactoryProvider(_ => factory()));
			Kernel.Add(plugin, true);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<IContext, TInstance> factory)
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new LambdaFactoryProvider(ctx => factory(ctx)));
			Kernel.Add(plugin, true);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public ITypeInstanceExpression<TInstance> Add<TInstance>()
		{
			var plugin = CreatePlugin(PluginLookup.GetRandomName(), new TypeFactoryProvider(typeof(TInstance)));
			Kernel.Add(plugin, true);
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