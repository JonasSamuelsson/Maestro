using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Maestro
{
	public class Container : IContainer, IDependencyResolver
	{
		private static IContainer _defaultContainer;
		private readonly ThreadSafeDictionary<Type, Plugin> _plugins;
		private readonly ThreadSafeDictionary<Type, IInstanceBuilder> _fallbackPipelines;
		private readonly DefaultSettings _defaultSettings;
		private long _requestId;
		private int _configId;

		public Container()
		{
			_plugins = new ThreadSafeDictionary<Type, Plugin>();
			_fallbackPipelines = new ThreadSafeDictionary<Type, IInstanceBuilder>();
			_defaultSettings = new DefaultSettings();
		}

		public Container(Action<IContainerConfiguration> action)
			: this()
		{
			Configure(action);
		}

		public static IContainer Default
		{
			get { return _defaultContainer ?? (_defaultContainer = new Container()); }
		}

		internal static string DefaultName { get { return string.Empty; } }

		public void Configure(Action<IContainerConfiguration> action)
		{
			action(new ContainerConfiguration(_plugins, _defaultSettings));
			_fallbackPipelines.Clear();
			Interlocked.Increment(ref _configId);
		}

		public object Get(Type type, string name = null)
		{
			try
			{
				name = name ?? DefaultName;
				var requestId = Interlocked.Increment(ref _requestId);
				using (var context = new Context(_configId, requestId, name, this))
				using (((TypeStack)context.TypeStack).Push(type))
				{
					IInstanceBuilder instanceBuilder;
					if (TryGetPipeline(_plugins, type, context, out instanceBuilder))
						return instanceBuilder.Get(context);

					IInstanceBuilder fallbackInstanceBuilder;
					if (TryGetFallbackPipeline(_fallbackPipelines, type, out fallbackInstanceBuilder))
						return fallbackInstanceBuilder.Get(context);
				}

				throw new ActivationException(string.Format("Can't get {0}-{1}.", name, type.FullName));
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get {0}-{1}.", name, type.FullName), exception);
			}
		}

		private static bool TryGetFallbackPipeline(ThreadSafeDictionary<Type, IInstanceBuilder> fallbackPipelines, Type type, out IInstanceBuilder instanceBuilder)
		{
			instanceBuilder = null;

			if (!type.IsConcreteClosedClass() || type.IsArray)
				return false;

			instanceBuilder = fallbackPipelines.GetOrAdd(type, x => new InstanceBuilder(new TypeInstanceFactory(x)));
			return true;
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

				var requestId = Interlocked.Increment(ref _requestId);
				using (var context = new Context(_configId, requestId, DefaultName, this))
					return plugin.GetNames().ToList()
						.Select(name => new { name, engine = plugin.Get(name) })
						.Select(x =>
								  {
									  context.Name = x.name;
									  return x.engine.Get(context);
								  })
						.ToList();
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get all {0}.", type.FullName), exception);
			}
		}

		public IEnumerable<T> GetAll<T>()
		{
			return GetAll(typeof(T)).Cast<T>().ToList();
		}

		public string GetConfiguration()
		{
			var builder = new DiagnosticsBuilder();
			foreach (var pair in _plugins.OrderBy(x => x.Key.FullName))
			{
				using (builder.Category(pair.Key))
					foreach (var name in pair.Value.GetNames().OrderBy(x => x))
					{
						var prefix = name == DefaultName ? "{default}" : name;
						builder.Prefix(prefix + " : ");
						pair.Value.Get(name).GetConfiguration(builder);
					}
				builder.Line();
			}
			return builder.ToString();
		}

		bool IDependencyResolver.CanGet(Type type, IContext context)
		{
			IInstanceBuilder instanceBuilder;
			if (TryGetPipeline(_plugins, type, context, out instanceBuilder))
				using (((TypeStack)context.TypeStack).Push(type))
					return instanceBuilder.CanGet(context);

			if (TryGetFallbackPipeline(_fallbackPipelines, type, out instanceBuilder))
				using (((TypeStack)context.TypeStack).Push(type))
					return instanceBuilder.CanGet(context);

			return false;
		}

		object IDependencyResolver.Get(Type type, IContext context)
		{
			try
			{
				IInstanceBuilder instanceBuilder;
				if (TryGetPipeline(_plugins, type, context, out instanceBuilder))
					using (((TypeStack)context.TypeStack).Push(type))
						return instanceBuilder.Get(context);

				if (TryGetFallbackPipeline(_fallbackPipelines, type, out instanceBuilder))
					using (((TypeStack)context.TypeStack).Push(type))
						return instanceBuilder.Get(context);

				throw new ActivationException(string.Format("Can't get dependency {0}-{1}.", context.Name, type.FullName));
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get dependency {0}-{1}.", context.Name, type.FullName), exception);
			}
		}

		private static bool TryGetPipeline(ThreadSafeDictionary<Type, Plugin> plugins, Type type, IContext context,
			out IInstanceBuilder instanceBuilder)
		{
			instanceBuilder = null;

			Plugin plugin;
			if (!plugins.TryGet(type, out plugin))
				return type.IsGenericType && TryGetGenericPipeline(plugins, type, context, out instanceBuilder);

			if (plugin.TryGet(context.Name, out instanceBuilder))
				return true;

			if (context.Name != DefaultName && plugin.TryGet(DefaultName, out instanceBuilder))
				return true;

			return false;
		}

		private static bool TryGetGenericPipeline(ThreadSafeDictionary<Type, Plugin> plugins, Type type, IContext context, out IInstanceBuilder instanceBuilder)
		{
			instanceBuilder = null;

			lock (plugins)
			{
				Plugin plugin;
				if (plugins.TryGet(type, out plugin))
					return TryGetPipeline(plugins, type, context, out instanceBuilder);

				var typeDefinition = type.GetGenericTypeDefinition();
				if (!plugins.TryGet(typeDefinition, out plugin))
					return false;

				var genericArguments = type.GetGenericArguments();
				var names = plugin.GetNames().ToList();
				var genericPlugin = plugins.GetOrAdd(type, x => new Plugin());
				foreach (var name in names)
					genericPlugin.Add(name, plugin.Get(name).MakeGenericPipelineEngine(genericArguments));
			}

			return TryGetPipeline(plugins, type, context, out instanceBuilder);
		}

		IEnumerable<object> IDependencyResolver.GetAll(Type type, IContext context)
		{
			try
			{
				Plugin plugin;
				if (!_plugins.TryGet(type, out plugin))
					return Enumerable.Empty<object>();

				using (((TypeStack)context.TypeStack).Push(type))
					return plugin.GetNames().ToList()
						.Select(name => plugin.Get(name))
						.Select(engine => engine.Get(context));
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get all dependencies {0}-{1}.", context.Name, type.FullName), exception);
			}
		}
	}
}