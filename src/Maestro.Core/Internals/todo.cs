using System;

namespace Maestro.Internals
{
	interface IInstanceFactoryResolver
	{
		bool CanHandle(Type type, Context context);
		IFactoryProvider GetInstanceFactory(Type type);
	}
}