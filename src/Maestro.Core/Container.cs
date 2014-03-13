using Maestro.Configuration;
using Maestro.Factories;
using Maestro.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Maestro
{
	public class Container : IContainer, IContextContainer
	{
		private static IContainer _defaultContainer;

		private readonly Guid _id;
		private readonly ThreadSafeDictionary<Type, Plugin> _plugins;
		private readonly ThreadSafeDictionary<long, IInstanceBuilder> _instanceBuilderCache;
		private readonly DefaultSettings _defaultSettings;
		private long _contextId;
		private int _configVersion;
		private event Action<Guid> DisposedEvent;

		/// <summary>
		/// Instantiates a new empty container.
		/// </summary>
		public Container()
		{
			_id = Guid.NewGuid();
			_plugins = new ThreadSafeDictionary<Type, Plugin>();
			_instanceBuilderCache = new ThreadSafeDictionary<long, IInstanceBuilder>();
			_defaultSettings = new DefaultSettings();
		}

		/// <summary>
		/// Instantiates a new container with configuration.
		/// </summary>
		public Container(Action<IContainerExpression> action)
			: this()
		{
			Configure(action);
		}

		/// <summary>
		/// The static default container instance.
		/// </summary>
		public static IContainer Default
		{
			get { return _defaultContainer ?? (_defaultContainer = new Container()); }
		}

		internal static string DefaultName
		{
			get { return string.Empty; }
		}

		public void Configure(Action<IContainerExpression> action)
		{
			action(new ContainerExpression(_plugins, _defaultSettings));
			_instanceBuilderCache.Clear();
			Interlocked.Increment(ref _configVersion);
		}

		public object Get(Type type, string name = null)
		{
			try
			{
				name = name ?? DefaultName;
				var contextId = Interlocked.Increment(ref _contextId);
				var configVersion = _configVersion;
				using (var context = new Context(configVersion, contextId, name, this))
				using (((TypeStack)context.TypeStack).Push(type))
				{
					IInstanceBuilder instanceBuilder;
					if (TryGetInstanceBuilder(type, context, out instanceBuilder))
						return instanceBuilder.Get(context);
				}

				var message = name == DefaultName
										? string.Format("Can't get default instance of type {0}.", type.FullName)
										: string.Format("Can't get instance named '{0}' of type {1}.", name, type.FullName);
				throw new ActivationException(message);
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				var message = name == DefaultName
										? string.Format("Can't get default instance of type {0}.", type.FullName)
										: string.Format("Can't get instance named '{0}' of type {1}.", name, type.FullName);
				throw new ActivationException(message, exception);
			}
		}

		public T Get<T>(string name = null)
		{
			return (T)Get(typeof(T), name);
		}

		public IEnumerable<object> GetAll(Type type)
		{
			try
			{
				Plugin plugin;
				if (!_plugins.TryGet(type, out plugin))
					return Enumerable.Empty<object>();

				var contextId = Interlocked.Increment(ref _contextId);
				var configVersion = _configVersion;
				using (var context = new Context(configVersion, contextId, DefaultName, this))
					// ReSharper disable once AccessToDisposedClosure
					return plugin.Each(x => context.Name = x.Key)
									 .Select(x => x.Value.Get(context))
									 .ToList();
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get all instances of type {0}.", type.FullName), exception);
			}
		}

		public IEnumerable<T> GetAll<T>()
		{
			return GetAll(typeof(T)).Cast<T>().ToList();
		}

		public string GetConfiguration()
		{
			var builder = new DiagnosticsBuilder();
			foreach (var pair1 in _plugins.OrderBy(x => x.Key.FullName))
			{
				using (builder.Category(pair1.Key))
					foreach (var pair2 in pair1.Value.OrderBy(x => x.Key))
					{
						var prefix = pair2.Key == DefaultName ? "{default}" : pair2.Key;
						builder.Prefix(prefix + " : ");
						pair2.Value.GetConfiguration(builder);
					}
				builder.Line();
			}
			return builder.ToString();
		}

		bool IContextContainer.CanGet(Type type, IContext context)
		{
			IInstanceBuilder instanceBuilder;
			if (TryGetInstanceBuilder(type, context, out instanceBuilder))
				using (((TypeStack)context.TypeStack).Push(type))
					return instanceBuilder.CanGet(context);

			return false;
		}

		object IContextContainer.Get(Type type, IContext context)
		{
			try
			{
				IInstanceBuilder instanceBuilder;
				if (TryGetInstanceBuilder(type, context, out instanceBuilder))
					using (((TypeStack)context.TypeStack).Push(type))
						return instanceBuilder.Get(context);

				var message = context.Name == DefaultName
										? string.Format("Can't get default dependency of type {0}.", type.FullName)
										: string.Format("Can't get dependency named '{0} of type {1}.", context.Name, type.FullName);
				throw new ActivationException(message);
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				var message = context.Name == DefaultName
										? string.Format("Can't get default dependency of type {0}.", type.FullName)
										: string.Format("Can't get dependency named '{0} of type {1}.", context.Name, type.FullName);
				throw new ActivationException(message, exception);
			}
		}

		private bool TryGetInstanceBuilder(Type type, IContext context, out IInstanceBuilder instanceBuilder)
		{
			var key = (long)type.GetHashCode() << 32 | (uint)context.Name.GetHashCode();

			if (_instanceBuilderCache.TryGet(key, out instanceBuilder))
				return true;

			if (TryGetInstanceBuilder(_plugins, type, context, out instanceBuilder))
			{
				_instanceBuilderCache.Add(key, instanceBuilder);
				return true;
			}

			if (type.IsConcreteClosedClass() && !type.IsArray)
			{
				instanceBuilder = new InstanceBuilder(new TypeInstanceFactory(type));
				_instanceBuilderCache.Add(key, instanceBuilder);
				return true;
			}

			return false;
		}

		private static bool TryGetInstanceBuilder(ThreadSafeDictionary<Type, Plugin> plugins, Type type, IContext context,
			 out IInstanceBuilder instanceBuilder)
		{
			Plugin plugin;
			instanceBuilder = null;

			if (plugins.TryGet(type, out plugin))
				return TryGetInstanceBuilder(plugin, context, out instanceBuilder);

			if (!type.IsGenericType)
				return false;

			lock (plugins)
			{
				if (plugins.TryGet(type, out plugin))
					return TryGetInstanceBuilder(plugin, context, out instanceBuilder);

				Plugin typeDefinitionPlugin;
				var typeDefinition = type.GetGenericTypeDefinition();
				if (!plugins.TryGet(typeDefinition, out typeDefinitionPlugin))
					return false;

				var genericArguments = type.GetGenericArguments();
				plugin = new Plugin(typeDefinitionPlugin.Select(x => new KeyValuePair<string, IInstanceBuilder>(x.Key, x.Value.MakeGenericPipelineEngine(genericArguments))));
				plugins.Add(type, plugin);
				return TryGetInstanceBuilder(plugin, context, out instanceBuilder);
			}
		}

		private static bool TryGetInstanceBuilder(Plugin plugin, IContext context, out IInstanceBuilder instanceBuilder)
		{
			if (plugin.TryGet(context.Name, out instanceBuilder))
				return true;

			if (context.Name != DefaultName && plugin.TryGet(DefaultName, out instanceBuilder))
				return true;

			return false;
		}

		IEnumerable<object> IContextContainer.GetAll(Type type, IContext context)
		{
			try
			{
				Plugin plugin;
				if (!_plugins.TryGet(type, out plugin))
					return Enumerable.Empty<object>();

				using (((TypeStack)context.TypeStack).Push(type))
					return plugin.Select(x => x.Value.Get(context))
									 .ToList();
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get dependencies of type {0}.", type.FullName), exception);
			}
		}

		Guid IContextContainer.Id
		{
			get { return _id; }
		}

		public event Action<Guid> Disposed
		{
			add { DisposedEvent += value; }
			remove { DisposedEvent -= value; }
		}

		public void Dispose()
		{
			var disposed = DisposedEvent;
			if (disposed != null)
				disposed(_id);
		}
	}
}