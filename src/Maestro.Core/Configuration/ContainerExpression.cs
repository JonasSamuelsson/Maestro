using System;
using System.Linq.Expressions;
using Maestro.FactoryProviders;
using Maestro.Interceptors;
using Maestro.Internals;
using Maestro.Utils;

namespace Maestro.Configuration
{
	internal class ContainerExpression : IContainerExpression
	{
		private readonly Kernel _kernel;
		private readonly DefaultSettings _defaultSettings;

		public ContainerExpression(Kernel kernel, DefaultSettings defaultSettings)
		{
			_kernel = kernel;
			_defaultSettings = defaultSettings;
		}

		public IDefaultPluginExpression For(Type type)
		{
			return new PluginExpression<object>(type, PluginLookup.DefaultName, _kernel, _defaultSettings);
		}

		public IPluginExpression<object> For(Type type, string name)
		{
			return new PluginExpression<object>(type, name, _kernel, _defaultSettings);
		}

		public IDefaultPluginExpression<T> For<T>()
		{
			return new PluginExpression<T>(typeof(T), PluginLookup.DefaultName, _kernel, _defaultSettings);
		}

		public IPluginExpression<T> For<T>(string name)
		{
			return new PluginExpression<T>(typeof(T), name, _kernel, _defaultSettings);
		}

		public IConventionExpression Scan
		{
			get { return new ConventionExpression(this, _defaultSettings); }
		}

		public IDefaultSettingsExpression Default
		{
			get { return _defaultSettings; }
		}
	}

	public interface IPluginExpression
	{
		void Use(object instance);
		IFactoryInstanceExpression<object> Use(Func<object> factory);
		IFactoryInstanceExpression<object> Use(Func<IContext, object> factory);
		ITypeInstanceExpression<object> Use(Type type);
	}

	public interface IDefaultPluginExpression : IPluginExpression
	{
		void Add(object instance);
		IFactoryInstanceExpression<object> Add(Func<object> factory);
		IFactoryInstanceExpression<object> Add(Func<IContext, object> factory);
		ITypeInstanceExpression<object> Add(Type type);
	}

