using System;

namespace Maestro.Internals.FactoryProviders
{
	interface IFactoryProvider
	{
		Func<Context, object> GetFactory(Context context);
	}
}