using System;
using Maestro.Internals;

namespace Maestro.Configuration
{
	internal class ServiceConfigurator<TService> : IServiceConfigurator, IServiceConfigurator<TService>, INamedServiceConfigurator, INamedServiceConfigurator<TService>
	{
		internal ServiceConfigurator(Type type, string name, Kernel kernel, DefaultSettings defaultSettings)
		{
			Type = type;
			Name = name;
			Kernel = kernel;
			DefaultSettings = defaultSettings;
		}

		internal Type Type { get; }
		internal string Name { get; set; }
		internal Kernel Kernel { get; }
		internal DefaultSettings DefaultSettings { get; set; }

		IServiceInstanceExpression IUseServiceConfigurator.Use
		{
			get { return new ServiceInstanceExpression<object>(Type, Name, Kernel, DefaultSettings, true); }
		}

		IServiceInstanceExpression ITryUseServiceConfigurator.TryUse
		{
			get { return new ServiceInstanceExpression<object>(Type, Name, Kernel, DefaultSettings, false); }
		}

		IServiceInstanceExpression IAddServiceConfigurator.Add
		{
			get { return new ServiceInstanceExpression<object>(Type, ServiceNames.Anonymous, Kernel, DefaultSettings, true); }
		}

		IServiceInstanceExpression<TService> IUseServiceConfigurator<TService>.Use
		{
			get { return new ServiceInstanceExpression<TService>(Type, Name, Kernel, DefaultSettings, true); }
		}

		IServiceInstanceExpression<TService> ITryUseServiceConfigurator<TService>.TryUse
		{
			get { return new ServiceInstanceExpression<TService>(Type, Name, Kernel, DefaultSettings, false); }
		}

		IServiceInstanceExpression<TService> IAddServiceConfigurator<TService>.Add
		{
			get { return new ServiceInstanceExpression<TService>(Type, ServiceNames.Anonymous, Kernel, DefaultSettings, true); }
		}
	}
}