using Maestro.FactoryProviders;
using Maestro.Interceptors;
using Maestro.Lifetimes;
using Maestro.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	class ServiceDescriptor
	{
		public Guid CorrelationId { get; set; } = Guid.NewGuid();
		public Type Type { get; set; }
		public string Name { get; set; }
		public IFactoryProvider FactoryProvider { get; set; }
		public List<IInterceptor> Interceptors { get; set; } = new List<IInterceptor>();
		public Lifetime Lifetime { get; set; } = TransientLifetime.Instance;
		public int SortOrder { get; set; }

		public ServiceDescriptor MakeGeneric(Type[] genericArguments)
		{
			return new ServiceDescriptor
			{
				CorrelationId = CorrelationId,
				Type = Type.MakeGenericType(genericArguments),
				FactoryProvider = FactoryProvider.MakeGeneric(genericArguments),
				Interceptors = Interceptors.Select(x => GenericInstanceFactory.Create<IInterceptor>(x, genericArguments)).ToList(),
				Lifetime = Lifetime.MakeGeneric(genericArguments),
				Name = Name,
				SortOrder = SortOrder
			};
		}
	}
}