using System;
using Maestro.Internals;

namespace Maestro.Configuration
{
	internal class ServiceExpression<TService> : IServiceExpression, IServiceExpression<TService>, INamedServiceExpression, INamedServiceExpression<TService>
	{
		internal ServiceExpression(Type type, string name, Kernel kernel)
		{
			Type = type;
			Name = name;
			Kernel = kernel;
		}

		internal Type Type { get; }
		internal string Name { get; set; }
		internal Kernel Kernel { get; }

		IInstanceKindSelector IUseServiceExpression.Use
		{
			get { return new InstanceKindSelector<object>(Type, Name, Kernel, true); }
		}

		IInstanceKindSelector ITryUseServiceExpression.TryUse
		{
			get { return new InstanceKindSelector<object>(Type, Name, Kernel, false); }
		}

		IInstanceKindSelector IAddServiceExpression.Add
		{
			get { return new InstanceKindSelector<object>(Type, ServiceNames.Anonymous, Kernel, true); }
		}

		IInstanceKindSelector<TService> IUseServiceExpression<TService>.Use
		{
			get { return new InstanceKindSelector<TService>(Type, Name, Kernel, true); }
		}

		IInstanceKindSelector<TService> ITryUseServiceExpression<TService>.TryUse
		{
			get { return new InstanceKindSelector<TService>(Type, Name, Kernel, false); }
		}

		IInstanceKindSelector<TService> IAddServiceExpression<TService>.Add
		{
			get { return new InstanceKindSelector<TService>(Type, ServiceNames.Anonymous, Kernel, true); }
		}
	}
}