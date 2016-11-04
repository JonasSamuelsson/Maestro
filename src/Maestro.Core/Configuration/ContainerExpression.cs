using System;
using Maestro.Internals;

namespace Maestro.Configuration
{
	internal class ContainerExpression : IContainerExpression
	{
		private readonly Kernel _kernel;
		private readonly DefaultSettings _defaultSettings;

		public ContainerExpression(Kernel kernel, DefaultSettings defaultSettings)
		{
			_kernel = kernel;
			_defaultSettings = defaultSettings;
		}

		public IConventionExpression Scan
		{
			get { return new ConventionExpression(this, _defaultSettings); }
		}

		public IDefaultSettingsExpression Default
		{
			get { return _defaultSettings; }
		}

		public IDefaultPluginExpression For(Type type)
		{
			return new PluginExpression<object>(type, PluginLookup.DefaultName, _kernel, _defaultSettings);
		}

		public IPluginExpression For(Type type, string name)
		{
			return new PluginExpression<object>(type, name, _kernel, _defaultSettings);
		}

		public IDefaultPluginExpression<T> For<T>()
		{
			return new PluginExpression<T>(typeof(T), PluginLookup.DefaultName, _kernel, _defaultSettings);
		}

		public IPluginExpression<T> For<T>(string name)
		{
			return new PluginExpression<T>(typeof(T), name, _kernel, _defaultSettings);
		}
	}
}