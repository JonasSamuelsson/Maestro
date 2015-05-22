using System.Collections.Generic;
using Maestro.FactoryProviders;
using Maestro.Interceptors;
using Maestro.Lifetimes;

namespace Maestro.Internals
{
	class Plugin : IPlugin
	{
		public IFactoryProvider FactoryProvider { get; set; }
		public List<IInterceptor> Interceptors { get; set; } = new List<IInterceptor>();
		public ILifetime Lifetime { get; set; }
	}
}