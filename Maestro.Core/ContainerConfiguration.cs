using System;
using Maestro.Fluent;

namespace Maestro
{
	internal class ContainerConfiguration : IContainerConfiguration
	{
		private readonly ICustomDictionary<IPlugin> _plugins;
		private readonly DefaultSettings _defaultSettings;

		public ContainerConfiguration(ICustomDictionary<IPlugin> plugins, DefaultSettings defaultSettings)
		{
			_plugins = plugins;
			_defaultSettings = defaultSettings;
		}

		public IProviderSelector<object> For(Type type)
		{
			var plugin = _plugins.GetOrAdd(type);
			return new ProviderSelector<object>(x => plugin.Add(Container.DefaultName, x), _defaultSettings);
		}

		public IProviderSelector<TPlugin> For<TPlugin>()
		{
			var plugin = _plugins.GetOrAdd(typeof(TPlugin));
			return new ProviderSelector<TPlugin>(x => plugin.Add(Container.DefaultName, x), _defaultSettings);
		}

		public IProviderSelector<object> Add(Type type, string name = null)
		{
			name = name ?? Guid.NewGuid().ToString();
			var plugin = _plugins.GetOrAdd(type);
			return new ProviderSelector<object>(x => plugin.Add(name, x), _defaultSettings);
		}

		public IProviderSelector<TPlugin> Add<TPlugin>(string name = null)
		{
			name = name ?? Guid.NewGuid().ToString();
			var plugin = _plugins.GetOrAdd(typeof(TPlugin));
			return new ProviderSelector<TPlugin>(x => plugin.Add(name, x), _defaultSettings);
		}

		public IConventionExpression Scan
		{
			get { return new ConventionExpression(this, _defaultSettings); }
		}

		public IDefaultSettingsExpression Default
		{
			get { return _defaultSettings; }
		}
	}
}