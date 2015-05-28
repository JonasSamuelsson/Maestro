using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	interface IFactoryProviderResolver
	{
		bool TryGet(Type type, Context context, out IFactoryProvider factoryProvider);
	}
}