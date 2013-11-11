using System;
using Maestro.Utils;

namespace Maestro.Configuration
{
	internal class ContainerExpression : IContainerExpression
	{
		private readonly ThreadSafeDictionary<Type, Plugin> _plugins;
		private readonly DefaultSettings _defaultSettings;

		public ContainerExpression(ThreadSafeDictionary<Type, Plugin> plugins, DefaultSettings defaultSettings)
		{
			_plugins = plugins;
			_defaultSettings = defaultSettings;
		}

		public IInstanceFactoryExpression<object> For(Type type)
		{
			return Add<object>(type, Container.DefaultName);
		}

		public IInstanceFactoryExpression<TPlugin> For<TPlugin>()
		{
			return Add<TPlugin>(typeof(TPlugin), Container.DefaultName);
		}

		public IInstanceFactoryExpression<object> For(Type type, string name)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");
			return Add<object>(type, name);
		}

		public IInstanceFactoryExpression<TPlugin> For<TPlugin>(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");
			return Add<TPlugin>(typeof(TPlugin), name);
		}

		public IInstanceFactoryExpression<object> Add(Type type)
		{
			return Add<object>(type, Guid.NewGuid().ToString());
		}

		public IInstanceFactoryExpression<TPlugin> Add<TPlugin>()
		{
			return Add<TPlugin>(typeof(TPlugin), Guid.NewGuid().ToString());
		}

		private IInstanceFactoryExpression<T> Add<T>(Type type, string name)
		{
			var plugin = _plugins.GetOrAdd(type, x => new Plugin());
			return new InstanceFactoryExpression<T>(x => plugin.Add(name, x), _defaultSettings);
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