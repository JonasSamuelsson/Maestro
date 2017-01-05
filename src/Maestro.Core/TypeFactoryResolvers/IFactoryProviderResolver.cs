using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	interface IFactoryProviderResolver
	{
		bool TryGet(Type type, IContext context, out IFactoryProvider factoryProvider);
	}
}