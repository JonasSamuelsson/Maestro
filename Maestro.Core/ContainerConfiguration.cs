using System;
using Maestro.Fluent;
using Maestro.Lifecycles;

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

		public IProviderSelector For(Type type)
		{
			var plugin = _plugins.GetOrAdd(type);
			return new ProviderSelector(Container.DefaultName, plugin);
		}

		public IProviderSelector<TPlugin> For<TPlugin>()
		{
			var plugin = _plugins.GetOrAdd(typeof(TPlugin));
			return new ProviderSelector<TPlugin>(x => plugin.Add(Container.DefaultName, x), _defaultSettings);
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
			return new ProviderSelector<TPlugin>(x => plugin.Add(name, x), _defaultSettings);
		}

		public IConventionExpression Scan
		{
			get { return new ConventionExpression(this); }
		}

		public IDefaultsExpression Default
		{
			get { return _defaultSettings; }
		}
	}

	internal class DefaultSettings : IDefaultsExpression
	{
		private ILifecycle _lifecycle = TransientLifecycle.Instance;

		ILifecycleSelector<IDefaultsExpression> IDefaultsExpression.Lifecycle
		{
			get { return new LifecycleSelector<IDefaultsExpression>(this, x => _lifecycle = x); }
		}

		public ILifecycle GetLifecycle()
		{
			return _lifecycle.Clone();
		}
	}
}