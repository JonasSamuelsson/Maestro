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

		IInstanceKindSelector IUseServiceConfigurator.Use
		{
			get { return new InstanceKindSelector<object>(Type, Name, Kernel, DefaultSettings, true); }
		}

		IInstanceKindSelector ITryUseServiceConfigurator.TryUse
		{
			get { return new InstanceKindSelector<object>(Type, Name, Kernel, DefaultSettings, false); }
		}

		IInstanceKindSelector IAddServiceConfigurator.Add
		{
			get { return new InstanceKindSelector<object>(Type, ServiceNames.Anonymous, Kernel, DefaultSettings, true); }
		}

		IInstanceKindSelector<TService> IUseServiceConfigurator<TService>.Use
		{
			get { return new InstanceKindSelector<TService>(Type, Name, Kernel, DefaultSettings, true); }
		}

		IInstanceKindSelector<TService> ITryUseServiceConfigurator<TService>.TryUse
		{
			get { return new InstanceKindSelector<TService>(Type, Name, Kernel, DefaultSettings, false); }
		}

		IInstanceKindSelector<TService> IAddServiceConfigurator<TService>.Add
		{
			get { return new InstanceKindSelector<TService>(Type, ServiceNames.Anonymous, Kernel, DefaultSettings, true); }
		}
	}
}