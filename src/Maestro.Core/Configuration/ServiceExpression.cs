using System;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class ServiceExpression<TService> : IServiceExpression, IServiceExpression<TService>, IServicesExpression, IServicesExpression<TService>
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

		IServiceInstanceExpression IServiceExpression.Use
		{
			get { return new ServiceInstanceExpression<object>(Type, Name, Kernel, DefaultSettings, true); }
		}

		IServiceInstanceExpression IServiceExpression.TryUse
		{
			get { return new ServiceInstanceExpression<object>(Type, Name, Kernel, DefaultSettings, false); }
		}

		IServiceInstanceExpression<TService> IServiceExpression<TService>.Use
		{
			get { return new ServiceInstanceExpression<TService>(Type, Name, Kernel, DefaultSettings, true); }
		}

		IServiceInstanceExpression<TService> IServiceExpression<TService>.TryUse
		{
			get { return new ServiceInstanceExpression<TService>(Type, Name, Kernel, DefaultSettings, false); }
		}

		IServiceInstanceExpression IServicesExpression.Add
		{
			get { return new ServiceInstanceExpression<object>(Type, Name, Kernel, DefaultSettings, true); }
		}

		IServiceInstanceExpression<TService> IServicesExpression<TService>.Add
		{
			get { return new ServiceInstanceExpression<TService>(Type, Name, Kernel, DefaultSettings, true); }
		}
	}
}