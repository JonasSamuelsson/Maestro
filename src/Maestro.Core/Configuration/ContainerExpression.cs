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
		private readonly PluginLookup _pluginLookup;
		private readonly DefaultSettings _defaultSettings;

		public ContainerExpression(PluginLookup pluginLookup, DefaultSettings defaultSettings)
		{
			_pluginLookup = pluginLookup;
			_defaultSettings = defaultSettings;
		}

		public ITypeExpression<object> For(Type type)
		{
			return new KeyExpression<object>(type, _pluginLookup);
		}

		public ITypeExpression<T> For<T>()
		{
			return new KeyExpression<T>(typeof(T), _pluginLookup);
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

	public interface ITypeExpression<T>
	{
		IFactoryExpression<T> Add { get; }
		IFactoryExpression<T> Use { get; }
		INameExpression<T> Named(string name);
	}

	public interface INameExpression<T>
	{
		IFactoryExpression<T> Use { get; }
	}

	class KeyExpression<T> : ITypeExpression<T>
	{
		public KeyExpression(Type type, PluginLookup plugins)
		{
			Type = type;
			Plugins = plugins;
		}

		public Type Type { get; }
		public PluginLookup Plugins { get; }

		IFactoryExpression<T> ITypeExpression<T>.Add => GetFactoryExpression(null);
		IFactoryExpression<T> ITypeExpression<T>.Use => GetFactoryExpression(PluginLookup.DefaultName);

		INameExpression<T> ITypeExpression<T>.Named(string name)
		{
			return new Temp { Name = name, Parent = this };
		}

		private IFactoryExpression<T> GetFactoryExpression(string name)
		{
			return new FactoryExpression<T>(Type, name, Plugins);
		}

		class Temp : INameExpression<T>
		{
			public string Name { get; set; }
			public KeyExpression<T> Parent { get; set; }

			public IFactoryExpression<T> Use => Parent.GetFactoryExpression(Name);
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
			_plugins.Add(_type, _name, new Plugin
			{
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
				FactoryProvider = new LambdaFactoryProvider(ctx => factory(ctx))
			};
			_plugins.Add(_type, _name, plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public ITypeInstanceExpression<TInstance> Type<TInstance>()
		{
			var plugin = new Plugin
			{
				FactoryProvider = new TypeFactoryProvider(_type)
			};
			_plugins.Add(_type, _name, plugin);
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

		public Plugin Plugin { get; set; }
		public TParent Parent { get; set; }

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
	{ }

	class TypeInstanceExpression<T> : ITypeInstanceExpression<T>
	{
		public TypeInstanceExpression(Plugin plugin)
		{
			InstanceExpression = new InstanceExpression<T, ITypeInstanceExpression<T>>(plugin, this);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> InstanceExpression { get; set; }

		public ILifetimeExpression<ITypeInstanceExpression<T>> Lifetime => InstanceExpression.Lifetime;

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Visit(Action<T, IContext> action)
		{
			return InstanceExpression.Visit(action);
		}

		public IInstanceExpression<T, ITypeInstanceExpression<T>> Intercept(IInterceptor interceptor)
		{
			return InstanceExpression.Intercept(interceptor);
		}
	}
}