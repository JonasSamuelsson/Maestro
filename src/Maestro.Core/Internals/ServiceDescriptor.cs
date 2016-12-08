using System;
using System.Collections.Generic;
using Maestro.FactoryProviders;
using Maestro.Interceptors;
using Maestro.Lifetimes;

namespace Maestro.Internals
{
	class ServiceDescriptor
	{
		public Type Type { get; set; }
		public string Name { get; set; }
		public IFactoryProvider FactoryProvider { get; set; }
		public List<IInterceptor> Interceptors { get; set; } = new List<IInterceptor>();
		public ILifetime Lifetime { get; set; } = new TransientLifetime();
	}
}