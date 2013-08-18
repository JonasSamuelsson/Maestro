using System;

namespace Maestro
{
	internal interface IProvider
	{
		bool CanGet(IContext context);
		object Get(IContext context);
		IProvider MakeGenericProvider(Type[] types);
	}
}