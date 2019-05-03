using System;
using Maestro.FactoryProviders;

namespace Maestro.TypeFactoryResolvers
{
	internal interface IFactoryProviderResolver
	{
		bool TryGet(Type type, string name, Context context, out IFactoryProvider factoryProvider);
	}
}