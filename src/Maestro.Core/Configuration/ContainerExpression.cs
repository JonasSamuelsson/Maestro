using System;
using System.Collections.Concurrent;

namespace Maestro.Configuration
{
	internal class ContainerExpression : IContainerExpression
	{
		private readonly ConcurrentDictionary<Type, Plugin> _plugins;
		private readonly DefaultSettings _defaultSettings;

        public ContainerExpression(ConcurrentDictionary<Type, Plugin> plugins, DefaultSettings defaultSettings)
		{
			_plugins = plugins;
			_defaultSettings = defaultSettings;
		}

		public IInstanceExpression<object> For(Type type)
		{
			return For<object>(type);
		}

		public IInstanceExpression<TPlugin> For<TPlugin>()
		{
			return For<TPlugin>(typeof(TPlugin));
		}

		private IInstanceExpression<T> For<T>(Type type)
		{
			var plugin = _plugins.GetOrAdd(type, x => new Plugin());
			var defaultInstanceRegistrator = new InstanceRegistrator<T>(x => plugin.Add(Container.DefaultName, x), _defaultSettings);
			var anonymousInstanceRegistrator = new InstanceRegistrator<T>(x => plugin.Add(Guid.NewGuid().ToString(), x), _defaultSettings);
			return new InstanceExpression<T>(defaultInstanceRegistrator, anonymousInstanceRegistrator);
		}

		public INamedInstanceExpression<object> For(Type type, string name)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");
			return For<object>(type, name);
		}

		public INamedInstanceExpression<TPlugin> For<TPlugin>(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name");
			return For<TPlugin>(typeof(TPlugin), name);
		}

		private INamedInstanceExpression<T> For<T>(Type type, string name)
		{
			var plugin = _plugins.GetOrAdd(type, x => new Plugin());
			var instanceRegistrator = new InstanceRegistrator<T>(x => plugin.Add(name, x), _defaultSettings);
			return new NamedInstanceExpression<T>(instanceRegistrator);
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