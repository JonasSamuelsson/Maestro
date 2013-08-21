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

		public IProviderSelector For(Type type)
		{
			var plugin = _plugins.GetOrAdd(type);
			return new ProviderSelector(Container.DefaultName, plugin);
		}

		public IProviderSelector<TPlugin> For<TPlugin>()
		{
			var plugin = _plugins.GetOrAdd(typeof(TPlugin));
			return new ProviderSelector<TPlugin>(x => plugin.Add(Container.DefaultName, x));
		}

		public IProviderSelector Add(Type type, string name = null)
		{
			name = name ?? Guid.NewGuid().ToString();
			var plugin = _plugins.GetOrAdd(type);
			return new ProviderSelector(name, plugin);
		}

		public IProviderSelector<TPlugin> Add<TPlugin>(string name = null)
		{
			name = name ?? Guid.NewGuid().ToString();
			var plugin = _plugins.GetOrAdd(typeof(TPlugin));
			return new ProviderSelector<TPlugin>(x => plugin.Add(name, x));
		}

		public IConventionExpression Scan
		{
			get { return new ConventionExpression(this); }
		}
	}
}