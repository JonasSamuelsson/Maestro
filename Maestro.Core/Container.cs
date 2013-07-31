using System;

namespace Maestro
{
	public class Container : IContainer
	{
		private static IContainer _default;
		private readonly IPluginDictionary _plugins;

		public Container()
		{
			_plugins = new PluginDictionary();
		}

		public Container(Action<IContainerConfiguration> action)
			: this()
		{
			Configure(action);
		}

		public void Configure(Action<IContainerConfiguration> action)
		{
			action(new ContainerConfiguration(_plugins));
		}

		public static IContainer Default
		{
			get { return _default ?? (_default = new Container()); }
		}

		internal static string DefaultName { get { return string.Empty; } }

		public object Get(Type type, string name = null)
		{
			IPlugin plugin;
			if (!_plugins.TryGet(type, out plugin))
				throw new ActivationException();
			var pipeline = plugin.Get(name ?? DefaultName);
			return pipeline.Get();
		}
	}
}