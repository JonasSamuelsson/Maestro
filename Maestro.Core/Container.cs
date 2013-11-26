using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Maestro.Configuration;
using Maestro.Factories;
using Maestro.Lifetimes;
using Maestro.Utils;

namespace Maestro
{
	public class Container : IContainer, IContextContainer
	{
		private static IContainer _defaultContainer;

		private readonly Guid _id;
		private readonly ThreadSafeDictionary<Type, Plugin> _plugins;
		private readonly Container _parentContainer;
		private readonly ThreadSafeDictionary<Type, IInstanceBuilder> _fallbackInstanceBuilders;
		private readonly DefaultSettings _defaultSettings;
		private long _requestId;
		private int _configVersion;
		private event Action<Guid> DisposedEvent;

		private event Action ConfigVersionChanged = delegate { };

		/// <summary>
		/// Instantiates a new empty container.
		/// </summary>
		public Container()
		{
			_id = Guid.NewGuid();
			_plugins = new ThreadSafeDictionary<Type, Plugin>();
			_fallbackInstanceBuilders = new ThreadSafeDictionary<Type, IInstanceBuilder>();
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

		private Container(Container container)
			: this()
		{
			_parentContainer = container;
			_parentContainer.ConfigVersionChanged += ParentContainerConfigVersionChanged;
			Configure(x => x.Default.Lifetime.Custom<ContainerSingletonLifetime>());
		}

		private void ParentContainerConfigVersionChanged()
		{
			Interlocked.Increment(ref _configVersion);
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

		private bool IsChildContainer
		{
			get { return _parentContainer != null; }
		}

		public void Configure(Action<IContainerExpression> action)
		{
			action(new ContainerExpression(_plugins, _defaultSettings));
			_fallbackInstanceBuilders.Clear();
			Interlocked.Increment(ref _configVersion);
			ConfigVersionChanged();
		}

		public object Get(Type type, string name = null)
		{
			try
			{
				name = name ?? DefaultName;
				var requestId = Interlocked.Increment(ref _requestId);
				var configVersion = _configVersion;
				using (var context = new Context(configVersion, requestId, name, this))
				using (((TypeStack)context.TypeStack).Push(type))
				{
					IInstanceBuilder instanceBuilder;

					if (TryGetInstanceBuilder(_plugins, type, context, out instanceBuilder))
						return instanceBuilder.Get(context);

					if (IsChildContainer && TryGetInstanceBuilder(_parentContainer._plugins, type, context, out instanceBuilder))
						return instanceBuilder.Get(context);

					IInstanceBuilder fallbackInstanceBuilder;
					if (TryGetFallbackInstanceBuilder(_fallbackInstanceBuilders, type, out fallbackInstanceBuilder))
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

		private static bool TryGetFallbackInstanceBuilder(ThreadSafeDictionary<Type, IInstanceBuilder> fallbackPipelines, Type type, out IInstanceBuilder instanceBuilder)
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
				Plugin parentPlugin = null;
				if (!_plugins.TryGet(type, out plugin) && (!IsChildContainer || !_parentContainer._plugins.TryGet(type, out parentPlugin)))
					return Enumerable.Empty<object>();

				var set = new HashSet<string>();
				var requestId = Interlocked.Increment(ref _requestId);
				var configVersion = _configVersion;
				using (var context = new Context(configVersion, requestId, DefaultName, this))
					return new[] { plugin ?? Plugin.Empty, parentPlugin ?? Plugin.Empty }
						.SelectMany(x => x)
						.Where(x => !set.Contains(x.Key))
						.Each(x => set.Add(x.Key))
						.Select(x =>
								  {
									  context.Name = x.Key;
									  return x.Value.Get(context);
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
			if (TryGetInstanceBuilder(_plugins, type, context, out instanceBuilder))
				using (((TypeStack)context.TypeStack).Push(type))
					return instanceBuilder.CanGet(context);

			if (IsChildContainer && TryGetInstanceBuilder(_parentContainer._plugins, type, context, out instanceBuilder))
				using (((TypeStack)context.TypeStack).Push(type))
					return instanceBuilder.CanGet(context);

			var fallbackInstanceBuilders = IsChildContainer ? _parentContainer._fallbackInstanceBuilders : _fallbackInstanceBuilders;
			if (TryGetFallbackInstanceBuilder(fallbackInstanceBuilders, type, out instanceBuilder))
				using (((TypeStack)context.TypeStack).Push(type))
					return instanceBuilder.CanGet(context);

			return false;
		}

		object IContextContainer.Get(Type type, IContext context)
		{
			try
			{
				IInstanceBuilder instanceBuilder;

				if (TryGetInstanceBuilder(_plugins, type, context, out instanceBuilder))
					using (((TypeStack)context.TypeStack).Push(type))
						return instanceBuilder.Get(context);

				if (IsChildContainer && TryGetInstanceBuilder(_parentContainer._plugins, type, context, out instanceBuilder))
					using (((TypeStack)context.TypeStack).Push(type))
						return instanceBuilder.Get(context);

				var fallbackInstanceBuilders = IsChildContainer ? _parentContainer._fallbackInstanceBuilders : _fallbackInstanceBuilders;
				if (TryGetFallbackInstanceBuilder(fallbackInstanceBuilders, type, out instanceBuilder))
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

		private static bool TryGetInstanceBuilder(ThreadSafeDictionary<Type, Plugin> plugins, Type type, IContext context,
			out IInstanceBuilder instanceBuilder)
		{
			instanceBuilder = null;

			Plugin plugin;
			if (!plugins.TryGet(type, out plugin))
				return type.IsGenericType && TryGetGenericInstanceBuilder(plugins, type, context, out instanceBuilder);

			if (plugin.TryGet(context.Name, out instanceBuilder))
				return true;

			if (context.Name != DefaultName && plugin.TryGet(DefaultName, out instanceBuilder))
				return true;

			return false;
		}

		private static bool TryGetGenericInstanceBuilder(ThreadSafeDictionary<Type, Plugin> plugins, Type type, IContext context, out IInstanceBuilder instanceBuilder)
		{
			instanceBuilder = null;

			lock (plugins)
			{
				Plugin plugin;
				if (plugins.TryGet(type, out plugin))
					return TryGetInstanceBuilder(plugins, type, context, out instanceBuilder);

				var typeDefinition = type.GetGenericTypeDefinition();
				if (!plugins.TryGet(typeDefinition, out plugin))
					return false;

				var genericArguments = type.GetGenericArguments();
				var genericPlugin = plugins.GetOrAdd(type, x => new Plugin());
				foreach (var pair in plugin)
					genericPlugin.Add(pair.Key, pair.Value.MakeGenericPipelineEngine(genericArguments));
			}

			return TryGetInstanceBuilder(plugins, type, context, out instanceBuilder);
		}

		IEnumerable<object> IContextContainer.GetAll(Type type, IContext context)
		{
			try
			{
				Plugin plugin;
				Plugin parentPlugin = null;
				if (!_plugins.TryGet(type, out plugin) && (!IsChildContainer || !_parentContainer._plugins.TryGet(type, out parentPlugin)))
					return Enumerable.Empty<object>();

				var set = new HashSet<string>();
				using (((TypeStack)context.TypeStack).Push(type))
					return new[] { plugin ?? Plugin.Empty, parentPlugin ?? Plugin.Empty }
						.SelectMany(x => x)
						.Where(x => !set.Contains(x.Key))
						.Each(x => set.Add(x.Key))
						.Select(x => x.Value.Get(context))
						.ToList();
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

		Guid IContextContainer.Id
		{
			get { return _id; }
		}

		public event Action<Guid> Disposed
		{
			add { DisposedEvent += value; }
			remove { DisposedEvent -= value; }
		}

		public IContainer GetChildContainer(Action<IContainerExpression> action = null)
		{
			if (IsChildContainer) throw new NotSupportedException();
			var container = new Container(this);
			if (action != null) container.Configure(action);
			return container;
		}

		public void Dispose()
		{
			if (IsChildContainer)
				_parentContainer.ConfigVersionChanged -= ParentContainerConfigVersionChanged;

			var disposed = DisposedEvent;
			if (disposed != null)
				disposed(_id);
		}
	}
}