using Maestro.FactoryProviders.Factories;
using System;

namespace Maestro.FactoryProviders
{
	interface IFactoryProvider
	{
		Factory GetFactory(Context context);
		IFactoryProvider MakeGeneric(Type[] genericArguments);
		Type GetInstanceType();
	}
}