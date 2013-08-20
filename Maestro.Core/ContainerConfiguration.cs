using Maestro.Fluent;
using System;

namespace Maestro
{
	internal class ContainerConfiguration : IContainerConfiguration
	{
		private readonly ICustomDictionary<IPlugin> _plugins;

		public ContainerConfiguration(ICustomDictionary<IPlugin> plugins)
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
			return new PipelineSelector<TPlugin>(x => plugin.Add(Container.DefaultName, x));
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
			return new PipelineSelector<TPlugin>(x => plugin.Add(name, x));
		}

		public IConventionExpression Scan
		{
			get { return new ConventionExpression(this); }
		}
	}
}