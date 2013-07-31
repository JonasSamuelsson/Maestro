using System;
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

		internal static string DefaultName { get { return string.Empty; } }

		public void Configure(Action<IContainerConfiguration> action)
		{
			action(new ContainerConfiguration(_plugins));
		}

		public object Get(Type type, string name = null)
		{
			IPlugin plugin;
			if (!_plugins.TryGet(type, out plugin))
				throw new ActivationException();

			name = name ?? DefaultName;
			var pipeline = plugin.Get(name);
			var requestId = Interlocked.Increment(ref _requestId);
			var context = new Context(requestId, name, this);
			return pipeline.Get(context);
		}

		public T Get<T>(string name = null)
		{
			return (T)Get(typeof(T), name);
		}

		bool IDependencyContainer.CanGet(Type type, IContext context)
		{
			IPlugin plugin;
			if (!_plugins.TryGet(type, out plugin))
				return false;

			IPipeline pipeline;
			if (!plugin.TryGet(context.Name, out pipeline))
				return false;

			return true;
		}

		object IDependencyContainer.Get(Type type, IContext context)
		{
			IPlugin plugin;
			if (!_plugins.TryGet(type, out plugin))
				throw new ActivationException();

			IPipeline pipeline;
			if (!plugin.TryGet(context.Name, out pipeline))
				throw new ActivationException();

			return pipeline.Get(context);
		}
	}
}