using Maestro.Fluent;
using System;

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
			return Add<object>(type, Container.DefaultName);
		}

		public IProviderSelector<TPlugin> For<TPlugin>()
		{
			return Add<TPlugin>(typeof(TPlugin), Container.DefaultName);
		}

		public IProviderSelector<object> For(Type type, string name)
		{
			if (name == null) throw new ArgumentNullException("name");
			return Add<object>(type, name);
		}

		public IProviderSelector<TPlugin> For<TPlugin>(string name)
		{
			if (name == null) throw new ArgumentNullException("name");
			return Add<TPlugin>(typeof(TPlugin), name);
		}

		public IProviderSelector<object> Add(Type type)
		{
			return Add<object>(type, Guid.NewGuid().ToString());
		}

		public IProviderSelector<TPlugin> Add<TPlugin>()
		{
			return Add<TPlugin>(typeof(TPlugin), Guid.NewGuid().ToString());
		}

		private IProviderSelector<T> Add<T>(Type type, string name)
		{
			var plugin = _plugins.GetOrAdd(type);
			return new ProviderSelector<T>(x => plugin.Add(name, x), _defaultSettings);
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