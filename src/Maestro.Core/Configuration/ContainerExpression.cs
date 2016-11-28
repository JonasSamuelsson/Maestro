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

		public IServiceExpression Service(Type type)
		{
			return new ServiceExpression<object>(type, PluginLookup.DefaultName, _kernel, _defaultSettings);
		}

		public IServiceExpression Service(Type type, string name)
		{
			return new ServiceExpression<object>(type, name, _kernel, _defaultSettings);
		}

		public IServiceExpression<T> Service<T>()
		{
			return new ServiceExpression<T>(typeof(T), PluginLookup.DefaultName, _kernel, _defaultSettings);
		}

		public IServiceExpression<T> Service<T>(string name)
		{
			return new ServiceExpression<T>(typeof(T), name, _kernel, _defaultSettings);
		}

		public IServicesExpression Services(Type type)
		{
			return new ServiceExpression<object>(type, PluginLookup.GetRandomName(), _kernel, _defaultSettings);
		}

		public IServicesExpression<T> Services<T>()
		{
			return new ServiceExpression<T>(typeof(T), PluginLookup.GetRandomName(), _kernel, _defaultSettings);
		}
	}
}