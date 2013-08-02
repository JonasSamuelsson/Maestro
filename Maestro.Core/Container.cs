using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Maestro
{
	public class Container : IContainer, IDependencyContainer
	{
		private static IContainer _default;
		private readonly IPluginDictionary _plugins;
		private long _requestId;

		public Container()
		{
			_plugins = new PluginDictionary();
		}

		public Container(Action<IContainerConfiguration> action)
			: this()
		{
			Configure(action);
		}

		public static IContainer Default
		{
			get { return _default ?? (_default = new Container()); }
		}

		internal static string DefaultName { get { return "default"; } }

		public void Configure(Action<IContainerConfiguration> action)
		{
			action(new ContainerConfiguration(_plugins));
		}

		public object Get(Type type, string name = null)
		{
			try
			{
				IPlugin plugin;
				if (_plugins.TryGet(type, out plugin))
				{
					name = name ?? DefaultName;
					IPipeline pipeline;
					if (plugin.TryGet(name, out pipeline))
					{
						var requestId = Interlocked.Increment(ref _requestId);
						var context = new Context(requestId, name, this);
						return pipeline.Get(context);
					}
				}
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get {0}-{1}.", name, type.FullName), exception);
			}

			throw new ActivationException(string.Format("Can't get {0}-{1}.", name, type.FullName));
		}

		public T Get<T>(string name = null)
		{
			return (T)Get(typeof(T), name);
		}

		bool IDependencyContainer.CanGet(Type type, IContext context)
		{
			IPipeline pipeline;
			if (TryGetDependencyPipeline(_plugins, type, context, out pipeline))
				return true;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				return true;

			return false;
		}

		object IDependencyContainer.Get(Type type, IContext context)
		{
			try
			{
				IPipeline pipeline;
				if (TryGetDependencyPipeline(_plugins, type, context, out pipeline))
					return pipeline.Get(context);

				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
					return ((IDependencyContainer)this).GetAll(type.GetGenericArguments().Single(), context);
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get dependency {0}-{1}.", context.Name, type.FullName), exception);
			}

			throw new ActivationException(string.Format("Can't get dependency {0}-{1}.", context.Name, type.FullName));
		}

		private static bool TryGetDependencyPipeline(IPluginDictionary plugins, Type type, IContext context,
			out IPipeline pipeline)
		{
			pipeline = null;

			IPlugin plugin;
			if (!plugins.TryGet(type, out plugin))
				return false;

			if (plugin.TryGet(context.Name, out pipeline))
				return true;

			if (context.Name != DefaultName && plugin.TryGet(DefaultName, out pipeline))
				return true;

			return false;
		}

		IEnumerable<object> IDependencyContainer.GetAll(Type type, IContext context)
		{
			try
			{
				IPlugin plugin;
				if (!_plugins.TryGet(type, out plugin))
					return (IEnumerable<object>)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

				return plugin.GetNames().Select(x => plugin.Get(x).Get(context)).ToList();
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