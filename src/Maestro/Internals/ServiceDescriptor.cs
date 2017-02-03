using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.FactoryProviders;
using Maestro.Interceptors;
using Maestro.Lifetimes;

namespace Maestro.Internals
{
	class ServiceDescriptor
	{
		public Guid CorrelationId { get; set; } = Guid.NewGuid();
		public Type Type { get; set; }
		public string Name { get; set; }
		public IFactoryProvider FactoryProvider { get; set; }
		public List<IInterceptor> Interceptors { get; set; } = new List<IInterceptor>();
		public ILifetime Lifetime { get; set; } = TransientLifetime.Instance;

		public ServiceDescriptor MakeGeneric(Type[] genericArguments)
		{
			return new ServiceDescriptor
			{
				CorrelationId = CorrelationId,
				Type = Type.MakeGenericType(genericArguments),
				FactoryProvider = FactoryProvider.MakeGeneric(genericArguments),
				Interceptors = Interceptors.Select(x => x.MakeGeneric(genericArguments)).ToList(),
				Lifetime = Lifetime.MakeGeneric(genericArguments),
				Name = Name
			};
		}
	}
}