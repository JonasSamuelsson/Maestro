using System;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class ServiceExpression<TService> : IServiceExpression, IServiceExpression<TService>, INamedServiceExpression, INamedServiceExpression<TService>
	{
		public ServiceExpression(Type type, string name, Kernel kernel, DefaultSettings defaultSettings)
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

		IServiceInstanceExpression IUseServiceExpression.Use
		{
			get { return new ServiceInstanceExpression<object>(Type, Name, Kernel, DefaultSettings, true); }
		}

		IServiceInstanceExpression ITryUseServiceExpression.TryUse
		{
			get { return new ServiceInstanceExpression<object>(Type, Name, Kernel, DefaultSettings, false); }
		}

		IServiceInstanceExpression<TService> IUseServiceExpression<TService>.Use
		{
			get { return new ServiceInstanceExpression<TService>(Type, Name, Kernel, DefaultSettings, true); }
		}

		IServiceInstanceExpression<TService> ITryUseServiceExpression<TService>.TryUse
		{
			get { return new ServiceInstanceExpression<TService>(Type, Name, Kernel, DefaultSettings, false); }
		}

		IServiceInstanceExpression IAddServiceExpression.Add
		{
			get { return new ServiceInstanceExpression<object>(Type, ServiceDescriptorLookup.GetRandomName(), Kernel, DefaultSettings, true); }
		}

		IServiceInstanceExpression<TService> IAddServiceExpression<TService>.Add
		{
			get { return new ServiceInstanceExpression<TService>(Type, ServiceDescriptorLookup.GetRandomName(), Kernel, DefaultSettings, true); }
		}
	}
}