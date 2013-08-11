using Maestro.Fluent;
using System;

namespace Maestro
{
	internal class ContainerConfiguration : IContainerConfiguration
	{
		private readonly IPluginDictionary _plugins;

		public ContainerConfiguration(IPluginDictionary plugins)
		{
			_plugins = plugins;
		}

		public IPipelineSelector For(Type type)
		{
			var plugin = _plugins.GetOrAdd(type);
			return new PipelineSelector(Container.DefaultName, plugin);
		}

		public IPipelineSelector<TPlugin> For<TPlugin>()
		{
			var plugin = _plugins.GetOrAdd(typeof(TPlugin));
			return new PipelineSelector<TPlugin>(Container.DefaultName, plugin);
		}

		public IPipelineSelector Add(Type type, string name = null)
		{
			name = name ?? Guid.NewGuid().ToString();
			var plugin = _plugins.GetOrAdd(type);
			return new PipelineSelector(name, plugin);
		}

		public IPipelineSelector<TPlugin> Add<TPlugin>(string name = null)
		{
			name = name ?? Guid.NewGuid().ToString();
			var plugin = _plugins.GetOrAdd(typeof(TPlugin));
			return new PipelineSelector<TPlugin>(name, plugin);
		}
	}
}