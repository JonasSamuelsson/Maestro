using System;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;

namespace Maestro.FactoryProviders
{
	interface IFactoryProvider
	{
		IFactory GetFactory(Context context);
		IFactoryProvider MakeGeneric(Type[] genericArguments);
		Type GetInstanceType();
	}
}