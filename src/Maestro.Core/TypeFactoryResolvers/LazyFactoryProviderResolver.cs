using System;
using System.Linq;
using System.Threading;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	class LazyFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, Context context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;
			if (!type.IsConcreteClassClosing(typeof(Lazy<>))) return false;
			var typeArgument = type.GetGenericArguments().Single();
			if (!context.Kernel.CanGetDependency(typeArgument, context)) return false;

			factoryProvider = new TypeFactoryProvider(type)
			                  {
				                  Dependencies =
				                  {
					                  {
						                  typeof (LazyThreadSafetyMode),
						                  LazyThreadSafetyMode.ExecutionAndPublication
					                  }
				                  }
			                  };
			return true;
		}
	}
}