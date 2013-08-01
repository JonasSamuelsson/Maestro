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

		public IPipelineSelector Default(Type type)
		{
			var plugin = _plugins.GetOrAdd(type);
			return new PipelineSelector(Container.DefaultName, plugin);
		}

		public IPipelineSelector<TPlugin> Default<TPlugin>()
		{
			var plugin = _plugins.GetOrAdd(typeof(TPlugin));
			return new PipelineSelector<TPlugin>(Container.DefaultName, plugin);
		}

		public IPipelineSelector<TPlugin> Add<TPlugin>(string name = null)
		{
			var plugin = _plugins.GetOrAdd(typeof(TPlugin));
			return new PipelineSelector<TPlugin>(name ?? Guid.NewGuid().ToString(), plugin);
		}
	}
}