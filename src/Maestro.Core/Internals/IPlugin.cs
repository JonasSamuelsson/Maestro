using System.Collections.Generic;
using Maestro.FactoryProviders;
using Maestro.Interceptors;
using Maestro.Lifetimes;

namespace Maestro.Internals
{
	interface IPlugin
	{
		IFactoryProvider FactoryProvider { get; set; }
		List<IInterceptor> Interceptors { get; }
		ILifetime Lifetime { get; set; }
	}
}