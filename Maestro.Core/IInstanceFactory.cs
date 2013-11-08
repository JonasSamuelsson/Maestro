using System;

namespace Maestro
{
	internal interface IInstanceFactory
	{
		bool CanGet(IContext context);
		object Get(IContext context);
		IInstanceFactory MakeGenericInstanceFactory(Type[] types);
	}
}