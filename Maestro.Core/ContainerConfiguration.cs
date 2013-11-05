using Maestro.Fluent;
using System;

namespace Maestro
{
	internal class ContainerConfiguration : IContainerConfiguration
	{
		private readonly IThreadSafeDictionary<Type, IPlugin> _plugins;
		private readonly DefaultSettings _defaultSettings;

		public ContainerConfiguration(IThreadSafeDictionary<Type, IPlugin> plugins, DefaultSettings defaultSettings)
		{
			_plugins = plugins;
			_defaultSettings = defaultSettings;
		}

		public IInstanceExpression<object> For(Type type)
		{
			return Add<object>(type, Container.DefaultName);
		}

		public IInstanceExpression<TPlugin> For<TPlugin>()
		{
			return Add<TPlugin>(typeof(TPlugin), Container.DefaultName);
		}

		public IInstanceExpression<object> For(Type type, string name)
		{
			if (name == null) throw new ArgumentNullException("name");
			return Add<object>(type, name);
		}

		public IInstanceExpression<TPlugin> For<TPlugin>(string name)
		{
			if (name == null) throw new ArgumentNullException("name");
			return Add<TPlugin>(typeof(TPlugin), name);
		}

		public IInstanceExpression<object> Add(Type type)
		{
			return Add<object>(type, Guid.NewGuid().ToString());
		}

		public IInstanceExpression<TPlugin> Add<TPlugin>()
		{
			return Add<TPlugin>(typeof(TPlugin), Guid.NewGuid().ToString());
		}

		private IInstanceExpression<T> Add<T>(Type type, string name)
		{
			var plugin = _plugins.GetOrAdd(type, x => new Plugin());
			return new InstanceExpression<T>(x => plugin.Add(name, x), _defaultSettings);
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