	public interface IPluginExpression<T>
	{
		void Use(T instance);
		IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<TInstance> factory);
		IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> factory);
		ITypeInstanceExpression<TInstance> Use<TInstance>();
	}

	public interface IDefaultPluginExpression<T> : IPluginExpression<T>
	{
		void Add(T instance);
		IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<TInstance> factory);
		IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<IContext, TInstance> factory);
		ITypeInstanceExpression<TInstance> Add<TInstance>();
	}

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
			var plugin = CreatePlugin(PluginLookup.AnonymousName, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin);
		}

		public IFactoryInstanceExpression<object> Add(Func<object> factory)
		{
			var plugin = CreatePlugin(PluginLookup.AnonymousName, new LambdaFactoryProvider(_ => factory()));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<object>(plugin);
		}

		public IFactoryInstanceExpression<object> Add(Func<IContext, object> factory)
		{
			var plugin = CreatePlugin(PluginLookup.AnonymousName, new LambdaFactoryProvider(factory));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<object>(plugin);
		}

		public ITypeInstanceExpression<object> Add(Type type)
		{
			var plugin = CreatePlugin(PluginLookup.AnonymousName, new TypeFactoryProvider(type));
			Kernel.Add(plugin);
			return new TypeInstanceExpression<object>(plugin);
		}

		public void Use(T instance)
		{
			var plugin = CreatePlugin(Name, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin);
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<TInstance> factory)
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(_ => factory()));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public IFactoryInstanceExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> factory)
		{
			var plugin = CreatePlugin(Name, new LambdaFactoryProvider(ctx => factory(ctx)));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public ITypeInstanceExpression<TInstance> Use<TInstance>()
		{
			var plugin = CreatePlugin(Name, new TypeFactoryProvider(typeof(TInstance)));
			Kernel.Add(plugin);
			return new TypeInstanceExpression<TInstance>(plugin);
		}

		public void Add(T instance)
		{
			var plugin = CreatePlugin(PluginLookup.AnonymousName, new InstanceFactoryProvider(instance));
			Kernel.Add(plugin);
		}

		public IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<TInstance> factory)
		{
			var plugin = CreatePlugin(PluginLookup.AnonymousName, new LambdaFactoryProvider(_ => factory()));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public IFactoryInstanceExpression<TInstance> Add<TInstance>(Func<IContext, TInstance> factory)
		{
			var plugin = CreatePlugin(PluginLookup.AnonymousName, new LambdaFactoryProvider(ctx => factory(ctx)));
			Kernel.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public ITypeInstanceExpression<TInstance> Add<TInstance>()
		{
			var plugin = CreatePlugin(PluginLookup.AnonymousName, new TypeFactoryProvider(typeof(TInstance)));
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

	public interface IFactoryExpression<T>
	{
		void Instance(T instance);
		IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<TInstance> factory);
		IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<IContext, TInstance> factory);
		ITypeInstanceExpression<TInstance> Type<TInstance>();
	}

	class FactoryExpression<T> : IFactoryExpression<T>
	{
		private readonly Type _type;
		private readonly string _name;
		private readonly PluginLookup _plugins;

		public FactoryExpression(Type type, string name, PluginLookup plugins)
		{
			_type = type;
			_name = name;
			_plugins = plugins;
		}

		public void Instance(T instance)
		{
			_plugins.Add(new Plugin
			{
				Type = _type,
				Name = _name,
				FactoryProvider = new InstanceFactoryProvider(instance)
			});
		}

		public IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<TInstance> factory)
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<IContext, TInstance> factory)
		{
			var plugin = new Plugin
			{
				Type = _type,
				Name = _name,
				FactoryProvider = new LambdaFactoryProvider(ctx => factory(ctx))
			};
			_plugins.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public ITypeInstanceExpression<TInstance> Type<TInstance>()
		{
			var plugin = new Plugin
			{
				Type = _type,
				Name = _name,
				FactoryProvider = new TypeFactoryProvider(_type)
			};
			_plugins.Add(plugin);
			return new TypeInstanceExpression<TInstance>(plugin);
		}
	}

	public interface IInstanceExpression<TInstance, TParent>
	{
		ILifetimeExpression<TParent> Lifetime { get; }

		///// <summary>
		///// Adds an action to execute against the instance.
		///// </summary>
		///// <param name="action"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Visit(Action<TInstance> action);

		/// <summary>
		/// Adds an action to execute against the instance.
		/// </summary>
		/// <param name = "action" ></param>
		/// <returns></returns>
		IInstanceExpression<TInstance, TParent> Visit(Action<TInstance, IContext> action);

		/// <summary>
		/// Adds <paramref name="interceptor"/> to the pipeline.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		IInstanceExpression<TInstance, TParent> Intercept(IInterceptor interceptor);

		///// <summary>
		///// Adds <paramref name="interceptor"/> to the pipeline.
		///// </summary>
		///// <typeparam name="TOut"></typeparam>
		///// <param name="interceptor"></param>
		///// <returns></returns>
		//IInstanceExpression<TOut, TParent> Intercept<TOut>(IInterceptor<TInstance, TOut> interceptor);

		///// <summary>
		///// Adds <paramref name="lambda"/> to the pipeline.
		///// </summary>
		///// <typeparam name="TOut"></typeparam>
		///// <param name="lambda"></param>
		///// <returns></returns>
		//IInstanceExpression<TOut, TParent> Intercept<TOut>(Func<TInstance, TOut> lambda);

		///// <summary>
		///// Adds <paramref name="lambda"/> to the pipeline.
		///// </summary>
		///// <typeparam name="TOut"></typeparam>
		///// <param name="lambda"></param>
		///// <returns></returns>
		//IInstanceExpression<TOut, TParent> Intercept<TOut>(Func<TInstance, IContext, TOut> lambda);

		///// <summary>
		///// Set property <paramref name="property"/>.
		///// </summary>
		///// <param name="property"></param>
		///// <returns></returns>
		///// <remarks>Throws if the property type can't be resolved.</remarks>
		//IInstanceExpression<TInstance, TParent> Set(string property);

		///// <summary>
		///// Set property <paramref name="property"/> with value <paramref name="value"/>.
		///// </summary>
		///// <param name="property"></param>
		///// <param name="value"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set(string property, object value);

		///// <summary>
		///// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		///// </summary>
		///// <param name="property"></param>
		///// <param name="factory"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set(string property, Func<object> factory);

		///// <summary>
		///// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		///// </summary>
		///// <param name="property"></param>
		///// <param name="factory"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set(string property, Func<IContext, object> factory);

		///// <summary>
		///// Set property <paramref name="property"/>.
		///// </summary>
		///// <typeparam name="TValue"></typeparam>
		///// <param name="property"></param>
		///// <returns></returns>
		///// <remarks>Throws if the property type can't be resolved.</remarks>
		//IInstanceExpression<TInstance, TParent> Set<TValue>(Expression<Func<TInstance, TValue>> property);

		///// <summary>
		///// Set property <paramref name="property"/> with value <paramref name="value"/>.
		///// </summary>
		///// <typeparam name="TValue"></typeparam>
		///// <param name="property"></param>
		///// <param name="value"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set<TValue>(Expression<Func<TInstance, TValue>> property, TValue value);

		///// <summary>
		///// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		///// </summary>
		///// <typeparam name="TValue"></typeparam>
		///// <param name="property"></param>
		///// <param name="factory"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory);

		///// <summary>
		///// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		///// </summary>
		///// <typeparam name="TValue"></typeparam>
		///// <param name="property"></param>
		///// <param name="factory"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory);

		///// <summary>
		///// Set property <paramref name="property"/> if the property type can be resolved.
		///// </summary>
		///// <param name="property"></param>
		///// <returns></returns>
		///// <remarks>Does not throw if the property type can't be resolved.</remarks>
		//IInstanceExpression<TInstance, TParent> TrySet(string property);

		///// <summary>
		///// Set property <paramref name="property"/> if the property type can be resolved.
		///// </summary>
		///// <typeparam name="TValue"></typeparam>
		///// <param name="property"></param>
		///// <returns></returns>
		///// <remarks>Does not throw if the property type can't be resolved.</remarks>
		//IInstanceExpression<TInstance, TParent> TrySet<TValue>(Expression<Func<TInstance, TValue>> property);
	}

	class InstanceExpression<TInstance, TParent> : IInstanceExpression<TInstance, TParent>
	{
		public InstanceExpression(Plugin plugin, TParent parent)
		{
			Plugin = plugin;
			Parent = parent;
		}

		internal Plugin Plugin { get; set; }
		internal TParent Parent { get; set; }

		public ILifetimeExpression<TParent> Lifetime
		{
			get { return new LifetimeExpression<TParent>(Parent, lifetime => Plugin.Lifetime = lifetime); }
		}

		public IInstanceExpression<TInstance, TParent> Visit(Action<TInstance, IContext> action)
		{
			throw new NotImplementedException();
		}

		public IInstanceExpression<TInstance, TParent> Intercept(IInterceptor interceptor)
		{
			throw new NotImplementedException();
		}
	}

	public interface IFactoryInstanceExpression<T> : IInstanceExpression<T, IFactoryInstanceExpression<T>>
	{ }

	class FactoryInstanceExpression<T> : IFactoryInstanceExpression<T>
	{
		public FactoryInstanceExpression(Plugin plugin)
		{
			InstanceExpression = new InstanceExpression<T, IFactoryInstanceExpression<T>>(plugin, this);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> InstanceExpression { get; set; }

		public ILifetimeExpression<IFactoryInstanceExpression<T>> Lifetime => InstanceExpression.Lifetime;
		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Visit(Action<T, IContext> action)
		{
			return InstanceExpression.Visit(action);
		}

		public IInstanceExpression<T, IFactoryInstanceExpression<T>> Intercept(IInterceptor interceptor)
		{
			return InstanceExpression.Intercept(interceptor);
		}
	}

	public interface ITypeInstanceExpression<T> : IInstanceExpression<T, ITypeInstanceExpression<T>>
	{
		ITypeInstanceExpression<T> ConstructorDependency<TDependency>(TDependency dependency);
		ITypeInstanceExpression<T> ConstructorDependency(Type type, object dependency);
	}

	class TypeInstanceExpression<T> : ITypeInstanceExpression<T>
	{
		public TypeInstanceExpression(Plugin plugin)
		{
			Plugin = plugin;
			InstanceExpression = new InstanceExpression<T, ITypeInstanceExpression<T>>(plugin, this);
		}

		internal Plugin Plugin { get; }
		public IInstanceExpression<T, ITypeInstanceExpression<T>> InstanceExpression { get; }

		public ILifetimeExpression<ITypeInstanceExpression<T>> Lifetime => InstanceExpression.Lifetime;

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Visit(Action<T, IContext> action)
		{
			return InstanceExpression.Visit(action);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Intercept(IInterceptor interceptor)
		{
			return InstanceExpression.Intercept(interceptor);
		}

		public ITypeInstanceExpression<T> ConstructorDependency<TDependency>(TDependency dependency)
		{
			return ConstructorDependency(typeof(TDependency), dependency);
		}

		public ITypeInstanceExpression<T> ConstructorDependency(Type type, object dependency)
		{
			((TypeFactoryProvider)Plugin.FactoryProvider).Dependencies.Add(type, dependency);
			return this;
		}
	}
}